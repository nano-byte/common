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
using JetBrains.Annotations;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Provides extension methods for arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Appends an element to an array.
        /// </summary>
        [NotNull, Pure]
        public static T[] Append<T>([NotNull] this T[] array, T element)
        {
            #region Sanity checks
            if (array == null) throw new ArgumentNullException(nameof(array));
            #endregion

            var result = new T[array.LongLength + 1];
            Array.Copy(array, result, array.LongLength);
            result[array.LongLength] = element;
            return result;
        }

        /// <summary>
        /// Prepends an element to an array.
        /// </summary>
        [NotNull, Pure]
        public static T[] Prepend<T>([NotNull] this T[] array, T element)
        {
            #region Sanity checks
            if (array == null) throw new ArgumentNullException(nameof(array));
            #endregion

            var result = new T[array.LongLength + 1];
            Array.Copy(array, 0, result, 1, array.LongLength);
            result[0] = element;
            return result;
        }

        /// <summary>
        /// Determines whether two arrays contain the same elements in the same order.
        /// </summary>
        /// <param name="first">The first of the two collections to compare.</param>
        /// <param name="second">The first of the two collections to compare.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        [Pure]
        public static bool SequencedEquals<T>([NotNull] this T[] first, [NotNull] T[] second, [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            #endregion

            if (first.Length != second.Length) return false;
            if (comparer == null) comparer = EqualityComparer<T>.Default;

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (int i = 0; i < first.Length; i++)
                if (!comparer.Equals(first[i], second[i])) return false;
            return true;
        }

        /// <summary>
        /// Assumes two sorted arrays. Determines which elements are present in <paramref name="newArray"/> but not in <paramref name="oldArray"/>.
        /// </summary>
        /// <param name="newArray">The new list of elements; can be <c>null</c> (will be treated as an empty array).</param>
        /// <param name="oldArray">The original list of elements; can be <c>null</c> (will be treated as an empty array).</param>
        /// <returns>An array of elements that were added.</returns>
        /// <remarks>Elements that are present in <paramref name="oldArray"/> but not in <paramref name="newArray"/> are ignored. Elements that are equal for <see cref="IComparable{T}.CompareTo"/> but have been otherwise modified will be added.</remarks>
        [NotNull, Pure]
        public static T[] GetAddedElements<T>([CanBeNull] this T[] newArray, [CanBeNull] T[] oldArray)
            where T : IComparable<T>, IEquatable<T>
        {
            return GetAddedElements(newArray, oldArray, DefaultComparer<T>.Instance);
        }

        private sealed class DefaultComparer<T> : IComparer<T> where T : IComparable<T>
        {
            /// <summary>A singleton instance of the comparer.</summary>
            public static readonly DefaultComparer<T> Instance = new DefaultComparer<T>();

            private DefaultComparer()
            {}

            public int Compare(T x, T y)
            {
                return x.CompareTo(y);
            }
        }

        /// <summary>
        /// Assumes two sorted arrays. Determines which elements are present in <paramref name="newArray"/> but not in <paramref name="oldArray"/>.
        /// </summary>
        /// <param name="newArray">The new list of elements; can be <c>null</c> (will be treated as an empty array).</param>
        /// <param name="oldArray">The original list of elements; can be <c>null</c> (will be treated as an empty array).</param>
        /// <param name="comparer">An object that compares to elements to determine which one is bigger.</param>
        /// <returns>An array of elements that were added.</returns>
        /// <remarks>Elements that are present in <paramref name="oldArray"/> but not in <paramref name="newArray"/> are ignored. Elements that are equal for <see cref="IComparable{T}.CompareTo"/> but have been otherwise modified will be added.</remarks>
        [NotNull, Pure]
        public static T[] GetAddedElements<T>([CanBeNull] this T[] newArray, [CanBeNull] T[] oldArray, [NotNull] IComparer<T> comparer)
        {
            #region Sanity checks
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            #endregion

            if (newArray == null) return new T[0];
            if (oldArray == null) return newArray;

            var added = new LinkedList<T>();

            int oldCounter = 0;
            int newCounter = 0;
            while (newCounter < newArray.Length)
            {
                T newElement = newArray[newCounter];
                int comparison = (oldCounter < oldArray.Length)
                    // In-range, compare elements
                    ? comparer.Compare(oldArray[oldCounter], newElement)
                    // Out-of-range, add all remaining new elements
                    : 1;

                if (comparison == 0)
                { // old == new
                    oldCounter++;
                    newCounter++;
                }
                else if (comparison < 0)
                { // old < new
                    oldCounter++;
                }
                else if (comparison > 0)
                { // old > new
                    added.AddLast(newElement);
                    newCounter++;
                }
            }

            var result = new T[added.Count];
            added.CopyTo(result, 0);
            return result;
        }
    }
}
