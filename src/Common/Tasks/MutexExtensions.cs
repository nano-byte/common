// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Threading;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Provides extension methods for <seealso cref="Mutex"/>.
    /// </summary>
    public static class MutexExtensions
    {
        /// <summary>
        /// Acquires the mutex, blocking until it becomes available.
        /// </summary>
        /// <param name="mutex">The mutex to acquire.</param>
        /// <param name="timeout">The maximum timespan to wait for the mutex to become available.</param>
        /// <param name="cancellationToken">Used to cancel waiting for the mutex to become available.</param>
        /// <exception cref="TimeoutException"><paramref name="timeout"/> elapsed without the mutex becoming available.</exception>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was signaled while waiting for the mutex to become available.</exception>
        /// <remarks>Automatically handles <see cref="AbandonedMutexException"/> with <see cref="Log.Warn(Exception)"/>.</remarks>
        public static void WaitOne(this Mutex mutex, TimeSpan timeout, CancellationToken cancellationToken)
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
                // Abandoned mutexes also get acquired, but indicate something may have gone wrong elsewhere
                Log.Warn(ex);
            }
        }
    }
}
