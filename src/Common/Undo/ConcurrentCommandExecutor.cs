// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// Decorator for <see cref="ICommandExecutor"/> that adds locking for thread-safety.
/// </summary>
public class ConcurrentCommandExecutor(ICommandExecutor inner) : ICommandExecutor
{
#if NET9_0_OR_GREATER
    private static readonly Lock _lock = new();
#else
    private static readonly object _lock = new();
#endif

    /// <inheritdoc/>
    public void Execute(IUndoCommand command)
    {
        lock (_lock)
            inner.Execute(command);
    }

    /// <inheritdoc/>
    // ReSharper disable once InconsistentlySynchronizedField
    public string? Path => inner.Path;
}
