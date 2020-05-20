// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
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
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            #endregion

            list.AddRange(elements.Cast<TList>());
        }

        /// <summary>
        /// Removes the last n elements from the list.
        /// </summary>
        /// <param name="list">The list to remove the elements from.</param>
        /// <param name="number">The number of elements to remove.</param>
        public static void RemoveLast<T>(this List<T> list, int number = 1)
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
        public static bool AddOrReplace<T, TKey>(this List<T> list, T element, Func<T, TKey> keySelector)
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
    }
}
