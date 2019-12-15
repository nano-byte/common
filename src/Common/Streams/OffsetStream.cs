// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// This wrapper stream applies a constant byte offset to all access to an underlying stream.
    /// </summary>
    public class OffsetStream : Stream
    {
        private readonly Stream _baseStream;

        private readonly long _offset;

        /// <summary>
        /// Creates a new offset stream
        /// </summary>
        /// <param name="baseStream">Underlying stream for which all access will be offset.</param>
        /// <param name="offset">Number of bytes to offset the <paramref name="baseStream"/>.</param>
        public OffsetStream(Stream baseStream, long offset)
        {
            _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            _baseStream.Position = _offset = offset;
        }

        /// <inheritdoc/>
        public override bool CanRead => _baseStream.CanRead;

        /// <inheritdoc/>
        public override bool CanWrite => _baseStream.CanWrite;

        /// <inheritdoc/>
        public override bool CanSeek => _baseStream.CanSeek;

        /// <inheritdoc/>
        public override long Length => _baseStream.Length - _offset;

        /// <inheritdoc/>
        public override long Position { get => _baseStream.Position - _offset; set => _baseStream.Position = value + _offset; }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count) => _baseStream.Read(buffer, offset, count);

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => _baseStream.Write(buffer, offset, count);

        /// <inheritdoc/>
        public override int ReadByte() => _baseStream.ReadByte();

        /// <inheritdoc/>
        public override void WriteByte(byte value) => _baseStream.WriteByte(value);

        /// <inheritdoc/>
        public override void Flush() => _baseStream.Flush();

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => _baseStream.Seek((origin == SeekOrigin.Begin ? _offset : 0L) + offset, origin) - _offset;

        /// <inheritdoc/>
        public override void SetLength(long value) => _baseStream.SetLength(value + _offset);

        /// <inheritdoc/>
        public override void Close() => _baseStream.Close();
    }
}
