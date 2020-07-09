// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Mono.Unix;
using Mono.Unix.Native;
using NanoByte.Common.Cli;

#if NETSTANDARD
using System.Runtime.InteropServices;
#endif

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides helper methods for Unix-specific features of the Mono library.
    /// </summary>
    /// <remarks>
    /// This class has a dependency on <c>Mono.Posix</c>.
    /// Make sure to check <see cref="IsUnix"/> before calling any methods in this class to avoid exceptions.
    /// </remarks>
    public static class UnixUtils
    {
        #region OS
        /// <summary>
        /// <c>true</c> if the current operating system is a Unixoid system (e.g. Linux or MacOS X).
        /// </summary>
        public static bool IsUnix
#if NETSTANDARD
            => RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || IsMacOSX;
#else
            => Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == (PlatformID)128;
#endif

        /// <summary>
        /// <c>true</c> if the current operating system is MacOS X.
        /// </summary>
        public static bool IsMacOSX
#if NETSTANDARD
            => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#else
            => IsUnix && (OSName == "Darwin") && File.Exists("/System/Library/Frameworks/Carbon.framework");
#endif

        /// <summary>
        /// <c>true</c> if there is an X Server running or the current operating system is MacOS X.
        /// </summary>
        public static bool HasGui => IsMacOSX || (IsUnix && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")));

        /// <summary>
        /// The operating system name as reported by the "uname" system call.
        /// </summary>
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
        public static string CpuType
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                Syscall.uname(out var buffer);
                string cpuType = buffer.machine;

                // Normalize names
                return cpuType switch
                {
                    "x86" => "i386",
                    "amd64" => "x86_64",
                    "Power Macintosh" => "ppc",
                    "i86pc" => "i686",
                    _ => cpuType
                };
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
        public static void CreateSymlink([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)
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
        public static void CreateHardlink([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)
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
        public static bool AreHardlinked([Localizable(false)] string path1, [Localizable(false)] string path2) =>
            UnixFileSystemInfo.GetFileSystemEntry(path1 ?? throw new ArgumentNullException(nameof(path1))).Inode ==
            UnixFileSystemInfo.GetFileSystemEntry(path2 ?? throw new ArgumentNullException(nameof(path2))).Inode;

        /// <summary>
        /// Renames a file. Atomically replaces the destination if present.
        /// </summary>
        /// <param name="source">The path of the file to rename.</param>
        /// <param name="destination">The new path of the file. Must reside on the same file system as <paramref name="source"/>.</param>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Rename([Localizable(false)] string source, [Localizable(false)] string destination)
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
        public static bool IsRegularFile([Localizable(false)] string path)
            => UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path))).IsRegularFile;

        /// <summary>
        /// Checks whether a file is a Unix symbolic link.
        /// </summary>
        /// <param name="path">The path of the file to check.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
        /// <remarks>Will return <c>false</c> for non-existing files.</remarks>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static bool IsSymlink([Localizable(false)] string path)
            => UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path))).IsSymbolicLink;

        /// <summary>
        /// Checks whether a file is a Unix symbolic link.
        /// </summary>
        /// <param name="path">The path of the file to check.</param>
        /// <param name="target">Returns the target the symbolic link points to if it exists.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static bool IsSymlink(
            [Localizable(false)] string path,
#if NETSTANDARD2_1
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            out string? target)
        {
            if (IsSymlink(path ?? throw new ArgumentNullException(nameof(path))))
            {
                var symlinkInfo = new UnixSymbolicLinkInfo(path);
                target = symlinkInfo.ContentsPath;
                return true;
            }
            else
            {
                target = null;
                return false;
            }
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
        public static void MakeReadOnly([Localizable(false)] string path)
        {
            var fileSysInfo = UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path)));
            fileSysInfo.FileAccessPermissions &= ~AllWritePermission;
        }

        /// <summary>
        /// Sets write permissions for the owner on a filesystem object (file or directory).
        /// </summary>
        /// <param name="path">The filesystem object (file or directory) to make writable by the owner.</param>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="UnixIOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static void MakeWritable([Localizable(false)] string path)
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
        public static bool IsExecutable([Localizable(false)] string path)
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
        public static void SetExecutable([Localizable(false)] string path, bool executable)
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
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static byte[]? GetXattr([Localizable(false)] string path, [Localizable(false)] string name)
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
        public static void SetXattr([Localizable(false)] string path, [Localizable(false)] string name, byte[] data)
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

            private Stat() {}
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
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetFileSystem([Localizable(false)] string path)
        {
            string fileSystem = Stat.Instance.FileSystem(path ?? throw new ArgumentNullException(nameof(path)));
            if (fileSystem == "fuseblk")
            { // FUSE mounts need to be looked up in /etc/fstab to determine actual file system
                var fstabData = Syscall.getfsfile(Stat.Instance.MountPoint(path));
                if (fstabData != null) return fstabData.fs_vfstype;
            }
            return fileSystem;
        }
        #endregion
    }
}
