/*
 * Copyright 2006-2015 Bastian Eicher
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
using System.Text;
using JetBrains.Annotations;
using NanoByte.Common.Collections;
using NanoByte.Common.Properties;
using NanoByte.Common.Streams;
using NanoByte.Common.Values;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides access to Cygwin-related filesystem features. Cygwin provides Unix-like functionality on Windows systems.
    /// </summary>
    public static class CygwinUtils
    {
        /// <summary>
        /// Byte sequence used to mark the start of a Cygwin symlink file.
        /// </summary>
        internal static readonly byte[] SymlinkCookie = Encoding.ASCII.GetBytes("!<symlink>");

        /// <summary>
        /// Checks whether a file is a Cygwin symbolic link (http://cygwin.com/cygwin-ug-net/using.html#pathnames-symlinks).
        /// </summary>
        /// <param name="path">The path of the file to check.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
        /// <exception cref="IOException">There was an IO problem reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Read access to the file was denied.</exception>
        public static bool IsSymlink([NotNull, Localizable(false)] string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            if (!HasSystemAttribute(path)) return false;

            using (var stream = File.OpenRead(path))
            {
                var header = new byte[SymlinkCookie.Length];
                stream.Read(header, 0, SymlinkCookie.Length);
                return header.SequencedEquals(SymlinkCookie);
            }
        }

        /// <summary>
        /// Checks whether a file is a Cygwin symbolic link (http://cygwin.com/cygwin-ug-net/using.html#pathnames-symlinks).
        /// </summary>
        /// <param name="path">The path of the file to check.</param>
        /// <param name="target">Returns the target the symbolic link points to if it exists.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
        /// <exception cref="IOException">There was an IO problem reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Read access to the file was denied.</exception>
        public static bool IsSymlink([NotNull, Localizable(false)] string path, out string target)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            if (!HasSystemAttribute(path))
            {
                target = null;
                return false;
            }

            using (var stream = File.OpenRead(path))
            {
                var header = new byte[SymlinkCookie.Length];
                stream.Read(header, 0, SymlinkCookie.Length);
                if (header.SequencedEquals(SymlinkCookie))
                {
                    target = new StreamReader(stream, detectEncodingFromByteOrderMarks: true)
                        .ReadToEnd().TrimEnd('\0');
                    return true;
                }
                else
                {
                    target = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks whether a file has the <see cref="FileAttributes.System"/> attribute set.
        /// Always <c>true</c> on non-Windows systems since they do not expose this attribute, so we assume it might be set.
        /// </summary>
        private static bool HasSystemAttribute(string path)
            => !WindowsUtils.IsWindows || File.GetAttributes(path).HasFlag(FileAttributes.System);

        /// <summary>
        /// Creates a new Cygwin symbolic link (http://cygwin.com/cygwin-ug-net/using.html#pathnames-symlinks).
        /// </summary>
        /// <param name="sourcePath">The path of the link to create.</param>
        /// <param name="targetPath">The path of the existing file or directory to point to (relative to <paramref name="sourcePath"/>).</param>
        /// <exception cref="IOException">There was an IO problem writing the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Write access to the file was denied.</exception>
        /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows.</exception>
        public static void CreateSymlink([NotNull, Localizable(false)] string sourcePath, [NotNull, Localizable(false)] string targetPath)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentNullException(nameof(sourcePath));
            if (string.IsNullOrEmpty(targetPath)) throw new ArgumentNullException(nameof(targetPath));
            #endregion

            if (!WindowsUtils.IsWindows) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

            using (var stream = File.Create(sourcePath))
            {
                stream.Write(SymlinkCookie);
                stream.Write(Encoding.Unicode.GetPreamble());
                stream.Write(Encoding.Unicode.GetBytes(targetPath + '\0'));
            }

            File.SetAttributes(sourcePath, FileAttributes.System);
        }
    }
}
