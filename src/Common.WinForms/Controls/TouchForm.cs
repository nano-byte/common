// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Native;

#if NETFRAMEWORK
using System.Security.Permissions;
#endif

namespace NanoByte.Common.Controls;

/// <summary>
/// Represents a window that reacts to touch gestures on Windows 7 or newer.
/// </summary>
public class TouchForm : Form, ITouchControl
{
    /// <inheritdoc/>
    public event EventHandler<PanGestureEventArgs>? Pan;

    /// <inheritdoc/>
    public event EventHandler<ZoomGestureEventArgs>? Zoom;

    /// <inheritdoc/>
    public event EventHandler<RotateGestureEventArgs>? Rotate;

    /// <inheritdoc/>
    public event EventHandler<TapGestureEventArgs>? Tap;

    /// <inheritdoc/>
    public event EventHandler<PressAndTapGestureEventArgs>? PressAndTap;

    protected override CreateParams CreateParams
    {
#if NETFRAMEWORK
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
#endif
        get
        {
            var createParams = base.CreateParams;
            createParams.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
            return createParams;
        }
    }

    protected override void CreateHandle()
    {
        base.CreateHandle();
        WinFormsUtils.RegisterGestureWindow(this);
    }

#if NETFRAMEWORK
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
#endif
    protected override void WndProc(ref Message m)
    {
        bool handled = WinFormsUtils.HandleGestureMessage(ref m, this, Pan, Zoom, Rotate, Tap, PressAndTap);
        base.WndProc(ref m);
        if (handled)
            m.Result = new IntPtr(1);
    }
}
