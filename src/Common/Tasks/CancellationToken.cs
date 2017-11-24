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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using JetBrains.Annotations;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Propagates notification that operations should be canceled.
    /// </summary>
    /// <remarks>Unlike the built-in CancellationToken type of .NET the NanoByte.Common variant supports remoting.</remarks>
    [Serializable]
    public struct CancellationToken
    {
        [SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Justification = "Access to this field is remoted.")]
        [CanBeNull]
        private readonly CancellationTokenSource _source;

        /// <summary>
        /// Creates a new token controlled by a specific <see cref="CancellationTokenSource"/>.
        /// </summary>
        internal CancellationToken([NotNull] CancellationTokenSource source) => _source = source;

        /// <summary>
        /// Registers a delegate that will be called when cancellation has been requested.
        /// </summary>
        /// <param name="callback">The delegate to be executed when cancellation has been requested.</param>
        /// <returns>A handle that can be used to deregister the callback.</returns>
        /// <remarks>
        /// The callback is called from a background thread. Wrap via synchronization context to update UI elements.
        /// Handling this blocks the task, therefore observers should handle the event quickly.
        /// </remarks>
        public CancellationTokenRegistration Register([NotNull] Action callback) => new CancellationTokenRegistration(_source, callback);

        /// <summary>
        /// Indicates whether cancellation has been requested.
        /// </summary>
        public bool IsCancellationRequested => (_source != null) && _source.IsCancellationRequested;

        /// <summary>
        /// Throws an <see cref="OperationCanceledException"/> if cancellation has been requested.
        /// </summary>
        /// <exception cref="OperationCanceledException">Cancellation has been requested.</exception>
        [Pure]
        public void ThrowIfCancellationRequested()
        {
            if (IsCancellationRequested) throw new OperationCanceledException();
        }

        /// <summary>
        /// Gets a wait handle that is signaled when cancellation has been requested.
        /// </summary>
        public WaitHandle WaitHandle => (_source == null) ? new ManualResetEvent(false) : _source.WaitHandle;

        /// <inheritdoc/>
        public override string ToString() => "CancellationToken {IsCancellationRequested=" + IsCancellationRequested + "}";

#if NET40 || NET45 || NETSTANDARD2_0
        /// <summary>
        /// Converts a NanoByte.Common cancellation token to a regular .NET cancellation token.
        /// </summary>
        public static implicit operator System.Threading.CancellationToken(CancellationToken token)
        {
            var tokenSource = new System.Threading.CancellationTokenSource();
            token.Register(tokenSource.Cancel);
            return tokenSource.Token;
        }

        /// <summary>
        /// Converts a regular .NET cancellation token to a NanoByte.Common cancellation token.
        /// </summary>
        public static implicit operator CancellationToken(System.Threading.CancellationToken token)
        {
            var tokenSource = new CancellationTokenSource();
            token.Register(tokenSource.Cancel);
            return tokenSource.Token;
        }
#endif
    }
}
