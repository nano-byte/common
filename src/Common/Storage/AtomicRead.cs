// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

/// <summary>
/// Ensures that a read operation for a file does not conflict with an <see cref="AtomicWrite"/> for the same file.
/// </summary>
/// <param name="path">The path of the file that will be read.</param>
/// <example><code>
/// using (new AtomicRead(filePath))
///     return File.ReadAllBytes(filePath);
/// </code></example>
public sealed class AtomicRead([Localizable(false)] string path) : IDisposable
{
    private readonly IDisposable _lock = AtomicWrite.Lock(path ?? throw new ArgumentNullException(nameof(path)));

    /// <inheritdoc/>
    public void Dispose() => _lock.Dispose();
}
