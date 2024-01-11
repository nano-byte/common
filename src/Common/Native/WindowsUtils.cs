// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text;
using NanoByte.Common.Storage;
using static System.Environment;
using static System.Environment.SpecialFolder;
using static System.IO.Path;
using Resources = NanoByte.Common.Properties.Resources;

namespace NanoByte.Common.Native;

/// <summary>
/// Provides helper methods and API calls specific to the Windows platform.
/// </summary>
[SupportedOSPlatform("windows")]
public static partial class WindowsUtils
{
    #region Win32 Error Codes
    /// <summary>
    /// The <see cref="Win32Exception.NativeErrorCode"/> value indicating that a file was not found.
    /// </summary>
    internal const int Win32ErrorFileNotFound = 2;

    /// <summary>
    /// The <see cref="Win32Exception.NativeErrorCode"/> value indicating that access to a resource was denied.
    /// </summary>
    internal const int Win32ErrorAccessDenied = 5;

    /// <summary>
    /// The <see cref="Win32Exception.NativeErrorCode"/> value indicating that write access to a resource failed.
    /// </summary>
    internal const int Win32ErrorWriteFault = 29;

    /// <summary>
    /// The <see cref="Win32Exception.NativeErrorCode"/> value indicating that an operation timed out.
    /// </summary>
    internal const int Win32ErrorSemTimeout = 121;

    /// <summary>
    /// The <see cref="Win32Exception.NativeErrorCode"/> value indicating that an element (e.g. a file) already exists.
    /// </summary>
    internal const int Win32ErrorAlreadyExists = 183;

    /// <summary>
    /// The <see cref="Win32Exception.NativeErrorCode"/> value indicating that more data is available and the query should be repeated with a larger output buffer/array.
    /// </summary>
    internal const int Win32ErrorMoreData = 234;

    /// <summary>
    /// The <see cref="Win32Exception.NativeErrorCode"/> value indicating that the requested application needs UAC elevation.
    /// </summary>
    [SupportedOSPlatformGuard("windows")]
    internal const int Win32ErrorRequestedOperationRequiresElevation = 740;

