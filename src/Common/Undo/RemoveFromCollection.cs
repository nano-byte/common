// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// An undo command that removes an element from a collection.
/// </summary>
/// <param name="collection">The collection to be modified.</param>
/// <param name="element">The element to be removed from <paramref name="collection"/>.</param>
/// <typeparam name="T">The type of elements the collection contains.</typeparam>
[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "The complete name is not ambiguous.")]
public sealed class RemoveFromCollection<T>(ICollection<T> collection, T element) : CollectionCommand<T>(collection, element)
    where T : notnull
{
    /// <summary>
    /// Removes the element from the collection.
    /// </summary>
    protected override void OnExecute() => Collection.Remove(Element);

    /// <summary>
    /// Adds the element to the collection.
    /// </summary>
    protected override void OnUndo() => Collection.Add(Element);
}

/// <summary>
/// Factory methods for <see cref="RemoveFromCollection{T}"/>.
/// </summary>
public static class RemoveFromCollection
{
    /// <summary>
    /// Creates a new remove from collection command.
    /// </summary>
    /// <param name="collection">The collection to be modified.</param>
    /// <param name="element">The element to be removed from <paramref name="collection"/>.</param>
    /// <typeparam name="T">The type of elements the collection contains.</typeparam>
    public static RemoveFromCollection<T> For<T>(ICollection<T> collection, T element)
        where T : notnull
        => new(collection, element);
}
