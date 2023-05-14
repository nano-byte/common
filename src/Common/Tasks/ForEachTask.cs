// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks;

/// <summary>
/// A task that executes an action once for each element of a collection.
/// </summary>
public sealed class ForEachTask<T> : TaskBase
{
    private readonly IEnumerable<T> _target;
    private readonly Action<T> _action;

    /// <inheritdoc/>
    public override string Name { get; }

    /// <inheritdoc/>
    protected override bool UnitsByte => false;

    /// <summary>
    /// Creates a new task that executes an action once for each element of a collection.
    /// </summary>
    /// <param name="name">A name describing the task in human-readable form.</param>
    /// <param name="target">A list of objects to execute the action for. Cancellation is possible between any two elements.</param>
    /// <param name="action">The action to be executed once per element in <paramref name="target"/>.</param>
    public ForEachTask([Localizable(true)] string name, IEnumerable<T> target, Action<T> action)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _action = action ?? throw new ArgumentNullException(nameof(action));
        _target = target ?? throw new ArgumentNullException(nameof(target));

        // Detect collections that know their own length
        if (target is ICollection<T> collection) UnitsTotal = collection.Count;
    }

    /// <inheritdoc/>
    protected override void Execute()
    {
        State = TaskState.Data;

        foreach (var element in _target)
        {
            CancellationToken.ThrowIfCancellationRequested();
            _action(element);
            UnitsProcessed++;
        }
    }
}

/// <summary>
/// Provides a static factory method for <see cref="ForEachTask{T}"/> as an alternative to calling the constructor to exploit type inference.
/// </summary>
public static class ForEachTask
{
    /// <summary>
    /// Creates a new task that executes an action once for each element of a collection.
    /// </summary>
    /// <param name="name">A name describing the task in human-readable form.</param>
    /// <param name="target">A list of objects to execute the action for. Cancellation is possible between any two elements.</param>
    /// <param name="action">The action to be executed once per element in <paramref name="target"/>.</param>
    public static ForEachTask<T> Create<T>([Localizable(true)] string name, IEnumerable<T> target, Action<T> action)
        => new(name, target, action);
}
