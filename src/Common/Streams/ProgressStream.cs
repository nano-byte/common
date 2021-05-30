// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using System.Threading;

#if !NET20 && !NET40
using System.Threading.Tasks;
#endif

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Decorator that adds progress-reporting and cancellation to another <see cref="Stream"/>.
    /// </summary>
    public sealed class ProgressStream : DelegatingStream
    {
        private readonly IProgress<long>? _progress;
        private readonly CancellationToken _cancellationToken;

        /// <summary>
        /// Creates a new progress-reporting stream.
        /// </summary>
        /// <param name="underlyingStream">Underlying stream to delegate to. Will be disposed together with this stream.</param>
        /// <param name="progress">Used to report back the number of bytes that have been read or written.</param>
        /// <param name="cancellationToken">Used to signal when the user wishes to cancel the stream. If signaled read an write requests will start throwing <see cref="OperationCanceledException"/>.</param>
        public ProgressStream(Stream underlyingStream, IProgress<long>? progress = null, CancellationToken cancellationToken = default)
            : base(underlyingStream)
        {
            _progress = progress;
            _cancellationToken = cancellationToken;
        }

        private long? _length;

        /// <inheritdoc/>
        public override long Length => _length ?? base.Length;

        /// <summary>
        /// Overrides the <see cref="Stream.Length"/> provided by the underlying stream.
        /// </summary>
        public override void SetLength(long value) => _length = value;

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
            => Report(UnderlyingStream.Read(buffer, offset, count));

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
            => UnderlyingStream.Write(buffer, offset, Report(count));

#if !NET20 && !NET40
        /// <inheritdoc/>
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => Report(await UnderlyingStream.ReadAsync(buffer, offset, count, CombineTokens(cancellationToken)));

        /// <inheritdoc/>
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => await UnderlyingStream.WriteAsync(buffer, offset, Report(count), CombineTokens(cancellationToken));

        private CancellationToken CombineTokens(CancellationToken cancellationToken)
            => CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationToken).Token;
#endif

#if !NETFRAMEWORK
        /// <inheritdoc/>
        public override int Read(Span<byte> buffer)
            => Report(UnderlyingStream.Read(buffer));

        /// <inheritdoc/>
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            => Report(await UnderlyingStream.ReadAsync(buffer, CombineTokens(cancellationToken)));

        /// <inheritdoc/>
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            Report(buffer.Length);
            UnderlyingStream.Write(buffer);
        }

        /// <inheritdoc/>
        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            Report(buffer.Length);
            await UnderlyingStream.WriteAsync(buffer, CombineTokens(cancellationToken));
        }
#endif

        private long _counter;

        /// <summary>
        /// Reports the current progress.
        /// </summary>
        /// <param name="count">Number of bytes read or written in the current operation.</param>
        /// <returns><paramref name="count"/></returns>
        /// <exception cref="OperationCanceledException">Cancellation has been requested.</exception>
        private int Report(int count)
        {
            _cancellationToken.ThrowIfCancellationRequested();
            _progress?.Report(Interlocked.Add(ref _counter, count));
            return count;
        }
    }
}
