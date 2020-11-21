// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;

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
        public ForEachTask([Localizable(true)] string name, IEnumerable<T> target, Action<T> work)
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
        public static ForEachTask<T> Create<T>([Localizable(true)] string name, IEnumerable<T> target, Action<T> work)
            => new(name, target, work);
    }
}
