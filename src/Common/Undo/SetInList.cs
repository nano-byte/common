// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// Replaces an entry in a <see cref="IList{T}"/> with a new one.
/// </summary>
/// <param name="list">The list to be modified.</param>
/// <param name="oldElement">The old element currently in the <paramref name="list"/> to be replaced.</param>
/// <param name="newElement">The new element to take the place of <paramref name="oldElement"/> in the <paramref name="list"/>.</param>
/// <typeparam name="T">The type of elements the list contains.</typeparam>
public sealed class SetInList<T>(IList<T> list, T oldElement, T newElement) : SimpleCommand, IValueCommand
    where T : notnull
{
    private int _index = -1;

    /// <inheritdoc/>
    public object Value => newElement;

    /// <summary>
    /// Sets the new entry in the list.
    /// </summary>
    protected override void OnExecute()
    {
        if (_index < 0) _index = list.IndexOfByReference(oldElement);
        list[_index] = newElement;
    }

    /// <summary>
    /// Restores the old entry in the list.
    /// </summary>
    protected override void OnUndo() => list[_index] = oldElement;
}

/// <summary>
/// Factory methods for <see cref="SetInList{T}"/>.
/// </summary>
public static class SetInList
{
    /// <summary>
    /// Creates a new set in list command.
    /// </summary>
    /// <param name="list">The list to be modified.</param>
    /// <param name="oldElement">The old element currently in the <paramref name="list"/> to be replaced.</param>
    /// <param name="newElement">The new element to take the place of <paramref name="oldElement"/> in the <paramref name="list"/>.</param>
    /// <typeparam name="T">The type of elements the list contains.</typeparam>
    public static SetInList<T> For<T>(IList<T> list, T oldElement, T newElement)
        where T : notnull
        => new(list, oldElement, newElement);
}
