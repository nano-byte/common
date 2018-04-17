// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Reports progress updates using callbacks/events. Performs the callbacks immediately on the same thread.
    /// </summary>
    public class SynchronousProgress<T> : MarshalByRefObject, IProgress<T>
    {
        /// <summary>
        /// Raised for each reported progress value.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
        public event Action<T> ProgressChanged;

        /// <summary>
        /// Captures the current synchronization context for callbacks.
        /// </summary>
        public SynchronousProgress(Action<T> callback = null)
        {
            if (callback != null) ProgressChanged += callback;
        }

        void IProgress<T>.Report(T value) => OnReport(value);

        protected virtual void OnReport(T value)
        {
            var callback = ProgressChanged;
            callback?.Invoke(value);
        }
    }
}
