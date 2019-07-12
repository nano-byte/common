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
    public struct TaskSnapshot
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
        public double Value
        {
            get
            {
                switch (UnitsTotal)
                {
                    case -1:
                        return -1;
                    case 0:
                        return 1;
                    default:
                        return UnitsProcessed / (double)UnitsTotal;
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            switch (State)
            {
                default:
                case TaskState.Ready:
                case TaskState.Started:
                    return "";

                case TaskState.Header:
                    return Resources.StateHeader;

                case TaskState.Data:
                    if (UnitsTotal == -1)
                        return (UnitsProcessed == 0) ? Resources.StateData : UnitsToString(UnitsProcessed);
                    else
                        return UnitsToString(UnitsProcessed) + @" / " + UnitsToString(UnitsTotal);

                case TaskState.Complete:
                    return Resources.StateComplete;

                case TaskState.WebError:
                    return Resources.StateWebError;

                case TaskState.IOError:
                    return Resources.StateIOError;
            }
        }

        private string UnitsToString(long units)
            => UnitsByte
                ? units.FormatBytes()
                : units.ToString();
    }
}