    /// <summary>
    /// The <see cref="Win32Exception.NativeErrorCode"/> value indicating that an operation was cancelled by the user.
    /// </summary>
    [SupportedOSPlatformGuard("windows")]
    [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Cancelled", Justification = "Naming matches the Win32 docs")]
    internal const int Win32ErrorCancelled = 1223;

    /// <summary>
    /// The file or directory is not a reparse point.
    /// </summary>
    internal const int Win32ErrorNotAReparsePoint = 4390;

    /// <summary>
    /// Builds a suitable <see cref="Exception"/> for a given <see cref="Win32Exception.NativeErrorCode"/>.
    /// </summary>
    internal static Exception BuildException(int error)
    {
        var ex = new Win32Exception(error);
        return error switch
        {
            Win32ErrorAlreadyExists => new IOException(ex.Message, ex),
            Win32ErrorWriteFault => new IOException(ex.Message, ex),
            Win32ErrorFileNotFound => new FileNotFoundException(ex.Message, ex),
            Win32ErrorAccessDenied => new UnauthorizedAccessException(ex.Message, ex),
            Win32ErrorRequestedOperationRequiresElevation => new NotAdminException(ex.Message, ex),
            Win32ErrorSemTimeout => new TimeoutException(),
            Win32ErrorCancelled => new OperationCanceledException(),
            _ => ex
        };
    }
    #endregion

    #region OS
    /// <summary>
    /// <c>true</c> if the current operating system is Windows (9x- or NT-based); <c>false</c> otherwise.
    /// </summary>
    [SupportedOSPlatformGuard("windows")]
    public static bool IsWindows
#if NET
        => OperatingSystem.IsWindows();
#elif NET20 || NET40
        => Environment.OSVersion.Platform is PlatformID.Win32Windows or PlatformID.Win32NT;
#else
        => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif

    /// <summary>
    /// <c>true</c> if the current operating system is a modern Windows version (NT-based); <c>false</c> otherwise.
    /// </summary>
    [SupportedOSPlatformGuard("windows")]
    public static bool IsWindowsNT
#if NET20 || NET40
        => Environment.OSVersion.Platform is PlatformID.Win32NT;
#else
        => IsWindows;
#endif

    [SupportedOSPlatformGuard("windows")]
    private static bool IsWindowsNTVersion(Version version)
#if NET
        => OperatingSystem.IsWindowsVersionAtLeast(version.Major, version.Minor, version.Build, version.Revision);
#else
        => IsWindowsNT && OSVersion.Version >= version;
#endif

    /// <summary>
    /// <c>true</c> if the current operating system is Windows XP or newer; <c>false</c> otherwise.
    /// </summary>
    [SupportedOSPlatformGuard("windows5.1")]
    public static bool IsWindowsXP => IsWindowsNTVersion(new(5, 1));

    /// <summary>
    /// <c>true</c> if the current operating system is Windows Vista or newer; <c>false</c> otherwise.
    /// </summary>
    [SupportedOSPlatformGuard("windows6.0")]
    public static bool IsWindowsVista => IsWindowsNTVersion(new(6, 0));

    /// <summary>
    /// <c>true</c> if the current operating system is Windows 7 or newer; <c>false</c> otherwise.
    /// </summary>
    [SupportedOSPlatformGuard("windows6.1")]
    public static bool IsWindows7 => IsWindowsNTVersion(new(6, 1));

    /// <summary>
    /// <c>true</c> if the current operating system is Windows 8 or newer; <c>false</c> otherwise.
    /// </summary>
    [SupportedOSPlatformGuard("windows6.2")]
    public static bool IsWindows8 => IsWindowsNTVersion(new(6, 2));

    /// <summary>
    /// <c>true</c> if the current operating system is Windows 10 or newer; <c>false</c> otherwise.
    /// </summary>
    [SupportedOSPlatformGuard("windows10.0")]
    public static bool IsWindows10 => IsWindowsNTVersion(new(10, 0));

    /// <summary>
    /// <c>true</c> if the current operating system is Windows 10 Anniversary Update (Redstone 1) or newer; <c>false</c> otherwise.
    /// </summary>
    [SupportedOSPlatformGuard("windows10.0.14393")]
    public static bool IsWindows10Redstone => IsWindowsNTVersion(new(10, 0, 14393));

    /// <summary>
    /// <c>true</c> if the current operating system is Windows 10, Version 2004 or newer; <c>false</c> otherwise.
    /// </summary>
    [SupportedOSPlatformGuard("windows10.0.19041")]
    public static bool IsWindows102004 => IsWindowsNTVersion(new(10, 0, 19041));

    /// <summary>
    /// <c>true</c> if the current operating system is Windows 11 or newer; <c>false</c> otherwise.
    /// </summary>
    [SupportedOSPlatformGuard("windows10.0.22000")]
    public static bool IsWindows11 => IsWindowsNTVersion(new(10, 0, 22000));

    /// <summary>
    /// <c>true</c> if the current operating system supports UAC and it is enabled; <c>false</c> otherwise.
    /// </summary>
    [SupportedOSPlatformGuard("windows6.0")]
    public static bool HasUac => IsWindowsVista && RegistryUtils.GetDword(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableLUA", defaultValue: 1) == 1;

    /// <summary>
    /// Indicates whether the current user is an administrator. Always returns <c>true</c> on non-Windows NT systems.
    /// </summary>
    public static bool IsAdministrator
    {
        get
        {
            if (!IsWindowsNT) return true;
            try
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Indicates whether the current process is running in a GUI session (rather than, e.g., as a service or in an SSH session).
    /// </summary>
    public static bool IsGuiSession { get; } = DetectGuiSession();

    private static bool DetectGuiSession()
    {
        try
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent())
               .IsInRole(new SecurityIdentifier(WellKnownSidType.InteractiveSid, null));
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    /// Determines the path of the executable the current process was launched from.
    /// </summary>
    public static string CurrentProcessPath
    {
        get
        {
            var fileName = new StringBuilder(255);
            SafeNativeMethods.GetModuleFileName(IntPtr.Zero, fileName, fileName.Capacity);
            return fileName.ToString();
        }
    }
    #endregion

    #region .NET Framework
    /// <summary>The directory version number for .NET Framework 2.0. This release includes the C# 2.0 compiler and the CLR 2.0 runtime.</summary>
    public const string NetFx20 = "v2.0.50727";

    /// <summary>The directory version number for .NET Framework 3.0.</summary>
    public const string NetFx30 = "v3.0";

    /// <summary>The directory version number for .NET Framework 3.5. This release includes the C# 3.0 compiler.</summary>
    public const string NetFx35 = "v3.5";

    /// <summary>The directory version number for .NET Framework 4.x.  This release includes a C# 4.0+ compiler and the CLR 4.0 runtime.</summary>
    public const string NetFx40 = "v4.0.30319";

    /// <summary>
    /// Returns the .NET Framework root directory for a specific version of the .NET Framework. Does not verify the directory actually exists!
    /// </summary>
    /// <param name="version">The full .NET version number including the leading "v". Use predefined constants when possible.</param>
    /// <returns>The path to the .NET Framework root directory.</returns>
    /// <remarks>Returns 64-bit directories if on 64-bit Windows is <c>true</c>.</remarks>
    public static string GetNetFxDirectory(string version)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
        #endregion

        string microsoftDotNetDir = Combine(
            GetEnvironmentVariable("windir") ?? @"C:\Windows",
            "Microsoft.NET");
        string framework32Dir = Combine(microsoftDotNetDir, "Framework");
        string framework64Dir = Combine(microsoftDotNetDir, "Framework64");
        return Combine(
            Directory.Exists(framework64Dir) ? framework64Dir : framework32Dir,
            version);
    }
    #endregion

    #region Command-line
    /// <summary>
    /// Tries to split a command-line into individual arguments.
    /// </summary>
    /// <param name="commandLine">The command-line to be split.</param>
    /// <returns>
    /// An array of individual arguments.
    /// Will return the entire command-line as one argument when not running on Windows or if splitting failed for some other reason.
    /// </returns>
    public static string[] SplitArgs(string? commandLine)
    {
        if (string.IsNullOrEmpty(commandLine)) return [];
        if (!IsWindows) return [commandLine];

        var ptrToSplitArgs = SafeNativeMethods.CommandLineToArgvW(commandLine, out int numberOfArgs);
        if (ptrToSplitArgs == IntPtr.Zero) return [commandLine];

        try
        {
            // Copy result to managed array
            var splitArgs = new string[numberOfArgs];
            for (int i = 0; i < numberOfArgs; i++)
                splitArgs[i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(ptrToSplitArgs, i * IntPtr.Size))!;
            return splitArgs;
        }
        finally
        {
            NativeMethods.LocalFree(ptrToSplitArgs);
        }
    }

    /// <summary>
    /// Tries to attach to a command-line console owned by the parent process.
    /// </summary>
    /// <returns><c>true</c> if the console was successfully attached; <c>false</c> if the parent process did not own a console.</returns>
    public static bool AttachConsole() => IsWindowsNT && NativeMethods.AttachConsole(uint.MaxValue);
    #endregion

    #region Performance counter
    private static long _performanceFrequency;

    /// <summary>
    /// A time index in seconds that continuously increases.
    /// </summary>
    /// <remarks>Depending on the operating system this may be the time of the system clock or the time since the system booted.</remarks>
    public static double AbsoluteTime
    {
        get
        {
            if (IsWindowsNT)
            {
                if (_performanceFrequency == 0)
                    SafeNativeMethods.QueryPerformanceFrequency(out _performanceFrequency);

                SafeNativeMethods.QueryPerformanceCounter(out long time);
                return time / (double)_performanceFrequency;
            }
            else return TickCount / 1000f;
        }
    }
    #endregion

    #region File system
    /// <summary>
    /// Gets the path to the specified system folder.
    /// Uses well-known environment variables and hard-coded paths as fallbacks when necessary.
    /// </summary>
    /// <exception cref="IOException">The folder could not be resolved to a path.</exception>
    public static string GetFolderPath(SpecialFolder folder)
    {
        try
        {
            if (Environment.GetFolderPath(folder
#if !NET20 || NET40
                    , SpecialFolderOption.DoNotVerify
#endif
                ) is {Length: >0} path)
                return path;
        }
        #region Error handling
        catch (ArgumentException)
        {
            // Environment.GetFolderPath() internally tries to validate some ACLs, which can sometimes fail with ArgumentException
        }
        #endregion

        // Fallback rules
        return folder switch
        {
            ProgramFiles => GetEnvironmentVariable("ProgramFiles") ?? @"C:\Program Files",
#if !NET20 || NET40
            ProgramFilesX86 => Is64BitProcess
                ? GetEnvironmentVariable("ProgramFiles(x86)") ?? @"C:\Program Files (x86)"
                : GetFolderPath(ProgramFiles),
            Windows => GetEnvironmentVariable("windir") ?? @"C:\Windows",
            CommonDesktopDirectory => Combine(GetEnvironmentVariable("PUBLIC") ?? @"C:\Users\Public", "Desktop"),
            CommonApplicationData => GetEnvironmentVariable("ProgramData") ?? @"C:\ProgramData",
            CommonStartMenu => Combine(GetFolderPath(CommonApplicationData), @"Microsoft\Windows\Start Menu"),
            CommonPrograms => Combine(GetFolderPath(CommonStartMenu), "Programs"),
            CommonStartup => Combine(GetFolderPath(CommonPrograms), "Startup"),
            UserProfile => Locations.HomeDir,
#endif
            DesktopDirectory or Desktop => Combine(Locations.HomeDir, "Desktop"),
            LocalApplicationData => GetEnvironmentVariable("LOCALAPPDATA") ?? Combine(Locations.HomeDir, @"AppData\Local"),
            ApplicationData => GetEnvironmentVariable("APPDATA") ?? Combine(Locations.HomeDir, @"AppData\Roaming"),
            StartMenu => Combine(GetFolderPath(ApplicationData), @"Microsoft\Windows\Start Menu"),
            Programs => Combine(GetFolderPath(StartMenu), "Programs"),
            Startup => Combine(GetFolderPath(Programs), "Startup"),
            SendTo => Combine(GetFolderPath(ApplicationData), @"Microsoft\SendTo"),
            _ => throw new IOException($"Unable to resolve {folder} to a path.")
        };
    }

    /// <summary>
    /// Reads the entire contents of a file using the Win32 API.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <returns>The contents of the file as a byte array; <c>null</c> if there was a problem reading the file.</returns>
    /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows.</exception>
    /// <remarks>This method works like <see cref="File.ReadAllBytes"/>, but bypasses .NET's file path validation logic.</remarks>
    public static byte[]? ReadAllBytes([Localizable(false)] string path)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        #endregion

        if (!IsWindows) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

        using var handle = NativeMethods.CreateFile(path, FileAccess.Read, FileShare.Read, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
        if (handle.IsInvalid) return null;

        uint size = NativeMethods.GetFileSize(handle, IntPtr.Zero);
        byte[] buffer = new byte[size];
        uint read = uint.MinValue;
        var lpOverlapped = new NativeOverlapped();
        return NativeMethods.ReadFile(handle, buffer, size, ref read, ref lpOverlapped) ? buffer : null;
    }

    /// <summary>
    /// Writes the entire contents of a byte array to a file using the Win32 API. Existing files with the same name are overwritten.
    /// </summary>
    /// <param name="path">The path of the file to write to.</param>
    /// <param name="data">The data to write to the file.</param>
    /// <exception cref="IOException">There was an IO problem writing the file.</exception>
    /// <exception cref="UnauthorizedAccessException">Write access to the file was denied.</exception>
    /// <exception cref="Win32Exception">There was a problem writing the file.</exception>
    /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows.</exception>
    /// <remarks>This method works like <see cref="File.WriteAllBytes"/>, but bypasses .NET's file path validation logic.</remarks>
    public static void WriteAllBytes([Localizable(false)] string path, byte[] data)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        if (!IsWindows) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

        using var handle = NativeMethods.CreateFile(path, FileAccess.Write, FileShare.Write, IntPtr.Zero, FileMode.Create, 0, IntPtr.Zero);
        if (handle.IsInvalid) throw BuildException(Marshal.GetLastWin32Error());

        uint bytesWritten = 0;
        var lpOverlapped = new NativeOverlapped();
        if (!NativeMethods.WriteFile(handle, data, (uint)data.Length, ref bytesWritten, ref lpOverlapped))
            throw BuildException(Marshal.GetLastWin32Error());
    }

    /// <summary>
    /// Creates a symbolic link for a file or directory.
    /// </summary>
    /// <param name="sourcePath">The path of the link to create.</param>
    /// <param name="targetPath">The path of the existing file or directory to point to (relative to <paramref name="sourcePath"/>).</param>
    /// <exception cref="IOException">There was an IO problem creating the symlink.</exception>
    /// <exception cref="UnauthorizedAccessException">You have insufficient rights to create the symbolic link.</exception>
    /// <exception cref="Win32Exception">The symbolic link creation failed.</exception>
    /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows NT 6.0 (Vista) or newer.</exception>
    public static void CreateSymlink([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentNullException(nameof(sourcePath));
        if (string.IsNullOrEmpty(targetPath)) throw new ArgumentNullException(nameof(targetPath));
        #endregion

        if (!IsWindowsVista) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

        var flags = IsWindows10Redstone
            ? NativeMethods.CreateSymbolicLinkFlags.SYMBOLIC_LINK_FLAG_ALLOW_UNPRIVILEGED_CREATE
            : NativeMethods.CreateSymbolicLinkFlags.NONE;

        if (Directory.Exists(Combine(GetDirectoryName(sourcePath) ?? Directory.GetCurrentDirectory(), targetPath)))
            flags |= NativeMethods.CreateSymbolicLinkFlags.SYMBOLIC_LINK_FLAG_DIRECTORY;

        if (!NativeMethods.CreateSymbolicLink(sourcePath, targetPath, flags))
            throw BuildException(Marshal.GetLastWin32Error());
    }

    /// <summary>
    /// Checks whether a file is an NTFS symbolic link.
    /// </summary>
    /// <param name="path">The path of the file to check.</param>
    /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
    /// <remarks>Will return <c>false</c> for non-existing files.</remarks>
    /// <exception cref="IOException">There was an IO problem getting link information.</exception>
    /// <exception cref="UnauthorizedAccessException">You have insufficient rights to get link information.</exception>
    /// <exception cref="Win32Exception">Getting link information failed.</exception>
    /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows NT 6.0 (Vista) or newer.</exception>
    public static bool IsSymlink([Localizable(false)] string path)
        => IsSymlink(path, out _);

    /// <summary>
    /// Checks whether a file is an NTFS symbolic link.
    /// </summary>
    /// <param name="path">The path of the file to check.</param>
    /// <param name="target">Returns the target the symbolic link points to if it exists.</param>
    /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
    /// <exception cref="IOException">There was an IO problem getting link information.</exception>
    /// <exception cref="UnauthorizedAccessException">You have insufficient rights to get link information.</exception>
    /// <exception cref="Win32Exception">Getting link information failed.</exception>
    /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows NT 6.0 (Vista) or newer.</exception>
    [SupportedOSPlatformGuard("windows6.0")]
    public static bool IsSymlink(
        [Localizable(false)] string path,
        [MaybeNullWhen(false)] out string target)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        #endregion

        if (!IsWindowsVista) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

        using var handle = NativeMethods.CreateFile(GetFullPath(path), 0, 0, IntPtr.Zero, FileMode.Open, NativeMethods.FILE_FLAG_OPEN_REPARSE_POINT | NativeMethods.FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero);
        if (handle.IsInvalid) throw BuildException(Marshal.GetLastWin32Error());


        if (!NativeMethods.DeviceIoControl(handle, NativeMethods.FSCTL_GET_REPARSE_POINT, IntPtr.Zero, 0, out var buffer, Marshal.SizeOf(typeof(NativeMethods.REPARSE_DATA_BUFFER)), out _, IntPtr.Zero))
        {
            int error = Marshal.GetLastWin32Error();
            if (error == Win32ErrorNotAReparsePoint)
            {
                target = null;
                return false;
            }
            else throw BuildException(error);
        }

        if (buffer.ReparseTag != NativeMethods.IO_REPARSE_TAG_SYMLINK)
        {
            target = null;
            return false;
        }

        target = new string(buffer.PathBuffer, buffer.SubstituteNameOffset / 2, buffer.SubstituteNameLength / 2);
        return true;
    }

    /// <summary>
    /// Creates a hard link between two files.
    /// </summary>
    /// <param name="sourcePath">The path of the link to create.</param>
    /// <param name="targetPath">The absolute path of the existing file to point to.</param>
    /// <remarks>Only available on Windows 2000 or newer.</remarks>
    /// <exception cref="IOException">There was an IO problem creating the hard link.</exception>
    /// <exception cref="UnauthorizedAccessException">You have insufficient rights to create the hard link.</exception>
    /// <exception cref="Win32Exception">The hard link creation failed.</exception>
    /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows NT.</exception>
    public static void CreateHardlink([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentNullException(nameof(sourcePath));
        if (string.IsNullOrEmpty(targetPath)) throw new ArgumentNullException(nameof(targetPath));
        #endregion

        if (!IsWindowsNT) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);
        if (!NativeMethods.CreateHardLink(sourcePath, targetPath, IntPtr.Zero))
            throw BuildException(Marshal.GetLastWin32Error());
    }

    /// <summary>
    /// Returns the file ID of a file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <exception cref="IOException">There was an IO problem checking the file.</exception>
    /// <exception cref="UnauthorizedAccessException">You have insufficient rights to check the files.</exception>
    /// <exception cref="Win32Exception">Checking the file failed.</exception>
    /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows NT.</exception>
    public static long GetFileID([Localizable(false)] string path)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        #endregion

        using var handle = NativeMethods.CreateFile(
            path ?? throw new ArgumentNullException(nameof(path)),
            FileAccess.Read, FileShare.Read, IntPtr.Zero, FileMode.Open, FileAttributes.Archive, IntPtr.Zero);
        if (handle.IsInvalid) throw BuildException(Marshal.GetLastWin32Error());

        if (!NativeMethods.GetFileInformationByHandle(handle, out var fileInfo))
            throw BuildException(Marshal.GetLastWin32Error());

        if (fileInfo.FileIndexLow is 0 or uint.MaxValue && fileInfo.FileIndexHigh is 0 or uint.MaxValue)
            throw new IOException($"The file system cannot provide a unique ID for '{path}'.");

        unchecked
        {
            return fileInfo.FileIndexLow + ((long)fileInfo.FileIndexHigh << 32);
        }
    }

    /// <summary>
    /// Moves a file on the next reboot of the OS. Replaces existing files.
    /// </summary>
    /// <param name="sourcePath">The source path to move the file from.</param>
    /// <param name="destinationPath">The destination path to move the file to. <c>null</c> to delete the file instead of moving it.</param>
    /// <remarks>Useful for replacing in-use files.</remarks>
    public static void MoveFileOnReboot(string sourcePath, string? destinationPath)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentNullException(nameof(sourcePath));
        #endregion

        if (!NativeMethods.MoveFileEx(sourcePath, destinationPath, NativeMethods.MoveFileFlags.MOVEFILE_REPLACE_EXISTING | NativeMethods.MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT))
            throw BuildException(Marshal.GetLastWin32Error());
    }
    #endregion

