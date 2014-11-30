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
using System.Drawing;
using System.Runtime.InteropServices;

namespace NanoByte.Common.Native
{
    partial class WindowsUtils
    {
        #region Structures
        [StructLayout(LayoutKind.Sequential)]
        private struct WinMessage
        {
// ReSharper disable InconsistentNaming
            public IntPtr hWnd;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point p;
// ReSharper restore InconsistentNaming
        }
        #endregion

        /// <summary>
        /// Determines whether this application is currently idle.
        /// </summary>
        /// <returns><see langword="true"/> if idle, <see langword="false"/> if handling window events.</returns>
        /// <remarks>Will always return <see langword="true"/> on non-Windows OSes.</remarks>
        public static bool AppIdle
        {
            get
            {
                if (IsWindows)
                {
                    WinMessage msg;
                    return !SafeNativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
                }
                return true; // Not supported on non-Windows OSes
            }
        }

        /// <summary>
        /// Text-box caret blink time in seconds.
        /// </summary>
        public static float CaretBlinkTime
        {
            get
            {
                return IsWindows
                    ? SafeNativeMethods.GetCaretBlinkTime() / 1000f
                    : 0.5f; // Default to 0.5 seconds on non-Windows OSes
            }
        }

        /// <summary>
        /// Prevents the mouse cursor from leaving a specific window.
        /// </summary>
        /// <param name="handle">The handle to the window to lock the mouse cursor into.</param>
        /// <returns>A handle to the window that had previously captured the mouse</returns>
        /// <remarks>Will do nothing on non-Windows OSes.</remarks>
        public static IntPtr SetCapture(IntPtr handle)
        {
            return IsWindows ? UnsafeNativeMethods.SetCapture(handle) : IntPtr.Zero;
        }

        /// <summary>
        /// Releases the mouse cursor after it was locked by <see cref="SetCapture"/>.
        /// </summary>
        /// <returns><see langword="true"/> if successful; <see langword="false"/> otherwise.</returns>
        /// <remarks>Will always return <see langword="false"/> on non-Windows OSes.</remarks>
        public static bool ReleaseCapture()
        {
            return IsWindows ? UnsafeNativeMethods.ReleaseCapture() : false;
        }

        //--------------------//

        private static long _performanceFrequency;

        /// <summary>
        /// A time index in seconds that continuously increases.
        /// </summary>
        /// <remarks>Depending on the operating system this may be the time of the system clock or the time since the system booted.</remarks>
        public static double AbsoluteTime
        {
            get
            {
                // Use high-accuracy kernel timing methods on NT
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    if (_performanceFrequency == 0)
                        SafeNativeMethods.QueryPerformanceFrequency(out _performanceFrequency);

                    long time;
                    SafeNativeMethods.QueryPerformanceCounter(out time);
                    return time / (double)_performanceFrequency;
                }

                return Environment.TickCount / 1000f;
            }
        }
    }
}
