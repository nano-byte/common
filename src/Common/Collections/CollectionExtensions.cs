/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Provides extension methods for <see cref="ICollection{T}"/>s and <see cref="List{T}"/>s.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds multiple elements to the list.
        /// </summary>
        /// <remarks>This is a covariant wrapper for <see cref="List{T}.AddRange"/>.</remarks>
        public static void AddRange<TList, TElements>([NotNull] this List<TList> list, [NotNull, InstantHandle] IEnumerable<TElements> elements)
            where TElements : TList
        {
            #region Sanity checks
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            #endregion

            list.AddRange(elements.Cast<TList>());
        }

        /// <summary>
        /// Adds an element to the collection if it does not already <see cref="ICollection{T}.Contains"/> the element.
        /// </summary>
        /// <returns><c>true</c> if the element was added to the collection; <c>true</c> if the collection already contained the element.</returns>
        /// <remarks>This makes it possible to use a <see cref="ICollection{T}"/> with semantics similar to a <see cref="HashSet{T}"/>.</remarks>
        public static bool AddIfNew<T>([NotNull] this ICollection<T> collection, T element)
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
        public static void AddRange<TCollection, TElements>([NotNull] this ICollection<TCollection> collection, [NotNull, InstantHandle] IEnumerable<TElements> elements)
            where TElements : TCollection
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            #endregion

            foreach (var element in elements) collection.Add(element);
        }

        /// <summary>
        /// Removes multiple elements from the collection.
        /// </summary>
        /// <seealso cref="List{T}.RemoveRange"/>
        public static void RemoveRange<TCollection, TElements>([NotNull] this ICollection<TCollection> collection, [NotNull, InstantHandle] IEnumerable<TElements> elements)
            where TElements : TCollection
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            #endregion

            foreach (var element in elements) collection.Remove(element);
        }

        /// <summary>
        /// Removes all items from a <paramref name="collection"/> that match a specific <paramref name="condition"/>.
        /// </summary>
        /// <returns><c>true</c> if any elements where removed.</returns>
        /// <seealso cref="List{T}.RemoveAll"/>
        public static bool RemoveAll<T>([NotNull, InstantHandle] this ICollection<T> collection,
            [NotNull] Func<T, bool> condition)
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
        /// Removes the last n elements from the list.
        /// </summary>
        /// <param name="list">The list to remove the elements from.</param>
        /// <param name="number">The number of elements to remove.</param>
        public static void RemoveLast<T>([NotNull] this List<T> list, int number = 1)
        {
            #region Sanity checks
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (number < 0) throw new ArgumentOutOfRangeException(nameof(number));
            #endregion

            list.RemoveRange(list.Count - number, number);
        }

        /// <summary>
        /// Adds or replaces an element in a list using a key selector for comparison.
        /// </summary>
        /// <param name="list">The list to update.</param>
        /// <param name="element">The element to add or update.</param>
        /// <param name="keySelector">Used to map elements to keys for comparison</param>
        /// <returns></returns>
        public static bool AddOrReplace<T, TKey>([NotNull] this List<T> list, [NotNull] T element, [NotNull, InstantHandle] Func<T, TKey> keySelector)
        {
            #region Sanity checks
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            #endregion

            int index = list.FindIndex(x => Equals(keySelector(x), keySelector(element)));

            if (index == -1)
            {
                list.Add(element);
                return true;
            }
            else if (!element.Equals(list[index]))
            {
                list[index] = element;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Adds or replaces an element in a list.
        /// </summary>
        /// <param name="list">The list to update.</param>
        /// <param name="element">The element to add or update.</param>
        /// <returns></returns>
        public static bool AddOrReplace<T>(this List<T> list, T element)
        {
            #region Sanity checks
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (element == null) throw new ArgumentNullException(nameof(element));
            #endregion

            return list.AddOrReplace(element, x => x);
        }

        /// <summary>
        /// Determines whether the collection contains an element or is null.
        /// </summary>
        /// <param name="collection">The list to check.</param>
        /// <param name="element">The element to look for.</param>
        /// <remarks>Useful for lists that contain an OR-ed list of restrictions, where an empty list means no restrictions.</remarks>
        public static bool ContainsOrEmpty<T>([NotNull] this ICollection<T> collection, [NotNull] T element)
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
        [Pure]
        public static bool ContainsAny<T>([NotNull, InstantHandle] this ICollection<T> first, [NotNull, InstantHandle] ICollection<T> second, [CanBeNull] IEqualityComparer<T> comparer = null)
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
        [Pure]
        public static bool SequencedEquals<T>([NotNull, InstantHandle] this ICollection<T> first, [NotNull, InstantHandle] ICollection<T> second, [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            #endregion

            if (first.Count != second.Count) return false;
            return first.SequenceEqual(second, comparer ?? EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Determines whether two collections contain the same elements disregarding the order they are in.
        /// </summary>
        /// <param name="first">The first of the two collections to compare.</param>
        /// <param name="second">The first of the two collections to compare.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        [Pure]
        public static bool UnsequencedEquals<T>([NotNull, InstantHandle] this ICollection<T> first, [NotNull, InstantHandle] ICollection<T> second, [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            #endregion

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
        [Pure]
        public static int GetSequencedHashCode<T>([NotNull, InstantHandle] this ICollection<T> collection, [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            #endregion

            if (comparer == null) comparer = EqualityComparer<T>.Default;
            unchecked
            {
                int result = 397;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (T unknown in collection.WhereNotNull())
                    result = (result * 397) ^ comparer.GetHashCode(unknown);
                return result;
            }
        }

        /// <summary>
        /// Generates a hash code for the contents of the collection. Changing the elements' order will not change the hash.
        /// </summary>
        /// <param name="collection">The collection to generate the hash for.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        /// <seealso cref="UnsequencedEquals{T}"/>
        [Pure]
        public static int GetUnsequencedHashCode<T>([NotNull, InstantHandle] this ICollection<T> collection, [CanBeNull, InstantHandle] IEqualityComparer<T> comparer = null)
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            #endregion

            if (comparer == null) comparer = EqualityComparer<T>.Default;
            unchecked
            {
                int result = 397;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (T unknown in collection.WhereNotNull())
                    result = result ^ comparer.GetHashCode(unknown);
                return result;
            }
        }
    }
}