    #region Shell
    /// <summary>
    /// Sets the current process' explicit application user model ID.
    /// </summary>
    /// <param name="appID">The application ID to set.</param>
    /// <remarks>The application ID is used to group related windows in the taskbar.</remarks>
    [SupportedOSPlatformGuard("windows6.1")]
    public static void SetCurrentProcessAppID(string appID)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(appID)) throw new ArgumentNullException(nameof(appID));
        #endregion

        if (!IsWindows7) return;
        NativeMethods.SetCurrentProcessExplicitAppUserModelID(appID);
    }

    /// <summary>
    /// Informs the Windows shell that changes were made to the file association data in the registry.
    /// </summary>
    /// <remarks>This should be called immediately after the changes in order to trigger a refresh of the Explorer UI.</remarks>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static void NotifyAssocChanged()
    {
        if (!IsWindows) return;

        NativeMethods.SHChangeNotify(NativeMethods.SHCNE_ASSOCCHANGED, NativeMethods.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
    }

    /// <summary>
    /// Informs all GUI applications that changes where made to the environment variables (e.g. PATH) and that they should re-pull them.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static void NotifyEnvironmentChanged()
    {
        if (!IsWindows) return;

        NativeMethods.SendMessageTimeout(NativeMethods.HWND_BROADCAST, NativeMethods.WM_SETTINGCHANGE, IntPtr.Zero, "Environment", NativeMethods.SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 5000, out _);
    }
    #endregion

    #region Window messages
    /// <summary>
    /// Registers a new message type that can be sent to windows.
    /// </summary>
    /// <param name="message">A unique string used to identify the message type session-wide.</param>
    /// <returns>A unique ID number used to identify the message type session-wide.</returns>
    public static int RegisterWindowMessage([Localizable(false)] string message) => IsWindows ? NativeMethods.RegisterWindowMessage(message) : 0;

    /// <summary>
    /// Sends a message of a specific type to all windows in the current session.
    /// </summary>
    /// <param name="messageID">A unique ID number used to identify the message type session-wide.</param>
    public static void BroadcastMessage(int messageID)
    {
        if (!IsWindows) return;

        NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, messageID, IntPtr.Zero, IntPtr.Zero);
    }
    #endregion

    #region Restart Manager
    /// <summary>
    /// Registers the current application for automatic restart after updates or crashes.
    /// </summary>
    /// <param name="arguments">The command-line arguments to pass to the application on restart. Must not be empty!</param>
    public static void RegisterApplicationRestart(string arguments)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(arguments)) throw new ArgumentNullException(nameof(arguments));
        #endregion

        if (!IsWindowsVista) return;

        int ret = NativeMethods.RegisterApplicationRestart(arguments, NativeMethods.RestartFlags.NONE);
        if (ret != 0) Log.Warn($"Failed to register application for restart with arguments: {arguments}");
    }

    /// <summary>
    /// Unregisters the current application for automatic restart after updates or crashes.
    /// </summary>
    public static void UnregisterApplicationRestart()
    {
        if (!IsWindowsVista) return;

        NativeMethods.UnregisterApplicationRestart();
    }
    #endregion
}
