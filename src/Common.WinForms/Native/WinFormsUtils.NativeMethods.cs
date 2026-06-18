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
    }
}
