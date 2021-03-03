// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Threading;
using NanoByte.Common.Native;

#if NET20
using NanoByte.Common.Tasks;
#endif

namespace NanoByte.Common
{
    /// <summary>
    /// Provides extension methods for <see cref="System.Threading.WaitHandle"/>.
    /// </summary>
    public static class WaitHandleExtensions
    {
        /// <summary>
        /// Waits for the handle to be signalled.
        /// </summary>
        /// <param name="handle">The handle to wait for.</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or -1 to wait indefinitely.</param>
        /// <param name="cancellationToken">Used to cancel waiting for the handle.</param>
        /// <exception cref="TimeoutException"><paramref name="millisecondsTimeout"/> elapsed without the handle being signalled.</exception>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was signaled while waiting for the handle.</exception>
        /// <remarks>Automatically handles <see cref="System.Threading.AbandonedMutexException"/> with <see cref="Log.Warn(Exception)"/>.</remarks>
        public static void WaitOne(this WaitHandle handle, CancellationToken cancellationToken, int millisecondsTimeout = -1)
        {
            try
            {
                // Wait operations on multiple wait handles including a named synchronization primitive are only supported on Windows
                if (!WindowsUtils.IsWindows && handle is Mutex)
                {
                    handle.WaitOne(millisecondsTimeout);
                    return;
                }

                switch (WaitHandle.WaitAny(new[]
                {
                    handle ?? throw new ArgumentNullException(nameof(handle)),
                    cancellationToken.WaitHandle
                }, millisecondsTimeout, exitContext: false))
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
