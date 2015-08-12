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
using System.IO;
using JetBrains.Annotations;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// This wrapper stream applies a constant byte offset to all access to an underlying stream.
    /// </summary>
    public class OffsetStream : Stream
    {
        [NotNull]
        private readonly Stream _baseStream;

        private readonly long _offset;

        /// <summary>
        /// Creates a new offset stream
        /// </summary>
        /// <param name="baseStream">Underlying stream for which all access will be offset.</param>
        /// <param name="offset">Number of bytes to offset the <paramref name="baseStream"/>.</param>
        public OffsetStream([NotNull] Stream baseStream, long offset)
        {
            #region Sanity checks
            if (baseStream == null) throw new ArgumentNullException(nameof(baseStream));
            #endregion

            _baseStream = baseStream;
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
        public override long Position { get { return _baseStream.Position - _offset; } set { _baseStream.Position = value + _offset; } }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _baseStream.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _baseStream.Write(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override int ReadByte()
        {
            return _baseStream.ReadByte();
        }

        /// <inheritdoc/>
        public override void WriteByte(byte value)
        {
            _baseStream.WriteByte(value);
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            _baseStream.Flush();
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _baseStream.Seek((origin == SeekOrigin.Begin ? _offset : 0L) + offset, origin) - _offset;
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            _baseStream.SetLength(value + _offset);
        }

        /// <inheritdoc/>
        public override void Close()
        {
            _baseStream.Close();
        }
    }
}
