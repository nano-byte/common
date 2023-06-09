// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// A task that executes a callback and the provides a result. Only completion is reported, no intermediate progress.
/// </summary>
public sealed class ResultTask<T> : TaskBase, IResultTask<T>
{
    /// <inheritdoc/>
    public override string Name { get; }

    /// <summary>The code to be executed by the task. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</summary>
    private readonly Func<T> _work;

    private T _result = default!;

    /// <inheritdoc/>
    public T Result
        => State == TaskState.Complete
            ? _result
            : throw new InvalidOperationException($"The task is in the state {State} and not Complete.");

    /// <summary>An optional callback to be called when cancellation is requested via a <see cref="CancellationToken"/>.</summary>
    private readonly Action? _cancellationCallback;

    /// <inheritdoc/>
    public override bool CanCancel => _cancellationCallback != null;

    /// <inheritdoc/>
    protected override bool UnitsByte => false;

    /// <summary>
    /// Creates a new task that executes a callback and the provides a result.
    /// </summary>
    /// <param name="name">A name describing the task in human-readable form.</param>
    /// <param name="work">The code to be executed by the task that provides a result. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</param>
    /// <param name="cancellationCallback">An optional callback to be called when cancellation is requested via a <see cref="CancellationToken"/>.</param>
    public ResultTask([Localizable(true)] string name, Func<T> work, Action? cancellationCallback = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _work = work ?? throw new ArgumentNullException(nameof(work));
        _cancellationCallback = cancellationCallback;
    }

    /// <inheritdoc/>
    protected override void Execute()
    {
        if (_cancellationCallback == null)
            _result = _work();
        else
        {
            using (CancellationToken.Register(_cancellationCallback))
                _result = _work();
        }
    }
}

/// <summary>
/// Provides a static factory method for <see cref="ResultTask{T}"/> as an alternative to calling the constructor to exploit type inference.
/// </summary>
public static class ResultTask
{
    /// <summary>
    /// Creates a new task that executes a callback and the provides a result. Only completion is reported, no intermediate progress.
    /// </summary>
    /// <param name="name">A name describing the task in human-readable form.</param>
    /// <param name="work">The code to be executed by the task that provides a result. May throw <see cref="WebException"/>, <see cref="IOException"/> or <see cref="OperationCanceledException"/>.</param>
    /// <param name="cancellationCallback">An optional callback to be called when cancellation is requested via a <see cref="CancellationToken"/>.</param>
    public static ResultTask<T> Create<T>([Localizable(true)] string name, Func<T> work, Action? cancellationCallback = null)
        => new(name, work, cancellationCallback);
}
