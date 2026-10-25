// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;
using NanoByte.Common.Native;
using Windows.Win32;
using Windows.Win32.UI.InteractionContext;
using Windows.Win32.UI.Input.Pointer;

namespace NanoByte.Common.Controls.Touch;

/// <summary>
/// Translates Windows pointer input on a <see cref="Control"/> into high-level touch gestures. Single finger taps are still reported as mouse click events.
/// </summary>
[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "CsWin32 annotates exact Windows versions, version is checked at runtime with IsWindows8")]
public sealed class TouchEventSource : IDisposable
{
    private const int WM_POINTERUPDATE = 0x0245, WM_POINTERDOWN = 0x0246, WM_POINTERUP = 0x0247, WM_POINTERCAPTURECHANGED = 0x024C;

    /// <summary>Value for <see cref="INTERACTION_CONTEXT_PROPERTY.INTERACTION_CONTEXT_PROPERTY_MEASUREMENT_UNITS"/>.</summary>
    private const uint MeasurementUnitsScreen = 1;

    private readonly Control _control;
    private readonly bool _enabled;
    private readonly HINTERACTIONCONTEXT _context;
    private readonly System.Windows.Forms.Timer? _inertiaTimer;

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void InteractionOutputCallback(IntPtr clientData, IntPtr output);

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    // Keep the delegate referenced in a field so the GC cannot collect it while native code still holds the function pointer
    private readonly InteractionOutputCallback? _outputCallback;

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
    public unsafe TouchEventSource(Control control)
    {
        _control = control ?? throw new ArgumentNullException(nameof(control));
        if (!WindowsUtils.IsWindows8) return;

        HINTERACTIONCONTEXT context;
        if (PInvoke.CreateInteractionContext(&context).Failed) return;
        _context = context;
        _enabled = true;

        // Operate in screen pixels rather than the default HIMETRIC units
        PInvoke.SetPropertyInteractionContext(_context, INTERACTION_CONTEXT_PROPERTY.INTERACTION_CONTEXT_PROPERTY_MEASUREMENT_UNITS, MeasurementUnitsScreen);

        ConfigureInteractions();

        // Keep the delegate referenced in a field so the GC cannot collect it while native code still holds the function pointer
        _outputCallback = OnOutput;
        PInvoke.RegisterOutputCallbackInteractionContext(_context, (delegate* unmanaged[Stdcall]<void*, INTERACTION_CONTEXT_OUTPUT*, void>)(void*)Marshal.GetFunctionPointerForDelegate(_outputCallback), null);

        _inertiaTimer = new System.Windows.Forms.Timer {Interval = 16};
        _inertiaTimer.Tick += InertiaTick;
    }

    private unsafe void ConfigureInteractions()
    {
        var configuration = new INTERACTION_CONTEXT_CONFIGURATION
        {
            interactionId = INTERACTION_ID.INTERACTION_ID_MANIPULATION,
            enable = INTERACTION_CONFIGURATION_FLAGS.INTERACTION_CONFIGURATION_FLAG_MANIPULATION
                   | INTERACTION_CONFIGURATION_FLAGS.INTERACTION_CONFIGURATION_FLAG_MANIPULATION_TRANSLATION_X | INTERACTION_CONFIGURATION_FLAGS.INTERACTION_CONFIGURATION_FLAG_MANIPULATION_TRANSLATION_Y
                   | INTERACTION_CONFIGURATION_FLAGS.INTERACTION_CONFIGURATION_FLAG_MANIPULATION_SCALING | INTERACTION_CONFIGURATION_FLAGS.INTERACTION_CONFIGURATION_FLAG_MANIPULATION_ROTATION
                   | INTERACTION_CONFIGURATION_FLAGS.INTERACTION_CONFIGURATION_FLAG_MANIPULATION_TRANSLATION_INERTIA | INTERACTION_CONFIGURATION_FLAGS.INTERACTION_CONFIGURATION_FLAG_MANIPULATION_SCALING_INERTIA | INTERACTION_CONFIGURATION_FLAGS.INTERACTION_CONFIGURATION_FLAG_MANIPULATION_ROTATION_INERTIA
        };
        PInvoke.SetInteractionConfigurationInteractionContext(_context, 1, &configuration);
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
                PInvoke.AddPointerInteractionContext(_context, pointerId);
                _activeContacts++;
                ProcessFrames(pointerId);
                return _manipulationActive;

            case WM_POINTERUPDATE:
                ProcessFrames(pointerId);
                return _manipulationActive;

            case WM_POINTERUP:
                ProcessFrames(pointerId); // May synchronously raise ManipulationCompleted if there is no inertia
                PInvoke.RemovePointerInteractionContext(_context, pointerId);
                if (_activeContacts > 0) _activeContacts--;
                // If the manipulation is still active after the last contact was lifted, inertia is pending.
                if (_activeContacts == 0 && _manipulationActive) _inertiaTimer?.Start();
                return _manipulationActive;

            case WM_POINTERCAPTURECHANGED:
                PInvoke.StopInteractionContext(_context);
                PInvoke.RemovePointerInteractionContext(_context, pointerId);
                _activeContacts = 0;
                return true;

            default:
                return false;
        }
    }

    private unsafe void ProcessFrames(uint pointerId)
    {
        uint entriesCount = 0, pointerCount = 0;
        if (!PInvoke.GetPointerFrameInfoHistory(pointerId, &entriesCount, &pointerCount, null)) return;
        if (entriesCount == 0 || pointerCount == 0) return;

        var pointerInfo = new POINTER_INFO[entriesCount * pointerCount];
        fixed (POINTER_INFO* pointerInfoPtr = pointerInfo)
        {
            if (!PInvoke.GetPointerFrameInfoHistory(pointerId, &entriesCount, &pointerCount, pointerInfoPtr)) return;

            PInvoke.ProcessPointerFramesInteractionContext(_context, entriesCount, pointerCount, pointerInfoPtr);
        }
    }

    private void InertiaTick(object? sender, EventArgs e)
    {
        if (_disposed) return;
        PInvoke.ProcessInertiaInteractionContext(_context);
        if (!_manipulationActive) _inertiaTimer?.Stop();
    }

    private unsafe void OnOutput(IntPtr clientData, IntPtr outputPtr)
    {
        var output = (INTERACTION_CONTEXT_OUTPUT*)outputPtr;
        var origin = _control.PointToClient(new Point((int)output->x, (int)output->y));

        switch (output->interactionId)
        {
            case INTERACTION_ID.INTERACTION_ID_MANIPULATION:
                var m = output->arguments.manipulation;
                var args = new ManipulationEventArgs(
                    origin,
                    delta: new(m.delta.translationX, m.delta.translationY, m.delta.scale, m.delta.expansion, m.delta.rotation),
                    cumulative: new(m.cumulative.translationX, m.cumulative.translationY, m.cumulative.scale, m.cumulative.expansion, m.cumulative.rotation),
                    velocity: new(m.velocity.velocityX, m.velocity.velocityY, m.velocity.velocityExpansion, m.velocity.velocityAngular),
                    isInertial: (output->interactionFlags & INTERACTION_FLAGS.INTERACTION_FLAG_INERTIA) != 0);

                if ((output->interactionFlags & INTERACTION_FLAGS.INTERACTION_FLAG_BEGIN) != 0)
                {
                    _manipulationActive = true;
                    ManipulationStarted?.Invoke(_control, args);
                }
                else if ((output->interactionFlags & INTERACTION_FLAGS.INTERACTION_FLAG_END) != 0)
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
        if (_enabled) PInvoke.DestroyInteractionContext(_context);
    }
}
