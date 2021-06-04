// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;

#if !NET20 && !NET40
using System.Threading;
using System.Threading.Tasks;
#endif

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Forwards all requests to another <see cref="System.IO.Stream"/>s. Useful as a base class for decorators/wrappers.
    /// </summary>
    public abstract class DelegatingStream : Stream
    {
        /// <summary>
        /// Underlying stream to delegate to.
        /// </summary>
        protected readonly Stream UnderlyingStream;

        /// <summary>
        /// Creates a new delegating stream.
        /// </summary>
        /// <param name="underlyingStream">Underlying stream to delegate to. Will be disposed together with this stream.</param>
        protected DelegatingStream(Stream underlyingStream)
        {
            UnderlyingStream = underlyingStream ?? throw new ArgumentNullException(nameof(underlyingStream));
        }

        /// <inheritdoc/>
        public override bool CanRead => UnderlyingStream.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => UnderlyingStream.CanSeek;

        /// <inheritdoc/>
        public override bool CanWrite => UnderlyingStream.CanWrite;

        /// <inheritdoc/>
        public override long Length => UnderlyingStream.Length;

        /// <inheritdoc/>
        public override bool CanTimeout => UnderlyingStream.CanTimeout;

        /// <inheritdoc/>
        public override int ReadTimeout => UnderlyingStream.ReadTimeout;

        /// <inheritdoc/>
        public override int WriteTimeout => UnderlyingStream.WriteTimeout;

        /// <inheritdoc/>
        public override long Position { get => UnderlyingStream.Position; set => UnderlyingStream.Position = value; }

        /// <inheritdoc/>
        public override void Flush() => UnderlyingStream.Flush();

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => UnderlyingStream.Seek(offset, origin);

        /// <inheritdoc/>
        public override void SetLength(long value) => UnderlyingStream.SetLength(value);

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count) => UnderlyingStream.Read(buffer, offset, count);

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => UnderlyingStream.Write(buffer, offset, count);

#if !NET20 && !NET40
        /// <inheritdoc/>
        public override Task FlushAsync(CancellationToken cancellationToken) => UnderlyingStream.FlushAsync(cancellationToken);

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => UnderlyingStream.ReadAsync(buffer, offset, count, cancellationToken);

        /// <inheritdoc/>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => UnderlyingStream.WriteAsync(buffer, offset, count, cancellationToken);
#endif

#if !NETFRAMEWORK
        /// <inheritdoc/>
        public override int Read(Span<byte> buffer) => UnderlyingStream.Read(buffer);

        /// <inheritdoc/>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => UnderlyingStream.ReadAsync(buffer, cancellationToken);

        /// <inheritdoc/>
        public override void Write(ReadOnlySpan<byte> buffer) => UnderlyingStream.Write(buffer);

        /// <inheritdoc/>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => UnderlyingStream.WriteAsync(buffer, cancellationToken);
#endif

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing) UnderlyingStream.Dispose();
        }
    }
}
