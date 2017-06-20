/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using JetBrains.Annotations;
using NanoByte.Common.Info;
using NanoByte.Common.Properties;
using NanoByte.Common.Streams;
using NanoByte.Common.Tasks;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Abstract base class for tasks that download a file from the web.
    /// </summary>
    public abstract class DownloadTask : TaskBase
    {
        /// <inheritdoc/>
        public override string Name => string.Format(Resources.Downloading, Source);

        /// <inheritdoc/>
        protected override bool UnitsByte => true;

        /// <summary>
        /// The URL the file is to be downloaded from.
        /// </summary>
        /// <remarks>This value may change once <see cref="TaskState.Data"/> has been reached, based on HTTP redirections.</remarks>
        [Description("The URL the file is to be downloaded from.")]
        [NotNull]
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
        [CanBeNull]
        [PublicAPI]
        public WebHeaderCollection ResponseHeaders { get; private set; }

        /// <summary>
        /// Creates a new download task.
        /// </summary>
        /// <param name="source">The URL the file is to be downloaded from.</param>
        /// <param name="bytesTotal">The number of bytes the file to be downloaded is long. The file will be rejected if it does not have this length. -1 if the size is unknown.</param>
        protected DownloadTask(Uri source, long bytesTotal = -1)
        {
            #region Sanity checks
            if (source == null) throw new ArgumentNullException(nameof(source));
            #endregion

            Source = source;
            UnitsTotal = bytesTotal;
        }

        /// <inheritdoc/>
        protected override void Execute()
        {
            Retry:
            WebRequest request;
            try
            {
                request = WebRequest.Create(Source);
            }
                #region Error handling
            catch (NotSupportedException ex)
            {
                // Wrap exception since only certain exception types are allowed
                throw new WebException(ex.Message, ex);
            }
            #endregion

            if (request is HttpWebRequest httpRequest)
            {
                httpRequest.UserAgent = AppInfo.Current.NameVersion;
                httpRequest.Credentials = CredentialProvider;
                if (NoCache) httpRequest.Headers.Add(HttpRequestHeader.CacheControl, "no-cache");
            }

            // Start HTTP request
            State = TaskState.Header;
            // ReSharper disable AssignNullToNotNullAttribute
            var responseRequest = request.BeginGetResponse(null, null);
            // ReSharper restore AssignNullToNotNullAttribute

            // Wait for the download request to complete or a cancel request to arrive
            if (WaitHandle.WaitAny(new[] {responseRequest.AsyncWaitHandle, CancellationToken.WaitHandle}) == 1)
                throw new OperationCanceledException();

            // Process the response
            try
            {
                using (var response = request.EndGetResponse(responseRequest))
                {
                    CancellationToken.ThrowIfCancellationRequested();
                    ReadHeader(response);
                    State = TaskState.Data;

                    if (response != null)
                    {
                        using (var sourceStream = response.GetResponseStream())
                        using (var targetStream = CreateTargetStream())
                        {
                            Debug.Assert(sourceStream != null);
                            sourceStream.CopyToEx(targetStream,
                                bufferSize: 8 * 1024,
                                cancellationToken: CancellationToken,
                                progress: new SynchronousProgress<long>(x => UnitsProcessed = x));
                        }
                    }
                }
            }
                #region Error handling
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.RequestCanceled) throw new OperationCanceledException();
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (ex.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.Unauthorized && CredentialProvider != null)
                    {
                        CredentialProvider.ReportInvalid(ex.Response.ResponseUri);
                        if (CredentialProvider.Interactive) goto Retry;
                    }
                }

                // Wrap exception to add context
                throw new WebException(string.Format(Resources.FailedToDownload, Source), ex, ex.Status, ex.Response);
            }
            #endregion
        }

        /// <summary>
        /// Creates the <see cref="Stream"/> to write the downloaded data to.
        /// </summary>
        [NotNull]
        protected abstract Stream CreateTargetStream();

        /// <summary>
        /// Reads the header information in the <paramref name="response"/> and stores it the object properties.
        /// </summary>
        /// <returns><c>true</c> if everything is ok; <c>false</c> if there was an error.</returns>
        private void ReadHeader(WebResponse response)
        {
            ResponseHeaders = response.Headers;

            // Update the source URL to reflect changes made by HTTP redirection
            Source = response.ResponseUri;

            // Determine file size and make sure predetermined sizes are valid
            if (UnitsTotal == -1 || response.ContentLength == -1) UnitsTotal = response.ContentLength;
            else if (UnitsTotal != response.ContentLength)
                throw new WebException(string.Format(Resources.FileNotExpectedSize, Source, UnitsTotal, response.ContentLength));
        }
    }
}
