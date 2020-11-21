// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Represents a progress snapshot of an <see cref="ITask"/>.
    /// </summary>
    [Serializable]
    public readonly struct TaskSnapshot
    {
        /// <summary>
        /// The current State of the task.
        /// </summary>
        public TaskState State { get; }

        /// <summary>
        /// <c>true</c> if <see cref="UnitsProcessed"/> and <see cref="UnitsTotal"/> are measured in bytes;
        /// <c>false</c> if they are measured in generic units.
        /// </summary>
        public bool UnitsByte { get; }

        /// <summary>
        /// The number of units that have been processed so far.
        /// </summary>
        public long UnitsProcessed { get; }

        /// <summary>
        /// The total number of units that are to be processed; -1 for unknown.
        /// </summary>
        public long UnitsTotal { get; }

        /// <summary>
        /// Create a new progress snapshot.
        /// </summary>
        /// <param name="state">The current State of the task.</param>
        /// <param name="unitsByte"><c>true</c> if <see cref="UnitsProcessed"/> and <see cref="UnitsTotal"/> are measured in bytes; <c>false</c> if they are measured in generic units.</param>
        /// <param name="unitsProcessed">The number of units that have been processed so far.</param>
        /// <param name="unitsTotal">The total number of units that are to be processed; -1 for unknown.</param>
        public TaskSnapshot(TaskState state, bool unitsByte = false, long unitsProcessed = 0, long unitsTotal = -1)
        {
            State = state;
            UnitsByte = unitsByte;
            UnitsProcessed = unitsProcessed;
            UnitsTotal = unitsTotal;
        }

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
                TaskState.Data => UnitsToString(UnitsProcessed) + @" / " + UnitsToString(UnitsTotal),
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
}
