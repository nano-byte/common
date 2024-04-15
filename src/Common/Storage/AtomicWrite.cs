// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Threading;

namespace NanoByte.Common.Storage;

/// <summary>
/// Provides a temporary path to write to and atomically inserts it at the destination location on disposal (if <see cref="Commit"/> was called).
/// </summary>
/// <example><code>
/// using (var atomic = new AtomicWrite(filePath))
/// {
///     File.WriteAllBytes(atomic.WritePath, fileData);
///     atomic.Commit();
/// }
/// </code></example>
/// <seealso cref="AtomicRead"/>
[MustDisposeResource]
public sealed class AtomicWrite : IDisposable
{
    /// <summary>
    /// The file path of the final destination.
    /// </summary>
    public string DestinationPath { get; }

    /// <summary>
    /// The temporary file path to write to.
    /// </summary>
    public string WritePath { get; }

    /// <summary>
    /// <c>true</c> if <see cref="Commit"/> has been called.
    /// </summary>
    public bool IsCommitted { get; private set; }

    /// <summary>
    /// Prepares an atomic write operation.
    /// </summary>
    /// <param name="path">The file path of the final destination.</param>
    public AtomicWrite([Localizable(false)] string path)
    {
        DestinationPath = path ?? throw new ArgumentNullException(nameof(path));

        // Make sure the containing directory exists
        string? directory = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

        // Prepend random string for temp file name
        WritePath = $"{directory}{Path.DirectorySeparatorChar}temp.{Path.GetRandomFileName()}.{Path.GetFileName(path)}";
    }

    /// <summary>
    /// Allows the new file to be deployed upon <see cref="Dispose"/>.
    /// </summary>
    public void Commit() => IsCommitted = true;

    /// <summary>
    /// Replaces <see cref="DestinationPath"/> with the contents of <see cref="WritePath"/>.
    /// </summary>
    public void Dispose()
    {
        if (!File.Exists(WritePath)) return;

        try
        {
            if (!IsCommitted) return;

            using (Lock(DestinationPath))
                FileUtils.Replace(WritePath, DestinationPath);
        }
        finally
        {
            File.Delete(WritePath);
        }
    }

    [MustDisposeResource]
    internal static IDisposable Lock(string path)
        => new MutexLock("atomic-file-" + path.GetHashCode());
}

