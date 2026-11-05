#if NET
// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;
using System.Text;

namespace NanoByte.Common.Native;

/// <summary>
/// P/Invoke declarations and helpers for the C standard library on Unixoid systems (Linux, MacOS X, FreeBSD).
/// </summary>
/// <remarks>Uses source-generated marshalling, which is compatible with trimming and Native AOT.</remarks>
internal static unsafe partial class LibC
{
    private const string Lib = "libc";

    #region errno
    private const int EPERM = 1, ENOENT = 2, EACCES = 13, ENOTDIR = 20; // Identical on Linux, MacOS X and FreeBSD

    /// <summary><c>errno</c> value for an operation that would have to block, e.g., a non-blocking attempt to get a lock held by someone else.</summary>
    public static int EWOULDBLOCK => OperatingSystem.IsLinux() ? 11 : 35; // 35 on MacOS X and FreeBSD

    /// <summary>
    /// Creates an exception for the <c>errno</c> value set by the last P/Invoke call.
    /// </summary>
    /// <param name="path">The path of the file the failed call operated on, if any.</param>
    public static Exception GetLastError(string? path = null)
    {
        int errno = Marshal.GetLastPInvokeError();
        string message = Marshal.GetPInvokeErrorMessage(errno);
        if (path != null) message += $": {path}";

        return errno switch
        {
            EPERM or EACCES => new UnauthorizedAccessException(message),
            ENOENT => new FileNotFoundException(message, path),
            ENOTDIR => new DirectoryNotFoundException(message),
            _ => new IOException(message)
        };
    }
    #endregion

    #region uname
    [LibraryImport(Lib, EntryPoint = "uname", SetLastError = true)]
    private static partial int uname(byte* buf);

    [LibraryImport(Lib, EntryPoint = "__xuname", SetLastError = true)]
    private static partial int xuname_freebsd(int namesize, byte* buf);

    /// <summary>
    /// Calls <c>uname</c> and returns the <c>sysname</c> and <c>machine</c> fields.
    /// </summary>
    public static (string SysName, string Machine) Uname()
    {
        // struct utsname consists of 5 (MacOS X, FreeBSD) or 6 (Linux) fixed-size char arrays
        int fieldSize = OperatingSystem.IsLinux() ? 65 : 256;
        var buffer = stackalloc byte[6 * 256];

        int result = OperatingSystem.IsFreeBSD()
            ? xuname_freebsd(fieldSize, buffer) // FreeBSD's uname() is an inline wrapper for __xuname()
            : uname(buffer);
        if (result != 0) throw GetLastError();

        return (ReadField(0), ReadField(4));

        string ReadField(int index)
        {
            var field = new ReadOnlySpan<byte>(buffer + index * fieldSize, fieldSize);
            int length = field.IndexOf((byte)0);
            return Encoding.UTF8.GetString(length < 0 ? field : field[..length]);
        }
    }
    #endregion

