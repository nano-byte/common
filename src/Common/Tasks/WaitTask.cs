// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Threading;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Waits for a <see cref="WaitHandle"/> to become available.
    /// </summary>
    public sealed class WaitTask : TaskBase
    {
        /// <summary>The <see cref="WaitHandle"/> to wait for.</summary>
        private readonly WaitHandle _waitHandle;

        /// <summary>The number of milliseconds to wait before raising <see cref="TimeoutException"/>; <see cref="Timeout.Infinite"/> to wait indefinitely</summary>
        private readonly int _millisecondsTimeout;

        /// <inheritdoc/>
        public override string Name { get; }

        /// <inheritdoc/>
        protected override bool UnitsByte => false;

        /// <summary>
        /// Creates a new handle-waiting task.
        /// </summary>
        /// <param name="name">A name describing the task in human-readable form.</param>
        /// <param name="waitHandle">>The <see cref="WaitHandle"/> to wait for.</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait before raising <see cref="TimeoutException"/>; <see cref="Timeout.Infinite"/> to wait indefinitely.</param>
        public WaitTask([Localizable(true)] string name, WaitHandle waitHandle, int millisecondsTimeout = Timeout.Infinite)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _waitHandle = waitHandle ?? throw new ArgumentNullException(nameof(waitHandle));
            _millisecondsTimeout = millisecondsTimeout;
        }

        /// <inheritdoc/>
        protected override void Execute()
        {
            try
            {
                switch (WaitHandle.WaitAny(new[] {_waitHandle, CancellationToken.WaitHandle}, _millisecondsTimeout, exitContext: false))
                {
                    case 0:
                        break;

                    case 1:
                        throw new OperationCanceledException();

                    default:
                    case WaitHandle.WaitTimeout:
                        throw new TimeoutException();
                }
            }
            catch (AbandonedMutexException ex)
            {
                // Abandoned mutexes also get owned, but indicate something may have gone wrong elsewhere
                Log.Warn(ex.Message);
            }
        }
    }
}
