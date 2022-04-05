// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

/// <summary>
/// Ensures that a read operation for a file does not conflict with an <see cref="AtomicWrite"/> for the same file.
/// </summary>
/// <example><code>
/// using (new AtomicRead(filePath))
///     return File.ReadAllBytes(filePath);
/// </code></example>
public sealed class AtomicRead : IDisposable
{
    private readonly IDisposable _lock;

    /// <summary>
    /// Prepares an atomic read operation.
    /// </summary>
    /// <param name="path">The path of the file that will be read.</param>
    public AtomicRead([Localizable(false)] string path)
    {
        _lock = AtomicWrite.Lock(path ?? throw new ArgumentNullException(nameof(path)));
    }

    /// <inheritdoc/>
    public void Dispose() => _lock.Dispose();
}
