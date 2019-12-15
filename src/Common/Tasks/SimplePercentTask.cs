// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.IO;
using System.Net;

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
        /// <inheritdoc/>
        public override string Name { get; }

        /// <summary>The code to be executed by the task. Is given a callback to report progress in percent. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</summary>
        private readonly Action<PercentProgressCallback> _work;

        /// <summary>A callback to be called when cancellation is requested via a <see cref="CancellationToken"/>.</summary>
        private readonly Action? _cancellationCallback;

        /// <inheritdoc/>
        public override bool CanCancel => _cancellationCallback != null;

        /// <inheritdoc/>
        protected override bool UnitsByte => false;

        /// <summary>
        /// Creates a new simple task.
        /// </summary>
        /// <param name="name">A name describing the task in human-readable form.</param>
        /// <param name="work">The code to be executed by the task. Is given a callback to report progress in percent. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</param>
        /// <param name="cancellationCallback">A callback to be called when cancellation is requested via a <see cref="CancellationToken"/>.</param>
        public SimplePercentTask([Localizable(true)] string name, Action<PercentProgressCallback> work, Action? cancellationCallback = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _work = work ?? throw new ArgumentNullException(nameof(work));
            _cancellationCallback = cancellationCallback;
        }

        /// <inheritdoc/>
        protected override void Execute()
        {
            UnitsTotal = 100;

            if (_cancellationCallback == null) _work(percent => UnitsProcessed = percent);
            else
            {
                using (CancellationToken.Register(_cancellationCallback))
                    _work(percent => UnitsProcessed = percent);
            }
        }
    }
}
