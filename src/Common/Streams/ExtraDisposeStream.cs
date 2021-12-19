// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Streams;

/// <summary>
/// Decorator that executes an additional delegate before <see cref="Stream.Dispose()"/>.
/// </summary>
public sealed class ExtraDisposeStream : DelegatingStream
{
    private readonly Action _disposeHandler;

    /// <summary>
    /// Creates a new dispose wrapper stream.
    /// </summary>
    /// <param name="underlyingStream">Underlying stream to delegate to. Will be disposed together with this stream.</param>
    /// <param name="disposeHandler">Executed before <paramref name="underlyingStream"/> is disposed.</param>
    public ExtraDisposeStream(Stream underlyingStream, Action disposeHandler)
        : base(underlyingStream)
    {
        _disposeHandler = disposeHandler ?? throw new ArgumentNullException(nameof(disposeHandler));
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing) _disposeHandler();
        }
        finally
        {
            base.Dispose(disposing);
        }
    }
}