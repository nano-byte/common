// Copyright Bastian Eicher
// Licensed under the MIT License

#if NETFRAMEWORK
using System.Security.Permissions;
#endif

namespace NanoByte.Common.Controls.Touch;

/// <summary>
/// A panel that reacts to touch gestures. Single finger taps are still reported as mouse click events.
/// </summary>
public class TouchPanel : Panel, ITouchControl
{
    private readonly TouchEventSource _touch;

    /// <summary>
    /// Creates a new touch panel.
    /// </summary>
    public TouchPanel()
    {
        _touch = new(this);
    }

    /// <inheritdoc/>
    public event EventHandler<ManipulationEventArgs> ManipulationStarted
    {
        add => _touch.ManipulationStarted += value;
        remove => _touch.ManipulationStarted -= value;
    }

    /// <inheritdoc/>
    public event EventHandler<ManipulationEventArgs> ManipulationUpdated
    {
        add => _touch.ManipulationUpdated += value;
        remove => _touch.ManipulationUpdated -= value;
    }

    /// <inheritdoc/>
    public event EventHandler<ManipulationEventArgs> ManipulationCompleted
    {
        add => _touch.ManipulationCompleted += value;
        remove => _touch.ManipulationCompleted -= value;
    }

#if NETFRAMEWORK
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
#endif
    protected override void WndProc(ref Message m)
    {
        if (_touch.ProcessMessage(ref m)) return;
        base.WndProc(ref m);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _touch.Dispose();
        base.Dispose(disposing);
    }
}
