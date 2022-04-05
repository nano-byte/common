// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

/// <summary>
/// Invokes a callback on <see cref="Dispose"/>.
/// </summary>
public sealed class Disposable : IDisposable
{
    private readonly Action _callback;

    /// <summary>
    /// Creates a new disposable.
    /// </summary>
    /// <param name="callback">The callback to invoke on <see cref="Dispose"/>.</param>
    public Disposable(Action callback)
    {
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
    }

    /// <summary>
    /// Invokes the callback.
    /// </summary>
    public void Dispose() => _callback();
}
