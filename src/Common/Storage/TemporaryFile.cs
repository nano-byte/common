// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

/// <summary>
/// Represents a temporary file that is automatically deleted when the object is disposed.
/// </summary>
public class TemporaryFile : IDisposable
{
    /// <summary>
    /// The fully qualified path of the temporary file.
    /// </summary>
    public string Path { get; }

    public static implicit operator string(TemporaryFile file) => file.Path;

    /// <summary>
    /// Creates a uniquely named, empty temporary file on disk.
    /// </summary>
    /// <param name="prefix">A short string the directory name should start with.</param>
    /// <param name="parentDirectory">The path of the directory the file should be created in. Leave <c>null</c> to use the default temp directory.</param>
    /// <exception cref="IOException">A problem occurred while creating the temporary file.</exception>
    /// <exception cref="UnauthorizedAccessException">Creating a file in <see cref="System.IO.Path.GetTempPath"/> is not permitted.</exception>
    public TemporaryFile([Localizable(false)] string prefix, [Localizable(false)] string? parentDirectory = null)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(prefix)) throw new ArgumentNullException(nameof(prefix));
        #endregion

        Path = System.IO.Path.Combine(parentDirectory ?? System.IO.Path.GetTempPath(), prefix + '-' +  System.IO.Path.GetRandomFileName());
        FileUtils.Touch(Path);
    }

    /// <summary>
    /// Deletes the temporary file.
    /// </summary>
    public virtual void Dispose()
    {
        if (!File.Exists(Path)) return;

        try
        {
            File.Delete(Path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Log.Warn($"Failed to delete temporary file: {Path}", ex);
        }
    }
}
