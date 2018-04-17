// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// An undo command that adds an element to a collection.
    /// </summary>
    /// <typeparam name="T">The type of elements the collection contains.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "The complete name is not ambiguous.")]
    public sealed class AddToCollection<T> : CollectionCommand<T>
    {
        /// <summary>
        /// Creates a new add to collection command.
        /// </summary>
        /// <param name="collection">The collection to be modified.</param>
        /// <param name="element">The element to be added to <paramref name="collection"/>.</param>
        public AddToCollection(ICollection<T> collection, T element)
            : base(collection, element)
        {}

        /// <summary>
        /// Adds the element to the collection.
        /// </summary>
        protected override void OnExecute() => Collection.Add(Element);

        /// <summary>
        /// Removes the element from the collection.
        /// </summary>
        protected override void OnUndo() => Collection.Remove(Element);
    }
}
