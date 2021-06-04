// Copyright Bastian Eicher
// Licensed under the MIT License

using System.IO;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Decorator that applies a constant byte offset to access to another <see cref="Stream"/>.
    /// </summary>
    public sealed class OffsetStream : DelegatingStream
    {
        private readonly long _offset;

        /// <summary>
        /// Creates a new offset stream.
        /// </summary>
        /// <param name="underlyingStream">Underlying stream to delegate to. Will be disposed together with this stream.</param>
        /// <param name="offset">Number of bytes to offset the <paramref name="underlyingStream"/>.</param>
        public OffsetStream(Stream underlyingStream, long offset)
            : base(underlyingStream)
        {
            underlyingStream.Position = _offset = offset;
        }

        /// <inheritdoc/>
        public override long Length
            => UnderlyingStream.Length - _offset;

        /// <inheritdoc/>
        public override long Position
        {
            get => UnderlyingStream.Position - _offset;
            set => UnderlyingStream.Position = value + _offset;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
            => UnderlyingStream.Seek((origin == SeekOrigin.Begin ? _offset : 0L) + offset, origin) - _offset;

        /// <inheritdoc/>
        public override void SetLength(long value)
            => UnderlyingStream.SetLength(value + _offset);
    }
}
