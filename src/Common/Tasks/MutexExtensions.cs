/*
 * Copyright 2006-2017 Bastian Eicher
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
using System.Threading;
using JetBrains.Annotations;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Provides extension methods for <seealso cref="Mutex"/>.
    /// </summary>
    [PublicAPI]
    public static class MutexExtensions
    {
        /// <summary>
        /// Aquires the mutex, blocking until it becomes available.
        /// </summary>
        /// <param name="mutex">The mutex to aquire.</param>
        /// <param name="timeout">The maximum timespan to wait for the mutex to become available.</param>
        /// <param name="cancellationToken">Used to cancel waiting for the mutex to become available.</param>
        /// <exception cref="TimeoutException"><paramref name="timeout"/> elapsed without the mutex becoming available.</exception>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was signaled while waiting for the mutex to become available.</exception>
        /// <remarks>Automatically handles <see cref="AbandonedMutexException"/> with <see cref="Log.Warn(Exception)"/>.</remarks>
        public static void WaitOne([NotNull] this Mutex mutex, TimeSpan timeout, CancellationToken cancellationToken)
        {
            try
            {
                switch (WaitHandle.WaitAny(new[]
                {
                    mutex ?? throw new ArgumentNullException(nameof(mutex)),
                    cancellationToken.WaitHandle
                }, timeout, exitContext: false))
                {
                    case 0:
                        return;
                    case 1:
                        throw new OperationCanceledException();
                    default:
                    case WaitHandle.WaitTimeout:
                        throw new TimeoutException();
                }
            }
            catch (AbandonedMutexException ex)
            {
                // Abandoned mutexes also get aquired, but indicate something may have gone wrong elsewhere
                Log.Warn(ex);
            }
        }
    }
}
