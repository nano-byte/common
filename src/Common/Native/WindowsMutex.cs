/*
 * Copyright 2006-2015 Bastian Eicher
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
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides Windows-specific API calls for cross-process Mutexes.
    /// </summary>
    public static partial class WindowsMutex
    {
        private const uint Synchronize = 0x00100000;

        /// <summary>
        /// Creates or opens a mutex.
        /// </summary>
        /// <param name="name">The name to be used as a mutex identifier.</param>
        /// <param name="handle">The handle created for the mutex. Can be used to close it before the process ends.</param>
        /// <returns><see langword="true"/> if an existing mutex was opened; <see langword="false"/> if a new one was created.</returns>
        /// <exception cref="Win32Exception">The native subsystem reported a problem.</exception>
        /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows.</exception>
        /// <remarks>The mutex will automatically be released once the process terminates.</remarks>
        public static bool Create([NotNull, Localizable(false)] string name, out IntPtr handle)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            #endregion

            if (!WindowsUtils.IsWindowsNT) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

            // Create new or open existing mutex
            handle = NativeMethods.CreateMutex(IntPtr.Zero, false, name);

            int error = Marshal.GetLastWin32Error();
            switch (error)
            {
                case 0:
                    // New mutex created, handle passed out
                    return false;

                case WindowsUtils.Win32ErrorAlreadyExists:
                    // Existing mutex opened, handle passed out
                    return true;

                case WindowsUtils.Win32ErrorAccessDenied:
                    // Try to open existing mutex
                    handle = NativeMethods.OpenMutex(Synchronize, false, name);

                    if (handle == IntPtr.Zero)
                    {
                        error = Marshal.GetLastWin32Error();
                        switch (error)
                        {
                            case WindowsUtils.Win32ErrorFileNotFound:
                                // No existing mutex found
                                return false;
                            default:
                                throw new Win32Exception(error);
                        }
                    }

                    // Existing mutex opened, handle passed out
                    return true;

                default:
                    throw new Win32Exception(error);
            }
        }

        /// <summary>
        /// Tries to open an existing mutex.
        /// </summary>
        /// <param name="name">The name to be used as a mutex identifier.</param>
        /// <returns><see langword="true"/> if an existing mutex was opened; <see langword="false"/> if none existed.</returns>
        /// <exception cref="Win32Exception">The native subsystem reported a problem.</exception>
        /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows.</exception>
        /// <remarks>Opening a mutex creates an additional handle to it, keeping it alive until the process terminates.</remarks>
        public static bool Open([NotNull, Localizable(false)] string name)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            #endregion

            if (!WindowsUtils.IsWindowsNT) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

            // Try to open existing mutex
            var handle = NativeMethods.OpenMutex(Synchronize, false, name);

            if (handle == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                switch (error)
                {
                    case WindowsUtils.Win32ErrorFileNotFound:
                        // No existing mutex found
                        return false;

                    default:
                        throw new Win32Exception(error);
                }
            }

            // Existing mutex opened, handle remains until process terminates
            return true;
        }

        /// <summary>
        /// Checks whether a specific mutex exists without openining a lasting handle.
        /// </summary>
        /// <param name="name">The name to be used as a mutex identifier.</param>
        /// <returns><see langword="true"/> if an existing mutex was found; <see langword="false"/> if none existed.</returns>
        /// <exception cref="Win32Exception">The native subsystem reported a problem.</exception>
        /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows.</exception>
        public static bool Probe([NotNull, Localizable(false)] string name)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            #endregion

            if (!WindowsUtils.IsWindowsNT) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

            // Try to open existing mutex
            var handle = NativeMethods.OpenMutex(Synchronize, false, name);

            if (handle == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                switch (error)
                {
                    case WindowsUtils.Win32ErrorFileNotFound:
                        // No existing mutex found
                        return false;

                    default:
                        throw new Win32Exception(error);
                }
            }

            // Existing mutex opened, close handle again
            NativeMethods.CloseHandle(handle);
            return true;
        }

        /// <summary>
        /// Closes an existing mutex handle. The mutex is destroyed if this is the last handle.
        /// </summary>
        /// <param name="handle">The mutex handle to be closed.</param>
        /// <exception cref="PlatformNotSupportedException">This method is called on a platform other than Windows.</exception>
        public static void Close(IntPtr handle)
        {
            if (!WindowsUtils.IsWindowsNT) throw new PlatformNotSupportedException(Resources.OnlyAvailableOnWindows);

            NativeMethods.CloseHandle(handle);
        }
    }
}