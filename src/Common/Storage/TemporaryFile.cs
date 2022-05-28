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
    /// <exception cref="IOException">A problem occurred while creating the temporary file.</exception>
    /// <exception cref="UnauthorizedAccessException">Creating a file in <see cref="System.IO.Path.GetTempPath"/> is not permitted.</exception>
    public TemporaryFile([Localizable(false)] string prefix)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(prefix)) throw new ArgumentNullException(nameof(prefix));
        #endregion

        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), prefix + '-' +  System.IO.Path.GetRandomFileName());
        FileUtils.Touch(Path);
    }

    /// <summary>
    /// Deletes the temporary file.
    /// </summary>
    public virtual void Dispose()
    {
        if (File.Exists(Path))
        {
            try
            {
                File.Delete(Path);
            }
            catch (IOException ex)
            {
                Log.Warn(ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Warn(ex);
            }
        }
    }
}
