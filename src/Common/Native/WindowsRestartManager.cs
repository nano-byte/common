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
using System.IO;
using JetBrains.Annotations;
using NanoByte.Common.Properties;
using NanoByte.Common.Tasks;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides an interface to the Windows Restart Manager. Supported on Windows Vista or better.
    /// </summary>
    /// <remarks>
    /// See https://msdn.microsoft.com/en-us/library/windows/desktop/cc948910
    /// </remarks>
    public sealed partial class WindowsRestartManager : MarshalByRefObject, IDisposable
    {
        #region Session
        private readonly IntPtr _sessionHandle;

        /// <summary>
        /// Starts a new Restart Manager session.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException">The current platform does not support the Restart Manager. Needs Windows Vista or better.</exception>
        /// <exception cref="Win32Exception">The restart manager API returned an error.</exception>
        public WindowsRestartManager()
        {
            if (!WindowsUtils.IsWindowsVista) throw new PlatformNotSupportedException();

            int ret = UnsafeNativeMethods.RmStartSession(out _sessionHandle, 0, Guid.NewGuid().ToString());
            if (ret != 0) throw new Win32Exception(ret);
        }

        /// <summary>
        /// Ends the Restart Manager session.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            DisposeNative();
        }

        /// <inheritdoc/>
        ~WindowsRestartManager()
        {
            DisposeNative();
        }

        private void DisposeNative()
        {
            int ret = UnsafeNativeMethods.RmEndSession(_sessionHandle);
            if (ret != 0) Log.Debug(new Win32Exception(ret));
        }
        #endregion

        #region Resources
        /// <summary>
        /// Registers resources to the Restart Manager session. The Restart Manager uses the list of resources registered with the session to determine which applications and services must be shut down and restarted.
        /// </summary>
        /// <param name="files">An array of full filename paths.</param>
        /// <exception cref="Win32Exception">The restart manager API returned an error.</exception>
        [PublicAPI]
        public void RegisterResources([NotNull, ItemNotNull] params string[] files)
        {
            #region Sanity checks
            if (files == null) throw new ArgumentNullException("files");
            #endregion

            int ret = UnsafeNativeMethods.RmRegisterResources(_sessionHandle, (uint)files.Length, files, 0, new UnsafeNativeMethods.RM_UNIQUE_PROCESS[0], 0, new string[0]);
            if (ret != 0) throw new Win32Exception(ret);
        }
        #endregion

        #region List
        /// <summary>
        /// Gets a list of all applications that are currently using resources that have been registered with <see cref="RegisterResources"/>.
        /// </summary>
        [PublicAPI]
        public string[] ListApps()
        {
            uint arrayLength;
            UnsafeNativeMethods.RM_REBOOT_REASON rebootReasons;
            var apps = ListAppsInternal(out arrayLength, out rebootReasons);

            var names = new string[arrayLength];
            for (int i = 0; i < arrayLength; i++)
                names[i] = apps[i].strAppName;
            return names;
        }

        /// <summary>
        /// Gets a list of all applications that have caused problems with <see cref="ShutdownApps"/> or <see cref="RestartApps"/>.
        /// </summary>
        /// <param name="permissionDenied">Indicates whether trying again as administrator may help.</param>
        private IEnumerable<string> ListAppProblems(out bool permissionDenied)
        {
            uint arrayLength;
            UnsafeNativeMethods.RM_REBOOT_REASON rebootReasons;
            var apps = ListAppsInternal(out arrayLength, out rebootReasons);

            permissionDenied = rebootReasons == UnsafeNativeMethods.RM_REBOOT_REASON.RmRebootReasonPermissionDenied;

            var names = new List<string>();
            for (int i = 0; i < arrayLength; i++)
            {
                if (apps[i].AppStatus == UnsafeNativeMethods.RM_APP_STATUS.RmStatusErrorOnStop || apps[i].AppStatus == UnsafeNativeMethods.RM_APP_STATUS.RmStatusErrorOnRestart)
                    names.Add(apps[i].strAppName);
            }
            return names;
        }

        private UnsafeNativeMethods.RM_PROCESS_INFO[] ListAppsInternal(out uint arrayLength, out UnsafeNativeMethods.RM_REBOOT_REASON rebootReasons)
        {
            int ret;
            uint arrayLengthNeeded = 1;
            UnsafeNativeMethods.RM_PROCESS_INFO[] processInfo;
            do
            {
                arrayLength = arrayLengthNeeded;
                processInfo = new UnsafeNativeMethods.RM_PROCESS_INFO[arrayLength];
                ret = UnsafeNativeMethods.RmGetList(_sessionHandle, out arrayLengthNeeded, ref arrayLength, processInfo, out rebootReasons);
            } while (ret == WindowsUtils.Win32MoreData);

            if (ret == WindowsUtils.Win32Cancelled) throw new OperationCanceledException();
            else if (ret != 0) throw new Win32Exception(ret);

            return processInfo;
        }
        #endregion

        #region Shutdown
        /// <summary>
        /// Initiates the shutdown of applications that are currently using resources that have been registered with <see cref="RegisterResources"/>.
        /// </summary>
        /// <param name="handler">A callback object used to report progress to the user and allow cancellation.</param>
        /// <exception cref="UnauthorizedAccessException">One or more applications could not be shut down. Trying again as administrator may help.</exception>
        /// <exception cref="IOException">One or more applications could not be shut down. A system reboot may be required.</exception>
        /// <exception cref="Win32Exception">The restart manager API returned an error.</exception>
        [PublicAPI]
        public void ShutdownApps([NotNull] ITaskHandler handler)
        {
            #region Sanity checks
            if (handler == null) throw new ArgumentNullException("handler");
            #endregion

            handler.RunTask(new SimplePercentTask(Resources.ShuttingDownApps, ShutdownAppsWork, CancellationCallback));
        }

        private void ShutdownAppsWork(PercentProgressCallback progressCallback)
        {
            int ret = UnsafeNativeMethods.RmShutdown(_sessionHandle, UnsafeNativeMethods.RM_SHUTDOWN_TYPE.RmForceShutdown, progressCallback);
            if (ret != 0) throw BuildException(ret);
        }
        #endregion

        #region Restart
        /// <summary>
        /// Restarts applications that have been shut down by <see cref="ShutdownApps"/> and that have been registered to be restarted.
        /// </summary>
        /// <param name="handler">A callback object used to report progress to the user and allow cancellation.</param>
        /// <exception cref="IOException">One or more applications could not be automatically restarted.</exception>
        /// <exception cref="Win32Exception">The restart manager API returned an error.</exception>
        [PublicAPI]
        public void RestartApps([NotNull] ITaskHandler handler)
        {
            #region Sanity checks
            if (handler == null) throw new ArgumentNullException("handler");
            #endregion

            handler.RunTask(new SimplePercentTask(Resources.RestartingApps, RestartAppsWork, CancellationCallback));
        }

        private void RestartAppsWork(PercentProgressCallback progressCallback)
        {
            int ret = UnsafeNativeMethods.RmRestart(_sessionHandle, 0, progressCallback);
            if (ret != 0) throw BuildException(ret);
        }
        #endregion

        #region Helpers
        private void CancellationCallback()
        {
            UnsafeNativeMethods.RmCancelCurrentTask(_sessionHandle);
        }

        private Exception BuildException(int error)
        {
            switch (error)
            {
                case WindowsUtils.Win32Cancelled:
                    return new OperationCanceledException();

                case UnsafeNativeMethods.ERROR_FAIL_NOACTION_REBOOT:
                case UnsafeNativeMethods.ERROR_FAIL_SHUTDOWN:
                case UnsafeNativeMethods.ERROR_FAIL_RESTART:
                    bool permissionDenied;
                    string message = new Win32Exception(error).Message + Environment.NewLine + StringUtils.Join(Environment.NewLine, ListAppProblems(out permissionDenied));

                    if (permissionDenied) return new UnauthorizedAccessException(message);
                    else return new IOException(message);

                default:
                    return new Win32Exception(error);
            }
        }
        #endregion
    }
}
