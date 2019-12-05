// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using JetBrains.Annotations;

#if NETFRAMEWORK
using System.Runtime.Remoting;
#endif

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Signals to <see cref="CancellationToken"/>s that they should be canceled.
    /// </summary>
    /// <remarks>Unlike the built-in CancellationToken type of .NET the NanoByte.Common variant supports remoting.</remarks>
    public sealed class CancellationTokenSource : MarshalNoTimeout, IDisposable
    {
        /// <summary>
        /// Gets a <see cref="CancellationToken"/> associated with this <see cref="CancellationTokenSource"/>.
        /// </summary>
        public CancellationToken Token => new CancellationToken(this);

        private volatile bool _isCancellationRequested; // Volatile justification: Write access is locked, many reads

        /// <summary>
        /// Raised the first time <see cref="Cancel"/> is called. Subsequent calls will not raise this event again.
        /// </summary>
        /// <remarks>
        /// The event is raised from a background thread. Wrap via synchronization context to update UI elements.
        /// Handling this blocks the task, therefore observers should handle the event quickly.
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
        public event Action CancellationRequested;

        /// <summary>
        /// Indicates whether <see cref="Cancel"/> has been called.
        /// </summary>
        public bool IsCancellationRequested => _isCancellationRequested;

        private readonly ManualResetEvent _waitEvent = new ManualResetEvent(initialState: false);

        /// <summary>
        /// Gets a wait handle that is signaled when see cref="Cancel"/> has been called.
        /// </summary>
        [NotNull]
        internal WaitHandle WaitHandle => _waitEvent;

        private readonly object _lock = new object();

        /// <summary>
        /// Notifies all listening <see cref="CancellationToken"/>s that operations should be canceled.
        /// </summary>
        public void Cancel()
        {
            lock (_lock)
            {
                // Don't trigger more than once
                if (_isCancellationRequested) return;

                _waitEvent.Set();

                _isCancellationRequested = true;
                if (CancellationRequested != null)
                {
#if NETFRAMEWORK
                    try
                    {
#endif
                        CancellationRequested();
#if NETFRAMEWORK
                    }
                    catch (RemotingException)
                    {}
#endif
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString() => "CancellationTokenSource {IsCancellationRequested=" + IsCancellationRequested + "}";

        /// <inheritdoc/>
        public void Dispose() => _waitEvent.Close();
    }
}
