// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.IO;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Disposable class to create a temporary directory and delete it again when disposed.
    /// </summary>
    public class TemporaryDirectory : IDisposable
    {
        /// <summary>
        /// The fully qualified path of the temporary directory.
        /// </summary>
        public string Path { get; }

#if NETSTANDARD2_1
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("dir")]
#endif
        public static implicit operator string?(TemporaryDirectory dir) => dir?.Path;

        /// <summary>
        /// Creates a uniquely named, empty temporary directory on disk.
        /// </summary>
        /// <param name="prefix">A short string the directory name should start with.</param>
        /// <exception cref="IOException">A problem occurred while creating a directory in <see cref="System.IO.Path.GetTempPath"/>.</exception>
        /// <exception cref="UnauthorizedAccessException">Creating a directory in <see cref="System.IO.Path.GetTempPath"/> is not permitted.</exception>
        public TemporaryDirectory([Localizable(false)] string prefix)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentNullException(nameof(prefix));
            #endregion

            Path = FileUtils.GetTempDirectory(prefix);
        }

        /// <summary>
        /// Deletes the temporary directory.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Do not trigger via GC
            if (!disposing) return;

            if (Directory.Exists(Path))
            {
                try
                {
                    // Write protection might prevent a directory from being deleted (especially on Unixoid systems)
                    FileUtils.DisableWriteProtection(Path);
                }
                #region Error handling
                catch (IOException)
                {}
                catch (UnauthorizedAccessException)
                {}
                #endregion

#if DEBUG
                Directory.Delete(Path, recursive: true);
#else
                try
                {
                    Directory.Delete(Path, recursive: true);
                }
                catch (IOException ex)
                {
                    Log.Warn(ex);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Log.Warn(ex);
                }
#endif
            }
        }
    }
}
