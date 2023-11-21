// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks;

/// <summary>
/// Represents a progress snapshot of an <see cref="ITask"/>.
/// </summary>
/// <param name="State">The current State of the task.</param>
/// <param name="UnitsByte"><c>true</c> if <see cref="UnitsProcessed"/> and <see cref="UnitsTotal"/> are measured in bytes; <c>false</c> if they are measured in generic units.</param>
/// <param name="UnitsProcessed">The number of units that have been processed so far.</param>
/// <param name="UnitsTotal">The total number of units that are to be processed; -1 for unknown.</param>
[Serializable]
public readonly record struct TaskSnapshot(TaskState State, bool UnitsByte = false, long UnitsProcessed = 0, long UnitsTotal = -1)
{
    /// <summary>
    /// The progress of the task as a value between 0 and 1; -1 when unknown.
    /// </summary>
    public double Value =>
        UnitsTotal switch
        {
            -1 => -1,
            0 => 1,
            _ => (UnitsProcessed / (double)UnitsTotal)
        };

    /// <inheritdoc/>
    public override string ToString()
        => State switch
        {
            TaskState.Ready or TaskState.Started => "",
            TaskState.Header => Resources.StateHeader,
            TaskState.Data when UnitsTotal == -1 && UnitsProcessed == 0 => Resources.StateData,
            TaskState.Data when UnitsTotal == -1 => UnitsToString(UnitsProcessed),
            TaskState.Data => $"{UnitsToString(UnitsProcessed)} / {UnitsToString(UnitsTotal)}",
            TaskState.Complete => Resources.StateComplete,
            TaskState.WebError => Resources.StateWebError,
            TaskState.IOError => Resources.StateIOError,
            _ => ""
        };

    private string UnitsToString(long units)
        => UnitsByte
            ? units.FormatBytes()
            : units.ToString();
}
