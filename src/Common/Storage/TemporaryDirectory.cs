// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

/// <summary>
/// Represents a temporary directory that is automatically deleted when the object is disposed.
/// </summary>
public class TemporaryDirectory : IDisposable
{
    /// <summary>
    /// The fully qualified path of the temporary directory.
    /// </summary>
    public string Path { get; }

    public static implicit operator string(TemporaryDirectory dir) => dir.Path;

    /// <summary>
    /// Creates a uniquely named, empty temporary directory on disk.
    /// </summary>
    /// <param name="prefix">A short string the directory name should start with.</param>
    /// <param name="parentDirectory">The path of the parent directory the new directory should be created in. Leave <c>null</c> to use the default temp directory.</param>
    /// <exception cref="IOException">A problem occurred while creating the temporary directory.</exception>
    /// <exception cref="UnauthorizedAccessException">Creating a directory in <see cref="System.IO.Path.GetTempPath"/> is not permitted.</exception>
    public TemporaryDirectory([Localizable(false)] string prefix, [Localizable(false)] string? parentDirectory = null)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(prefix)) throw new ArgumentNullException(nameof(prefix));
        #endregion

        Path = System.IO.Path.Combine(parentDirectory ?? System.IO.Path.GetTempPath(), prefix + '-' +  System.IO.Path.GetRandomFileName());
        Directory.CreateDirectory(Path);
    }

    /// <summary>
    /// Deletes the temporary directory.
    /// </summary>
    public virtual void Dispose()
    {
        if (Directory.Exists(Path))
        {
            try
            {
                // Write protection might prevent a directory from being deleted (especially on Unixoid systems)
                FileUtils.DisableWriteProtection(Path);
            }
            catch (IOException)
            {}
            catch (UnauthorizedAccessException)
            {}

            try
            {
                Directory.Delete(Path, recursive: true);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Log.Warn("Failed to delete temporary directory: " + Path, ex);
            }
        }
    }
}
