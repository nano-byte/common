// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// An undo command that adds an element to a collection.
/// </summary>
/// <param name="collection">The collection to be modified.</param>
/// <param name="element">The element to be added to <paramref name="collection"/>.</param>
/// <typeparam name="T">The type of elements the collection contains.</typeparam>
[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "The complete name is not ambiguous.")]
public sealed class AddToCollection<T>(ICollection<T> collection, T element) : CollectionCommand<T>(collection, element)
    where T : notnull
{
    /// <summary>
    /// Adds the element to the collection.
    /// </summary>
    protected override void OnExecute() => Collection.Add(Element);

    /// <summary>
    /// Removes the element from the collection.
    /// </summary>
    protected override void OnUndo() => Collection.Remove(Element);
}

/// <summary>
/// Factory methods for <see cref="AddToCollection{T}"/>.
/// </summary>
public static class AddToCollection
{
    /// <summary>
    /// Creates a new add to collection command.
    /// </summary>
    /// <param name="collection">The collection to be modified.</param>
    /// <param name="element">The element to be added to <paramref name="collection"/>.</param>
    /// <typeparam name="T">The type of elements the collection contains.</typeparam>
    public static AddToCollection<T> For<T>(ICollection<T> collection, T element)
        where T : notnull
        => new(collection, element);
}
