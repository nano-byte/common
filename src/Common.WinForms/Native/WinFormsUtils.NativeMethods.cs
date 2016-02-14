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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using JetBrains.Annotations;
using NanoByte.Common.Controls;

namespace NanoByte.Common.Native
{
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

            // ReSharper disable once MemberHidesStaticFromOuterClass
            [DllImport("user32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern IntPtr SetCapture(IntPtr handle);

            // ReSharper disable once MemberHidesStaticFromOuterClass
            [DllImport("user32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ReleaseCapture();

            [DllImport("user32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool RegisterTouchWindow(IntPtr hWnd, uint ulFlags);

            [DllImport("user32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetTouchInputInfo(IntPtr hTouchInput, int cInputs, [In, Out] TouchInput[] pInputs, int cbSize);

            [Flags]
            public enum TouchEvents
            {
                Move = 0x0001, // TOUCHEVENTF_MOVE
                Down = 0x0002, // TOUCHEVENTF_DOWN
                Up = 0x0004, // TOUCHEVENTF_UP
                InRange = 0x0008, // TOUCHEVENTF_INRANGE
                Primary = 0x0010, // TOUCHEVENTF_PRIMARY
                NoCoalesce = 0x0020, // TOUCHEVENTF_NOCOALESCE
                Palm = 0x0080 // TOUCHEVENTF_PALM
            }

            [StructLayout(LayoutKind.Sequential)]
            [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
            public struct TouchInput
            {
                public int x, y;
                public IntPtr hSource;
                public int dwID;
                public TouchEvents dwFlags;
                public TouchEventMask dwMask;
                public int dwTime;
                public IntPtr dwExtraInfo;
                public int cxContact;
                public int cyContact;
            }

            [StructLayout(LayoutKind.Sequential)]
            [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local"), UsedImplicitly]
            private struct Points
            {
                public short x, y;
            }

            [DllImport("user32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern void CloseTouchInputHandle(IntPtr lParam);
        }
    }
}
