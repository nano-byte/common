/*
 * Copyright 2010-2014 Bastian Eicher, Roland Leopold Walkling
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.IO;
using JetBrains.Annotations;

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
        [NotNull]
        public string Path { get; }

        [ContractAnnotation("null => null; notnull => notnull")]
        public static implicit operator string(TemporaryDirectory dir)
        {
            return dir?.Path;
        }

        /// <summary>
        /// Creates a uniquely named, empty temporary directory on disk.
        /// </summary>
        /// <param name="prefix">A short string the directory name should start with.</param>
        /// <exception cref="IOException">A problem occurred while creating a directory in <see cref="System.IO.Path.GetTempPath"/>.</exception>
        /// <exception cref="UnauthorizedAccessException">Creating a directory in <see cref="System.IO.Path.GetTempPath"/> is not permitted.</exception>
        public TemporaryDirectory([NotNull, Localizable(false)] string prefix)
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
