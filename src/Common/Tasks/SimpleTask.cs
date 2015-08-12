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
    /// A delegate-driven task. Only completion is reported, no intermediate progress.
    /// </summary>
    public sealed class SimpleTask : TaskBase
    {
        /// <inheritdoc/>
        public override string Name { get; }

        /// <summary>The code to be executed by the task. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</summary>
        private readonly Action _work;

        /// <summary>A callback to be called when cancellation is requested via a <see cref="CancellationToken"/>.</summary>
        private readonly Action _cancelationCallback;

        /// <inheritdoc/>
        public override bool CanCancel => (_cancelationCallback != null);

        /// <inheritdoc/>
        protected override bool UnitsByte => false;

        /// <summary>
        /// Creates a new simple task.
        /// </summary>
        /// <param name="name">A name describing the task in human-readable form.</param>
        /// <param name="work">The code to be executed by the task. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</param>
        /// <param name="cancellationCallback">A callback to be called when cancellation is requested via a <see cref="CancellationToken"/>.</param>
        public SimpleTask([NotNull, Localizable(true)] string name, [NotNull] Action work, [CanBeNull] Action cancellationCallback = null)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (work == null) throw new ArgumentNullException(nameof(work));
            #endregion

            Name = name;
            _work = work;
            _cancelationCallback = cancellationCallback;
        }

        /// <inheritdoc/>
        protected override void Execute()
        {
            State = TaskState.Started;
            if (_cancelationCallback == null) _work();
            else
            {
                using (CancellationToken.Register(_cancelationCallback))
                    _work();
            }
            State = TaskState.Complete;
        }
    }
}
