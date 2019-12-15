// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.IO;

namespace NanoByte.Common.Storage
{
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

        private readonly MutexLock _lock;

        /// <summary>
        /// Prepares an atomic write operation.
        /// </summary>
        /// <param name="path">The file path of the final destination.</param>
        public AtomicWrite([Localizable(false)] string path)
        {
            DestinationPath = path ?? throw new ArgumentNullException(nameof(path));

            // Make sure the containing directory exists
            string directory = Path.GetDirectoryName(Path.GetFullPath(path));
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

            // Prepend random string for temp file name
            WritePath = directory + Path.DirectorySeparatorChar + "temp." + Path.GetRandomFileName() + "." + Path.GetFileName(path);

            _lock = new MutexLock("atomic-file-" + path.GetHashCode());
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
            try
            {
                if (File.Exists(WritePath) && IsCommitted)
                    ExceptionUtils.Retry<IOException>(delegate { FileUtils.Replace(WritePath, DestinationPath); });
            }
            finally
            {
                _lock.Dispose();
                if (File.Exists(WritePath)) File.Delete(WritePath);
            }
        }
    }
}
