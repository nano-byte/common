/*
 * Copyright 2006-2014 Bastian Eicher
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
    /// Provides extension methods for <see cref="List{T}"/>s.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Adds multiple elements to the list.
        /// </summary>
        /// <remarks>This is a covariant wrapper for <see cref="List{T}.AddRange"/>.</remarks>
        public static void AddRange<TList, TElements>([NotNull] this List<TList> list, [NotNull, InstantHandle] IEnumerable<TElements> elements)
            where TElements : TList
        {
            #region Sanity checks
            if (list == null) throw new ArgumentNullException("list");
            if (elements == null) throw new ArgumentNullException("elements");
            #endregion

            list.AddRange(elements.Cast<TList>());
        }

        /// <summary>
        /// Adds an element to a <see cref="List{T}"/> if the list list does not already <see cref="List{T}.Contains"/> the element.
        /// </summary>
        /// <returns><see langword="true"/> if the element was added to the list; <see langword="true"/> if the list already contained the element.</returns>
        /// <remarks>This makes it possible to use a <see cref="List{T}"/> with semantics similar to a <see cref="SortedSet{T}"/>, but without ordering.</remarks>
        public static bool AddIfNew<T>([NotNull] this ICollection<T> list, T element)
        {
            #region Sanity checks
            if (list == null) throw new ArgumentNullException("list");
            #endregion

            if (list.Contains(element)) return false;
            else
            {
                list.Add(element);
                return true;
            }
        }

        /// <summary>
        /// Adds multiple elements to the list.
        /// </summary>
        public static void AddRange<TList, TElements>([NotNull] this ICollection<TList> list, [NotNull, InstantHandle] IEnumerable<TElements> elements)
            where TElements : TList
        {
            #region Sanity checks
            if (list == null) throw new ArgumentNullException("list");
            if (elements == null) throw new ArgumentNullException("elements");
            #endregion

            foreach (var element in elements) list.Add(element);
        }

        /// <summary>
        /// Removes multiple elements from the list.
        /// </summary>
        public static void RemoveRange<TList, TElements>([NotNull] this ICollection<TList> list, [NotNull, InstantHandle] IEnumerable<TElements> elements)
            where TElements : TList
        {
            #region Sanity checks
            if (list == null) throw new ArgumentNullException("list");
            if (elements == null) throw new ArgumentNullException("elements");
            #endregion

            foreach (var element in elements) list.Remove(element);
        }

        /// <summary>
        /// Removes the last n elements from the list.
        /// </summary>
        /// <param name="list">The list to remove the elements from.</param>
        /// <param name="number">The number of elements to remove.</param>
        public static void RemoveLast<T>([NotNull] this List<T> list, int number = 1)
        {
            #region Sanity checks
            if (list == null) throw new ArgumentNullException("list");
            if (number < 0) throw new ArgumentOutOfRangeException("number");
            #endregion

            list.RemoveRange(list.Count - number, number);
        }

        /// <summary>
        /// Determines whether the list contains an element or is null.
        /// </summary>
        /// <param name="list">The list to check.</param>
        /// <param name="element">The element to look for.</param>
        /// <remarks>This is usefull e.g. for lists that contain an OR-ed list of restrictions, where an empty list means no restrictions.</remarks>
        public static bool ContainsOrEmpty<T>([NotNull] this ICollection<T> list, T element)
        {
            #region Sanity checks
            if (list == null) throw new ArgumentNullException("list");
            if (element == null) throw new ArgumentNullException("element");
            #endregion

            return (list.Count == 0) || list.Contains(element);
        }
    }
}
