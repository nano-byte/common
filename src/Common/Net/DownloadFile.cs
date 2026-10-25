// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using System.Net.Http;
using NanoByte.Common.Info;
using NanoByte.Common.Streams;
using NanoByte.Common.Tasks;
using NanoByte.Common.Threading;
#if NETFRAMEWORK
using System.Configuration;
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

    /// <inheritdoc/>
    protected override void Execute()
    {
        HttpStatusCode? statusCode = null;
        try
        {
            State = TaskState.Header;
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

            CancellationToken.ThrowIfCancellationRequested();
            HandleHeaders(response);

            State = TaskState.Data;
            ContentStarted = true;
            using var stream = new ProgressStream(
                response.Content.ReadAsStream(CancellationToken),
                new SynchronousProgress<long>(x => UnitsProcessed = x),
                CancellationToken);
            if (UnitsTotal > 0) stream.SetLength(UnitsTotal);
            _callback(stream);
        }
        #region Error handling
        catch (Exception ex) when (ex.IsHttpRequestException())
        {
#if NETFRAMEWORK
            var webException = ex.InnerException as WebException;
            statusCode ??= (webException?.Response as HttpWebResponse)?.StatusCode;
#endif

            if (RetryWithCredentials(statusCode))
            {
                Log.Info($"Retrying download for {Source} with credentials");
                Execute();
                return;
            }

            // Wrap exception to add context and since only certain exception types are allowed
            throw new WebException(string.Format(Resources.FailedToDownload, Source),
#if NETFRAMEWORK
                (Exception?)webException ?? ex, webException?.Status ?? default, webException?.Response);
#else
                ex);
#endif
        }
#if NETFRAMEWORK
        catch (ConfigurationErrorsException ex)
        {
            // Wrap exception since only certain exception types are allowed
            throw new WebException(ex.Message, ex);
        }
#endif
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

#if NETFRAMEWORK
            case HttpStatusCode.ProxyAuthenticationRequired when WebRequest.DefaultWebProxy?.GetProxy(Source) is {} proxy:
                WebRequest.DefaultWebProxy.Credentials = CredentialProvider.GetCredential(proxy, previousIncorrect: WebRequest.DefaultWebProxy.HasCustomCredentials());
                return WebRequest.DefaultWebProxy.Credentials != null;
#else
            case HttpStatusCode.ProxyAuthenticationRequired when HttpClient.DefaultProxy.GetProxy(Source) is {} proxy:
                HttpClient.DefaultProxy.Credentials = CredentialProvider.GetCredential(proxy, previousIncorrect: HttpClient.DefaultProxy.HasCustomCredentials());
                return HttpClient.DefaultProxy.Credentials != null;
#endif

            default:
                return false;
        }
    }

    private void HandleHeaders(HttpResponseMessage response)
    {
        if (response.RequestMessage?.RequestUri is {} source) Source = source;
        if (response.Content.Headers.ContentLength is {} contentLength)
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
