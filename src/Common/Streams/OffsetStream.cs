// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using NanoByte.Common.Collections;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Decorator that transparently applies an offset to another <see cref="Stream"/>.
    /// </summary>
    public sealed class OffsetStream : DelegatingStream
    {
        /// <summary>
        /// Creates a new offset stream.
        /// </summary>
        /// <param name="underlyingStream">Underlying stream to delegate to. Will be disposed together with this stream.</param>
        public OffsetStream(Stream underlyingStream)
            : base(underlyingStream)
        {}

        private int _offset;

        /// <summary>
        /// Applies an offset to the underlying stream.
        /// </summary>
        /// <param name="offset">The number of bytes to offset by. Must not be negative.</param>
        /// <exception cref="IOException">The underlying stream was shorter than the specified <paramref name="offset"/>.</exception>
        public void ApplyOffset(int offset)
        {
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset must not be negative.");
            if (offset == 0) return;

            if (UnderlyingStream.CanSeek)
                UnderlyingStream.Seek(offset, SeekOrigin.Current);
            else
            {
                int toRead = offset;
                using (ArrayUtils.Rent(Math.Min(toRead, 64), out byte[] buffer))
                {
                    while (toRead > 0)
                    {
                        int read = UnderlyingStream.Read(buffer, 0, Math.Min(toRead, buffer.Length));
                        if (read == 0) throw new IOException("The underlying stream was shorter than the specified offset.");
                        toRead -= read;
                    }
                }
            }

            _offset += offset;
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
