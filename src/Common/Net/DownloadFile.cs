// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using NanoByte.Common.Info;
using NanoByte.Common.Properties;
using NanoByte.Common.Streams;
using NanoByte.Common.Tasks;
using NanoByte.Common.Threading;

namespace NanoByte.Common.Net
{
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
        /// The HTTP header data returned by the server for the download request. An empty collection in case of an FTP download.
        /// </summary>
        /// <remarks>This value is always <c>null</c> until <see cref="TaskState.Data"/> has been reached.</remarks>
        [Browsable(false)]
        public WebHeaderCollection? ResponseHeaders { get; private set; }

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

        /// <inheritdoc/>
        protected override void Execute()
        {
            // Try once without credentials, then retry with if required
            bool usedCredentials = false;
            while (true)
            {
                var request = BuildRequest(usedCredentials);

                try
                {
                    State = TaskState.Header;
                    using var response = GetResponse(request);
                    HandleHeaders(response);

                    State = TaskState.Data;
                    using var stream = new ProgressStream(response.GetResponseStream()!, new SynchronousProgress<long>(x => UnitsProcessed = x), CancellationToken);
                    if (UnitsTotal > 0) stream.SetLength(UnitsTotal);
                    _callback(stream);
                    return;
                }
                catch (WebException ex)
                {
                    switch (ex.Status)
                    {
                        case WebExceptionStatus.RequestCanceled:
                            throw new OperationCanceledException();

                        case WebExceptionStatus.ProtocolError when ex.Response is HttpWebResponse {StatusCode: HttpStatusCode.Unauthorized} && CredentialProvider != null:
                            if (usedCredentials)
                                CredentialProvider.ReportInvalid(ex.Response.ResponseUri);
                            else
                            {
                                usedCredentials = true;
                                continue; // Retry
                            }
                            break;
                    }

                    // Wrap exception to add context
                    throw new WebException(string.Format(Resources.FailedToDownload, Source), ex, ex.Status, ex.Response);
                }
            }
        }

        private WebRequest BuildRequest(bool useCredentials)
        {
            WebRequest request;
            try
            {
                request = WebRequest.Create(Source);
                if (request is HttpWebRequest httpRequest)
                {
                    httpRequest.UserAgent = AppInfo.Current.NameVersion;
                    if (useCredentials) httpRequest.Credentials = CredentialProvider;
                    if (NoCache) httpRequest.Headers.Add(HttpRequestHeader.CacheControl, "no-cache");
                }
            }
            #region Error handling
            catch (NotSupportedException ex)
            {
                // Wrap exception since only certain exception types are allowed
                throw new WebException(ex.Message, ex);
            }
            #endregion

            return request;
        }

        private WebResponse GetResponse(WebRequest request)
        {
            var responseHandler = request.BeginGetResponse(null!, null!);

            responseHandler.AsyncWaitHandle.WaitOne(CancellationToken);
            var response = request.EndGetResponse(responseHandler);
            return response;
        }

        private void HandleHeaders(WebResponse response)
        {
            CancellationToken.ThrowIfCancellationRequested();

            ResponseHeaders = response.Headers;

            // Update the source URL to reflect changes made by HTTP redirection
            Source = response.ResponseUri;

            if (UnitsTotal == -1 || response.ContentLength == -1) UnitsTotal = response.ContentLength;
            else if (UnitsTotal != response.ContentLength)
                throw new WebException(string.Format(Resources.FileNotExpectedSize, Source, UnitsTotal, response.ContentLength));
            if (response.ContentLength > BytesMaximum)
                throw new WebException(string.Format(Resources.FileNotExpectedSize, Source, BytesMaximum, response.ContentLength));
        }
    }
}
