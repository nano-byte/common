// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// An undo command that replaces an element in a list with a new one.
/// </summary>
/// <param name="list">The collection to be modified.</param>
/// <param name="oldElement">The element to be removed from <paramref name="list"/>.</param>
/// <param name="newElement">The element to be added to <paramref name="list"/>.</param>
/// <typeparam name="T">The type of elements the list contains.</typeparam>
[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "The complete name is not ambiguous.")]
public class ReplaceInList<T>(IList<T> list, T oldElement, T newElement) : SimpleCommand, IValueCommand
    where T : notnull
{
    private int _index = -1;

    /// <inheritdoc/>
    public object Value => newElement;

    protected override void OnExecute()
    {
        if (_index < 0) _index = list.IndexOfByReference(oldElement);
        list[_index] = newElement;
    }

    protected override void OnUndo() => list[_index] = oldElement;
}

/// <summary>
/// Factory methods for <see cref="ReplaceInList{T}"/>.
/// </summary>
public static class ReplaceInList
{
    /// <summary>
    /// Creates a new replace in list command.
    /// </summary>
    /// <param name="list">The collection to be modified.</param>
    /// <param name="oldElement">The element to be removed from <paramref name="list"/>.</param>
    /// <param name="newElement">The element to be added to <paramref name="list"/>.</param>
    /// <typeparam name="T">The type of elements the list contains.</typeparam>
    public static ReplaceInList<T> For<T>(IList<T> list, T oldElement, T newElement)
        where T : notnull
        => new(list, oldElement, newElement);
}
