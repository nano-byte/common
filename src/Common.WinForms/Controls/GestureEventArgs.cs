// Copyright Bastian Eicher
// Licensed under the MIT License

using System;

namespace NanoByte.Common.Controls;

/// <summary>
/// Flags for gesture information.
/// </summary>
[Flags]
public enum GestureFlags
{
    /// <summary>Marks the beginning of a gesture.</summary>
    Begin = 0x00000001,

    /// <summary>Indicates that the gesture is in inertia mode.</summary>
    Inertia = 0x00000002,

    /// <summary>Marks the end of a gesture.</summary>
    End = 0x00000004
}

/// <summary>
/// Base class for gesture event arguments.
/// </summary>
public abstract class GestureEventArgs : EventArgs
{
    /// <summary>
    /// The X coordinate of the gesture in client coordinates.
    /// </summary>
    public int LocationX { get; set; }

    /// <summary>
    /// The Y coordinate of the gesture in client coordinates.
    /// </summary>
    public int LocationY { get; set; }

    /// <summary>
    /// Flags indicating the state of the gesture.
    /// </summary>
    public GestureFlags Flags { get; set; }

    /// <summary>
    /// Unique identifier for this gesture sequence.
    /// </summary>
    public int SequenceId { get; set; }
}

/// <summary>
/// Event arguments for pan gesture.
/// </summary>
public class PanGestureEventArgs : GestureEventArgs
{
    /// <summary>
    /// The horizontal distance panned.
    /// </summary>
    public int PanDistanceX { get; set; }

    /// <summary>
    /// The vertical distance panned.
    /// </summary>
    public int PanDistanceY { get; set; }
}

/// <summary>
/// Event arguments for zoom gesture.
/// </summary>
public class ZoomGestureEventArgs : GestureEventArgs
{
    /// <summary>
    /// The distance between the two fingers.
    /// </summary>
    public long Distance { get; set; }
}

/// <summary>
/// Event arguments for rotate gesture.
/// </summary>
public class RotateGestureEventArgs : GestureEventArgs
{
    /// <summary>
    /// The angle of rotation in radians.
    /// </summary>
    public double Angle { get; set; }
}

/// <summary>
/// Event arguments for tap gesture (two-finger tap).
/// </summary>
public class TapGestureEventArgs : GestureEventArgs
{
}

/// <summary>
/// Event arguments for press and tap gesture.
/// </summary>
public class PressAndTapGestureEventArgs : GestureEventArgs
{
}
