// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls.Touch;

/// <summary>
/// A control that reacts to touch gestures.
/// </summary>
public interface ITouchControl
{
    /// <summary>
    /// Raised when a manipulation gesture (translation, scaling and/or rotation) begins.
    /// </summary>
    event EventHandler<ManipulationEventArgs> ManipulationStarted;

    /// <summary>
    /// Raised when a manipulation gesture changes.
    /// </summary>
    event EventHandler<ManipulationEventArgs> ManipulationUpdated;

    /// <summary>
    /// Raised when a manipulation gesture (including any inertia) ends.
    /// </summary>
    event EventHandler<ManipulationEventArgs> ManipulationCompleted;
}
