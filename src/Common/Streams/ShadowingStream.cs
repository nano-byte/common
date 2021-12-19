// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System.Threading.Tasks;
#endif

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Decorator that copies all bytes read from a <see cref="Stream"/> to another <see cref="Stream"/>.
    /// </summary>
    public sealed class ShadowingStream : DelegatingStream
    {
        private readonly Stream _shadowStream;

        /// <summary>
        /// Creates a new shadowing stream.
        /// </summary>
        /// <param name="underlyingStream">Underlying stream to delegate to. Will be disposed together with this stream.</param>
        /// <param name="shadowStream">The stream to copy all read bytes to.</param>
        public ShadowingStream(Stream underlyingStream, Stream shadowStream)
            : base(underlyingStream)
        {
            _shadowStream = shadowStream;
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = UnderlyingStream.Read(buffer, offset, count);
            _shadowStream.Write(buffer, offset, read);
            return read;
        }

#if !NET20 && !NET40
        /// <inheritdoc/>
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int read = await UnderlyingStream.ReadAsync(buffer, offset, count, cancellationToken);
            await _shadowStream.WriteAsync(buffer, offset, read, cancellationToken);
            return read;
        }
#endif

#if !NETFRAMEWORK
        /// <inheritdoc/>
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int read = await UnderlyingStream.ReadAsync(buffer, cancellationToken);
            await _shadowStream.WriteAsync(buffer[..read], cancellationToken);
            return read;
        }
#endif
    }
}
