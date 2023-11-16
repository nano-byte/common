// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// An undo command that adds or removes an element from a collection.
/// </summary>
/// <param name="collection">The collection to be modified.</param>
/// <param name="element">The element to be added or removed from <paramref name="collection"/>.</param>
/// <typeparam name="T">The type of elements the collection contains.</typeparam>
public abstract class CollectionCommand<T>(ICollection<T> collection, T element) : SimpleCommand, IValueCommand
    where T : notnull
{
    /// <summary>
    /// The collection to be modified.
    /// </summary>
    protected readonly ICollection<T> Collection = collection;

    /// <summary>
    /// The element to be added or removed from <see cref="Collection"/>.
    /// </summary>
    protected readonly T Element = element;

    /// <inheritdoc/>
    public object Value => Element;
}
