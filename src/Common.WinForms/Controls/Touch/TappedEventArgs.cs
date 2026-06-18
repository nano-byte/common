// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls.Touch;

/// <summary>
/// Event information about a tap (single-finger touch) gesture.
/// </summary>
/// <param name="position">The position of the tap in client coordinates.</param>
/// <param name="tapCount">The number of consecutive taps (e.g. <c>2</c> for a double tap).</param>
public sealed class TappedEventArgs(PointF position, int tapCount) : EventArgs
{
    /// <summary>
    /// The position of the tap in client coordinates.
    /// </summary>
    public PointF Position { get; } = position;

    /// <summary>
    /// The number of consecutive taps (e.g. <c>2</c> for a double tap).
    /// </summary>
    public int TapCount { get; } = tapCount;
}
