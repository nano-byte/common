// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using NanoByte.Common.Info;
using NanoByte.Common.Streams;
using NanoByte.Common.Tasks;
using NanoByte.Common.Threading;

#if !NET20 && !NET40
using System.Net.Http;
#endif

namespace NanoByte.Common.Net;

/// <summary>
/// Downloads a file from a specific internet address to a stream.
/// </summary>
public class DownloadFile : TaskBase
{
    /// <inheritdoc/>
    public override string Name => string.Format(Resources.Downloading, Source);

    /// <inheritdoc/>
    protected override bool UnitsByte => true;

    /// <summary>
    /// The maximum number of bytes to download.
    /// </summary>
    public long BytesMaximum { get; set; } = long.MaxValue;

    /// <summary>
    /// The URL the file is to be downloaded from.
    /// </summary>
    /// <remarks>This value may change once <see cref="TaskState.Data"/> has been reached, based on HTTP redirections.</remarks>
    [Description("The URL the file is to be downloaded from.")]
    public Uri Source { get; private set; }

    /// <summary>
    /// Set to <c>true</c> to add a No-Cache header to the request for any intermediate proxy servers.
    /// </summary>
    public bool NoCache { get; set; }

    /// <summary>
    /// Indicates whether the server has started sending content.
    /// </summary>
    public bool ContentStarted { get; private set; }

    private readonly Action<Stream> _callback;

    /// <summary>
    /// Creates a new download task.
    /// </summary>
    /// <param name="source">The URL the file is to be downloaded from.</param>
    /// <param name="callback">Called with a stream providing the download content.</param>
    /// <param name="bytesTotal">The number of bytes the file to be downloaded is long. The file will be rejected if it does not have this length. -1 if the size is unknown.</param>
    public DownloadFile(Uri source, Action<Stream> callback, long bytesTotal = -1)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
        UnitsTotal = bytesTotal;
    }

    /// <summary>
    /// Creates a new download task.
    /// </summary>
    /// <param name="source">The URL the file is to be downloaded from.</param>
    /// <param name="target">The local path to save the file to. A preexisting file will be overwritten.</param>
    /// <param name="bytesTotal">The number of bytes the file to be downloaded is long. The file will be rejected if it does not have this length. -1 if the size is unknown.</param>
    public DownloadFile(Uri source, [Localizable(false)] string target, long bytesTotal = -1)
        : this(source, stream =>
        {
            using var fileStream = File.Create(target);
            stream.CopyToEx(fileStream);
        }, bytesTotal)
    {}

    /// <summary>
    /// The HTTP Basic Auth credentials to use for downloading the file.
    /// </summary>
    private NetworkCredential? _credentials;

