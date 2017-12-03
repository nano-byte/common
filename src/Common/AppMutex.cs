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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using NanoByte.Common.Native;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides a cross-process object allowing easy dection of application instances (e.g., for use by installers and update tools). May be a no-op on some platforms.
    /// </summary>
    /// <remarks>Use <see cref="Mutex"/> or <seealso cref="MutexLock"/> instead for synchronizing access to shared resources.</remarks>
    public sealed class AppMutex
    {
        private readonly List<IntPtr> _handles;

        private AppMutex(params IntPtr[] handles) => _handles = handles.ToList();

        /// <summary>
        /// Creates or opens a mutex to signal that an application is running.
        /// </summary>
        /// <param name="name">The name to be used as a mutex identifier.</param>
        /// <returns>The handle for the mutex. Can be used to <see cref="Close"/> it again. Will automatically be released once the process terminates.</returns>
        public static AppMutex Create([NotNull, Localizable(false)] string name)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            #endregion

            if (!WindowsUtils.IsWindowsNT) return new AppMutex();

            try
            {
                return new AppMutex(
                    WindowsMutex.Create(name, out _),
                    WindowsMutex.Create("Global\\" + name, out _));
            }
            #region Error handling
            catch (Win32Exception ex)
            {
                Log.Warn(ex.Message);
                return new AppMutex();
            }
            #endregion
        }

        /// <summary>
        /// Closes the mutex handle, allowing it to be released if no other instances are running.
        /// </summary>
        public void Close()
        {
            foreach (var handle in _handles.Where(handle => handle != IntPtr.Zero))
                WindowsMutex.Close(handle);
            _handles.Clear();
        }

        /// <summary>
        /// Checks whether a specific mutex exists (local or global) without opening a lasting handle.
        /// </summary>
        /// <param name="name">The name to be used as a mutex identifier.</param>
        /// <returns><c>true</c> if an existing mutex was found; <c>false</c> if none existed.</returns>
        public static bool Probe([NotNull, Localizable(false)] string name)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            #endregion

            if (!WindowsUtils.IsWindowsNT) return false;

            try
            {
                return WindowsMutex.Probe(name) || WindowsMutex.Probe("Global\\" + name);
            }
            #region Error handling
            catch (Win32Exception ex)
            {
                Log.Warn(ex.Message);
                return false;
            }
            #endregion
        }
    }
}
