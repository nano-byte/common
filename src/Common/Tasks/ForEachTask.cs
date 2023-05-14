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
    private readonly Action<T>? _rollback;

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
    /// <param name="rollback">
    /// An optional action to try to undo changes made by <paramref name="action"/> in case one of the invocations failed or the task was cancelled.
    /// Called once for each element for which <paramref name="action"/> was called (even if it failed), in reverse order. Any exceptions thrown here are logged and then ignored.
    /// </param>
    public ForEachTask([Localizable(true)] string name, IEnumerable<T> target, Action<T> action, Action<T>? rollback = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _action = action ?? throw new ArgumentNullException(nameof(action));
        _target = target ?? throw new ArgumentNullException(nameof(target));
        _rollback = rollback;

        // Detect collections that know their own length
        if (target is ICollection<T> collection) UnitsTotal = collection.Count;
    }

    /// <inheritdoc/>
    protected override void Execute()
    {
        State = TaskState.Data;

        var attemptedElements = new Stack<T>();
        try
        {
            foreach (var element in _target)
            {
                CancellationToken.ThrowIfCancellationRequested();
                attemptedElements.Push(element);
                _action(element);
                UnitsProcessed++;
            }
        }
        catch when (_rollback != null)
        {
            while (attemptedElements.Count != 0)
            {
                var element = attemptedElements.Pop();
                try
                {
                    _rollback(element);
                }
                #region Error handling
                catch (Exception ex)
                {
                    // Suppress exceptions during rollback since they would hide the actual exception that caused the rollback in the first place
                    Log.Error(string.Format(Resources.FailedToRollback, element), ex);
                }
                #endregion
            }

            throw;
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
    /// <param name="rollback">
    /// An optional action to try to undo changes made by <paramref name="action"/> in case one of the invocations failed or the task was cancelled.
    /// Called once for each element for which <paramref name="action"/> was called (even if it failed), in reverse order. Any exceptions thrown here are logged and then ignored.
    /// </param>
    public static ForEachTask<T> Create<T>([Localizable(true)] string name, IEnumerable<T> target, Action<T> action, Action<T>? rollback = null)
        => new(name, target, action, rollback);
}
