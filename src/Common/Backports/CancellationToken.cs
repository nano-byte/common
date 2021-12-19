// Copyright Bastian Eicher
// Licensed under the MIT License

#if NET20
using System.Threading;

namespace System.Threading
{
    /// <summary>
    /// Propagates notification that operations should be canceled.
    /// </summary>
    public struct CancellationToken
    {
        private readonly CancellationTokenSource? _source;

        /// <summary>
        /// Creates a new token controlled by a specific <see cref="CancellationTokenSource"/>.
        /// </summary>
        internal CancellationToken(CancellationTokenSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Registers a delegate that will be called when cancellation has been requested.
        /// </summary>
        /// <param name="callback">The delegate to be executed when cancellation has been requested.</param>
        /// <returns>A handle that can be used to deregister the callback.</returns>
        /// <remarks>
        /// The callback is called from a background thread. Wrap via synchronization context to update UI elements.
        /// Handling this blocks the task, therefore observers should handle the event quickly.
        /// </remarks>
        public CancellationTokenRegistration Register(Action callback) => new(_source, callback);

        /// <summary>
        /// Indicates whether cancellation has been requested.
        /// </summary>
        public bool IsCancellationRequested => _source is {IsCancellationRequested: true};

        /// <summary>
        /// Throws an <see cref="OperationCanceledException"/> if cancellation has been requested.
        /// </summary>
        /// <exception cref="OperationCanceledException">Cancellation has been requested.</exception>
        // ReSharper disable once PureAttributeOnVoidMethod
        [Pure]
        public void ThrowIfCancellationRequested()
        {
            if (IsCancellationRequested) throw new OperationCanceledException();
        }

        private static readonly WaitHandle _dummyWaitHandle = new ManualResetEvent(initialState: false);

        /// <summary>
        /// Gets a wait handle that is signaled when cancellation has been requested.
        /// </summary>
        public WaitHandle WaitHandle => _source?.WaitHandle ?? _dummyWaitHandle;

        /// <inheritdoc/>
        public override string ToString() => "CancellationToken {IsCancellationRequested=" + IsCancellationRequested + "}";
    }
}
#else
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Threading.CancellationToken))]
#endif
