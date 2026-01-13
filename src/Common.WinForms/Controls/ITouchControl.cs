// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls;

/// <summary>
/// A control that can raise touch gesture events.
/// </summary>
public interface ITouchControl
{
    /// <summary>
    /// Raised when the user performs a pan gesture.
    /// </summary>
    event EventHandler<PanGestureEventArgs> Pan;

    /// <summary>
    /// Raised when the user performs a zoom gesture.
    /// </summary>
    event EventHandler<ZoomGestureEventArgs> Zoom;

    /// <summary>
    /// Raised when the user performs a rotate gesture.
    /// </summary>
    event EventHandler<RotateGestureEventArgs> Rotate;

    /// <summary>
    /// Raised when the user performs a two-finger tap gesture.
    /// </summary>
    event EventHandler<TapGestureEventArgs> Tap;

    /// <summary>
    /// Raised when the user performs a press and tap gesture.
    /// </summary>
    event EventHandler<PressAndTapGestureEventArgs> PressAndTap;
}
