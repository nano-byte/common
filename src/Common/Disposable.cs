// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

/// <summary>
/// Invokes a callback on <see cref="Dispose"/>.
/// </summary>
/// <param name="callback">The callback to invoke on <see cref="Dispose"/>.</param>
[MustDisposeResource]
public sealed class Disposable(Action callback) : IDisposable
{
    /// <summary>
    /// Invokes the callback.
    /// </summary>
    public void Dispose() => callback();
}
