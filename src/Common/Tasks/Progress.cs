// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Reports progress updates using callbacks/events. Performs the callbacks using the synchronization context of the original caller.
    /// </summary>
    /// <remarks>Unlike the built-in Progress type of .NET the NanoByte.Common variant supports remoting.</remarks>
    public class Progress<T> : MarshalByRefObject, IProgress<T>
    {
        /// <summary>
        /// Raised for each reported progress value.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
        public event Action<T>? ProgressChanged;

        private readonly SynchronizationContext? _synchronizationContext;

        /// <summary>
        /// Captures the current synchronization context for callbacks.
        /// </summary>
        public Progress(Action<T>? callback = null)
        {
            _synchronizationContext = SynchronizationContext.Current;

            if (callback != null) ProgressChanged += callback;
        }

        void IProgress<T>.Report(T value) => OnReport(value);

        protected virtual void OnReport(T value)
        {
            var callback = ProgressChanged;
            if (callback != null)
            {
                if (_synchronizationContext != null) _synchronizationContext.Post(_ => callback(value), null);
                else ThreadPool.QueueUserWorkItem(_ => callback(value));
            }
        }
    }
}
