/*
 * Copyright 2006-2014 Bastian Eicher
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
            // Command-line arguments
            [DllImport("shell32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);


            // Platform
            [DllImport("kernel32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsWow64Process([In] IntPtr hProcess, [Out, MarshalAs(UnmanagedType.Bool)] out bool lpSystemInfo);

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern uint GetModuleFileName(IntPtr hModule, [Out] StringBuilder lpFilename, int nSize);


            // Window messages
            [DllImport("user32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PeekMessage(out WinMessage msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

            [DllImport("user32")]
            public static extern int GetCaretBlinkTime();


            // Performance counters
            [DllImport("kernel32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool QueryPerformanceFrequency(out long lpFrequency);

            [DllImport("kernel32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool QueryPerformanceCounter(out long lpCounter);
        }

        [SuppressUnmanagedCodeSecurity]
        private static class UnsafeNativeMethods
        {
            [DllImport("shell32", SetLastError = true)]
            public static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string appID);

            [DllImport("kernel32", SetLastError = true)]
            public static extern int CloseHandle(IntPtr hObject);

            [DllImport("kernel32")]
            public static extern IntPtr LocalFree(IntPtr hMem);


            // Mutex
            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr OpenMutex(UInt32 desiredAccess, bool inheritHandle, string name);


            // Shell and window messages
            [DllImport("user32")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

            [DllImport("shell32", SetLastError = true)]
            public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

            [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr SendMessageTimeout(IntPtr hwnd, int msg, IntPtr wParam, string lParam, int flags, uint timeout, out IntPtr lpdwResult);

            [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern int RegisterWindowMessage(string message);


            // Filesystem
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr CreateFile(string lpFileName, [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess, [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode, IntPtr lpSecurityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition, [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool GetFileInformationByHandle(IntPtr handle, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

            [DllImport("kernel32", SetLastError = true)]
            public static extern uint GetFileSize(IntPtr handle, IntPtr size);

            [DllImport("kernel32", SetLastError = true)]
            public static extern bool ReadFile(IntPtr handle, byte[] buffer, uint byteToRead, ref uint bytesRead, [In] ref NativeOverlapped lpOverlapped);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, ref uint lpNumberOfBytesWritten, [In] ref NativeOverlapped lpOverlapped);

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern int CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

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


            // Window messages
            [DllImport("user32")]
            public static extern IntPtr SetCapture(IntPtr handle);

            [DllImport("user32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ReleaseCapture();
        }
    }
}
