// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Streams;

/// <summary>
/// Decorator that prevents a stream from being seeked.
/// </summary>
/// <param name="underlyingStream">Underlying stream to delegate to. Will be disposed together with this stream.</param>
public class NonSeekableStream(Stream underlyingStream) : DelegatingStream(underlyingStream)
{
    /// <inheritdoc/>
    public override bool CanSeek => false;

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    /// <inheritdoc />
    public override long Position { get => base.Position; set => throw new NotSupportedException(); }
}
