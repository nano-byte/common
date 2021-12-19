// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Disposable class to create a temporary file and delete it again when disposed.
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
        /// <exception cref="IOException">A problem occurred while creating a file in <see cref="System.IO.Path.GetTempPath"/>.</exception>
        /// <exception cref="UnauthorizedAccessException">Creating a file in <see cref="System.IO.Path.GetTempPath"/> is not permitted.</exception>
        public TemporaryFile([Localizable(false)] string prefix)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentNullException(nameof(prefix));
            #endregion

            Path = FileUtils.GetTempFile(prefix);
        }

        /// <summary>
        /// Deletes the temporary file.
        /// </summary>
        public virtual void Dispose()
        {
            if (File.Exists(Path)) File.Delete(Path);
        }
    }
}
