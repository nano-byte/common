// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
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
            [DllImport("shell32", SetLastError = true)]
            public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

            [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr SendMessageTimeout(IntPtr hwnd, int msg, IntPtr wParam, string lParam, int flags, uint timeout, out IntPtr lpdwResult);

            [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern int RegisterWindowMessage(string message);

            [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

            [DllImport("shell32", SetLastError = true)]
            public static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string appID);

            [DllImport("kernel32", SetLastError = true)]
            public static extern bool AttachConsole(uint dwProcessId);

            [DllImport("kernel32", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseHandle(IntPtr hObject);

            [DllImport("kernel32")]
            public static extern IntPtr LocalFree(IntPtr hMem);

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr CreateFile(string lpFileName, [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess, [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode, IntPtr lpSecurityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition, [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

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
            public static extern bool GetFileInformationByHandle(IntPtr handle, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

            [DllImport("kernel32", SetLastError = true)]
            public static extern uint GetFileSize(IntPtr handle, IntPtr size);

            [DllImport("kernel32", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ReadFile(IntPtr handle, byte[] buffer, uint byteToRead, ref uint bytesRead, [In] ref NativeOverlapped lpOverlapped);

            [DllImport("kernel32", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, ref uint lpNumberOfBytesWritten, [In] ref NativeOverlapped lpOverlapped);

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

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
