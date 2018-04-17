// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using NanoByte.Common.Tasks;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace NanoByte.Common.Native
{
    partial class WindowsRestartManager
    {
        [SuppressMessage("ReSharper", "InconsistentNaming"), SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
        private static class NativeMethods
        {
            [DllImport("rstrtmgr", CharSet = CharSet.Unicode)]
            public static extern int RmStartSession(out IntPtr pSessionHandle, int dwSessionFlags, string strSessionKey);

            [DllImport("rstrtmgr")]
            public static extern int RmEndSession(IntPtr pSessionHandle);

            [StructLayout(LayoutKind.Sequential)]
            public struct RM_UNIQUE_PROCESS
            {
                public int dwProcessId;
                public FILETIME ProcessStartTime;
            }

            [DllImport("rstrtmgr", CharSet = CharSet.Unicode)]
            public static extern int RmRegisterResources(IntPtr pSessionHandle, uint nFiles, string[] rgsFilenames, uint nApplications, RM_UNIQUE_PROCESS[] rgApplications, uint nServices, string[] rgsServiceNames);

            public enum RM_APP_TYPE : uint
            {
                RmUnknownApp = 0,
                RmMainWindow = 1,
                RmOtherWindow = 2,
                RmService = 3,
                RmExplorer = 4,
                RmConsole = 5,
                RmCritical = 1000
            }

            [Flags]
            public enum RM_APP_STATUS : uint
            {
                RmStatusUnknown = 0x0,
                RmStatusRunning = 0x1,
                RmStatusStopped = 0x2,
                RmStatusStoppedOther = 0x4,
                RmStatusRestarted = 0x8,
                RmStatusErrorOnStop = 0x10,
                RmStatusErrorOnRestart = 0x20,
                RmStatusShutdownMasked = 0x40,
                RmStatusRestartMasked = 0x80
            }

            private const int CCH_RM_MAX_APP_NAME = 255;
            private const int CCH_RM_MAX_SVC_NAME = 63;

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct RM_PROCESS_INFO
            {
                public RM_UNIQUE_PROCESS Process;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
                public string strAppName;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
                public string strServiceShortName;

                public RM_APP_TYPE ApplicationType;
                public RM_APP_STATUS AppStatus;
                public uint TSSessionId;

                [MarshalAs(UnmanagedType.Bool)]
                public bool bRestartable;
            }

            [Flags]
            public enum RM_REBOOT_REASON : uint
            {
                RmRebootReasonNone = 0x0,
                RmRebootReasonPermissionDenied = 0x1,
                RmRebootReasonSessionMismatch = 0x2,
                RmRebootReasonCriticalProcess = 0x4,
                RmRebootReasonCriticalService = 0x8,
                RmRebootReasonDetectedSelf = 0x10
            }

            [DllImport("rstrtmgr")]
            public static extern int RmGetList(IntPtr dwSessionHandle, out uint pnProcInfoNeeded, ref uint pnProcInfo, [In, Out] RM_PROCESS_INFO[] rgAffectedApps, out RM_REBOOT_REASON lpdwRebootReasons);

            [Flags]
            public enum RM_SHUTDOWN_TYPE : uint
            {
                RmForceShutdown = 0x1,
                RmShutdownOnlyRegistered = 0x10
            }

            [DllImport("rstrtmgr")]
            public static extern int RmShutdown(IntPtr pSessionHandle, RM_SHUTDOWN_TYPE lActionFlags, PercentProgressCallback fnStatus);

            [DllImport("rstrtmgr")]
            public static extern int RmRestart(IntPtr pSessionHandle, int dwRestartFlags, PercentProgressCallback fnStatus);

            [DllImport("rstrtmgr")]
            public static extern int RmCancelCurrentTask(IntPtr pSessionHandle);
        }
    }
}
