// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Provides extension methods for <see cref="ICollection{T}"/>s.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds an element to the collection if it does not already <see cref="ICollection{T}.Contains"/> the element.
        /// </summary>
        /// <returns><c>true</c> if the element was added to the collection; <c>true</c> if the collection already contained the element.</returns>
        /// <remarks>This makes it possible to use a <see cref="ICollection{T}"/> with semantics similar to a <see cref="HashSet{T}"/>.</remarks>
        public static bool AddIfNew<T>(this ICollection<T> collection, T element)
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
        /// <seealso cref="List{T}.AddRange"/>
        public static void AddRange<TCollection, TElements>(this ICollection<TCollection> collection, IEnumerable<TElements> elements)
            where TElements : TCollection
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            #endregion

            foreach (var element in elements)
                collection.Add(element);
        }

        /// <summary>
        /// Removes multiple elements from the collection.
        /// </summary>
        /// <seealso cref="List{T}.RemoveRange"/>
        public static void RemoveRange<TCollection, TElements>(this ICollection<TCollection> collection, IEnumerable<TElements> elements)
            where TElements : TCollection
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            #endregion

            foreach (var element in elements)
                collection.Remove(element);
        }

        /// <summary>
        /// Removes all items from a <paramref name="collection"/> that match a specific <paramref name="condition"/>.
        /// </summary>
        /// <returns><c>true</c> if any elements where removed.</returns>
        /// <seealso cref="List{T}.RemoveAll"/>
        public static bool RemoveAll<T>(this ICollection<T> collection, Func<T, bool> condition)
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

        /// <summary>
        /// Determines whether the collection contains an element or is null.
        /// </summary>
        /// <param name="collection">The list to check.</param>
        /// <param name="element">The element to look for.</param>
        /// <remarks>Useful for lists that contain an OR-ed list of restrictions, where an empty list means no restrictions.</remarks>
        public static bool ContainsOrEmpty<T>(this ICollection<T> collection, T element)
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (element == null) throw new ArgumentNullException(nameof(element));
            #endregion

            return (collection.Count == 0) || collection.Contains(element);
        }

        /// <summary>
        /// Determines whether one collection of elements contains any of the elements in another.
        /// </summary>
        /// <param name="first">The first of the two collections to compare.</param>
        /// <param name="second">The first of the two collections to compare.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        /// <returns><c>true</c> if <paramref name="first"/> contains any element from <paramref name="second"/>. <c>false</c> if <paramref name="first"/> or <paramref name="second"/> is empty.</returns>
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
        public static bool ContainsAny<T>(this ICollection<T> first, ICollection<T> second, IEqualityComparer<T>? comparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            #endregion

            var set = new HashSet<T>(first, comparer ?? EqualityComparer<T>.Default);
            return second.Any(set.Contains);
        }

        /// <summary>
        /// Determines whether two collections contain the same elements in the same order.
        /// </summary>
        /// <param name="first">The first of the two collections to compare.</param>
        /// <param name="second">The first of the two collections to compare.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
        public static bool SequencedEquals<T>(this ICollection<T> first, ICollection<T> second, IEqualityComparer<T>? comparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            #endregion

            if (ReferenceEquals(first, second)) return true;
            if (first.Count != second.Count) return false;
            return first.SequenceEqual(second, comparer ?? EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Determines whether two collections contain the same elements disregarding the order they are in.
        /// </summary>
        /// <param name="first">The first of the two collections to compare.</param>
        /// <param name="second">The first of the two collections to compare.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
        public static bool UnsequencedEquals<T>(this ICollection<T> first, ICollection<T> second, IEqualityComparer<T>? comparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            #endregion

            if (ReferenceEquals(first, second)) return true;
            if (first.Count != second.Count) return false;
            var set = new HashSet<T>(first, comparer ?? EqualityComparer<T>.Default);
            return second.All(set.Contains);
        }

        /// <summary>
        /// Generates a hash code for the contents of the collection. Changing the elements' order will change the hash.
        /// </summary>
        /// <param name="collection">The collection to generate the hash for.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        /// <seealso cref="SequencedEquals{T}(ICollection{T},ICollection{T},IEqualityComparer{T})"/>
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
        public static int GetSequencedHashCode<T>(this ICollection<T> collection, IEqualityComparer<T>? comparer = null)
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            #endregion

            var hash = new System.HashCode();
            foreach (T item in collection)
                hash.Add(item, comparer);
            return hash.ToHashCode();
        }

        /// <summary>
        /// Generates a hash code for the contents of the collection. Changing the elements' order will not change the hash.
        /// </summary>
        /// <param name="collection">The collection to generate the hash for.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        /// <seealso cref="UnsequencedEquals{T}"/>
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
        public static int GetUnsequencedHashCode<T>(this ICollection<T> collection, IEqualityComparer<T>? comparer = null)
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            #endregion

            comparer ??= EqualityComparer<T>.Default;
            unchecked
            {
                int result = 397;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (T item in collection)
                {
                    if (item != null)
                        result = result ^ comparer.GetHashCode(item);
                }
                return result;
            }
        }

        /// <summary>
        /// Generates all possible permutations of a set of <paramref name="elements"/>.
        /// </summary>
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
        public static IEnumerable<T[]> Permutate<T>(this IEnumerable<T> elements)
        {
            #region Sanity checks
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            #endregion

            IEnumerable<T[]> Helper(T[] array, int index)
            {
                if (index >= array.Length - 1)
                    yield return array;
                else
                {
                    for (int i = index; i < array.Length; i++)
                    {
                        var subArray = array.ToArray();
                        var t1 = subArray[index];
                        subArray[index] = subArray[i];
                        subArray[i] = t1;

                        foreach (var element in Helper(subArray, index + 1))
                            yield return element;
                    }
                }
            }

            return Helper(elements.ToArray(), index: 0);
        }
    }
}
