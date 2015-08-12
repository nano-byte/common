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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides helper methods and API calls specific to the <see cref="System.Windows.Forms"/> UI toolkit.
    /// </summary>
    public static partial class WinFormsUtils
    {
        /// <summary>
        /// Forces a window to the foreground or flashes the taskbar if another process has the focus.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This method operates only on windows and not on individual controls.")]
        public static void SetForegroundWindow([NotNull] this Form form)
        {
            #region Sanity checks
            if (form == null) throw new ArgumentNullException(nameof(form));
            #endregion

            if (!WindowsUtils.IsWindows) return;
            NativeMethods.SetForegroundWindow(form.Handle);
        }

        /// <summary>
        /// Configures a control to move the entire window when clicked and dragged.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This method operates only on windows and not on individual controls.")]
        public static void EnableWindowDrag([NotNull] this Control control)
        {
            #region Sanity checks
            if (control == null) throw new ArgumentNullException(nameof(control));
            #endregion

            if (!WindowsUtils.IsWindows) return;
            control.MouseDown += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    NativeMethods.ReleaseCapture();

                    var target = control;
                    while (target.Parent != null)
                        target = target.Parent;

                    NativeMethods.SendMessage(target.Handle, NativeMethods.MessageButtonDown, new IntPtr(NativeMethods.Caption), IntPtr.Zero);
                }
            };
        }

        /// <summary>
        /// Adds a UAC shield icon to a button. Does nothing if not running Windows Vista or newer.
        /// </summary>
        /// <remarks>This is purely cosmetic. UAC elevation is a separate concern.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Native API only applies to buttons."), SuppressMessage("ReSharper", "InconsistentNaming")]
        public static void AddShieldIcon([NotNull] this Button button)
        {
            #region Sanity checks
            if (button == null) throw new ArgumentNullException(nameof(button));
            #endregion

            const int BCM_FIRST = 0x1600, BCM_SETSHIELD = 0x000C;

            if (!WindowsUtils.IsWindowsVista) return;
            button.FlatStyle = FlatStyle.System;
            NativeMethods.SendMessage(button.Handle, BCM_FIRST + BCM_SETSHIELD, IntPtr.Zero, new IntPtr(1));
        }

        /// <summary>
        /// Determines whether <paramref name="key"/> is pressed right now.
        /// </summary>
        /// <remarks>Will always return <c>false</c> on non-Windows OSes.</remarks>
        public static bool IsKeyDown(Keys key)
        {
            if (!WindowsUtils.IsWindows) return false;
            return (SafeNativeMethods.GetAsyncKeyState((uint)key) & 0x8000) != 0;
        }

        /// <summary>
        /// Determines whether this application is currently idle.
        /// </summary>
        /// <returns><c>true</c> if idle, <c>false</c> if handling window events.</returns>
        /// <remarks>Will always return <c>true</c> on non-Windows OSes.</remarks>
        public static bool AppIdle
        {
            get
            {
                if (!WindowsUtils.IsWindows) return true;
                NativeMethods.WinMessage msg;
                return !NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        /// <summary>
        /// Text-box caret blink time in seconds.
        /// </summary>
        public static float CaretBlinkTime => WindowsUtils.IsWindows
            ? SafeNativeMethods.GetCaretBlinkTime() / 1000f
            : 0.5f;

        /// <summary>
        /// Prevents the mouse cursor from leaving a specific window.
        /// </summary>
        /// <param name="handle">The handle to the window to lock the mouse cursor into.</param>
        /// <returns>A handle to the window that had previously captured the mouse.</returns>
        /// <remarks>Will do nothing on non-Windows OSes.</remarks>
        public static IntPtr SetCapture(IntPtr handle)
        {
            if (!WindowsUtils.IsWindows) return IntPtr.Zero;
            return NativeMethods.SetCapture(handle);
        }

        /// <summary>
        /// Releases the mouse cursor after it was locked by <see cref="SetCapture"/>.
        /// </summary>
        /// <returns><c>true</c> if successful; <c>false</c> otherwise.</returns>
        /// <remarks>Will always return <c>false</c> on non-Windows OSes.</remarks>
        public static bool ReleaseCapture()
        {
            if (!WindowsUtils.IsWindows) return false;
            return NativeMethods.ReleaseCapture();
        }
    }
}
