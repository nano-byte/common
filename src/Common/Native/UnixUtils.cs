// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

#if NET
using Mono.Unix;
using Mono.Unix.Native;
#endif

#if !NET20 && !NET40 && !NET
using System.Runtime.InteropServices;
#endif

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides helper methods for Unix-specific features of the Mono library.
    /// </summary>
    /// <remarks>
    /// Make sure to check <see cref="IsUnix"/> before calling any methods in this class to avoid exceptions.
    /// </remarks>
    [SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"), SupportedOSPlatform("macos")]
    public static class UnixUtils
    {
        #region OS
        /// <summary>
        /// <c>true</c> if the current operating system is a Unixoid system (e.g. Linux or MacOS X).
        /// </summary>
        [SupportedOSPlatformGuard("linux"), SupportedOSPlatformGuard("freebsd"), SupportedOSPlatformGuard("macos")]
        public static bool IsUnix => IsLinux || IsFreeBSD || IsMacOSX;

        /// <summary>
        /// <c>true</c> if the current operating system is Linux.
        /// </summary>
        [SupportedOSPlatformGuard("linux")]
        public static bool IsLinux
#if NET
            => OperatingSystem.IsLinux();
#elif NET20 || NET40
            => Environment.OSVersion.Platform == PlatformID.Unix && OSName == "Linux";
#else
            => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#endif

        /// <summary>
        /// <c>true</c> if the current operating system is FreeBSD.
        /// </summary>
        [SupportedOSPlatformGuard("freebsd")]
        public static bool IsFreeBSD
#if NET
            => OperatingSystem.IsFreeBSD();
#elif NET20 || NET40
            => Environment.OSVersion.Platform == PlatformID.Unix && OSName == "FreeBSD";
#else
            => false;
#endif

        /// <summary>
        /// <c>true</c> if the current operating system is MacOS X.
        /// </summary>
        [SupportedOSPlatformGuard("macos")]
        public static bool IsMacOSX
#if NET
            => OperatingSystem.IsMacOS();
#elif NET20 || NET40
            => Environment.OSVersion.Platform == PlatformID.Unix && OSName == "Darwin";
#else
            => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#endif

        /// <summary>
        /// <c>true</c> if there is an X Server running or the current operating system is MacOS X.
        /// </summary>
        [SupportedOSPlatformGuard("linux"), SupportedOSPlatformGuard("freebsd"), SupportedOSPlatformGuard("macos")]
        public static bool HasGui
            => IsUnix && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY"))
            || IsMacOSX;

        /// <summary>
        /// The operating system name as reported by the "uname" system call.
        /// </summary>
        public static string OSName
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
#if NET
                Syscall.uname(out var buffer);
                return buffer.sysname;
#else
                throw new PlatformNotSupportedException();
#endif
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
#if NET
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
#else
                throw new PlatformNotSupportedException();
#endif
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
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void CreateSymlink([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)
#if NET
            => new UnixSymbolicLinkInfo(sourcePath ?? throw new ArgumentNullException(nameof(sourcePath)))
               .CreateSymbolicLinkTo(targetPath ?? throw new ArgumentNullException(nameof(targetPath)));
#else
            => throw new PlatformNotSupportedException();
#endif

        /// <summary>
        /// Creates a new Unix hard link between two files.
        /// </summary>
        /// <param name="sourcePath">The path of the link to create.</param>
        /// <param name="targetPath">The absolute path of the existing file to point to.</param>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void CreateHardlink([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)
#if NET
            => new UnixFileInfo(targetPath ?? throw new ArgumentNullException(nameof(targetPath)))
               .CreateLink(sourcePath ?? throw new ArgumentNullException(nameof(sourcePath)));
#else
            => throw new PlatformNotSupportedException();
#endif

        /// <summary>
        /// Returns the Inode ID of a file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static long GetInode([Localizable(false)] string path)
#if NET
            => UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path))).Inode;
#else
            => throw new PlatformNotSupportedException();
#endif

        /// <summary>
        /// Renames a file. Atomically replaces the destination if present.
        /// </summary>
        /// <param name="source">The path of the file to rename.</param>
        /// <param name="destination">The new path of the file. Must reside on the same file system as <paramref name="source"/>.</param>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Rename([Localizable(false)] string source, [Localizable(false)] string destination)
        {
#if NET
            if (Stdlib.rename(
                    source ?? throw new ArgumentNullException(nameof(source)),
                    destination ?? throw new ArgumentNullException(nameof(destination))) != 0)
                throw new UnixIOException(Stdlib.GetLastError());
#else
           throw new PlatformNotSupportedException();
#endif
        }
        #endregion

        #region File type
        /// <summary>
        /// Checks whether a file is a regular file (i.e. not a device file, symbolic link, etc.).
        /// </summary>
        /// <returns><c>true</c> if <paramref name="path"/> points to a regular file; <c>false</c> otherwise.</returns>
        /// <remarks>Will return <c>false</c> for non-existing files.</remarks>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static bool IsRegularFile([Localizable(false)] string path)
#if NET
            => UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path))).IsRegularFile;
#else
            => throw new PlatformNotSupportedException();
#endif

        /// <summary>
        /// Checks whether a file is a Unix symbolic link.
        /// </summary>
        /// <param name="path">The path of the file to check.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
        /// <remarks>Will return <c>false</c> for non-existing files.</remarks>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static bool IsSymlink([Localizable(false)] string path)
