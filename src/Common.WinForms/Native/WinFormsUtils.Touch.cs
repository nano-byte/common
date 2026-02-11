// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;

namespace NanoByte.Common.Native;

partial class WinFormsUtils
{
    // Note: The following code is based on Windows 7 Touch Gesture API
    // See: https://github.com/microsoft/Windows-classic-samples/blob/main/Samples/Win7Samples/Touch/MTGestures/CS/MTGestures.cs

    private static readonly int _gestureConfigSize = Marshal.SizeOf(typeof(NativeMethods.GestureConfig));

    /// <summary>
    /// Registers a control to receive gesture events.
    /// This method is kept for API compatibility but gesture configuration is done automatically.
    /// </summary>
    /// <param name="control">The control to register.</param>
    public static void RegisterGestureWindow(Control control)
    {
        #region Sanity checks
        if (control == null) throw new ArgumentNullException(nameof(control));
        #endregion

        // Gesture configuration is done in response to WM_GESTURENOTIFY
        // No registration call is needed upfront
    }

    /// <summary>
    /// Configures which gestures are enabled for a window.
    /// </summary>
    /// <param name="hWnd">Handle to the window.</param>
    public static void ConfigureGestures(IntPtr hWnd)
    {
        if (!WindowsUtils.IsWindows7) return;

        var gc = new NativeMethods.GestureConfig
        {
            dwID = 0,                                  // gesture ID
            dwWant = NativeMethods.GestureConfigAll,  // enable all gestures
            dwBlock = 0                                // block no gestures
        };

        NativeMethods.SetGestureConfig(
            hWnd,
            0,
            1,
            ref gc,
            _gestureConfigSize);
    }

    /// <summary>
    /// Handles gesture-related <see cref="Control.WndProc"/> <see cref="Message"/>s.
    /// </summary>
    /// <param name="m">The message to handle.</param>
    /// <param name="sender">The object to send possible events from.</param>
    /// <param name="onPan">The event handler to call for pan gestures; can be <c>null</c>.</param>
    /// <param name="onZoom">The event handler to call for zoom gestures; can be <c>null</c>.</param>
    /// <param name="onRotate">The event handler to call for rotate gestures; can be <c>null</c>.</param>
    /// <param name="onTap">The event handler to call for two-finger tap gestures; can be <c>null</c>.</param>
    /// <param name="onPressAndTap">The event handler to call for press and tap gestures; can be <c>null</c>.</param>
    /// <returns><c>true</c> if the message was handled; otherwise <c>false</c>.</returns>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static bool HandleGestureMessage(
        ref Message m,
        object? sender,
        EventHandler<PanGestureEventArgs>? onPan,
        EventHandler<ZoomGestureEventArgs>? onZoom,
        EventHandler<RotateGestureEventArgs>? onRotate,
        EventHandler<TapGestureEventArgs>? onTap,
        EventHandler<PressAndTapGestureEventArgs>? onPressAndTap)
    {
        const int WM_GESTURENOTIFY = 0x011A;
        const int WM_GESTURE = 0x0119;

        if (!WindowsUtils.IsWindows7) return false;

        if (m.Msg == WM_GESTURENOTIFY)
        {
            ConfigureGestures(m.HWnd);
            return true;
        }

        if (m.Msg != WM_GESTURE) return false;

        var gi = new NativeMethods.GestureInfo { cbSize = Marshal.SizeOf(typeof(NativeMethods.GestureInfo)) };

        if (!NativeMethods.GetGestureInfo(m.LParam, ref gi))
            return false;

        // Convert screen coordinates to client coordinates
        var location = Control.FromHandle(m.HWnd)?.PointToClient(new Point(gi.ptsLocation.x, gi.ptsLocation.y)) ?? new Point(gi.ptsLocation.x, gi.ptsLocation.y);

        var flags = (GestureFlags)gi.dwFlags;

        switch (gi.dwID)
        {
            case NativeMethods.GestureIdBegin:
            case NativeMethods.GestureIdEnd:
                // These are informational only
                break;

            case NativeMethods.GestureIdPan:
                if (onPan != null)
                {
                    // ullArguments contains the total pan distance as two 32-bit signed integers:
                    // Lower 32 bits = X distance (horizontal pan)
                    // Upper 32 bits = Y distance (vertical pan)
                    var args = new PanGestureEventArgs
                    {
                        LocationX = location.X,
                        LocationY = location.Y,
                        Flags = flags,
                        SequenceId = gi.dwSequenceID,
                        PanDistanceX = (int)(gi.ullArguments & 0xFFFFFFFF),
                        PanDistanceY = (int)((gi.ullArguments >> 32) & 0xFFFFFFFF)
                    };
                    onPan(sender, args);
                }
                break;

            case NativeMethods.GestureIdZoom:
                if (onZoom != null)
                {
                    var args = new ZoomGestureEventArgs
                    {
                        LocationX = location.X,
                        LocationY = location.Y,
                        Flags = flags,
                        SequenceId = gi.dwSequenceID,
                        Distance = gi.ullArguments
                    };
                    onZoom(sender, args);
                }
                break;

            case NativeMethods.GestureIdRotate:
                if (onRotate != null)
                {
                    var args = new RotateGestureEventArgs
                    {
                        LocationX = location.X,
                        LocationY = location.Y,
                        Flags = flags,
                        SequenceId = gi.dwSequenceID,
                        Angle = ArgToRadians(gi.ullArguments)
                    };
                    onRotate(sender, args);
                }
                break;

            case NativeMethods.GestureIdTwoFingerTap:
                if (onTap != null)
                {
                    var args = new TapGestureEventArgs
                    {
                        LocationX = location.X,
                        LocationY = location.Y,
                        Flags = flags,
                        SequenceId = gi.dwSequenceID
                    };
                    onTap(sender, args);
                }
                break;

            case NativeMethods.GestureIdPressAndTap:
                if (onPressAndTap != null)
                {
                    var args = new PressAndTapGestureEventArgs
                    {
                        LocationX = location.X,
                        LocationY = location.Y,
                        Flags = flags,
                        SequenceId = gi.dwSequenceID
                    };
                    onPressAndTap(sender, args);
                }
                break;
        }

        return true;
    }

    /// <summary>
    /// Converts from "binary radians" to traditional radians.
    /// </summary>
    private static double ArgToRadians(long arg)
    {
        return ((arg / 65535.0) * 4.0 * Math.PI) - 2.0 * Math.PI;
    }
}
