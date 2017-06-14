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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using JetBrains.Annotations;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// A task that performs an operation once for each element of a collection.
    /// </summary>
    public sealed class ForEachTask<T> : TaskBase
    {
        /// <summary>A list of objects to execute work for. Cancellation is possible between two elements.</summary>
        private readonly IEnumerable<T> _target;

        /// <summary>The code to be executed once per element in <see cref="_target"/>. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</summary>
        private readonly Action<T> _work;

        /// <inheritdoc/>
        public override string Name { get; }

        /// <inheritdoc/>
        protected override bool UnitsByte => false;

        /// <summary>
        /// Creates a new for-each task.
        /// </summary>
        /// <param name="name">A name describing the task in human-readable form.</param>
        /// <param name="target">A list of objects to execute work for. Cancellation is possible between two elements.</param>
        /// <param name="work">The code to be executed once per element in <paramref name="target"/>. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</param>
        public ForEachTask([NotNull, Localizable(true)] string name, [NotNull] IEnumerable<T> target, [NotNull] Action<T> work)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _work = work ?? throw new ArgumentNullException(nameof(work));
            _target = target ?? throw new ArgumentNullException(nameof(target));

            // Detect collections that know their own length
            if (target is ICollection<T> collection) UnitsTotal = collection.Count;
        }

        /// <inheritdoc/>
        protected override void Execute()
        {
            State = TaskState.Data;

            foreach (var element in _target)
            {
                CancellationToken.ThrowIfCancellationRequested();
                _work(element);
                UnitsProcessed++;
            }

            State = TaskState.Complete;
        }
    }

    /// <summary>
    /// Provides a static factory method for <seealso cref="ForEachTask{T}"/> as an alternative to calling the constructor to exploit type inference.
    /// </summary>
    public static class ForEachTask
    {
        /// <summary>
        /// Creates a new for-each task.
        /// </summary>
        /// <param name="name">A name describing the task in human-readable form.</param>
        /// <param name="target">A list of objects to execute work for. Cancellation is possible between two elements.</param>
        /// <param name="work">The code to be executed once per element in <paramref name="target"/>. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</param>
        public static ForEachTask<T> Create<T>([NotNull, Localizable(true)] string name, [NotNull] IEnumerable<T> target, [NotNull] Action<T> work)
            => new ForEachTask<T>(name, target, work);
    }
}
