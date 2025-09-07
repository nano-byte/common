// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// Provides extension methods for <see cref="List{T}"/>s.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Adds multiple elements to the list.
    /// </summary>
    /// <remarks>This is a covariant wrapper for <see cref="List{T}.AddRange"/>.</remarks>
    public static void AddRange<TList, TElements>(this IList<TList> list, [InstantHandle] IEnumerable<TElements> elements)
        where TElements : TList
    {
        #region Sanity checks
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (elements == null) throw new ArgumentNullException(nameof(elements));
        #endregion

        if (list is List<TList> x)
            x.AddRange(elements.Cast<TList>());
        else
        {
            foreach (var element in elements)
                list.Add(element);
        }
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
    public static bool AddOrReplace<T, TKey>(this List<T> list, T element, [InstantHandle] Func<T, TKey> keySelector)
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
    /// Assumes two sorted lists. Determines which elements are present in <paramref name="newList"/> but not in <paramref name="oldList"/>.
    /// </summary>
    /// <param name="newList">The new list of elements; can be <c>null</c> (will be treated as an empty list).</param>
    /// <param name="oldList">The original list of elements; can be <c>null</c> (will be treated as an empty list).</param>
    /// <param name="comparer">An object that compares to elements to determine which one is bigger.</param>
    /// <returns>A list of elements that were added.</returns>
    /// <remarks>Elements that are present in <paramref name="oldList"/> but not in <paramref name="newList"/> are ignored. Elements that are equal for <see cref="IComparable{T}.CompareTo"/> but have been otherwise modified will be added.</remarks>
    [Pure]
    public static IList<T> GetAddedElements<T>(this IList<T>? newList, IList<T>? oldList, IComparer<T> comparer)
    {
        if (newList == null) return new T[0];
        if (oldList == null) return newList;

        var added = new LinkedList<T>();

        int oldCounter = 0;
        int newCounter = 0;
        while (newCounter < newList.Count)
        {
            var newElement = newList[newCounter];
            int comparison = oldCounter < oldList.Count
                // In-range, compare elements
                ? comparer.Compare(oldList[oldCounter], newElement)
                // Out-of-range, add all remaining new elements
                : 1;

            switch (comparison)
            {
                case < 0: // old < new
                    oldCounter++;
                    break;
                case > 0: // old > new
                    added.AddLast(newElement);
                    newCounter++;
                    break;
                default: // old == new
                    oldCounter++;
                    newCounter++;
                    break;
            }
        }

        var result = new T[added.Count];
        added.CopyTo(result, 0);
        return result;
    }

    /// <summary>
    /// Assumes two sorted lists. Determines which elements are present in <paramref name="newList"/> but not in <paramref name="oldList"/>.
    /// </summary>
    /// <param name="newList">The new list of elements; can be <c>null</c> (will be treated as an empty list).</param>
    /// <param name="oldList">The original list of elements; can be <c>null</c> (will be treated as an empty list).</param>
    /// <returns>A list of elements that were added.</returns>
    /// <remarks>Elements that are present in <paramref name="oldList"/> but not in <paramref name="newList"/> are ignored. Elements that are equal for <see cref="IComparable{T}.CompareTo"/> but have been otherwise modified will be added.</remarks>
    [Pure]
    public static IList<T> GetAddedElements<T>(this IList<T>? newList, IList<T>? oldList)
        where T : IComparable<T>, IEquatable<T> => GetAddedElements(newList, oldList, DefaultComparer<T>.Instance);
}
