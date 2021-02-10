// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// This wrapper stream passes all operations through to an underlying <see cref="Stream"/> without modifying them. An additional delegate is executed before <see cref="Stream.Dispose()"/> is passed through.
    /// </summary>
    public class DisposeWrapperStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly Action _disposeHandler;

        /// <summary>
        /// Creates a new dispose wrapper stream.
        /// </summary>
        /// <param name="baseStream">The underlying <see cref="Stream"/> providing the actual data. Will be disposed.</param>
        /// <param name="disposeHandler">Executed before <paramref name="baseStream"/> is disposed.</param>
        public DisposeWrapperStream(Stream baseStream, Action disposeHandler)
        {
            _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            _disposeHandler = disposeHandler ?? throw new ArgumentNullException(nameof(disposeHandler));
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            try
            {
                _disposeHandler();
                _baseStream.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region Pass-through
        /// <inheritdoc/>
        public override bool CanRead => _baseStream.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => _baseStream.CanSeek;

        /// <inheritdoc/>
        public override bool CanWrite => _baseStream.CanWrite;

        /// <inheritdoc/>
        public override long Length => _baseStream.Length;

        /// <inheritdoc/>
        public override long Position { get => _baseStream.Position; set => _baseStream.Position = value; }

        /// <inheritdoc/>
        public override void Flush() => _baseStream.Flush();

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => _baseStream.Seek(offset, origin);

        /// <inheritdoc/>
        public override void SetLength(long value) => _baseStream.SetLength(value);

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count) => _baseStream.Read(buffer, offset, count);

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => _baseStream.Write(buffer, offset, count);
        #endregion
    }
}
