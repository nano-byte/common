// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Threading;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Ensures that a read operation for a file does not occur while an <see cref="AtomicWrite"/> for the same file is in progress.
    /// </summary>
    /// <example><code>
    /// using (new AtomicRead(filePath))
    ///     return File.ReadAllBytes(filePath);
    /// </code></example>
    public sealed class AtomicRead : IDisposable
    {
        private readonly MutexLock _lock;

        /// <summary>
        /// Prepares an atomic read operation.
        /// </summary>
        /// <param name="path">The path of the file that will be read.</param>
        public AtomicRead([Localizable(false)] string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            _lock = new("atomic-file-" + path.GetHashCode());
        }

        /// <inheritdoc/>
        public void Dispose() => _lock.Dispose();
    }
}
