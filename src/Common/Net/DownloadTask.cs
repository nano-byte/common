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
        public override string Name { get { return string.Format(Resources.Downloading, Source); } }

        /// <inheritdoc/>
        protected override bool UnitsByte { get { return true; } }

        /// <summary>
        /// The URL the file is to be downloaded from.
        /// </summary>
        /// <remarks>This value may change once <see cref="TaskState.Data"/> has been reached, based on HTTP redirections.</remarks>
        [Description("The URL the file is to be downloaded from.")]
        [NotNull]
        public Uri Source { get; private set; }

        /// <summary>
        /// The HTTP header data returned by the server for the download request. An empty collection in case of an FTP download.
        /// </summary>
        /// <remarks>This value is always <see langword="null"/> until <see cref="TaskState.Data"/> has been reached.</remarks>
        [Browsable(false)]
        [CanBeNull]
        [PublicAPI]
        public WebHeaderCollection Headers { get; private set; }

        /// <summary>
        /// Creates a new download task.
        /// </summary>
        /// <param name="source">The URL the file is to be downloaded from.</param>
        /// <param name="bytesTotal">The number of bytes the file to be downloaded is long. The file will be rejected if it does not have this length. -1 if the size is unknown.</param>
        protected DownloadTask(Uri source, long bytesTotal = -1)
        {
            #region Sanity checks
            if (source == null) throw new ArgumentNullException("source");
            #endregion

            Source = source;
            UnitsTotal = bytesTotal;
        }

        /// <inheritdoc/>
        protected override void Execute()
        {
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

            var httpRequest = request as HttpWebRequest;
            if (httpRequest != null) httpRequest.UserAgent = AppInfo.Current.NameVersion;

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

                    // Write data to file
                    if (response != null)
                    {
                        using (var sourceStream = response.GetResponseStream())
                        using (var targetStream = CreateTargetStream())
                        {
                            Debug.Assert(sourceStream != null);
                            sourceStream.CopyTo(targetStream,
                                bufferSize: 8 * 1024,
                                cancellationToken: CancellationToken,
                                progress: new Progress<long>(x => UnitsProcessed = x).LimitRate());
                        }
                    }
                }
            }
                #region Error handling
            catch (WebException ex)
            {
                // Wrap exception to add context
                throw new WebException(string.Format(Resources.FailedToDownload, Source), ex.InnerException, ex.Status, ex.Response);
            }
            #endregion

            State = TaskState.Complete;
        }

        /// <summary>
        /// Creates the <see cref="Stream"/> to write the downloaded data to.
        /// </summary>
        [NotNull]
        protected abstract Stream CreateTargetStream();

        /// <summary>
        /// Reads the header information in the <paramref name="response"/> and stores it the object properties.
        /// </summary>
        /// <returns><see langword="true"/> if everything is ok; <see langword="false"/> if there was an error.</returns>
        private void ReadHeader(WebResponse response)
        {
            Headers = response.Headers;

            // Update the source URL to reflect changes made by HTTP redirection
            Source = response.ResponseUri;

            // Determine file size and make sure predetermined sizes are valid
            if (UnitsTotal == -1 || response.ContentLength == -1) UnitsTotal = response.ContentLength;
            else if (UnitsTotal != response.ContentLength)
                throw new WebException(string.Format(Resources.FileNotExpectedSize, Source, UnitsTotal, response.ContentLength));
        }
    }
}
