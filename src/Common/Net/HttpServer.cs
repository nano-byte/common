// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using System.Net.Sockets;
using NanoByte.Common.Native;
using NanoByte.Common.Threading;

namespace NanoByte.Common.Net;

/// <summary>
/// A simple HTTP server.
/// </summary>
[CLSCompliant(false)]
[MustDisposeResource]
public abstract class HttpServer : IDisposable
{
    private readonly HttpListener _listener;
    private int _activeRequests;

    /// <summary>
    /// The TCP port the server is listening on.
    /// </summary>
    public ushort Port { get; }

    /// <summary>
    /// Indicates whether the server is currently accepting requests.
    /// </summary>
    /// <remarks>Becomes <c>false</c> after <see cref="Dispose"/> or if the server stopped due to an unrecoverable failure.</remarks>
    public bool IsListening => _listener.IsListening;

    /// <summary>
    /// The maximum number of requests to handle at the same time. Additional requests are rejected with <see cref="HttpStatusCode.ServiceUnavailable"/>.
    /// </summary>
    public int MaxConcurrentRequests { get; set; } = 64;

    /// <summary>
    /// Gets ready to serve HTTP requests.
    /// Call <see cref="StartHandlingRequests"/> after completing any additional setup.
    /// </summary>
    /// <param name="port">The TCP port to listen on; <c>0</c> to automatically pick free port.</param>
    /// <param name="localOnly"><c>true</c> to only respond to requests from the local machine instead of the network.</param>
    /// <exception cref="WebException">Unable to serve on the specified <paramref name="port"/>, or unable to find a free port if <paramref name="port"/> is <c>0</c>.</exception>
    /// <exception cref="NotAdminException">Needs admin rights to serve HTTP requests.</exception>
    protected HttpServer(ushort port = 0, bool localOnly = false)
    {
        try
        {
            _listener = port == 0
                ? BuildListenerOnFreePort(localOnly, out port)
                : BuildListener(port, localOnly);
            Port = port;
        }
        #region Error handling
        catch (Exception ex) when (IsAccessDenied(ex))
        {
            throw new NotAdminException(ex.Message, ex);
        }
        catch (Exception ex) when (ex is HttpListenerException or SocketException)
        {
            // Wrap exception since only certain exception types are allowed
            throw new WebException(ex.Message, ex);
        }
        #endregion
    }

    /// <summary>
    /// Determines whether an exception indicates that the current user is not allowed to serve HTTP requests at all.
    /// </summary>
    private static bool IsAccessDenied(Exception ex)
        => WindowsUtils.IsWindowsNT && ex is HttpListenerException {NativeErrorCode: 5};

    private static HttpListener BuildListenerOnFreePort(bool localOnly, out ushort port)
    {
        // Use separate port ranges for local-only and public to avoid conflicting http.sys registrations
        int minPort = localOnly ? 50000 : 55000;
        int maxPort = localOnly ? 54999 : 59999;

        Exception? lastException = null;
        for (ushort p = (ushort)minPort; p <= maxPort; p++)
        {
            try
            {
                var listener = BuildListener(p, localOnly);
                port = p;
                return listener;
            }
            catch (Exception ex) when (ex is HttpListenerException or SocketException && !IsAccessDenied(ex))
            {
                lastException = ex;
            }
        }
        throw new WebException($"Unable to find a free port between {minPort} and {maxPort}.", lastException);
    }

    private static HttpListener BuildListener(ushort port, bool localOnly)
    {
        var listener = new HttpListener();
        try
        {
            listener.Prefixes.Add($"http://{(localOnly ? "localhost" : "+")}:{port}/");
            listener.Start();
        }
        #region Error handling
        catch
        {
            listener.Close();
            throw;
        }
        #endregion
        return listener;
    }

    /// <summary>
    /// To be called by derived constructor when setup is complete.
    /// </summary>
    protected void StartHandlingRequests()
        => ThreadUtils.StartBackground(AcceptRequests, name: $"{nameof(HttpServer)}.Loop");

    /// <summary>
    /// Waits for incoming requests and dispatches them to <see cref="HandleRequest"/>.
    /// </summary>
    private void AcceptRequests()
    {
        const int MaxConsecutiveAcceptFailures = 10;
        int consecutiveFailures = 0;

        while (_listener.IsListening)
        {
            try
            {
                DispatchRequest(_listener.GetContext());
                consecutiveFailures = 0;
            }
            #region Error handling
            catch (Exception ex) when (IsShutdown(ex))
            {
                // Server shut down while waiting for requests
                return;
            }
            catch (Exception ex)
            {
                // A single broken connection (e.g., a client aborting mid-request) must not take down the entire server,
                // but stop rather than spin if the listener keeps failing without ever becoming unlistenable
                if (++consecutiveFailures > MaxConsecutiveAcceptFailures)
                {
                    Log.Error("Stopped listening for HTTP requests", ex);
                    StopListening();
                    return;
                }

                Log.Warn("Failed to accept HTTP request", ex);
            }
            #endregion
        }
    }

