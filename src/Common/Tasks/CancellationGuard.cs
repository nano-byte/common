#if NET40 || NET45
/*
 * Copyright 2006-2016 Bastian Eicher
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
using System.Threading.Tasks;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Ensures that a block of code that is not fully <see cref="CancellationToken"/>-aware cleanly exits before <see cref="CancellationTokenSource.Cancel()"/> calls complete.
    /// </summary>
    /// <example>
    /// This class is best used in a using-block:
    /// <code>
    /// using (new CancellationGuard(cancellationToken))
    /// {
    ///     // Your code
    /// }
    /// </code>
    /// </example>
    public class CancellationGuard : IDisposable
    {
        private CancellationTokenRegistration _registration;
        private readonly TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

        /// <summary>
        /// Registers a callback for the <paramref name="cancellationToken"/> that blocks calls to <see cref="CancellationTokenSource.Cancel()"/> until <see cref="Dispose"/> has been called.
        /// </summary>
        /// <param name="cancellationToken">Used to signal cancellation requests.</param>
        public CancellationGuard(CancellationToken cancellationToken)
        {
            _registration = cancellationToken.Register(_tcs.Task.Wait);
        }

        /// <summary>
        /// Registers a callback for the <paramref name="cancellationToken"/> that blocks calls to <see cref="CancellationTokenSource.Cancel()"/> until <see cref="Dispose"/> has been called.
        /// </summary>
        /// <param name="cancellationToken">Used to signal cancellation requests.</param>
        /// <param name="timeout">A timespan after which the cancellation will be considered completed even if <see cref="Dispose"/> has not been called yet.</param>
        public CancellationGuard(CancellationToken cancellationToken, TimeSpan timeout)
        {
            _registration = cancellationToken.Register(() => _tcs.Task.Wait(timeout));
        }

        /// <summary>
        /// Releases the block and allows <see cref="CancellationTokenSource.Cancel()"/> to complete.
        /// </summary>
        public void Dispose()
        {
            _tcs.SetResult(true);
            _registration.Dispose();
        }
    }
}
#endif