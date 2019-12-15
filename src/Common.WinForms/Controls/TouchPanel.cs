// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Security.Permissions;
using System.Windows.Forms;
using NanoByte.Common.Native;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// Represents a panel that reacts to touch input on Windows 7 or newer.
    /// </summary>
    public class TouchPanel : Panel, ITouchControl
    {
        /// <inheritdoc/>
        public event EventHandler<TouchEventArgs>? TouchDown;

        /// <inheritdoc/>
        public event EventHandler<TouchEventArgs>? TouchUp;

        /// <inheritdoc/>
        public event EventHandler<TouchEventArgs>? TouchMove;

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
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
            WinFormsUtils.RegisterTouchWindow(this);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            WinFormsUtils.HandleTouchMessage(ref m, this, TouchDown, TouchMove, TouchUp);
            base.WndProc(ref m);
        }
    }
}
