// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.Contracts;
using System.Threading;

namespace NanoByte.Common
{
    /// <summary>
    /// Combines an <see cref="EventWaitHandle"/> with a result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public sealed class Future<T> : IDisposable
    {
        private T _result = default!;
        private readonly EventWaitHandle _waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        /// <summary>
        /// Sets the result and signals anyone waiting for it.
        /// </summary>
        public void Set(T result)
        {
            _result = result;
            _waitHandle.Set();
        }

        /// <summary>
        /// Waits for the result and returns it when it is ready.
        /// </summary>
        [Pure]
        public T Get()
        {
            _waitHandle.WaitOne();
            return _result;
        }

        /// <summary>
        /// Creates a future with the result already set.
        /// </summary>
        public static implicit operator Future<T>(T value)
        {
            var future = new Future<T>();
            future.Set(value);
            return future;
        }

        /// <inheritdoc/>
        public void Dispose() => _waitHandle.Close();
    }
}
