// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NanoByte.Common.Native;

namespace NanoByte.Common.Net;

/// <summary>
/// A simple HTTP server.
/// </summary>
[CLSCompliant(false)]
public abstract class HttpServer : IDisposable
{
    private readonly HttpListener _listener;

    /// <summary>
    /// The TCP port the server is listing on.
    /// </summary>
    public ushort Port { get; }

    /// <summary>
    /// Gets ready to serve HTTP requests.
    /// Call <see cref="StartHandlingRequests"/> after completing any additional setup.
    /// </summary>
    /// <param name="port">The TCP port to listen on; <c>0</c> to automatically pick free port.</param>
    /// <param name="localOnly"><c>true</c> to only respond to requests from the local machine instead of the network.</param>
    /// <exception cref="WebException">Unable to serve on the specified <paramref name="port"/>.</exception>
    /// <exception cref="NotAdminException">Needs admin rights to serve HTTP requests.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="port"/> is not a valid TCP port number.</exception>
    protected HttpServer(ushort port = 0, bool localOnly = false)
    {
        // Use separate port ranges for local-only and public to avoid conflicting http.sys registrations
        ushort minRandomPort = localOnly ? (ushort)50000 : (ushort)54999;
        ushort maxRandomPort = localOnly ? (ushort)55000 : (ushort)60000;

        var random = new Random();
        try
        {
            Exception? lastException = null;
            for (ushort p = port == 0 ? minRandomPort : port;
                 p <= (port == 0 ? maxRandomPort : port);
                 p += (ushort)random.Next(1, 64))
            {
                try
                {
                    _listener = BuildListener(p, localOnly);
                    Port = p;
                    break;
                }
                catch (Exception ex) when (ex is HttpListenerException or SocketException)
                {
                    _listener?.Close();
                    lastException = ex;
                }
            }
            if (_listener == null) throw lastException!.Rethrow();
        }
        #region Error handling
        catch (HttpListenerException ex) when (WindowsUtils.IsWindowsNT && ex.NativeErrorCode == 5)
        {
            throw new NotAdminException(ex.Message, ex);
        }
        catch (HttpListenerException ex)
        {
            // Wrap exception since only certain exception types are allowed
            throw new WebException(ex.Message, ex);
        }
        #endregion
    }

    private static HttpListener BuildListener(int port, bool localOnly)
    {
        var listener = new HttpListener {Prefixes = {$"http://{(localOnly ? "localhost" : "+")}:{port}/"}};
        listener.Start();
        return listener;
    }

    /// <summary>
    /// To be called by derived constructor when setup is complete.
    /// </summary>
    protected void StartHandlingRequests()
        => Task.Factory.StartNew(async () =>
        {
            try
            {
                while (_listener.IsListening)
                {
                    var context = await _listener.GetContextAsync();
                    _ = Task.Factory.StartNew(() =>
                    {
                        HandleRequest(context);
                        context.Response.Close();
                    }, TaskCreationOptions.LongRunning);
                }
            }
            catch (HttpListenerException)
            {} // Shutdown
        }, CancellationToken.None, TaskCreationOptions.HideScheduler, TaskScheduler.Default);

    /// <summary>
    /// Handles a single HTTP request.
    /// </summary>
    protected abstract void HandleRequest(HttpListenerContext context);

    /// <summary>
    /// Stops serving HTTP requests.
    /// </summary>
    public virtual void Dispose() => _listener.Close();
}
#endif
