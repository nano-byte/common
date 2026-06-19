// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls.Touch;

/// <summary>
/// Event information about a manipulation gesture (translation, scaling and/or rotation by one or more contacts).
/// </summary>
/// <param name="origin">The center point of the manipulation in client coordinates.</param>
/// <param name="delta">The change since the last manipulation event.</param>
/// <param name="cumulative">The accumulated change since the start of the manipulation.</param>
/// <param name="velocity">The current velocities of the manipulation.</param>
/// <param name="isInertial">Indicates that the manipulation is continuing under inertia after the user lifted all contacts.</param>
public sealed class ManipulationEventArgs(PointF origin, ManipulationDelta delta, ManipulationDelta cumulative, ManipulationVelocity velocity, bool isInertial) : EventArgs
{
    /// <summary>The center point of the manipulation in client coordinates.</summary>
    public PointF Origin { get; } = origin;

    /// <summary>The change since the last manipulation event.</summary>
    public ManipulationDelta Delta { get; } = delta;

    /// <summary>The accumulated change since the start of the manipulation.</summary>
    public ManipulationDelta Cumulative { get; } = cumulative;

    /// <summary>The current velocities of the manipulation.</summary>
    public ManipulationVelocity Velocity { get; } = velocity;

    /// <summary>Indicates that the manipulation is continuing under inertia after the user lifted all contacts.</summary>
    public bool IsInertial { get; } = isInertial;
}

/// <summary>
/// A combined translation, scaling, rotation and expansion produced by a manipulation gesture.
/// </summary>
/// <param name="translationX">Horizontal translation in pixels.</param>
/// <param name="translationY">Vertical translation in pixels.</param>
/// <param name="scale">Scaling factor. <c>1</c> means no change.</param>
/// <param name="expansion">Change of the average radius (in pixels) between the contacts. <c>0</c> means no change.</param>
/// <param name="rotation">Rotation in radians. <c>0</c> means no change.</param>
public readonly struct ManipulationDelta(float translationX, float translationY, float scale, float expansion, float rotation)
{
    /// <summary>Horizontal translation in pixels.</summary>
    public float TranslationX { get; } = translationX;

    /// <summary>Vertical translation in pixels.</summary>
    public float TranslationY { get; } = translationY;

    /// <summary>Scaling factor. <c>1</c> means no change.</summary>
    public float Scale { get; } = scale;

    /// <summary>Change of the average radius (in pixels) between the contacts. <c>0</c> means no change.</summary>
    public float Expansion { get; } = expansion;

    /// <summary>Rotation in radians. <c>0</c> means no change.</summary>
    public float Rotation { get; } = rotation;
}

/// <summary>
/// The velocities of an ongoing manipulation gesture.
/// </summary>
/// <param name="linearX">Horizontal velocity in pixels per millisecond.</param>
/// <param name="linearY">Vertical velocity in pixels per millisecond.</param>
/// <param name="expansion">Expansion velocity in pixels per millisecond.</param>
/// <param name="angular">Angular velocity in radians per millisecond.</param>
public readonly struct ManipulationVelocity(float linearX, float linearY, float expansion, float angular)
{
    /// <summary>Horizontal velocity in pixels per millisecond.</summary>
    public float LinearX { get; } = linearX;

    /// <summary>Vertical velocity in pixels per millisecond.</summary>
    public float LinearY { get; } = linearY;

    /// <summary>Expansion velocity in pixels per millisecond.</summary>
    public float Expansion { get; } = expansion;

    /// <summary>Angular velocity in radians per millisecond.</summary>
    public float Angular { get; } = angular;
}
