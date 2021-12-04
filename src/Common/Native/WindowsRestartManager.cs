// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;
using NanoByte.Common.Properties;
using NanoByte.Common.Tasks;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides an interface to the Windows Restart Manager. Supported on Windows Vista or newer.
    /// </summary>
    /// <remarks>
    /// See https://msdn.microsoft.com/en-us/library/windows/desktop/cc948910
    /// </remarks>
    [SupportedOSPlatform("windows6.0")]
    public sealed partial class WindowsRestartManager : MarshalByRefObject, IDisposable
    {
        #region Win32 Error Codes
        private const int Win32ErrorFailNoactionReboot = 350;
        private const int Win32ErrorFailShutdown = 351;
        private const int Win32ErrorFailRestart = 352;

        /// <summary>
        /// Builds a suitable <see cref="Exception"/> for a given <see cref="Win32Exception.NativeErrorCode"/>.
        /// </summary>
        private Exception BuildException(int error)
        {
            switch (error)
            {
                case Win32ErrorFailNoactionReboot:
                case Win32ErrorFailShutdown:
                case Win32ErrorFailRestart:
                    string message = new Win32Exception(error).Message + Environment.NewLine + StringUtils.Join(Environment.NewLine, ListAppProblems(out bool permissionDenied));

                    if (permissionDenied) return new UnauthorizedAccessException(message);
                    else return new IOException(message);

                default:
                    return WindowsUtils.BuildException(error);
            }
        }
        #endregion

        #region Session
        private readonly IntPtr _sessionHandle;

        /// <summary>
        /// Starts a new Restart Manager session.
        /// </summary>
        /// <exception cref="Win32Exception">The Restart Manager API returned an error.</exception>
        /// <exception cref="PlatformNotSupportedException">The current platform does not support the Restart Manager. Needs Windows Vista or newer.</exception>
        public WindowsRestartManager()
        {
            if (!WindowsUtils.IsWindowsVista) throw new PlatformNotSupportedException();

            int ret = NativeMethods.RmStartSession(out _sessionHandle, 0, Guid.NewGuid().ToString());
            if (ret != 0) throw BuildException(ret);
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
            int ret = NativeMethods.RmEndSession(_sessionHandle);
            if (ret != 0) Log.Debug(BuildException(ret));
        }

        private void CancellationCallback() => NativeMethods.RmCancelCurrentTask(_sessionHandle);
        #endregion

        #region Resources
        /// <summary>
        /// Registers resources to the Restart Manager session. The Restart Manager uses the list of resources registered with the session to determine which applications and services must be shut down and restarted.
        /// </summary>
        /// <param name="files">An array of full filename paths.</param>
        /// <exception cref="Win32Exception">The Restart Manager API returned an error.</exception>
        public void RegisterResources(params string[] files)
        {
            #region Sanity checks
            if (files == null) throw new ArgumentNullException(nameof(files));
            #endregion

            int ret = NativeMethods.RmRegisterResources(_sessionHandle, (uint)files.Length, files, 0, new NativeMethods.RM_UNIQUE_PROCESS[0], 0, new string[0]);
            if (ret != 0) throw BuildException(ret);
        }
        #endregion

        #region List
        /// <summary>
        /// Gets a list of all applications that are currently using resources that have been registered with <see cref="RegisterResources"/>.
        /// </summary>
        /// <param name="cancellationToken">Used to signal cancellation requests.</param>
        /// <exception cref="IOException">The Restart Manager could not access the registry.</exception>
        /// <exception cref="TimeoutException">The Restart Manager could not obtain a Registry write mutex in the allotted time. A system restart is recommended.</exception>
        /// <exception cref="Win32Exception">The Restart Manager API returned an error.</exception>
        public string[] ListApps(CancellationToken cancellationToken = default)
        {
            Log.Debug(Resources.SearchingFileReferences);

            using (cancellationToken.Register(CancellationCallback))
            {
                var apps = ListAppsInternal(out uint arrayLength, out _);

                string[] names = new string[arrayLength];
                for (int i = 0; i < arrayLength; i++)
                    names[i] = apps[i].strAppName;
                return names;
            }
        }

        /// <summary>
        /// Gets a list of all applications that have caused problems with <see cref="ShutdownApps"/> or <see cref="RestartApps"/>.
        /// </summary>
        /// <param name="permissionDenied">Indicates whether trying again as administrator may help.</param>
        /// <exception cref="Win32Exception">The Restart Manager API returned an error.</exception>
        private IEnumerable<string> ListAppProblems(out bool permissionDenied)
        {
            var apps = ListAppsInternal(out uint arrayLength, out var rebootReasons);

            permissionDenied = rebootReasons == NativeMethods.RM_REBOOT_REASON.RmRebootReasonPermissionDenied;

            var names = new List<string>();
            for (int i = 0; i < arrayLength; i++)
            {
                if (apps[i].AppStatus == NativeMethods.RM_APP_STATUS.RmStatusErrorOnStop || apps[i].AppStatus == NativeMethods.RM_APP_STATUS.RmStatusErrorOnRestart)
                    names.Add(apps[i].strAppName);
            }
            return names;
        }

        private NativeMethods.RM_PROCESS_INFO[] ListAppsInternal(out uint arrayLength, out NativeMethods.RM_REBOOT_REASON rebootReasons)
        {
            int ret;
            uint arrayLengthNeeded = 1;
            NativeMethods.RM_PROCESS_INFO[] processInfo;
            do
            {
                arrayLength = arrayLengthNeeded;
                processInfo = new NativeMethods.RM_PROCESS_INFO[arrayLength];
                ret = NativeMethods.RmGetList(_sessionHandle, out arrayLengthNeeded, ref arrayLength, processInfo, out rebootReasons);
            } while (ret == WindowsUtils.Win32ErrorMoreData);

            if (ret != 0) throw BuildException(ret);

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
        /// <exception cref="Win32Exception">The Restart Manager API returned an error.</exception>
        public void ShutdownApps(ITaskHandler handler)
        {
            #region Sanity checks
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            #endregion

            handler.RunTask(new SimplePercentTask(Resources.ShuttingDownApps, ShutdownAppsWork, CancellationCallback));
        }

        private void ShutdownAppsWork(PercentProgressCallback progressCallback)
            => ExceptionUtils.Retry<IOException>(lastAttempt =>
            {
                int ret = NativeMethods.RmShutdown(_sessionHandle, lastAttempt ? NativeMethods.RM_SHUTDOWN_TYPE.RmForceShutdown : 0, progressCallback);
                if (ret != 0) throw BuildException(ret);
            }, maxRetries: 3);
        #endregion

        #region Restart
        /// <summary>
        /// Restarts applications that have been shut down by <see cref="ShutdownApps"/> and that have been registered to be restarted.
        /// </summary>
        /// <param name="handler">A callback object used to report progress to the user and allow cancellation.</param>
        /// <exception cref="IOException">One or more applications could not be automatically restarted.</exception>
        /// <exception cref="Win32Exception">The Restart Manager API returned an error.</exception>
        public void RestartApps(ITaskHandler handler)
        {
            #region Sanity checks
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            #endregion

            handler.RunTask(new SimplePercentTask(Resources.RestartingApps, RestartAppsWork, CancellationCallback));
        }

        private void RestartAppsWork(PercentProgressCallback progressCallback)
        {
            int ret = NativeMethods.RmRestart(_sessionHandle, 0, progressCallback);
            if (ret != 0) throw BuildException(ret);
        }
        #endregion
    }
}
