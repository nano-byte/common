// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;
using NanoByte.Common.Native;

namespace NanoByte.Common.Controls.Touch;

/// <summary>
/// Translates Windows pointer input on a <see cref="Control"/> into high-level touch gestures. Single finger taps are still reported as mouse click events.
/// </summary>
public sealed partial class TouchEventSource : IDisposable
{
    private const int WM_POINTERUPDATE = 0x0245, WM_POINTERDOWN = 0x0246, WM_POINTERUP = 0x0247, WM_POINTERCAPTURECHANGED = 0x024C;

    private readonly Control _control;
    private readonly bool _enabled;
    private readonly IntPtr _context;
    private readonly System.Windows.Forms.Timer? _inertiaTimer;

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    // Keep the delegate referenced in a field so the GC cannot collect it while native code still holds the function pointer
    private readonly NativeMethods.InteractionOutputCallback? _outputCallback;

    private int _activeContacts;
    private bool _manipulationActive;
    private bool _disposed;

    /// <summary>
    /// Raised when a manipulation gesture begins.
    /// </summary>
    public event EventHandler<ManipulationEventArgs>? ManipulationStarted;

    /// <summary>
    /// Raised when a manipulation gesture changes.
    /// </summary>
    public event EventHandler<ManipulationEventArgs>? ManipulationUpdated;

    /// <summary>
    /// Raised when a manipulation gesture (including any inertia) ends.
    /// </summary>
    public event EventHandler<ManipulationEventArgs>? ManipulationCompleted;

    /// <summary>
    /// Sets up gesture recognition for a control.
    /// </summary>
    /// <param name="control">The control to receive pointer input from. Pass its messages to <see cref="ProcessMessage"/>.</param>
    public TouchEventSource(Control control)
    {
        _control = control ?? throw new ArgumentNullException(nameof(control));
        if (!WindowsUtils.IsWindows8) return;

        if (NativeMethods.CreateInteractionContext(out _context) != 0) return;
        _enabled = true;

        // Operate in screen pixels rather than the default HIMETRIC units
        NativeMethods.SetPropertyInteractionContext(_context, NativeMethods.InteractionContextProperty.MeasurementUnits, NativeMethods.MeasurementUnitsScreen);

        ConfigureInteractions();

        // Keep the delegate referenced in a field so the GC cannot collect it while native code still holds the function pointer
        _outputCallback = OnOutput;
        NativeMethods.RegisterOutputCallbackInteractionContext(_context, _outputCallback, IntPtr.Zero);

        _inertiaTimer = new System.Windows.Forms.Timer {Interval = 16};
        _inertiaTimer.Tick += InertiaTick;
    }

    private void ConfigureInteractions()
    {
        var configuration = new NativeMethods.InteractionContextConfiguration[]
        {
            new()
            {
                interactionId = NativeMethods.InteractionId.Manipulation,
                enable = NativeMethods.ConfigManipulation
                       | NativeMethods.ConfigManipulationTranslationX | NativeMethods.ConfigManipulationTranslationY
                       | NativeMethods.ConfigManipulationScaling | NativeMethods.ConfigManipulationRotation
                       | NativeMethods.ConfigManipulationTranslationInertia | NativeMethods.ConfigManipulationScalingInertia | NativeMethods.ConfigManipulationRotationInertia
            }
        };
        NativeMethods.SetInteractionConfigurationInteractionContext(_context, (uint)configuration.Length, configuration);
    }