    /// <summary>
    /// Determines whether an exception was caused by the server shutting down.
    /// </summary>
    private bool IsShutdown(Exception ex)
        => !_listener.IsListening
        && ex is ObjectDisposedException or HttpListenerException or InvalidOperationException or IOException;

    /// <summary>
    /// Starts handling a request on a separate thread.
    /// </summary>
    private void DispatchRequest(HttpListenerContext context)
    {
        if (Interlocked.Increment(ref _activeRequests) > MaxConcurrentRequests)
        {
            Interlocked.Decrement(ref _activeRequests);
            CompleteResponse(context, HttpStatusCode.ServiceUnavailable);
            return;
        }

        try
        {
            ThreadUtils.StartBackground(() =>
            {
                try
                {
                    ProcessRequest(context);
                }
                finally
                {
                    Interlocked.Decrement(ref _activeRequests);
                }
            }, name: $"{nameof(HttpServer)}.{nameof(HandleRequest)}");
        }
        #region Error handling
        catch (Exception ex)
        {
            Interlocked.Decrement(ref _activeRequests);
            Log.Error("Unable to start thread for handling HTTP request", ex);
            CompleteResponse(context, HttpStatusCode.ServiceUnavailable);
        }
        #endregion
    }

    /// <summary>
    /// Handles a single request and completes its response, reporting any failures to the log and to the client.
    /// </summary>
    private void ProcessRequest(HttpListenerContext context)
    {
        HttpStatusCode? errorStatus = HttpStatusCode.InternalServerError;
        try
        {
            HandleRequest(context);
            errorStatus = null;
        }
        #region Error handling
        catch (Exception ex) when (IsShutdown(ex))
        {
            // Server shut down while handling request, so there is nobody left to report a status to
            errorStatus = null;
        }
        catch (Exception ex) when (ex is HttpListenerException or IOException)
        {
            Log.Debug("Connection closed or IO error while handling HTTP request", ex);
        }
        catch (Exception ex)
        {
            Log.Error("Failed to handle HTTP request", ex);
        }
        #endregion
        finally
        {
            CompleteResponse(context, errorStatus);
        }
    }

    /// <summary>
    /// Closes the listener, so that the port is released and clients get a connection refusal instead of waiting for a response that will never come.
    /// </summary>
    private void StopListening()
    {
        try
        {
            _listener.Close();
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Debug("Failed to close HTTP listener", ex);
        }
        #endregion
    }

    /// <summary>
    /// Handles a single HTTP request.
    /// </summary>
    protected abstract void HandleRequest(HttpListenerContext context);

    /// <summary>
    /// Tries to respond with a status code and an empty body.
    /// </summary>
    /// <returns><c>true</c> if the status code was set; <c>false</c> if the response headers have already been sent.</returns>
    private static bool TrySetStatus(HttpListenerContext context, HttpStatusCode statusCode)
    {
        try
        {
            // Set this first to probe for headers that are already on the wire.
            // Unlike StatusCode, which silently does nothing in that case, this throws.
            context.Response.ContentLength64 = 0;

            context.Response.StatusCode = (int)statusCode;
            return true;
        }
        #region Error handling
        catch (Exception ex) when (ex is InvalidOperationException or ObjectDisposedException or HttpListenerException)
        {
            Log.Debug($"Unable to report status {(int)statusCode} to client, response already (partially) sent", ex);
            return false;
        }
        #endregion
    }

    /// <summary>
    /// Completes the response, reporting a failure to the client if there was one.
    /// </summary>
    /// <param name="context">The request to respond to.</param>
    /// <param name="errorStatus">A status code to report to the client; <c>null</c> if the response completed normally.</param>
    private static void CompleteResponse(HttpListenerContext context, HttpStatusCode? errorStatus)
    {
        try
        {
            if (errorStatus is {} status && !TrySetStatus(context, status))
            {
                // Headers are already on the wire, so truncate the connection instead of terminating it cleanly.
                // Otherwise the client would mistake a partial response (e.g. a half-written body) for a complete one.
                context.Response.Abort();
            }
            else context.Response.Close();
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Debug("Failed to complete HTTP response", ex);
        }
        #endregion
    }

    /// <summary>
    /// Stops serving HTTP requests.
    /// Does not wait for requests that are currently being handled to complete.
    /// </summary>
    public virtual void Dispose() => StopListening();
}