    #region Files
    [LibraryImport(Lib, EntryPoint = "link", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int link(string oldpath, string newpath);

    [LibraryImport(Lib, EntryPoint = "rename", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int rename(string oldpath, string newpath);

    [LibraryImport(Lib, EntryPoint = "chmod", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int chmod(string path, uint mode);

    [LibraryImport(Lib, EntryPoint = "execvp", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int execvp(string file, nint* argv);
    #endregion

    #region stat
    /// <summary>Bit mask for the file type in <c>st_mode</c>.</summary>
    public const uint S_IFMT = 0xF000;

    /// <summary>File type in <c>st_mode</c> for regular files.</summary>
    public const uint S_IFREG = 0x8000;

    /// <summary>File type in <c>st_mode</c> for symbolic links.</summary>
    public const uint S_IFLNK = 0xA000;

    [LibraryImport(Lib, EntryPoint = "statx", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial int statx_linux(int dirfd, string pathname, int flags, uint mask, byte* statxbuf);

    [LibraryImport(Lib, EntryPoint = "lstat$INODE64", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial int lstat_macos_x64(string path, byte* buf);

    [LibraryImport(Lib, EntryPoint = "lstat", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial int lstat(string path, byte* buf);

    /// <summary>
    /// Gets the inode number and mode of a file without following symbolic links.
    /// </summary>
    /// <exception cref="IOException">The file could not be accessed.</exception>
    /// <exception cref="UnauthorizedAccessException">Access to the file was denied.</exception>
    public static (ulong Inode, uint Mode) LStat(string path)
    {
        // struct stat differs between operating systems and architectures, so only read the fields we need from a sufficiently large buffer
        var buffer = stackalloc byte[512];

        if (OperatingSystem.IsLinux())
        {
            // struct statx has the same layout on all architectures
            const int AT_FDCWD = -100, AT_SYMLINK_NOFOLLOW = 0x100;
            const uint STATX_TYPE = 0x1, STATX_MODE = 0x2, STATX_INO = 0x100;
            if (statx_linux(AT_FDCWD, path, AT_SYMLINK_NOFOLLOW, STATX_TYPE | STATX_MODE | STATX_INO, buffer) != 0) throw GetLastError(path);
            return (Inode: *(ulong*)(buffer + 32), Mode: *(ushort*)(buffer + 28));
        }
        else if (OperatingSystem.IsMacOS())
        {
            // struct stat with 64-bit inodes: int32 st_dev, uint16 st_mode, uint16 st_nlink, uint64 st_ino
            int result = RuntimeInformation.ProcessArchitecture == Architecture.X64 ? lstat_macos_x64(path, buffer) : lstat(path, buffer);
            if (result != 0) throw GetLastError(path);
            return (Inode: *(ulong*)(buffer + 8), Mode: *(ushort*)(buffer + 4));
        }
        else if (OperatingSystem.IsFreeBSD())
        {
            // struct stat (FreeBSD 12+): uint64 st_dev, uint64 st_ino, uint64 st_nlink, uint16 st_mode
            if (lstat(path, buffer) != 0) throw GetLastError(path);
            return (Inode: *(ulong*)(buffer + 8), Mode: *(ushort*)(buffer + 24));
        }
        else throw new PlatformNotSupportedException();
    }
    #endregion

    #region Extended attributes
    [LibraryImport(Lib, EntryPoint = "getxattr", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial nint getxattr_linux(string path, string name, byte* value, nuint size);

    [LibraryImport(Lib, EntryPoint = "setxattr", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial int setxattr_linux(string path, string name, byte* value, nuint size, int flags);

    [LibraryImport(Lib, EntryPoint = "getxattr", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial nint getxattr_macos(string path, string name, byte* value, nuint size, uint position, int options);

    [LibraryImport(Lib, EntryPoint = "setxattr", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial int setxattr_macos(string path, string name, byte* value, nuint size, uint position, int options);

    /// <summary>
    /// Calls <c>getxattr</c> on Linux or MacOS X.
    /// </summary>
    /// <returns>The size of the attribute value or -1 on error.</returns>
    public static nint GetXattr(string path, string name, Span<byte> value)
    {
        fixed (byte* ptr = value)
        {
            if (OperatingSystem.IsLinux()) return getxattr_linux(path, name, ptr, (nuint)value.Length);
            else if (OperatingSystem.IsMacOS()) return getxattr_macos(path, name, ptr, (nuint)value.Length, 0, 0);
            else throw new PlatformNotSupportedException();
        }
    }

    /// <summary>
    /// Calls <c>setxattr</c> on Linux or MacOS X.
    /// </summary>
    /// <returns>0 on success or -1 on error.</returns>
    public static int SetXattr(string path, string name, ReadOnlySpan<byte> value)
    {
        fixed (byte* ptr = value)
        {
            if (OperatingSystem.IsLinux()) return setxattr_linux(path, name, ptr, (nuint)value.Length, 0);
            else if (OperatingSystem.IsMacOS()) return setxattr_macos(path, name, ptr, (nuint)value.Length, 0, 0);
            else throw new PlatformNotSupportedException();
        }
    }
    #endregion
}
#endif
