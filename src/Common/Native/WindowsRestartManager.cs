// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using NanoByte.Common.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.RestartManager;

namespace NanoByte.Common.Native;

/// <summary>
/// Provides an interface to the Windows Restart Manager. Supported on Windows Vista or newer.
/// </summary>
/// <remarks>
/// See https://msdn.microsoft.com/en-us/library/windows/desktop/cc948910
/// </remarks>
[SupportedOSPlatform("windows6.0")]
[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "CsWin32 annotates exact Windows builds, versions are checked at runtime with IsWindows*")]
public sealed class WindowsRestartManager : MarshalByRefObject, IDisposable
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
    private readonly uint _sessionHandle;

    /// <summary>
    /// Starts a new Restart Manager session.
    /// </summary>
    /// <exception cref="Win32Exception">The Restart Manager API returned an error.</exception>
    /// <exception cref="PlatformNotSupportedException">The current platform does not support the Restart Manager. Needs Windows Vista or newer.</exception>
    /// <exception cref="DllNotFoundException">The Restart Manager API is missing.</exception>
    public WindowsRestartManager()
    {
        if (!WindowsUtils.IsWindowsVista) throw new PlatformNotSupportedException();

        Span<char> sessionKey = stackalloc char[(int)PInvoke.CCH_RM_SESSION_KEY + 1];
        var ret = PInvoke.RmStartSession(out _sessionHandle, sessionKey);
        if (ret != WIN32_ERROR.NO_ERROR) throw BuildException((int)ret);
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
        if (_sessionHandle == default) return; // Might be unset due to exception thrown in constructor

        var ret = PInvoke.RmEndSession(_sessionHandle);
        if (ret != WIN32_ERROR.NO_ERROR) Log.Debug("Failed to end Windows Restart Manager session", BuildException((int)ret));
    }

    private void CancellationCallback() => PInvoke.RmCancelCurrentTask(_sessionHandle);
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

        var ret = PInvoke.RmRegisterResources(_sessionHandle, files, default, default);
        if (ret != WIN32_ERROR.NO_ERROR) throw BuildException((int)ret);
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
                names[i] = apps[i].strAppName.ToString();
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

        permissionDenied = rebootReasons == RM_REBOOT_REASON.RmRebootReasonPermissionDenied;

        var names = new List<string>();
        for (int i = 0; i < arrayLength; i++)
        {
            if (apps[i].AppStatus is RM_APP_STATUS.RmStatusErrorOnStop or RM_APP_STATUS.RmStatusErrorOnRestart)
                names.Add(apps[i].strAppName.ToString());
        }
        return names;
    }

    private RM_PROCESS_INFO[] ListAppsInternal(out uint arrayLength, out RM_REBOOT_REASON rebootReasons)
    {
        WIN32_ERROR ret;
        uint arrayLengthNeeded = 1;
        RM_PROCESS_INFO[] processInfo;
        do
        {
            arrayLength = arrayLengthNeeded;
            processInfo = new RM_PROCESS_INFO[arrayLength];
            ret = PInvoke.RmGetList(_sessionHandle, out arrayLengthNeeded, ref arrayLength, processInfo, out uint reasons);
            rebootReasons = (RM_REBOOT_REASON)reasons;
        } while (ret == WIN32_ERROR.ERROR_MORE_DATA);

        if (ret != WIN32_ERROR.NO_ERROR) throw BuildException((int)ret);

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

        handler.RunTask(new PercentageTask(Resources.ShuttingDownApps, ShutdownAppsWork, CancellationCallback));
    }

    private unsafe void ShutdownAppsWork(PercentProgressCallback progressCallback)
    {
        void Shutdown(bool force)
        {
            WriteStatusCallback callback = percent => progressCallback((int)percent);
            var ret = PInvoke.RmShutdown(_sessionHandle, (uint)(force ? RM_SHUTDOWN_TYPE.RmForceShutdown : 0), ToFunctionPointer(callback));
            GC.KeepAlive(callback);
            if (ret != WIN32_ERROR.NO_ERROR) throw BuildException((int)ret);
        }

        try
        {
            ExceptionUtils.Retry<IOException>(() => Shutdown(false));
        }
        catch (IOException)
        {
            Shutdown(force: true);
        }
    }
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

        handler.RunTask(new PercentageTask(Resources.RestartingApps, RestartAppsWork, CancellationCallback));
    }

    private unsafe void RestartAppsWork(PercentProgressCallback progressCallback)
    {
        WriteStatusCallback callback = percent => progressCallback((int)percent);
        var ret = PInvoke.RmRestart(_sessionHandle, ToFunctionPointer(callback));
        GC.KeepAlive(callback);
        if (ret != WIN32_ERROR.NO_ERROR) throw BuildException((int)ret);
    }
    #endregion

    #region Callback
    /// <summary>RM_WRITE_STATUS_CALLBACK</summary>
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void WriteStatusCallback(uint percentComplete);

    /// <summary>
    /// Gets a native function pointer for a callback. The caller must keep <paramref name="callback"/> alive until native code stops using the pointer.
    /// </summary>
    private static unsafe delegate* unmanaged[Stdcall]<uint, void> ToFunctionPointer(WriteStatusCallback callback)
        => (delegate* unmanaged[Stdcall]<uint, void>)(void*)Marshal.GetFunctionPointerForDelegate(callback);
    #endregion
}
