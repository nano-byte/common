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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Utils
{
    static partial class WindowsUtils
    {
        /// <summary>
        /// Reads the entire contents of a file using the Win32 API.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <returns>The contents of the file as a byte array; <see langword="null"/> if there was a problem reading the file.</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown when this method is called on a platform other than Windows.</exception>
        /// <remarks>This method works like <see cref="File.ReadAllBytes"/>, but bypasses .NET's file path validation logic.</remarks>
        public static byte[] ReadAllBytes(string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            #endregion

            if (!IsWindows) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

            var handle = UnsafeNativeMethods.CreateFile(path, FileAccess.Read, FileShare.Read, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            if (handle.ToInt32() == -1) return null;

            try
            {
                uint size = UnsafeNativeMethods.GetFileSize(handle, IntPtr.Zero);
                byte[] buffer = new byte[size];
                uint read = uint.MinValue;
                var lpOverlapped = new NativeOverlapped();
                if (UnsafeNativeMethods.ReadFile(handle, buffer, size, ref read, ref lpOverlapped)) return buffer;
                else return null;
            }
            finally
            {
                UnsafeNativeMethods.CloseHandle(handle);
            }
        }

        /// <summary>
        /// Writes the entire contents of a byte array to a file using the Win32 API. Existing files with the same name are overwritten.
        /// </summary>
        /// <param name="path">The path of the file to write to.</param>
        /// <param name="data">The data to write to the file.</param>
        /// <exception cref="Win32Exception">Thrown if there was a problem writing the file.</exception>
        /// <exception cref="PlatformNotSupportedException">Thrown when this method is called on a platform other than Windows.</exception>
        /// <remarks>This method works like <see cref="File.WriteAllBytes"/>, but bypasses .NET's file path validation logic.</remarks>
        public static void WriteAllBytes(string path, byte[] data)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (data == null) throw new ArgumentNullException("data");
            #endregion

            if (!IsWindows) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

            var handle = UnsafeNativeMethods.CreateFile(path, FileAccess.Write, FileShare.Write, IntPtr.Zero, FileMode.Create, 0, IntPtr.Zero);
            var errrorCode = Marshal.GetLastWin32Error();
            if (handle == new IntPtr(-1)) throw new Win32Exception(errrorCode);

            try
            {
                uint bytesWritten = 0;
                var lpOverlapped = new NativeOverlapped();
                if (!UnsafeNativeMethods.WriteFile(handle, data, (uint)data.Length, ref bytesWritten, ref lpOverlapped))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                UnsafeNativeMethods.CloseHandle(handle);
            }
        }

        /// <summary>
        /// Creates a symbolic link for a file or directory.
        /// </summary>
        /// <param name="source">The path of the link to create.</param>
        /// <param name="target">The path of the existing file or directory to point to (relative to <paramref name="source"/>).</param>
        /// <exception cref="Win32Exception">Thrown if the symbolic link creation failed.</exception>
        /// <exception cref="PlatformNotSupportedException">Thrown when this method is called on a platform other than Windows NT 6.0 (Vista) or newer.</exception>
        public static void CreateSymlink(string source, string target)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(source)) throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(target)) throw new ArgumentNullException("target");
            #endregion

            if (!IsWindowsVista) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

            string targetAbsolute = Path.Combine(Path.GetDirectoryName(source) ?? Environment.CurrentDirectory, target);
            int retval = UnsafeNativeMethods.CreateSymbolicLink(source, target, Directory.Exists(targetAbsolute) ? 1 : 0);
            if (retval != 1) throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Creates a hard link between two files.
        /// </summary>
        /// <param name="source">The path of the link to create.</param>
        /// <param name="target">The absolute path of the existing file to point to.</param>
        /// <remarks>Only available on Windows 2000 or newer.</remarks>
        /// <exception cref="Win32Exception">Thrown if the hard link creation failed.</exception>
        /// <exception cref="PlatformNotSupportedException">Thrown when this method is called on a platform other than Windows NT.</exception>
        public static void CreateHardlink(string source, string target)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(source)) throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(target)) throw new ArgumentNullException("target");
            #endregion

            if (!IsWindowsNT) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);
            if (!UnsafeNativeMethods.CreateHardLink(source, target, IntPtr.Zero)) throw new Win32Exception();
        }

        /// <summary>
        /// Determines whether to files are hardlinked.
        /// </summary>
        /// <param name="path1">The path of the first file.</param>
        /// <param name="path2">The path of the second file.</param>
        /// <exception cref="PlatformNotSupportedException">Thrown when this method is called on a platform other than Windows NT.</exception>
        public static bool AreHardlinked(string path1, string path2)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path1)) throw new ArgumentNullException("path1");
            if (string.IsNullOrEmpty(path2)) throw new ArgumentNullException("path2");
            #endregion

            if (!IsWindowsNT) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);
            return GetFileIndex(path1) == GetFileIndex(path2);
        }

        private static ulong GetFileIndex(string path)
        {
            var handle = UnsafeNativeMethods.CreateFile(path, FileAccess.Read, FileShare.Read, IntPtr.Zero, FileMode.Open, FileAttributes.Archive, IntPtr.Zero);
            if (handle == IntPtr.Zero) throw new Win32Exception();

            try
            {
                UnsafeNativeMethods.BY_HANDLE_FILE_INFORMATION fileInfo;
                if (!UnsafeNativeMethods.GetFileInformationByHandle(handle, out fileInfo)) throw new Win32Exception();
                return fileInfo.FileIndexLow + (fileInfo.FileIndexHigh << 32);
            }
            finally
            {
                UnsafeNativeMethods.CloseHandle(handle);
            }
        }
    }
}
