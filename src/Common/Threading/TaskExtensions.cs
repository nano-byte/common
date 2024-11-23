// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System.Threading.Tasks;

namespace NanoByte.Common.Threading;

public static class TaskExtensions
{
    /// <summary>
    /// Gets a <see cref="Task"/> that will complete when <paramref name="task"/> completes or when the specified <paramref name="cancellationToken"/> has cancellation requested.
    /// </summary>
    /// <exception cref="OperationCanceledException">Cancellation has been requested.</exception>
    public static async Task WaitAsync(this Task task, CancellationToken cancellationToken)
#if NETFRAMEWORK
    {
        var cancellationCompletion = new TaskCompletionSource<bool>();
        var token = cancellationToken;
        using (token.Register(() => cancellationCompletion.SetCanceled()))
            await Task.WhenAny(task, cancellationCompletion.Task);
        token.ThrowIfCancellationRequested();
        await task;
    }
#else
        => await task.WaitAsync(cancellationToken);
#endif

    /// <summary>
    /// Gets a <see cref="Task"/> that will complete when <paramref name="task"/> completes or when the specified <paramref name="cancellationToken"/> has cancellation requested.
    /// </summary>
    /// <exception cref="OperationCanceledException">Cancellation has been requested.</exception>
    public static async Task<T> WaitAsync<T>(this Task<T> task, CancellationToken cancellationToken)
#if NETFRAMEWORK
    {
        var cancellationCompletion = new TaskCompletionSource<bool>();
        var token = cancellationToken;
        using (token.Register(() => cancellationCompletion.SetCanceled()))
            await Task.WhenAny(task, cancellationCompletion.Task);
        token.ThrowIfCancellationRequested();
        return await task;
    }
#else
        => await task.WaitAsync(cancellationToken);
#endif
}
#endif
