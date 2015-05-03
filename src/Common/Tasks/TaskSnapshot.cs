/*
 * Copyright 2006-2014 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Globalization;
using NanoByte.Common.Properties;
using NanoByte.Common.Storage;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Represents a progress snapshot of an <see cref="ITask"/>.
    /// </summary>
    [Serializable]
    public struct TaskSnapshot
    {
        private readonly TaskState _state;

        /// <summary>
        /// The current State of the task.
        /// </summary>
        public TaskState State { get { return _state; } }

        private readonly bool _unitsByte;

        /// <summary>
        /// <see langword="true"/> if <see cref="UnitsProcessed"/> and <see cref="UnitsTotal"/> are measured in bytes;
        /// <see langword="false"/> if they are measured in generic units.
        /// </summary>
        public bool UnitsByte { get { return _unitsByte; } }

        private readonly long _unitsProcessed;

        /// <summary>
        /// The number of units that have been processed so far.
        /// </summary>
        public long UnitsProcessed { get { return _unitsProcessed; } }

        private readonly long _unitsTotal;

        /// <summary>
        /// The total number of units that are to be processed; -1 for unknown.
        /// </summary>
        public long UnitsTotal { get { return _unitsTotal; } }

        /// <summary>
        /// Create a new progress snapshot.
        /// </summary>
        /// <param name="state">The current State of the task.</param>
        /// <param name="unitsByte"><see langword="true"/> if <see cref="UnitsProcessed"/> and <see cref="UnitsTotal"/> are measured in bytes; <see langword="false"/> if they are measured in generic units.</param>
        /// <param name="unitsProcessed">The number of units that have been processed so far.</param>
        /// <param name="unitsTotal">The total number of units that are to be processed; -1 for unknown.</param>
        public TaskSnapshot(TaskState state, bool unitsByte = false, long unitsProcessed = 0, long unitsTotal = -1)
        {
            _state = state;
            _unitsByte = unitsByte;
            _unitsProcessed = unitsProcessed;
            _unitsTotal = unitsTotal;
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
                        return _unitsProcessed / (double)_unitsTotal;
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
        {
            return UnitsByte
                ? units.FormatBytes(CultureInfo.CurrentCulture)
                : units.ToString(CultureInfo.CurrentCulture);
        }
    }
}