#if !NET20 && !NET40
    private static readonly HttpClient _httpClient = new(
#if NETFRAMEWORK
        new HttpClientHandler
        {
#else
        new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(1),
#endif
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
        }
    )
    {
        Timeout = Timeout.InfiniteTimeSpan
    };
    #endif

    /// <inheritdoc/>
    protected override void Execute()
    {
        HttpStatusCode? statusCode = null;
        try
        {
            State = TaskState.Header;
#if !NET20 && !NET40
            using var response = _httpClient.Send(new(HttpMethod.Get, Source)
            {
                Headers =
                {
                    UserAgent = { new(AppInfo.Current.Name ?? "dotnet", AppInfo.Current.Version) },
                    CacheControl = new() { NoCache = NoCache },
                    Authorization = _credentials?.ToBasicAuth()
                }
            }, HttpCompletionOption.ResponseHeadersRead, CancellationToken);
            statusCode = response.StatusCode;
            response.EnsureSuccessStatusCode();
#else
            var request = WebRequest.Create(Source);
            if (request is HttpWebRequest httpRequest)
            {
                httpRequest.UserAgent = AppInfo.Current.NameVersion;
                httpRequest.Credentials = _credentials;
                if (NoCache) httpRequest.Headers.Add(HttpRequestHeader.CacheControl, "no-cache");
            }
            var responseHandler = request.BeginGetResponse(null!, null!);
            responseHandler.AsyncWaitHandle.WaitOne(CancellationToken);
            using var response = request.EndGetResponse(responseHandler);
#endif

            CancellationToken.ThrowIfCancellationRequested();
            HandleHeaders(response);

            State = TaskState.Data;
            ContentStarted = true;
            using var stream = new ProgressStream(
#if !NET20 && !NET40
                response.Content.ReadAsStream(CancellationToken),
#else
                // ReSharper disable once AssignNullToNotNullAttribute
                response.GetResponseStream(),
#endif
                new SynchronousProgress<long>(x => UnitsProcessed = x),
                CancellationToken);
            if (UnitsTotal > 0) stream.SetLength(UnitsTotal);
            _callback(stream);
        }
        #region Error handling
#if NET
        catch (HttpRequestException ex)
        {
            statusCode ??= ex.StatusCode;
#elif !NET20 && !NET40
        catch (HttpRequestException ex)
        {
            var webException = ex.InnerException as WebException;
            statusCode ??= (webException?.Response as HttpWebResponse)?.StatusCode;
#else
        catch (WebException webException)
        {
            statusCode ??= (webException.Response as HttpWebResponse)?.StatusCode;
#endif

            if (RetryWithCredentials(statusCode))
            {
                Log.Info($"Retrying download for {Source} with credentials");
                Execute();
                return;
            }

            // Wrap exception to add context and since only certain exception types are allowed
            throw new WebException(string.Format(Resources.FailedToDownload, Source),
#if NET
                ex);
#elif !NET20 && !NET40
                (Exception)webException ?? ex, webException?.Status ?? default, webException?.Response);
#else
                webException, webException.Status, webException.Response);
#endif
        }
        catch (Exception ex) when (ex is NotSupportedException or ArgumentOutOfRangeException)
        {
            // Wrap exception since only certain exception types are allowed
            throw new WebException(ex.Message, ex);
        }
        #endregion
    }

    private bool RetryWithCredentials(HttpStatusCode? statusCode)
    {
        if (CredentialProvider == null) return false;

        switch (statusCode)
        {
            case HttpStatusCode.Unauthorized:
                _credentials = CredentialProvider.GetCredential(Source, previousIncorrect: _credentials != null);
                return _credentials != null;

#if NET
            case HttpStatusCode.ProxyAuthenticationRequired when HttpClient.DefaultProxy.GetProxy(Source) is {} proxy:
                HttpClient.DefaultProxy.Credentials = CredentialProvider.GetCredential(proxy, previousIncorrect: HttpClient.DefaultProxy.HasCustomCredentials());
                return HttpClient.DefaultProxy.Credentials != null;
#else
            case HttpStatusCode.ProxyAuthenticationRequired when WebRequest.DefaultWebProxy?.GetProxy(Source) is {} proxy:
                WebRequest.DefaultWebProxy.Credentials = CredentialProvider.GetCredential(proxy, previousIncorrect: WebRequest.DefaultWebProxy.HasCustomCredentials());
                return WebRequest.DefaultWebProxy.Credentials != null;
#endif

            default:
                return false;
        }
    }

#if !NET20 && !NET40
    private void HandleHeaders(HttpResponseMessage response)
    {
        if (response.RequestMessage?.RequestUri is {} source) Source = source;
        if (response.Content.Headers.ContentLength is {} contentLength)
#else
    private void HandleHeaders(WebResponse response)
    {
        Source = response.ResponseUri;
        if (response.ContentLength is var contentLength and not -1)
#endif
        {
            if (UnitsTotal == -1)
                UnitsTotal = contentLength;
            else if (UnitsTotal != contentLength)
                throw new WebException(string.Format(Resources.FileNotExpectedSize, Source, UnitsTotal, contentLength));

            if (contentLength > BytesMaximum)
                throw new WebException(string.Format(Resources.FileNotExpectedSize, Source, BytesMaximum, contentLength));
        }
    }
}
