// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Downloads a file from a specific internet address to an in-memory array.
    /// </summary>
    public class DownloadMemory : DownloadTask
    {
        /// <summary>
        /// Creates a new download task.
        /// </summary>
        /// <param name="source">The URL the file is to be downloaded from.</param>
        /// <param name="bytesTotal">The number of bytes the file to be downloaded is long. The file will be rejected if it does not have this length. -1 if the size is unknown.</param>
        public DownloadMemory(Uri source, long bytesTotal = -1)
            : base(source, bytesTotal)
        {}

        private MemoryStream? _targetStream;

        /// <inheritdoc/>
        protected override Stream CreateTargetStream() => _targetStream = new();

        /// <summary>
        /// Returns the downloaded data.
        /// </summary>
        /// <exception cref="InvalidOperationException">The download is not finished yet.</exception>
        [Pure]
        public byte[] GetData()
        {
            if (_targetStream == null) throw new InvalidOperationException();

            return _targetStream.ToArray();
        }
    }
}
