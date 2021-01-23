// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NanoByte.Common.Controls;

#if NET20
using NanoByte.Common.Values;
#endif

namespace NanoByte.Common.Native
{
    partial class WinFormsUtils
    {
        // Note: The following code is based on Windows API Code Pack for Microsoft .NET Framework 1.0.1

        /// <summary>
        /// Registers a control as a receiver for touch events.
        /// </summary>
        /// <param name="control">The control to register.</param>
        public static void RegisterTouchWindow(Control control)
        {
            #region Sanity checks
            if (control == null) throw new ArgumentNullException(nameof(control));
            #endregion

            if (WindowsUtils.IsWindows7) NativeMethods.RegisterTouchWindow(control.Handle, 0);
        }

        private static readonly int _touchInputSize = Marshal.SizeOf(new NativeMethods.TouchInput());

        /// <summary>
        /// Handles touch-related <see cref="Control.WndProc"/> <see cref="Message"/>s.
        /// </summary>
        /// <param name="m">The message to handle.</param>
        /// <param name="sender">The object to send possible events from.</param>
        /// <param name="onTouchDown">The event handler to call for touch down events; can be <c>null</c>.</param>
        /// <param name="onTouchMove">The event handler to call for touch move events; can be <c>null</c>.</param>
        /// <param name="onTouchUp">The event handler to call for touch up events; can be <c>null</c>.</param>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static void HandleTouchMessage(ref Message m, object? sender, EventHandler<TouchEventArgs>? onTouchDown, EventHandler<TouchEventArgs>? onTouchMove, EventHandler<TouchEventArgs>? onTouchUp)
        {
            const int WM_TOUCHMOVE = 0x0240, WM_TOUCHDOWN = 0x0241, WM_TOUCHUP = 0x0242;

            if (!WindowsUtils.IsWindows7) return;
            if (m.Msg != WM_TOUCHDOWN && m.Msg != WM_TOUCHMOVE && m.Msg != WM_TOUCHUP) return;

            // More than one touchinput may be associated with a touch message,
            // so an array is needed to get all event information.
            short inputCount = (short)(m.WParam.ToInt32() & 0xffff); // Number of touch inputs, actual per-contact messages

            if (inputCount < 0) return;
            var inputs = new NativeMethods.TouchInput[inputCount];

            // Unpack message parameters into the array of TOUCHINPUT structures, each
            // representing a message for one single contact.
            //Exercise2-Task1-Step3
            if (!NativeMethods.GetTouchInputInfo(m.LParam, inputCount, inputs, _touchInputSize))
                return;

            // For each contact, dispatch the message to the appropriate message
            // handler.
            // Note that for WM_TOUCHDOWN you can get down & move notifications
            // and for WM_TOUCHUP you can get up & move notifications
            // WM_TOUCHMOVE will only contain move notifications
            // and up & down notifications will never come in the same message
            for (int i = 0; i < inputCount; i++)
            {
                NativeMethods.TouchInput ti = inputs[i];

                // Assign a handler to this message.
                EventHandler<TouchEventArgs>? handler = null; // Touch event handler
                if (ti.dwFlags.HasFlag(NativeMethods.TouchEvents.Down)) handler = onTouchDown;
                else if (ti.dwFlags.HasFlag(NativeMethods.TouchEvents.Up)) handler = onTouchUp;
                else if (ti.dwFlags.HasFlag(NativeMethods.TouchEvents.Move)) handler = onTouchMove;

                // Convert message parameters into touch event arguments and handle the event.
                if (handler != null)
                {
                    // TOUCHINFO point coordinates and contact size is in 1/100 of a pixel; convert it to pixels.
                    // Also convert screen to client coordinates.
                    var te = new TouchEventArgs
                    {
                        ContactY = ti.cyContact / 100,
                        ContactX = ti.cxContact / 100,
                        ID = ti.dwID,
                        LocationX = ti.x / 100,
                        LocationY = ti.y / 100,
                        Time = ti.dwTime,
                        Mask = ti.dwMask,
                        InRange = ti.dwFlags.HasFlag(NativeMethods.TouchEvents.InRange),
                        Primary = ti.dwFlags.HasFlag(NativeMethods.TouchEvents.Primary),
                        NoCoalesce = ti.dwFlags.HasFlag(NativeMethods.TouchEvents.NoCoalesce),
                        Palm = ti.dwFlags.HasFlag(NativeMethods.TouchEvents.Palm)
                    };

                    handler(sender, te);

                    m.Result = new IntPtr(1); // Indicate to Windows that the message was handled
                }
            }

            NativeMethods.CloseTouchInputHandle(m.LParam);
        }
    }
}
