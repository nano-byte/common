﻿// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System.Threading.Tasks;
#endif

namespace NanoByte.Common.Streams;

/// <summary>
/// Decorator that adds progress-tracking and cancellation to another <see cref="Stream"/>.
/// </summary>
/// <param name="underlyingStream">Underlying stream to delegate to. Will be disposed together with this stream.</param>
/// <param name="progress">Used to report back the number of bytes that have been read or written.</param>
/// <param name="cancellationToken">Used to signal when the user wants to cancel the stream. If signaled read an write requests will start throwing <see cref="OperationCanceledException"/>.</param>
public sealed class ProgressStream(Stream underlyingStream, IProgress<long>? progress = null, CancellationToken cancellationToken = default) : DelegatingStream(underlyingStream)
{
    private long? _length;

    /// <summary>
    /// The length of the underlying stream if <see cref="Stream.CanSeek"/> is <c>true</c>. Otherwise, the number of bytes read or written so far.
    /// This value can also be overriden by <see cref="SetLength"/>.
    /// </summary>
    public override long Length => _length ?? (UnderlyingStream.CanSeek ? UnderlyingStream.Length : _counter);

    /// <summary>
    /// Overrides the value returned by <see cref="Length"/>. Does not affect the underlying stream.
    /// </summary>
    public override void SetLength(long value) => _length = value;

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
        => Report(UnderlyingStream.Read(buffer, offset, count));

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
        UnderlyingStream.Write(buffer, offset, count);
        Report(count);
    }

#if !NET20 && !NET40
    /// <inheritdoc/>
    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken ct)
        => Report(await UnderlyingStream.ReadAsync(buffer, offset, count, CombineTokens(ct)));

    /// <inheritdoc/>
    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken ct)
    {
        await UnderlyingStream.WriteAsync(buffer, offset, count, CombineTokens(ct));
        Report(count);
    }

    private CancellationToken CombineTokens(CancellationToken otherToken)
        => CancellationTokenSource.CreateLinkedTokenSource(otherToken, cancellationToken).Token;
#endif

#if NET
    /// <inheritdoc/>
    public override int Read(Span<byte> buffer)
        => Report(UnderlyingStream.Read(buffer));

    /// <inheritdoc/>
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken ct = default)
        => Report(await UnderlyingStream.ReadAsync(buffer, CombineTokens(ct)));

    /// <inheritdoc/>
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        UnderlyingStream.Write(buffer);
        Report(buffer.Length);
    }

    /// <inheritdoc/>
    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct = default)
    {
        await UnderlyingStream.WriteAsync(buffer, CombineTokens(ct));
        Report(buffer.Length);
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
        cancellationToken.ThrowIfCancellationRequested();
        progress?.Report(Interlocked.Add(ref _counter, count));
        return count;
    }
}
