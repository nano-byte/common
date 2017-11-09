/*
 * Copyright 2006-2017 Bastian Eicher
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Mono.Unix;
using Mono.Unix.Native;
using NanoByte.Common.Cli;
using NanoByte.Common.Collections;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides helper methods for Unix-specific features of the Mono library.
    /// </summary>
    /// <remarks>
    /// This class has a dependency on <c>Mono.Posix</c>.
    /// Make sure to check <see cref="IsUnix"/> before calling any methods in this class except <see cref="ExpandVariables(string,IDictionary{string,string})"/> or <see cref="ExpandVariables(string,StringDictionary)"/> to avoid missing assembly exceptions.
    /// </remarks>
    public static class UnixUtils
    {
        #region Environment
        private static readonly Regex _varStyle1 = new Regex(@"\${([^}]+)}"), _varStyle2 = new Regex(@"\$([^\$\s\\/-]+)");

        /// <summary>
        /// Expands/substitutes any Unix-style environment variables in the string.
        /// </summary>
        /// <param name="value">The string containing variables to be expanded.</param>
        /// <param name="variables">The list of variables available for expansion.</param>
        [NotNull]
        public static string ExpandVariables([NotNull] string value, [NotNull] IDictionary<string, string> variables)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (variables == null) throw new ArgumentNullException(nameof(variables));

            return _varStyle2.Replace(_varStyle1.Replace(value, x => variables.GetOrDefault(x.Groups[1].Value) ?? ""), x => variables.GetOrDefault(x.Groups[1].Value) ?? "");
        }

        /// <summary>
        /// Expands/substitutes any Unix-style environment variables in the string.
        /// </summary>
        /// <param name="value">The string containing variables to be expanded.</param>
        /// <param name="variables">The list of variables available for expansion.</param>
        [NotNull]
        public static string ExpandVariables([NotNull] string value, [NotNull] StringDictionary variables)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (variables == null) throw new ArgumentNullException(nameof(variables));

            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string key in variables.Keys)
                dictionary[key] = variables[key];

            return ExpandVariables(value, dictionary);
        }
        #endregion

        #region OS
        /// <summary>
        /// <c>true</c> if the current operating system is a Unixoid system (e.g. Linux or MacOS X).
        /// </summary>
        public static bool IsUnix => Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == (PlatformID)128;

        /// <summary>
        /// <c>true</c> if the current operating system is MacOS X.
        /// </summary>
        public static bool IsMacOSX => IsUnix && (OSName == "Darwin") && File.Exists("/System/Library/Frameworks/Carbon.framework");

        /// <summary>
        /// <c>true</c> if there is an X Server running or the current operating system is MacOS X.
        /// </summary>
        public static bool HasGui => IsMacOSX || (IsUnix && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")));

        /// <summary>
        /// The operating system name as reported by the "uname" system call.
        /// </summary>
        [NotNull]
        public static string OSName
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                Syscall.uname(out var buffer);
                return buffer.sysname;
            }
        }

        /// <summary>
        /// The CPU type as reported by the "uname" system call (after applying some normalization).
        /// </summary>
        [NotNull]
        public static string CpuType
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                Syscall.uname(out var buffer);
                string cpuType = buffer.machine;

                // Normalize names
                switch (cpuType)
                {
                    case "x86":
                        return "i386";
                    case "amd64":
                        return "x86_64";
                    case "Power Macintosh":
                        return "ppc";
                    case "i86pc":
                        return "i686";
                    default:
                        return cpuType;
                }
            }
        }
        #endregion

        #region Links
        /// <summary>
        /// Creates a new Unix symbolic link to a file or directory.
        /// </summary>
        /// <param name="sourcePath">The path of the link to create.</param>
        /// <param name="targetPath">The path of the existing file or directory to point to (relative to <paramref name="sourcePath"/>).</param>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void CreateSymlink([NotNull, Localizable(false)] string sourcePath, [NotNull, Localizable(false)] string targetPath)
            => new UnixSymbolicLinkInfo(sourcePath ?? throw new ArgumentNullException(nameof(sourcePath)))
                .CreateSymbolicLinkTo(targetPath ?? throw new ArgumentNullException(nameof(targetPath)));

        /// <summary>
        /// Creates a new Unix hard link between two files.
        /// </summary>
        /// <param name="sourcePath">The path of the link to create.</param>
        /// <param name="targetPath">The absolute path of the existing file to point to.</param>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void CreateHardlink([NotNull, Localizable(false)] string sourcePath, [NotNull, Localizable(false)] string targetPath)
            => new UnixFileInfo(targetPath ?? throw new ArgumentNullException(nameof(sourcePath)))
                .CreateLink(sourcePath ?? throw new ArgumentNullException(nameof(targetPath)));

        /// <summary>
        /// Determines whether to files are hardlinked.
        /// </summary>
        /// <param name="path1">The path of the first file.</param>
        /// <param name="path2">The path of the second file.</param>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool AreHardlinked([NotNull, Localizable(false)] string path1, [NotNull, Localizable(false)] string path2) =>
            UnixFileSystemInfo.GetFileSystemEntry(path1 ?? throw new ArgumentNullException(nameof(path1))).Inode ==
            UnixFileSystemInfo.GetFileSystemEntry(path2 ?? throw new ArgumentNullException(nameof(path2))).Inode;

        /// <summary>
        /// Renames a file. Atomically replaces the destination if present.
        /// </summary>
        /// <param name="source">The path of the file to rename.</param>
        /// <param name="destination">The new path of the file. Must reside on the same file system as <paramref name="source"/>.</param>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Rename([NotNull, Localizable(false)] string source, [NotNull, Localizable(false)] string destination)
        {
            if (Stdlib.rename(
                    source ?? throw new ArgumentNullException(nameof(source)),
                    destination ?? throw new ArgumentNullException(nameof(destination))) != 0)
                throw new UnixIOException(Stdlib.GetLastError());
        }
        #endregion

        #region File type
        /// <summary>
        /// Checks whether a file is a regular file (i.e. not a device file, symbolic link, etc.).
        /// </summary>
        /// <returns><c>true</c> if <paramref name="path"/> points to a regular file; <c>false</c> otherwise.</returns>
        /// <remarks>Will return <c>false</c> for non-existing files.</remarks>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static bool IsRegularFile([NotNull, Localizable(false)] string path)
            => UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path))).IsRegularFile;

        /// <summary>
        /// Checks whether a file is a Unix symbolic link.
        /// </summary>
        /// <param name="path">The path of the file to check.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
        /// <remarks>Will return <c>false</c> for non-existing files.</remarks>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static bool IsSymlink([NotNull, Localizable(false)] string path)
            => UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path))).IsSymbolicLink;

        /// <summary>
        /// Checks whether a file is a Unix symbolic link.
        /// </summary>
        /// <param name="path">The path of the file to check.</param>
        /// <param name="target">Returns the target the symbolic link points to if it exists.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static bool IsSymlink([NotNull, Localizable(false)] string path, out string target)
        {
            bool result = IsSymlink(path ?? throw new ArgumentNullException(nameof(path)));
            if (result)
            {
                var symlinkInfo = new UnixSymbolicLinkInfo(path);
                target = symlinkInfo.ContentsPath;
            }
            else target = null;
            return result;
        }
        #endregion

        #region Permissions
        /// <summary>A combination of bit flags to grant everyone writing permissions.</summary>
        private const FileAccessPermissions AllWritePermission = FileAccessPermissions.UserWrite | FileAccessPermissions.GroupWrite | FileAccessPermissions.OtherWrite;

        /// <summary>
        /// Removes write permissions for everyone on a filesystem object (file or directory).
        /// </summary>
        /// <param name="path">The filesystem object (file or directory) to make read-only.</param>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static void MakeReadOnly([NotNull, Localizable(false)] string path)
        {
            var fileSysInfo = UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path)));
            fileSysInfo.FileAccessPermissions &= ~AllWritePermission;
        }

        /// <summary>
        /// Sets write permissions for the owner on a filesystem object (file or directory).
        /// </summary>
        /// <param name="path">The filesystem object (file or directory) to make writeable by the owner.</param>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static void MakeWritable([NotNull, Localizable(false)] string path)
        {
            var fileSysInfo = UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path)));
            fileSysInfo.FileAccessPermissions |= FileAccessPermissions.UserWrite;
        }

        /// <summary>A combination of bit flags to grant everyone executing permissions.</summary>
        private const FileAccessPermissions AllExecutePermission = FileAccessPermissions.UserExecute | FileAccessPermissions.GroupExecute | FileAccessPermissions.OtherExecute;

        /// <summary>
        /// Checks whether a file is marked as Unix-executable.
        /// </summary>
        /// <param name="path">The file to check for executable rights.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to an executable; <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <remarks>Will return <c>false</c> for non-existing files.</remarks>
        public static bool IsExecutable([NotNull, Localizable(false)] string path)
        {
            // Check if any execution rights are set
            var fileInfo = UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path)));
            return (fileInfo.FileAccessPermissions & AllExecutePermission) > 0;
        }

        /// <summary>
        /// Marks a file as Unix-executable or not Unix-executable.
        /// </summary>
        /// <param name="path">The file to mark as executable or not executable.</param>
        /// <param name="executable"><c>true</c> to mark the file as executable, <c>true</c> to mark it as not executable.</param>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static void SetExecutable([NotNull, Localizable(false)] string path, bool executable)
        {
            var fileInfo = UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path)));
            if (executable) fileInfo.FileAccessPermissions |= AllExecutePermission; // Set all execution rights
            else fileInfo.FileAccessPermissions &= ~AllExecutePermission; // Unset all execution rights
        }
        #endregion

        #region Extended file attributes
        /// <summary>
        /// Gets an extended file attribute.
        /// </summary>
        /// <param name="path">The path of the file to read the attribute from.</param>
        /// <param name="name">The name of the attribute to read.</param>
        /// <returns>The contents of the attribute as a byte array; <c>null</c> if there was a problem reading the file.</returns>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static byte[] GetXattr([NotNull, Localizable(false)] string path, [NotNull, Localizable(false)] string name)
            => Syscall.getxattr(
                   path ?? throw new ArgumentNullException(nameof(path)),
                   name ?? throw new ArgumentNullException(nameof(name)),
                   out var data) == -1
                ? null
                : data;

        /// <summary>
        /// Sets an extended file attribute.
        /// </summary>
        /// <param name="path">The path of the file to set the attribute for.</param>
        /// <param name="name">The name of the attribute to set.</param>
        /// <param name="data">The data to write to the attribute.</param>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SetXattr([NotNull, Localizable(false)] string path, [NotNull, Localizable(false)] string name, [NotNull] byte[] data)
        {
            if (Syscall.setxattr(
                    path ?? throw new ArgumentNullException(nameof(path)),
                    name ?? throw new ArgumentNullException(nameof(name)),
                    data) == -1)
                throw new UnixIOException(Stdlib.GetLastError());
        }
        #endregion

        #region Mount point
        private class Stat : CliAppControl
        {
            #region Singleton
            public static readonly Stat Instance = new Stat();

            private Stat()
            {}
            #endregion

            protected override string AppBinary => "stat";

            protected override string HandleStderr(string line) => throw new IOException(line);

            public string FileSystem(string path) => Execute("--file-system", "--printf", "%T", path).TrimEnd('\n');

            public string MountPoint(string path) => Execute("--printf", "%m", path).TrimEnd('\n');
        }

        /// <summary>
        /// Determines the file system type a file or directory is stored on.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>The name of the file system in fstab format (e.g. ext3 or ntfs-3g).</returns>
        /// <remarks>Only works on Linux, not on other Unixes (e.g. MacOS X).</remarks>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        [NotNull]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetFileSystem([NotNull, Localizable(false)] string path)
        {
            string fileSystem = Stat.Instance.FileSystem(path ?? throw new ArgumentNullException(nameof(path)));
            if (fileSystem == "fuseblk")
            { // FUSE mounts need to be looked up in /etc/fstab to determine actualy file system
                var fstabData = Syscall.getfsfile(Stat.Instance.MountPoint(path));
                if (fstabData != null) return fstabData.fs_vfstype;
            }
            return fileSystem;
        }
        #endregion
    }
}
