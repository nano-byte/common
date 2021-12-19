// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls;

/// <summary>
/// A control that can raise touch events.
/// </summary>
public interface ITouchControl
{
    /// <summary>
    /// Raised when the user begins touching the screen.
    /// </summary>
    event EventHandler<TouchEventArgs> TouchDown;

    /// <summary>
    /// Raised when the user stops touching the screen.
    /// </summary>
    event EventHandler<TouchEventArgs> TouchUp;

    /// <summary>
    /// Raised when the user moves fingers while touching the screen.
    /// </summary>
    event EventHandler<TouchEventArgs> TouchMove;
}