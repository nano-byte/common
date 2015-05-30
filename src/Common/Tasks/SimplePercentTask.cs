/*
 * Copyright 2006-2015 Bastian Eicher
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
using System.ComponentModel;
using System.IO;
using System.Net;
using JetBrains.Annotations;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// A callback to be called by a workload to report its progress in percent.
    /// </summary>
    /// <param name="percent">The workload's progress in percent.</param>
    [CLSCompliant(false)]
    public delegate void PercentProgressCallback(uint percent);

    /// <summary>
    /// A delegate-driven task. Progress is reported in percent.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class SimplePercentTask : TaskBase
    {
        private readonly string _name;

        /// <inheritdoc/>
        public override string Name { get { return _name; } }

        /// <summary>The code to be executed by the task. Is given a callback to report progress in percent. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</summary>
        private readonly Action<PercentProgressCallback> _work;

        /// <summary>A callback to be called when cancellation is requested via a <see cref="CancellationToken"/>.</summary>
        private readonly Action _cancelationCallback;

        /// <inheritdoc/>
        public override bool CanCancel { get { return (_cancelationCallback != null); } }

        /// <inheritdoc/>
        protected override bool UnitsByte { get { return false; } }

        /// <summary>
        /// Creates a new simple task.
        /// </summary>
        /// <param name="name">A name describing the task in human-readable form.</param>
        /// <param name="work">The code to be executed by the task. Is given a callback to report progress in percent. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</param>
        /// <param name="cancellationCallback">A callback to be called when cancellation is requested via a <see cref="CancellationToken"/>.</param>
        public SimplePercentTask([NotNull, Localizable(true)] string name, [NotNull] Action<PercentProgressCallback> work, [CanBeNull] Action cancellationCallback = null)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (work == null) throw new ArgumentNullException("work");
            #endregion

            _name = name;
            _work = work;
            _cancelationCallback = cancellationCallback;
        }

        /// <inheritdoc/>
        protected override void Execute()
        {
            UnitsTotal = 100;
            State = TaskState.Started;

            if (_cancelationCallback == null) _work(percent => UnitsProcessed = percent);
            else
            {
                using (CancellationToken.Register(_cancelationCallback))
                    _work(percent => UnitsProcessed = percent);
            }

            State = TaskState.Complete;
        }
    }
}
