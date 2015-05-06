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

        /// <summary>
        /// Gets a list of all applications that are currently using resources that have been registered with <see cref="RegisterResources"/>.
        /// </summary>
        [PublicAPI]
        public string[] ListApps()
        {
            int ret;
            uint arrayLengthNeeded = 1, arrayLength;
            UnsafeNativeMethods.RM_PROCESS_INFO[] processInfo;
            UnsafeNativeMethods.RM_REBOOT_REASON rebootReasons;
            do
            {
                arrayLength = arrayLengthNeeded;
                processInfo = new UnsafeNativeMethods.RM_PROCESS_INFO[arrayLength];
                ret = UnsafeNativeMethods.RmGetList(_sessionHandle, out arrayLengthNeeded, ref arrayLength, processInfo, out rebootReasons);
            } while (ret == UnsafeNativeMethods.ERROR_MORE_DATA);

            if (ret == UnsafeNativeMethods.ERROR_CANCELLED) throw new OperationCanceledException();
            else if (ret != 0) throw new Win32Exception(ret);

            var names = new string[arrayLength];
            for (int i = 0; i < arrayLength; i++)
                names[i] = processInfo[i].strAppName;
            return names;
        }

        /// <summary>
        /// Initiates the shutdown of applications that are currently using resources that have been registered with <see cref="RegisterResources"/>.
        /// </summary>
        /// <param name="handler">A callback object used to report progress to the user and allow cancellation.</param>
        /// <exception cref="UnauthorizedAccessException">One or more applications could not be shut down. A system reboot may be required.</exception>
        /// <exception cref="Win32Exception">The restart manager API returned an error.</exception>
        [PublicAPI]
        public void ShutdownApps([NotNull] ITaskHandler handler)
        {
            #region Sanity checks
            if (handler == null) throw new ArgumentNullException("handler");
            #endregion

            handler.RunTask(new SimplePercentTask(Resources.ShuttingDownApps,
                progressCallback =>
                {
                    int ret = UnsafeNativeMethods.RmShutdown(_sessionHandle, 0, progressCallback);
                    switch (ret)
                    {
                        case 0:
                            break;
                        case UnsafeNativeMethods.ERROR_CANCELLED:
                            throw new OperationCanceledException();
                        case UnsafeNativeMethods.ERROR_FAIL_NOACTION_REBOOT:
                        case UnsafeNativeMethods.ERROR_FAIL_SHUTDOWN:
                            throw new UnauthorizedAccessException(new Win32Exception(ret).Message);
                        default:
                            throw new Win32Exception(ret);
                    }
                },
                cancellationCallback: () => UnsafeNativeMethods.RmCancelCurrentTask(_sessionHandle)));
        }

        /// <summary>
        /// Restarts applications that have been shut down by <see cref="ShutdownApps"/> and that have been registered to be restarted.
        /// </summary>
        /// <param name="handler">A callback object used to report progress to the user and allow cancellation.</param>
        /// <exception cref="UnauthorizedAccessException">One or more applications could not be automatically restarted.</exception>
        /// <exception cref="Win32Exception">The restart manager API returned an error.</exception>
        [PublicAPI]
        public void RestartApps([NotNull] ITaskHandler handler)
        {
            #region Sanity checks
            if (handler == null) throw new ArgumentNullException("handler");
            #endregion

            handler.RunTask(new SimplePercentTask(Resources.RestartingApps,
                progressCallback =>
                {
                    int ret = UnsafeNativeMethods.RmRestart(_sessionHandle, 0, progressCallback);
                    switch (ret)
                    {
                        case 0:
                            break;
                        case UnsafeNativeMethods.ERROR_CANCELLED:
                            throw new OperationCanceledException();
                        case UnsafeNativeMethods.ERROR_FAIL_RESTART:
                            throw new UnauthorizedAccessException(new Win32Exception(ret).Message);
                        default:
                            throw new Win32Exception(ret);
                    }
                },
                cancellationCallback: () => UnsafeNativeMethods.RmCancelCurrentTask(_sessionHandle)));
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
    }
}
