// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

using System.Runtime.InteropServices;

#if NET
using System.Text.RegularExpressions;
#endif

namespace NanoByte.Common.Native;

/// <summary>
/// Provides helper methods for Unix-specific features.
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
                string sysName = LibC.Uname().SysName;
                return string.IsNullOrEmpty(sysName) ? "unknown" : sysName;
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
                string cpuType = LibC.Uname().Machine;

                // Normalize names
                return cpuType switch
                {
                    "x86" => "i386",
                    "amd64" => "x86_64",
                    "Power Macintosh" => "ppc",
                    "i86pc" => "i686",
                    "" => "unknown",
                    _ => cpuType
                };
#else
            throw new PlatformNotSupportedException();
#endif
        }
    }
    #endregion

    #region Exec
#if NET
    /// <summary>
    /// Replaces the currently running process with a new one.
    /// </summary>
    /// <param name="path">The path or file name (resolved via <c>PATH</c> if it contains no slash) of the executable to run.</param>
    /// <param name="arguments">The command-line arguments to pass to the executable.</param>
    /// <remarks>Inherits the current process' environment variables and working directory. Does not return on success; set any desired environment variables and <see cref="Environment.CurrentDirectory"/> beforehand.</remarks>
    /// <exception cref="IOException">The call failed (e.g., the executable was not found).</exception>
    /// <exception cref="UnauthorizedAccessException">The executable could not be run because of insufficient rights.</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    [DoesNotReturn]
    public static unsafe void Exec([Localizable(false)] string path, [Localizable(false)] params string[] arguments)
    {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));

            // argv must be null-terminated
            var argv = new nint[arguments.Length + 2];
            Exception error;
            try
            {
                argv[0] = Marshal.StringToCoTaskMemUTF8(path);
                for (int i = 0; i < arguments.Length; i++)
                    argv[i + 1] = Marshal.StringToCoTaskMemUTF8(arguments[i]);

                fixed (nint* argvPtr = argv)
                    LibC.execvp(path, argvPtr); // Only returns on error
                error = LibC.GetLastError(path);
            }
            finally
            {
                foreach (nint arg in argv)
                    Marshal.FreeCoTaskMem(arg);
            }
            throw error;
    }
#endif
    #endregion

    #region Links
    /// <summary>
    /// Creates a new Unix symbolic link to a file or directory.
    /// </summary>
    /// <param name="sourcePath">The path of the link to create.</param>
    /// <param name="targetPath">The path of the existing file or directory to point to (relative to <paramref name="sourcePath"/>).</param>
    /// <exception cref="IOException">The underlying Unix subsystem failed to process the request.</exception>
    /// <exception cref="UnauthorizedAccessException">The underlying Unix subsystem denied the request because of insufficient rights.</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void CreateSymlink([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)
#if NET
            => File.CreateSymbolicLink(
                sourcePath ?? throw new ArgumentNullException(nameof(sourcePath)),
                targetPath ?? throw new ArgumentNullException(nameof(targetPath)));
#else
        => throw new PlatformNotSupportedException();
#endif

    /// <summary>
    /// Creates a new Unix hard link between two files.
    /// </summary>
    /// <param name="sourcePath">The path of the link to create.</param>
    /// <param name="targetPath">The absolute path of the existing file to point to.</param>
    /// <exception cref="IOException">The underlying Unix subsystem failed to process the request.</exception>
    /// <exception cref="UnauthorizedAccessException">The underlying Unix subsystem denied the request because of insufficient rights.</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void CreateHardlink([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)
    {
#if NET
            if (LibC.link(
                    targetPath ?? throw new ArgumentNullException(nameof(targetPath)),
                    sourcePath ?? throw new ArgumentNullException(nameof(sourcePath))) != 0)
                throw LibC.GetLastError(targetPath);
#else
        throw new PlatformNotSupportedException();
#endif
    }

    /// <summary>
    /// Returns the Inode ID of a file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <exception cref="IOException">The underlying Unix subsystem failed to process the request.</exception>
    /// <exception cref="UnauthorizedAccessException">The underlying Unix subsystem denied the request because of insufficient rights.</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static long GetInode([Localizable(false)] string path)
#if NET
            => (long)LibC.LStat(path ?? throw new ArgumentNullException(nameof(path))).Inode;
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
            if (LibC.rename(
                    source ?? throw new ArgumentNullException(nameof(source)),
                    destination ?? throw new ArgumentNullException(nameof(destination))) != 0)
                throw LibC.GetLastError(source);
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
    /// <exception cref="FileNotFoundException"><paramref name="path"/> does not exist.</exception>
    /// <exception cref="IOException">The underlying Unix subsystem failed to process the request.</exception>
    /// <exception cref="UnauthorizedAccessException">The underlying Unix subsystem denied the request because of insufficient rights.</exception>
    public static bool IsRegularFile([Localizable(false)] string path)