#if NET
            => UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path))).IsSymbolicLink;
#else
            => throw new PlatformNotSupportedException();
#endif

        /// <summary>
        /// Checks whether a file is a Unix symbolic link.
        /// </summary>
        /// <param name="path">The path of the file to check.</param>
        /// <param name="target">Returns the target the symbolic link points to if it exists.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static bool IsSymlink(
            [Localizable(false)] string path,
            [MaybeNullWhen(false)] out string target)
        {
#if NET
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
#else
            throw new PlatformNotSupportedException();
#endif
        }
        #endregion

        #region Permissions
#if NET
        /// <summary>A combination of bit flags to grant everyone writing permissions.</summary>
        private const FileAccessPermissions AllWritePermission = FileAccessPermissions.UserWrite | FileAccessPermissions.GroupWrite | FileAccessPermissions.OtherWrite;
#endif

        /// <summary>
        /// Removes write permissions for everyone on a filesystem object (file or directory).
        /// </summary>
        /// <param name="path">The filesystem object (file or directory) to make read-only.</param>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static void MakeReadOnly([Localizable(false)] string path)
        {
#if NET
            var fileSysInfo = UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path)));
            fileSysInfo.FileAccessPermissions &= ~AllWritePermission;
#else
            throw new PlatformNotSupportedException();
#endif
        }

        /// <summary>
        /// Sets write permissions for the owner on a filesystem object (file or directory).
        /// </summary>
        /// <param name="path">The filesystem object (file or directory) to make writable by the owner.</param>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static void MakeWritable([Localizable(false)] string path)
        {
#if NET
            var fileSysInfo = UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path)));
            fileSysInfo.FileAccessPermissions |= FileAccessPermissions.UserWrite;
#else
            throw new PlatformNotSupportedException();
#endif
        }

#if NET
        /// <summary>A combination of bit flags to grant everyone executing permissions.</summary>
        private const FileAccessPermissions AllExecutePermission = FileAccessPermissions.UserExecute | FileAccessPermissions.GroupExecute | FileAccessPermissions.OtherExecute;
#endif

        /// <summary>
        /// Checks whether a file is marked as Unix-executable.
        /// </summary>
        /// <param name="path">The file to check for executable rights.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to an executable; <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <remarks>Will return <c>false</c> for non-existing files.</remarks>
        public static bool IsExecutable([Localizable(false)] string path)
        {
#if NET
            // Check if any execution rights are set
            var fileInfo = UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path)));
            return (fileInfo.FileAccessPermissions & AllExecutePermission) > 0;
#else
            throw new PlatformNotSupportedException();
#endif
        }

        /// <summary>
        /// Marks a file as Unix-executable or not Unix-executable.
        /// </summary>
        /// <param name="path">The file to mark as executable or not executable.</param>
        /// <param name="executable"><c>true</c> to mark the file as executable, <c>true</c> to mark it as not executable.</param>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        public static void SetExecutable([Localizable(false)] string path, bool executable)
        {
#if NET
            var fileInfo = UnixFileSystemInfo.GetFileSystemEntry(path ?? throw new ArgumentNullException(nameof(path)));
            if (executable) fileInfo.FileAccessPermissions |= AllExecutePermission; // Set all execution rights
            else fileInfo.FileAccessPermissions &= ~AllExecutePermission; // Unset all execution rights
#else
            throw new PlatformNotSupportedException();
#endif
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
#if NET
            => Syscall.getxattr(
                   path ?? throw new ArgumentNullException(nameof(path)),
                   name ?? throw new ArgumentNullException(nameof(name)),
                   out var data) == -1
                ? null
                : data;
#else
            => throw new PlatformNotSupportedException();
#endif

        /// <summary>
        /// Sets an extended file attribute.
        /// </summary>
        /// <param name="path">The path of the file to set the attribute for.</param>
        /// <param name="name">The name of the attribute to set.</param>
        /// <param name="data">The data to write to the attribute.</param>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SetXattr([Localizable(false)] string path, [Localizable(false)] string name, byte[] data)
        {
#if NET
            if (Syscall.setxattr(
                    path ?? throw new ArgumentNullException(nameof(path)),
                    name ?? throw new ArgumentNullException(nameof(name)),
                    data) == -1)
                throw new UnixIOException(Stdlib.GetLastError());
#else
            throw new PlatformNotSupportedException();
#endif
        }
        #endregion

        #region Mount point
#if NET
        private static readonly ProcessLauncher _stat = new("stat");
#endif

        /// <summary>
        /// Determines the file system type a file or directory is stored on.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>The name of the file system in fstab format (e.g. ext3 or ntfs-3g); <c>null</c> if unable to determine.</returns>
        /// <remarks>Only works on Linux, not on other Unixes (e.g. MacOS X).</remarks>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string? GetFileSystem([Localizable(false)] string path)
        {
            if (!IsLinux) return null;

#if NET
            string fileSystem = _stat.RunAndCapture("--file-system", "--printf", "%T", path).TrimEnd('\n');
            if (fileSystem == "fuseblk")
            { // FUSE mounts need to be looked up in /etc/fstab to determine actual file system
                if (Syscall.getfsfile(_stat.RunAndCapture("--printf", "%m", path).TrimEnd('\n')) is {} fstabData)
                    return fstabData.fs_vfstype;
            }
            return fileSystem;
#else
            throw new PlatformNotSupportedException();
#endif
        }
        #endregion
    }
}
