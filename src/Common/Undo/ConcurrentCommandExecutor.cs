// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// Decorator for <see cref="ICommandExecutor"/> that adds locking for thread-safety.
/// </summary>
public class ConcurrentCommandExecutor : ICommandExecutor
{
    private readonly ICommandExecutor _inner;

    public ConcurrentCommandExecutor(ICommandExecutor inner)
    {
        _inner = inner;
    }

    private readonly object _lock = new();

    /// <inheritdoc/>
    public void Execute(IUndoCommand command)
    {
        lock (_lock)
            _inner.Execute(command);
    }

    /// <inheritdoc/>
    // ReSharper disable once InconsistentlySynchronizedField
    public string? Path => _inner.Path;
}
