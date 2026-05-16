// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System.Threading.Tasks;

namespace NanoByte.Common.Threading;

/// <summary>
/// Manages a single background computation, exposing only the most recent result thread-safely.
/// </summary>
/// <typeparam name="T">The type of result produced by the calculation.</typeparam>
/// <remarks>Starting a new computation automatically cancels and discards any in-progress one.</remarks>
public sealed class LatestBackgroundResult<T> where T : class
{
#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#else
    private readonly object _lock = new();
#endif

    private CancellationTokenSource? _cts;
    private T? _result;

    /// <summary>
    /// The most recent successfully completed result, or <c>null</c> if no result is available yet.
    /// </summary>
    public T? Result
    {
        get { lock (_lock) return _result; }
    }

    /// <summary>
    /// Atomically reads and clears the latest result.
    /// Returns <c>null</c> if no result is available.
    /// </summary>
    public T? ConsumeResult()
    {
        lock (_lock)
        {
            var result = _result;
            if (result == null) return null;
            _result = null;
            return result;
        }
    }

    /// <summary>
    /// Cancels any in-progress computation and discards the current result.
    /// </summary>
    public void Cancel()
    {
        lock (_lock)
        {
            _cts?.Cancel();
            _cts = null;
            _result = null;
        }
    }

    /// <summary>
    /// Cancels any in-progress computation and starts <paramref name="computation"/> on a background thread.
    /// The result becomes available via <see cref="Result"/> or <see cref="ConsumeResult"/> once complete.
    /// </summary>
    /// <remarks>If the computation is canceled before it finishes, the result is silently discarded.</remarks>
    public void Run(Func<CancellationToken, T> computation)
    {
        CancellationTokenSource cts;
        lock (_lock)
        {
            _cts?.Cancel();
            _result = null;
            _cts = cts = new();
        }

        Task.Run(() =>
        {
            try
            {
                var result = computation(cts.Token);
                lock (_lock)
                {
                    if (cts == _cts)
                        _result = result;
                }
            }
            catch (OperationCanceledException)
            {}
            finally
            {
                lock (_lock)
                {
                    if (cts == _cts) _cts = null;
                }
                cts.Dispose();
            }
        }, cts.Token);
    }
}
#endif
