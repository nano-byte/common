// Copyright Bastian Eicher
// Licensed under the MIT License

#if NETFRAMEWORK
using System.Security.Permissions;
#endif

namespace NanoByte.Common.Controls.Touch;

/// <summary>
/// A window that reacts to touch gestures.
/// </summary>
public class TouchForm : Form, ITouchControl
{
    private readonly TouchInputProvider _touch;

    /// <summary>
    /// Creates a new touch form.
    /// </summary>
    public TouchForm()
    {
        _touch = new(this);
    }

    /// <inheritdoc/>
    [DefaultValue(false)]
    public bool DoubleTapEnabled
    {
        get => _touch.DoubleTapEnabled;
        set => _touch.DoubleTapEnabled = value;
    }

    /// <inheritdoc/>
    public event EventHandler<TappedEventArgs> Tapped
    {
        add => _touch.Tapped += value;
        remove => _touch.Tapped -= value;
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
