// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Decorator that prevents a stream from being seeked.
    /// </summary>
    public class NonSeekableStream : DelegatingStream
    {
        public NonSeekableStream(Stream underlyingStream)
            : base(underlyingStream)
        {}

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <inheritdoc />
        public override long Position { get => base.Position; set => throw new NotSupportedException(); }
    }
}