    /// <summary>
    /// Processes a window message. Call this from the control's <see cref="Control.WndProc"/>.
    /// </summary>
    /// <param name="m">The message to process.</param>
    /// <returns><c>true</c> if the message was a touch message and was handled; <c>false</c> otherwise.</returns>
    public bool ProcessMessage(ref Message m)
    {
        if (!_enabled || _disposed) return false;
        switch (m.Msg)
        {
            case WM_POINTERDOWN or WM_POINTERUPDATE or WM_POINTERUP or WM_POINTERCAPTURECHANGED:
                break;

            default:
                return false;
        }

        uint pointerId = (uint)(m.WParam.ToInt64() & 0xFFFF);
        switch (m.Msg)
        {
            case WM_POINTERDOWN:
                NativeMethods.AddPointerInteractionContext(_context, pointerId);
                _activeContacts++;
                ProcessFrames(pointerId);
                return _manipulationActive;

            case WM_POINTERUPDATE:
                ProcessFrames(pointerId);
                return _manipulationActive;

            case WM_POINTERUP:
                ProcessFrames(pointerId); // May synchronously raise ManipulationCompleted if there is no inertia
                NativeMethods.RemovePointerInteractionContext(_context, pointerId);
                if (_activeContacts > 0) _activeContacts--;
                // If the manipulation is still active after the last contact was lifted, inertia is pending.
                if (_activeContacts == 0 && _manipulationActive) _inertiaTimer?.Start();
                return _manipulationActive;

            case WM_POINTERCAPTURECHANGED:
                NativeMethods.StopInteractionContext(_context);
                NativeMethods.RemovePointerInteractionContext(_context, pointerId);
                _activeContacts = 0;
                return true;

            default:
                return false;
        }
    }

    private void ProcessFrames(uint pointerId)
    {
        uint entriesCount = 0, pointerCount = 0;
        if (!NativeMethods.GetPointerFrameInfoHistory(pointerId, ref entriesCount, ref pointerCount, null)) return;
        if (entriesCount == 0 || pointerCount == 0) return;

        var pointerInfo = new NativeMethods.PointerInfo[entriesCount * pointerCount];
        if (!NativeMethods.GetPointerFrameInfoHistory(pointerId, ref entriesCount, ref pointerCount, pointerInfo)) return;

        NativeMethods.ProcessPointerFramesInteractionContext(_context, entriesCount, pointerCount, pointerInfo);
    }

    private void InertiaTick(object? sender, EventArgs e)
    {
        if (_disposed) return;
        NativeMethods.ProcessInertiaInteractionContext(_context);
        if (!_manipulationActive) _inertiaTimer?.Stop();
    }

    private void OnOutput(IntPtr clientData, IntPtr outputPtr)
    {
        var output = Marshal.PtrToStructure<NativeMethods.InteractionContextOutput>(outputPtr);
        var origin = _control.PointToClient(new Point((int)output.x, (int)output.y));

        switch (output.interactionId)
        {
            case NativeMethods.InteractionId.Manipulation:
                var m = output.manipulation;
                var args = new ManipulationEventArgs(
                    origin,
                    delta: new(m.delta.translationX, m.delta.translationY, m.delta.scale, m.delta.expansion, m.delta.rotation),
                    cumulative: new(m.cumulative.translationX, m.cumulative.translationY, m.cumulative.scale, m.cumulative.expansion, m.cumulative.rotation),
                    velocity: new(m.velocity.velocityX, m.velocity.velocityY, m.velocity.velocityExpansion, m.velocity.velocityAngular),
                    isInertial: (output.interactionFlags & NativeMethods.InteractionFlags.Inertia) != 0);

                if ((output.interactionFlags & NativeMethods.InteractionFlags.Begin) != 0)
                {
                    _manipulationActive = true;
                    ManipulationStarted?.Invoke(_control, args);
                }
                else if ((output.interactionFlags & NativeMethods.InteractionFlags.End) != 0)
                {
                    _manipulationActive = false;
                    _inertiaTimer?.Stop();
                    ManipulationCompleted?.Invoke(_control, args);
                }
                else ManipulationUpdated?.Invoke(_control, args);
                break;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _inertiaTimer?.Stop();
        _inertiaTimer?.Dispose();
        if (_enabled) NativeMethods.DestroyInteractionContext(_context);
    }
}
