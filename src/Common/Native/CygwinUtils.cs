// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using NanoByte.Common.Collections;
using NanoByte.Common.Properties;
using NanoByte.Common.Streams;

#if NET20
using NanoByte.Common.Values;
#endif

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
        public static bool IsSymlink([Localizable(false)] string path)
            => IsSymlink(path, out _);

        /// <summary>
        /// Checks whether a file is a Cygwin symbolic link (http://cygwin.com/cygwin-ug-net/using.html#pathnames-symlinks).
        /// </summary>
        /// <param name="path">The path of the file to check.</param>
        /// <param name="target">Returns the target the symbolic link points to if it exists.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
        /// <exception cref="IOException">There was an IO problem reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Read access to the file was denied.</exception>
        public static bool IsSymlink(
            [Localizable(false)] string path,
            [NotNullWhen(true)] out string? target)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            var file = new FileInfo(path);

            if (file.Attributes.HasFlag(FileAttributes.System) && file.Length >= SymlinkCookie.Length)
            {
                using var stream = File.OpenRead(path);
                if (stream.Read(SymlinkCookie.Length).SequencedEquals(SymlinkCookie))
                {
                    target = new StreamReader(stream, detectEncodingFromByteOrderMarks: true).ReadToEnd().TrimEnd('\0');
                    return true;
                }
            }

            target = null;
            return false;
        }

        /// <summary>
        /// Creates a new Cygwin symbolic link (http://cygwin.com/cygwin-ug-net/using.html#pathnames-symlinks).
        /// </summary>
        /// <param name="sourcePath">The path of the link to create.</param>
        /// <param name="targetPath">The path of the existing file or directory to point to (relative to <paramref name="sourcePath"/>).</param>
        /// <exception cref="IOException">There was an IO problem writing the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Write access to the file was denied.</exception>
        /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows.</exception>
        public static void CreateSymlink([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)
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
