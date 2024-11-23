// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System.Threading.Tasks;

namespace NanoByte.Common.Threading;

/// <summary>
/// Helper for racing multiple operations against each other, providing the result of the first one that finishes.
/// </summary>
/// <param name="cancellationToken">Used to cancel all pending operations.</param>
/// <typeparam name="T">The type of the result.</typeparam>
public class ResultRacer<T>(CancellationToken cancellationToken = default)
    where T : notnull
{
    private readonly TaskCompletionSource<T> _completion = new();
    private readonly CancellationTokenSource _competitionCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

    /// <summary>
    /// Trys to set a result, racing against other calls of this method.
    /// </summary>
    /// <param name="factory">
    /// A function that takes a cancellation token (triggered when another call won the race) and returns a possible result.
    /// Return null to indicate that the function was unable to provide a result.
    /// Exceptions (except <see cref="OperationCanceledException"/>) are passed through to <see cref="GetResult"/> and <see cref="GetResultAsync"/>.
    /// </param>
    public void TrySetResult(Func<CancellationToken, T?> factory)
    {
        try
        {
            if (factory(_competitionCancellation.Token) is {} result)
            {
                _competitionCancellation.Cancel(); // Cancel all competing runs
                _completion.TrySetResult(result);
            }
        }
        catch (OperationCanceledException) {}
        catch (Exception ex)
        {
            _competitionCancellation.Cancel(); // Cancel all competing runs
            _completion.TrySetException(ex);
        }
    }

    /// <summary>
    /// Trys to set a result, racing against other calls of this method.
    /// </summary>
    /// <param name="factory">
    /// A function that takes a cancellation token (triggered when another call won the race) and returns a <see cref="Task{T}"/> returning a possible result.
    /// Return null to indicate that the function was unable to provide a result.
    /// Exceptions (except <see cref="OperationCanceledException"/>) are passed through to <see cref="GetResult"/> and <see cref="GetResultAsync"/>.
    /// </param>
    public async Task TrySetResultAsync(Func<CancellationToken, Task<T?>> factory)
    {
        try
        {
            if (await factory(_competitionCancellation.Token) is {} result)
            {
                _competitionCancellation.Cancel(); // Cancel all competing runs
                _completion.TrySetResult(result);
            }
        }
        catch (OperationCanceledException) {}
        catch (Exception ex)
        {
            _competitionCancellation.Cancel(); // Cancel all competing runs
            _completion.TrySetException(ex);
        }
    }

    /// <summary>
    /// Blocks until at least one call to <see cref="TrySetResult"/> or <see cref="TrySetResultAsync"/> succeeded and returns its result.
    /// </summary>
    public T GetResult()
    {
        var task = _completion.Task;
        task.Wait(cancellationToken);
        return task.Result;
    }

    /// <summary>
    /// Waits until at least one call to <see cref="TrySetResult"/> or <see cref="TrySetResultAsync"/> succeeded and returns its result.
    /// </summary>
    public async Task<T> GetResultAsync()
        => await _completion.Task.WaitAsync(cancellationToken);
}

/// <summary>
/// Helper for racing multiple operations against each other, providing the result of the first one that finishes.
/// </summary>
public static class ResultRacer
{
    /// <summary>
    /// Races an operation for multiple input elements against each other.
    /// </summary>
    /// <typeparam name="TInput">The type of the input elements.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="input">The input elements to try to produce outputs for.</param>
    /// <param name="factory">
    /// A function that takes an input element and a cancellation token (triggered when another call won the race) and returns a possible result.
    /// Return null to indicate that the function was unable to provide a result.
    /// Exceptions (except <see cref="OperationCanceledException"/>) are passed through to <see cref="ResultRacer{TResult}.GetResult"/> and <see cref="ResultRacer{TResult}.GetResultAsync"/>.
    /// </param>
    /// <param name="cancellationToken">Used to cancel all pending operations.</param>
    public static ResultRacer<TResult> For<TInput, TResult>(IEnumerable<TInput> input, Func<TInput, CancellationToken, TResult?> factory, CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        var racer = new ResultRacer<TResult>(cancellationToken);
        foreach (var element in input)
            Task.Run(() => racer.TrySetResult(token => factory(element, token)), cancellationToken);
        return racer;
    }

    /// <summary>
    /// Races an operation for multiple input elements against each other.
    /// </summary>
    /// <typeparam name="TInput">The type of the input elements.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="input">The input elements to try to produce outputs for.</param>
    /// <param name="factory">
    /// A function that takes an input element and a cancellation token (triggered when another call won the race) and returns a <see cref="Task{T}"/> returning a possible result.
    /// Return null to indicate that the function was unable to provide a result.
    /// Exceptions (except <see cref="OperationCanceledException"/>) are passed through to <see cref="ResultRacer{TResult}.GetResult"/> and <see cref="ResultRacer{TResult}.GetResultAsync"/>.
    /// </param>
    /// <param name="cancellationToken">Used to cancel all pending operations.</param>
    public static ResultRacer<TResult> For<TInput, TResult>(IEnumerable<TInput> input, Func<TInput, CancellationToken, Task<TResult?>> factory, CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        var racer = new ResultRacer<TResult>(cancellationToken);
        foreach (var element in input)
            Task.Run(() => racer.TrySetResultAsync(token => factory(element, token)), cancellationToken);
        return racer;
    }
}
#endif