#if NET
            => (LibC.LStat(path ?? throw new ArgumentNullException(nameof(path))).Mode & LibC.S_IFMT) == LibC.S_IFREG;
#else
        => throw new PlatformNotSupportedException();
#endif

    /// <summary>
    /// Checks whether a file is a Unix symbolic link.
    /// </summary>
    /// <param name="path">The path of the file to check.</param>
    /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
    /// <exception cref="FileNotFoundException"><paramref name="path"/> does not exist.</exception>
    /// <exception cref="IOException">The underlying Unix subsystem failed to process the request.</exception>
    /// <exception cref="UnauthorizedAccessException">The underlying Unix subsystem denied the request because of insufficient rights.</exception>
    public static bool IsSymlink([Localizable(false)] string path)
#if NET
            => (LibC.LStat(path ?? throw new ArgumentNullException(nameof(path))).Mode & LibC.S_IFMT) == LibC.S_IFLNK;
#else
        => throw new PlatformNotSupportedException();
#endif

    /// <summary>
    /// Checks whether a file is a Unix symbolic link.
    /// </summary>
    /// <param name="path">The path of the file to check.</param>
    /// <param name="target">Returns the target the symbolic link points to if it exists.</param>
    /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
    /// <exception cref="IOException">The underlying Unix subsystem failed to process the request.</exception>
    /// <exception cref="UnauthorizedAccessException">The underlying Unix subsystem denied the request because of insufficient rights.</exception>
    public static bool IsSymlink(
        [Localizable(false)] string path,
        [MaybeNullWhen(false)] out string target)
    {
#if NET
            if (IsSymlink(path ?? throw new ArgumentNullException(nameof(path))))
            {
                target = new FileInfo(path).LinkTarget!;
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
        private const uint AllWritePermission = 0b010_010_010;

        /// <summary>The bit flag for granting the owner writing permissions.</summary>
        private const uint UserWritePermission = 0b010_000_000;

        /// <summary>A bit mask for all permission bits (including setuid, setgid and sticky) in a file mode.</summary>
        private const uint AllPermissions = 0xFFF;

        private static uint GetPermissions(string path)
            => LibC.LStat(path).Mode & AllPermissions;

        private static void SetPermissions(string path, uint permissions)
        {
            if (LibC.chmod(path, permissions) != 0) throw LibC.GetLastError(path);
        }
#endif

    /// <summary>
    /// Removes write permissions for everyone on a filesystem object (file or directory).
    /// </summary>
    /// <param name="path">The filesystem object (file or directory) to make read-only.</param>
    /// <exception cref="IOException">The underlying Unix subsystem failed to process the request.</exception>
    /// <exception cref="UnauthorizedAccessException">The underlying Unix subsystem denied the request because of insufficient rights.</exception>
    public static void MakeReadOnly([Localizable(false)] string path)
    {
#if NET
            if (path == null) throw new ArgumentNullException(nameof(path));
            SetPermissions(path, GetPermissions(path) & ~AllWritePermission);
#else
        throw new PlatformNotSupportedException();
#endif
    }

    /// <summary>
    /// Sets write permissions for the owner on a filesystem object (file or directory).
    /// </summary>
    /// <param name="path">The filesystem object (file or directory) to make writable by the owner.</param>
    /// <exception cref="IOException">The underlying Unix subsystem failed to process the request.</exception>
    /// <exception cref="UnauthorizedAccessException">The underlying Unix subsystem denied the request because of insufficient rights.</exception>
    public static void MakeWritable([Localizable(false)] string path)
    {
#if NET
            if (path == null) throw new ArgumentNullException(nameof(path));
            SetPermissions(path, GetPermissions(path) | UserWritePermission);
#else
        throw new PlatformNotSupportedException();
#endif
    }

#if NET
        /// <summary>A combination of bit flags to grant everyone executing permissions.</summary>
        private const uint AllExecutePermission = 0b001_001_001;
#endif

    /// <summary>
    /// Checks whether a file is marked as Unix-executable.
    /// </summary>
    /// <param name="path">The file to check for executable rights.</param>
    /// <returns><c>true</c> if <paramref name="path"/> points to an executable; <c>false</c> otherwise.</returns>
    /// <exception cref="IOException">The underlying Unix subsystem failed to process the request.</exception>
    /// <exception cref="UnauthorizedAccessException">The underlying Unix subsystem denied the request because of insufficient rights.</exception>
    /// <exception cref="FileNotFoundException"><paramref name="path"/> does not exist.</exception>
    public static bool IsExecutable([Localizable(false)] string path)
    {
#if NET
            // Check if any execution rights are set
            return (GetPermissions(path ?? throw new ArgumentNullException(nameof(path))) & AllExecutePermission) != 0;
#else
        throw new PlatformNotSupportedException();
#endif
    }

    /// <summary>
    /// Marks a file as Unix-executable or not Unix-executable.
    /// </summary>
    /// <param name="path">The file to mark as executable or not executable.</param>
    /// <param name="executable"><c>true</c> to mark the file as executable, <c>false</c> to mark it as not executable.</param>
    /// <exception cref="IOException">The underlying Unix subsystem failed to process the request.</exception>
    /// <exception cref="UnauthorizedAccessException">The underlying Unix subsystem denied the request because of insufficient rights.</exception>
    public static void SetExecutable([Localizable(false)] string path, bool executable)
    {
#if NET
            if (path == null) throw new ArgumentNullException(nameof(path));
            uint permissions = GetPermissions(path);
            SetPermissions(path, executable
                ? permissions | AllExecutePermission // Set all execution rights
                : permissions & ~AllExecutePermission); // Unset all execution rights
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
    /// <returns>The contents of the attribute as a byte array; <c>null</c> if there was a problem reading the file or extended attributes are not supported on this platform.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static byte[]? GetXattr([Localizable(false)] string path, [Localizable(false)] string name)
    {
#if NET
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (!IsLinux && !IsMacOSX) return null;

            // Query size first
            nint size = LibC.GetXattr(path, name, Span<byte>.Empty);
            if (size < 0) return null;

            var data = new byte[size];
            size = LibC.GetXattr(path, name, data);
            if (size < 0) return null;
            return size == data.Length ? data : data[..(int)size];
#else
        throw new PlatformNotSupportedException();
#endif
    }

    /// <summary>
    /// Sets an extended file attribute.
    /// </summary>
    /// <param name="path">The path of the file to set the attribute for.</param>
    /// <param name="name">The name of the attribute to set.</param>
    /// <param name="data">The data to write to the attribute.</param>
    /// <exception cref="IOException">The underlying Unix subsystem failed to process the request.</exception>
    /// <exception cref="UnauthorizedAccessException">The underlying Unix subsystem denied the request because of insufficient rights.</exception>
    /// <exception cref="PlatformNotSupportedException">Extended attributes are not supported on this platform.</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void SetXattr([Localizable(false)] string path, [Localizable(false)] string name, byte[] data)
    {
#if NET
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (!IsLinux && !IsMacOSX) throw new PlatformNotSupportedException();

            if (LibC.SetXattr(path, name, data) != 0)
                throw LibC.GetLastError(path);
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
                if (GetFstabFileSystem(_stat.RunAndCapture("--printf", "%m", path).TrimEnd('\n')) is {} fstabFileSystem)
                    return fstabFileSystem;
            }
            return fileSystem;
#else
        throw new PlatformNotSupportedException();
#endif
    }

#if NET
    /// <summary>
    /// Looks up the file system type of a mount point in <c>/etc/fstab</c>.
    /// </summary>
    private static string? GetFstabFileSystem(string mountPoint)
    {
        try
        {
            using var reader = File.OpenText("/etc/fstab");
            return GetFstabFileSystem(reader, mountPoint);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    /// <summary>
    /// Looks up the file system type of a mount point in fstab-formatted data.
    /// </summary>
    /// <param name="reader">The fstab data to parse.</param>
    /// <param name="mountPoint">The mount point to look for.</param>
    /// <returns>The file system type (third field) of the first matching entry; <c>null</c> if there is none.</returns>
    internal static string? GetFstabFileSystem(TextReader reader, string mountPoint)
    {
        while (reader.ReadLine() is {} line)
        {
            line = line.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;

            var fields = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length >= 3 && UnescapeFstab(fields[1]) == mountPoint)
                return UnescapeFstab(fields[2]);
        }
        return null;
    }

    /// <summary>
    /// Decodes octal escape sequences (e.g., <c>\040</c> for space) used in fstab fields.
    /// </summary>
    private static string UnescapeFstab(string value)
        => value.Contains('\\')
            ? Regex.Replace(value, @"\\([0-7]{3})", match => ((char)Convert.ToInt32(match.Groups[1].Value, 8)).ToString())
            : value;
#endif
    #endregion
}
