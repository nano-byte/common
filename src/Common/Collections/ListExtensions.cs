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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
        public static void AddRange<TList, TElements>(this List<TList> list, IEnumerable<TElements> elements)
            where TElements : TList
        {
            #region Sanity checks
            if (list == null) throw new ArgumentNullException("list");
            #endregion

            list.AddRange(elements.Cast<TList>());
        }

        /// <summary>
        /// Adds an element to a <see cref="List{T}"/> if the list list does not already <see cref="List{T}.Contains"/> the element.
        /// </summary>
        /// <returns><see langword="true"/> if the element was added to the list; <see langword="true"/> if the list already contained the element.</returns>
        /// <remarks>This makes it possible to use a <see cref="List{T}"/> with semantics similar to a <see cref="SortedSet{T}"/>, but without ordering.</remarks>
        public static bool AddIfNew<T>(this List<T> list, T element)
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
        /// Removes multiple elements from the list.
        /// </summary>
        public static void RemoveRange<TList, TElements>(this List<TList> list, IEnumerable<TElements> elements)
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
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Specifically extends List<T>")]
        public static void RemoveLast<T>(this List<T> list, int number = 1)
        {
            #region Sanity checks
            if (list == null) throw new ArgumentNullException("list");
            if (number < 0) throw new ArgumentOutOfRangeException("number");
            #endregion

            list.RemoveRange(list.Count - number, number);
        }
    }
}
