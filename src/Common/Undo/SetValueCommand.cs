// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20
using System.Linq.Expressions;
#endif

namespace NanoByte.Common.Undo;

/// <summary>
/// An undo command that uses a delegates for getting and setting values from a backing model.
/// </summary>
/// <typeparam name="T">The type of the value to set.</typeparam>
public class SetValueCommand<T> : SimpleCommand, IValueCommand
{
    private readonly PropertyPointer<T> _pointer;
    private readonly T _newValue;
    private T _oldValue = default!;

    /// <inheritdoc/>
    public object? Value => _newValue;

    /// <summary>
    /// Creates a new command for setting a value.
    /// </summary>
    /// <param name="pointer">The object controlling how to read/write the value to be modified.</param>
    /// <param name="newValue">The new value to be set.</param>
    public SetValueCommand(PropertyPointer<T> pointer, T newValue)
    {
        _newValue = newValue;
        _pointer = pointer ?? throw new ArgumentNullException(nameof(pointer));
    }

    /// <summary>
    /// Sets the new value in the model.
    /// </summary>
    protected override void OnExecute()
    {
        _oldValue = _pointer.Value;
        _pointer.Value = _newValue;
    }

    /// <summary>
    /// Restores the old value in the model.
    /// </summary>
    protected override void OnUndo()
        => _pointer.Value = _oldValue;
}

/// <summary>
/// Factory methods for <see cref="SetValueCommand{T}"/>.
/// </summary>
public static class SetValueCommand
{
    /// <summary>
    /// Creates a new command for setting a value.
    /// </summary>
    /// <param name="pointer">The object controlling how to read/write the value to be modified.</param>
    /// <param name="newValue">The new value to be set.</param>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    public static SetValueCommand<T> For<T>(PropertyPointer<T> pointer, T newValue)
        => new(pointer, newValue);

    /// <summary>
    /// Creates a new command for setting a value.
    /// </summary>
    /// <param name="getValue">A delegate that returns the current value.</param>
    /// <param name="setValue">A delegate that sets the value.</param>
    /// <param name="newValue">The new value to be set.</param>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    public static SetValueCommand<T> For<T>(Func<T> getValue, Action<T> setValue, T newValue)
        where T : notnull
        => new(PropertyPointer.For(getValue, setValue), newValue);

    /// <summary>
    /// Creates a new command for setting a nullable value.
    /// </summary>
    /// <param name="getValue">A delegate that returns the current value.</param>
    /// <param name="setValue">A delegate that sets the value.</param>
    /// <param name="newValue">The new value to be set.</param>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    public static SetValueCommand<T?> ForNullable<T>(Func<T?> getValue, Action<T?> setValue, T? newValue)
        => new(PropertyPointer.ForNullable(getValue, setValue), newValue);

#if !NET20
    /// <summary>
    /// Creates a new command for setting a value.
    /// </summary>
    /// <typeparam name="T">The type of value the property contains.</typeparam>
    /// <param name="expression">An expression pointing to the property.</param>
    /// <param name="newValue">The new value to be set.</param>
    public static SetValueCommand<T> For<T>(Expression<Func<T>> expression, T newValue)
        where T : notnull
        => new(PropertyPointer.For(expression), newValue);

    /// <summary>
    /// Creates a new command for setting a nullable value.
    /// </summary>
    /// <typeparam name="T">The type of value the property contains.</typeparam>
    /// <param name="expression">An expression pointing to the property.</param>
    /// <param name="newValue">The new value to be set.</param>
    public static SetValueCommand<T?> ForNullable<T>(Expression<Func<T?>> expression, T? newValue)
        => new(PropertyPointer.ForNullable(expression), newValue);
#endif
}
