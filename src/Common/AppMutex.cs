// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using NanoByte.Common.Native;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides a cross-process object allowing easy detection of application instances (e.g., for use by installers and update tools). May be a no-op on some platforms.
    /// </summary>
    /// <remarks>Use <see cref="Mutex"/> or <seealso cref="MutexLock"/> instead for synchronizing access to shared resources.</remarks>
    public sealed class AppMutex
    {
        private readonly List<IntPtr> _handles;

        private AppMutex(params IntPtr[] handles)
        {
            _handles = handles.ToList();
        }

        /// <summary>
        /// Creates or opens a mutex to signal that an application is running.
        /// </summary>
        /// <param name="name">The name to be used as a mutex identifier.</param>
        /// <returns>The handle for the mutex. Can be used to <see cref="Close"/> it again. Will automatically be released once the process terminates.</returns>
        public static AppMutex Create([Localizable(false)] string name)
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
        public static bool Probe([Localizable(false)] string name)
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
