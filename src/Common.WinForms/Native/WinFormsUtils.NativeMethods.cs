// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;

namespace NanoByte.Common.Native;

partial class WinFormsUtils
{
    [SuppressUnmanagedCodeSecurity]
    private static class SafeNativeMethods
    {
        [DllImport("user32")]
        public static extern short GetAsyncKeyState(uint key);

        [DllImport("user32")]
        public static extern int GetCaretBlinkTime();
    }

    private static class NativeMethods
    {
        public const int MessageButtonDown = 0xA1; // WM_NCLBUTTONDOWN
        public const int Caption = 0x2; // HT_CAPTION

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local"), SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        public struct WinMessage
        {
            public IntPtr hWnd;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point p;
        }

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage(out WinMessage msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        [DllImport("user32")]
        public static extern IntPtr SetCapture(IntPtr handle);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReleaseCapture();

        // Gesture-related constants
        public const int GestureConfigAll = 0x00000001; // GC_ALLGESTURES

        // Gesture IDs
        public const int GestureIdBegin = 1;
        public const int GestureIdEnd = 2;
        public const int GestureIdZoom = 3;
        public const int GestureIdPan = 4;
        public const int GestureIdRotate = 5;
        public const int GestureIdTwoFingerTap = 6;
        public const int GestureIdPressAndTap = 7;

        // Gesture flags
        public const int GestureFlagBegin = 0x00000001;
        public const int GestureFlagInertia = 0x00000002;
        public const int GestureFlagEnd = 0x00000004;

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        public struct GestureConfig
        {
            public int dwID;    // gesture ID
            public int dwWant;  // settings related to gesture ID that are to be turned on
            public int dwBlock; // settings related to gesture ID that are to be turned off
        }

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        public struct Points
        {
            public short x;
            public short y;
        }

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        public struct GestureInfo
        {
            public int cbSize;              // size, in bytes, of this structure
            public int dwFlags;             // see GF_* flags
            public int dwID;                // gesture ID, see GID_* defines
            public IntPtr hwndTarget;       // handle to window targeted by this gesture
            [MarshalAs(UnmanagedType.Struct)]
            public Points ptsLocation;      // current location of this gesture
            public int dwInstanceID;        // internally used
            public int dwSequenceID;        // internally used
            public long ullArguments;       // arguments for gestures whose arguments fit in 8 BYTES
            public int cbExtraArgs;         // size, in bytes, of extra arguments, if any, that accompany this gesture
        }

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetGestureConfig(IntPtr hWnd, int dwReserved, int cIDs, ref GestureConfig pGestureConfig, int cbSize);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetGestureInfo(IntPtr hGestureInfo, ref GestureInfo pGestureInfo);
    }
}
