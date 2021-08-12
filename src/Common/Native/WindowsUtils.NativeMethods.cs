// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace NanoByte.Common.Native
{
    static partial class WindowsUtils
    {
        [SuppressUnmanagedCodeSecurity]
        private static class SafeNativeMethods
        {
            [DllImport("shell32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern uint GetModuleFileName(IntPtr hModule, [Out] StringBuilder lpFilename, int nSize);

            [DllImport("kernel32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool QueryPerformanceFrequency(out long lpFrequency);

            [DllImport("kernel32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool QueryPerformanceCounter(out long lpCounter);
        }

        private static class NativeMethods
        {
            public const uint SHCNE_ASSOCCHANGED = 0x08000000, SHCNF_IDLIST = 0;

            [DllImport("shell32", SetLastError = true)]
            public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

            public const uint WM_SETTINGCHANGE = 0x001A;
            public static readonly IntPtr HWND_BROADCAST = new(0xFFFF);

            [Flags]
            public enum SendMessageTimeoutFlags
            {
                SMTO_NORMAL = 0,
                SMTO_BLOCK = 1,
                SMTO_ABORTIFHUNG = 2,
                SMTO_NOTIMEOUTIFNOTHUNG = 8,
            }

            [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr SendMessageTimeout(IntPtr hwnd, uint msg, IntPtr wParam, string lParam, SendMessageTimeoutFlags flags, uint timeout, out IntPtr lpdwResult);

            [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern int RegisterWindowMessage(string message);

            [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

            [DllImport("shell32", SetLastError = true)]
            public static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string appID);

            [DllImport("kernel32", SetLastError = true)]
            public static extern bool AttachConsole(uint dwProcessId);

            [DllImport("kernel32")]
            public static extern IntPtr LocalFree(IntPtr hMem);

            public const FileAttributes
                FILE_FLAG_OPEN_REPARSE_POINT = (FileAttributes)0x00200000,
                FILE_FLAG_BACKUP_SEMANTICS = (FileAttributes)0x02000000;

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern SafeFileHandle CreateFile(string lpFileName, [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess, [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode, IntPtr lpSecurityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition, [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

            [StructLayout(LayoutKind.Sequential)]
            public struct BY_HANDLE_FILE_INFORMATION
            {
                public uint FileAttributes;
                public FILETIME CreationTime;
                public FILETIME LastAccessTime;
                public FILETIME LastWriteTime;
                public uint VolumeSerialNumber;
                public uint FileSizeHigh;
                public uint FileSizeLow;
                public uint NumberOfLinks;
                public uint FileIndexHigh;
                public uint FileIndexLow;
            }

            [DllImport("kernel32", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetFileInformationByHandle(SafeFileHandle handle, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

            [DllImport("kernel32", SetLastError = true)]
            public static extern uint GetFileSize(SafeFileHandle handle, IntPtr size);

            [DllImport("kernel32", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ReadFile(SafeFileHandle handle, byte[] buffer, uint byteToRead, ref uint bytesRead, [In] ref NativeOverlapped lpOverlapped);

            [DllImport("kernel32", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, ref uint lpNumberOfBytesWritten, [In] ref NativeOverlapped lpOverlapped);

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

            [Flags]
            public enum CreateSymbolicLinkFlags
            {
                NONE = 0,
                SYMBOLIC_LINK_FLAG_DIRECTORY = 1,
                SYMBOLIC_LINK_FLAG_ALLOW_UNPRIVILEGED_CREATE = 2
            }

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, CreateSymbolicLinkFlags dwFlags);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct REPARSE_DATA_BUFFER
            {
                public uint ReparseTag;
                public short ReparseDataLength;
                public short Reserved;
                public short SubstituteNameOffset;
                public short SubstituteNameLength;
                public short PrintNameOffset;
                public short PrintNameLength;
                public uint Flags;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16 * 1204)]
                public char[] PathBuffer;
            }

            public const int FSCTL_GET_REPARSE_POINT = 0x000900A8;
            public const uint IO_REPARSE_TAG_SYMLINK = 0xA000000C;

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr InBuffer, int nInBufferSize, [Out] out REPARSE_DATA_BUFFER OutBuffer, int nOutBufferSize, out int pBytesReturned, IntPtr lpOverlapped);

            [Flags]
            public enum MoveFileFlags
            {
                NONE = 0,
                MOVEFILE_REPLACE_EXISTING = 1,
                MOVEFILE_COPY_ALLOWED = 2,
                MOVEFILE_DELAY_UNTIL_REBOOT = 4,
                MOVEFILE_WRITE_THROUGH = 8
            }

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool MoveFileEx(string lpExistingFileName, string? lpNewFileName, MoveFileFlags dwFlags);

            [Flags]
            public enum RestartFlags
            {
                NONE = 0,
                RESTART_CYCLICAL = 1,
                RESTART_NOTIFY_SOLUTION = 2,
                RESTART_NOTIFY_FAULT = 4,
                RESTART_NO_CRASH = 8,
                RESTART_NO_HANG = 16,
                RESTART_NO_PATCH = 32,
                RESTART_NO_REBOOT = 64
            }

            [DllImport("kernel32", CharSet = CharSet.Unicode)]
            public static extern int RegisterApplicationRestart(string pwzCommandLine, RestartFlags dwFlags);

            [DllImport("kernel32")]
            public static extern int UnregisterApplicationRestart();
        }
    }
}
