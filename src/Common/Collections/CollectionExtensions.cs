// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// Provides extension methods for <see cref="ICollection{T}"/>s.
/// </summary>
public static class CollectionExtensions
{
    extension<T>(ICollection<T> collection)
    {
        /// <summary>
        /// Adds an element to the collection if it does not already <see cref="ICollection{T}.Contains"/> the element.
        /// </summary>
        /// <returns><c>true</c> if the element was added to the collection; <c>true</c> if the collection already contained the element.</returns>
        /// <remarks>This makes it possible to use a <see cref="ICollection{T}"/> with semantics similar to a <see cref="HashSet{T}"/>.</remarks>
        public bool AddIfNew(T element)
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            #endregion

            if (collection.Contains(element)) return false;
            else
            {
                collection.Add(element);
                return true;
            }
        }

        /// <summary>
        /// Adds multiple elements to the collection.
        /// </summary>
        public void Add<TElement>([InstantHandle] IEnumerable<TElement> elements)
            where TElement : T
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            #endregion

            if (collection is List<TElement> list) list.AddRange(elements);
            else
            {
                foreach (var element in elements)
                    collection.Add(element);
            }
        }

        /// <summary>
        /// Removes multiple elements from the collection.
        /// </summary>
        /// <returns><c>true</c> if any elements where removed.</returns>
        public bool Remove<TElement>([InstantHandle] IEnumerable<TElement> elements)
            where TElement : T
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            #endregion

            bool removedAny = false;
            foreach (var element in elements)
                removedAny |= collection.Remove(element);
            return removedAny;
        }

        /// <summary>
        /// Removes all items from a <paramref name="collection"/> that match a specific <paramref name="condition"/>.
        /// </summary>
        /// <returns><c>true</c> if any elements where removed.</returns>
        /// <seealso cref="List{T}.RemoveAll"/>
        public bool RemoveAll([InstantHandle] Func<T, bool> condition)
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            #endregion

            bool removedAny = false;
            foreach (var item in collection.Where(condition).ToList())
            {
                collection.Remove(item);
                removedAny = true;
            }
            return removedAny;
        }
    }
}
