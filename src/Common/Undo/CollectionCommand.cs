// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// An undo command that adds or removes an element from a collection.
    /// </summary>
    /// <typeparam name="T">The type of elements the collection contains.</typeparam>
    public abstract class CollectionCommand<T> : SimpleCommand, IValueCommand
    {
        /// <summary>
        /// The collection to be modified.
        /// </summary>
        protected readonly ICollection<T> Collection;

        /// <summary>
        /// The element to be added or removed from <see cref="Collection"/>.
        /// </summary>
        protected readonly T Element;

        /// <inheritdoc/>
        public object Value => Element;

        /// <summary>
        /// Creates a new collection command.
        /// </summary>
        /// <param name="collection">The collection to be modified.</param>
        /// <param name="element">The element to be added or removed from <paramref name="collection"/>.</param>
        protected CollectionCommand(ICollection<T> collection, T element)
        {
            Collection = collection;
            Element = element;
        }
    }
}
