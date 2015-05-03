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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using NanoByte.Common.Native;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides a cross-process object allowing easy dection of application instances (e.g., for use by installers and update tools).
    /// </summary>
    /// <remarks><see cref="System.Threading.Mutex"/> is intended for synchronizing access to shared resources while this class is intended to detect application instances.</remarks>
    public sealed class AppMutex
    {
        #region Handles
        private readonly List<IntPtr> _handles;

        private AppMutex(IEnumerable<IntPtr> handles)
        {
            _handles = new List<IntPtr>(handles);
        }

        /// <summary>
        /// Closes all contained handles, allowing the mutex to be released.
        /// </summary>
        public void Close()
        {
            foreach (var handle in _handles.Where(handle => handle != IntPtr.Zero).ToList())
            {
                WindowsUtils.CloseMutex(handle);
                _handles.Remove(handle);
            }
        }
        #endregion

        /// <summary>
        /// Creates or opens a mutex (local and global) to signal that an application is running.
        /// </summary>
        /// <param name="name">The name to be used as a mutex identifier.</param>
        /// <returns><see langword="true"/> if an existing mutex was opened; <see langword="false"/> if a new one was created.</returns>
        /// <remarks>The mutex will automatically be released once the process terminates. You can check the return value to prevent multiple instances from running.</remarks>
        [PublicAPI]
        public static bool Create([NotNull, Localizable(false)] string name)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            #endregion

            AppMutex mutex;
            return Create(name, out mutex);
        }

        /// <summary>
        /// Creates or opens a mutex (local and global) to signal that an application is running.
        /// </summary>
        /// <param name="name">The name to be used as a mutex identifier.</param>
        /// <param name="mutex">A pointer to the mutex.</param>
        /// <returns><see langword="true"/> if an existing mutex was opened; <see langword="false"/> if a new one was created.</returns>
        /// <remarks>The mutex will automatically be released once the process terminates or you call <see cref="Close"/> on <paramref name="mutex"/>.</remarks>
        [PublicAPI]
        public static bool Create([NotNull, Localizable(false)] string name, out AppMutex mutex)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            #endregion

            if (!WindowsUtils.IsWindowsNT)
            {
                mutex = null;
                return false;
            }

            bool existingMutex = false;

            IntPtr handle1 = IntPtr.Zero, handle2 = IntPtr.Zero;
            try
            {
                if (WindowsUtils.CreateMutex("Global\\" + name, out handle1))
                {
                    existingMutex = true;
                    Log.Debug("Opened existing global mutex: " + name);
                }
                else Log.Debug("Created global mutex: " + name);
            }
                #region Error handling
            catch (Win32Exception ex)
            {
                Log.Warn(ex.Message);
            }
            #endregion

            try
            {
                if (WindowsUtils.CreateMutex(name, out handle2))
                {
                    existingMutex = true;
                    Log.Debug("Opened existing local mutex: " + name);
                }
                else Log.Debug("Created local mutex: " + name);
            }
                #region Error handling
            catch (Win32Exception ex)
            {
                Log.Warn(ex.Message);
            }
            #endregion

            mutex = new AppMutex(new[] {handle1, handle2});
            return existingMutex;
        }

        /// <summary>
        /// Tries to open an existing mutex (local and global) signaling that an application is running.
        /// </summary>
        /// <param name="name">The name to be used as a mutex identifier.</param>
        /// <returns><see langword="true"/> if an existing mutex was opened; <see langword="false"/> if none existed.</returns>
        /// <remarks>Opening a mutex creates an additional handle to it, keeping it alive until the process terminates.</remarks>
        [PublicAPI]
        public static bool Open([NotNull, Localizable(false)] string name)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            #endregion

            if (!WindowsUtils.IsWindowsNT) return false;

            bool existingMutex = false;

            try
            {
                if (WindowsUtils.OpenMutex("Global\\" + name))
                {
                    existingMutex = true;
                    Log.Debug("Opened existing global mutex: " + name);
                }
            }
                #region Error handling
            catch (Win32Exception ex)
            {
                Log.Warn(ex.Message);
            }
            #endregion

            try
            {
                if (WindowsUtils.OpenMutex(name))
                {
                    existingMutex = true;
                    Log.Debug("Opened existing local mutex: " + name);
                }
            }
                #region Error handling
            catch (Win32Exception ex)
            {
                Log.Warn(ex.Message);
            }
            #endregion

            return existingMutex;
        }

        /// <summary>
        /// Checks whether a specific mutex exists (local or global) without opening a lasting handle.
        /// </summary>
        /// <param name="name">The name to be used as a mutex identifier.</param>
        /// <returns><see langword="true"/> if an existing mutex was found; <see langword="false"/> if none existed.</returns>
        [PublicAPI]
        public static bool Probe([NotNull, Localizable(false)] string name)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            #endregion

            if (!WindowsUtils.IsWindowsNT) return false;

            bool existingMutex = false;

            try
            {
                if (WindowsUtils.ProbeMutex("Global\\" + name))
                {
                    existingMutex = true;
                    Log.Debug("Found existing global mutex: " + name);
                }
            }
                #region Error handling
            catch (Win32Exception ex)
            {
                Log.Warn(ex.Message);
            }
            #endregion

            try
            {
                if (WindowsUtils.ProbeMutex(name))
                {
                    existingMutex = true;
                    Log.Debug("Found existing local mutex: " + name);
                }
            }
                #region Error handling
            catch (Win32Exception ex)
            {
                Log.Warn(ex.Message);
            }
            #endregion

            return existingMutex;
        }
    }
}
