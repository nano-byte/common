// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// Provides array-related helper methods.
/// </summary>
public static class ArrayUtils
{
    /// <summary>
    /// Appends an element to an array.
    /// </summary>
    [Pure]
    public static T[] Append<T>(this T[] array, T element)
    {
        #region Sanity checks
        if (array == null) throw new ArgumentNullException(nameof(array));
        #endregion

        var result = new T[array.Length + 1];
        Array.Copy(array, result, array.Length);
        result[array.Length] = element;
        return result;
    }

    /// <summary>
    /// Prepends an element to an array.
    /// </summary>
    [Pure]
    public static T[] Prepend<T>(this T[] array, T element)
    {
        #region Sanity checks
        if (array == null) throw new ArgumentNullException(nameof(array));
        #endregion

        var result = new T[array.Length + 1];
        Array.Copy(array, 0, result, 1, array.Length);
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
    public static bool SequencedEquals<T>(this T[] first, T[] second, IEqualityComparer<T>? comparer = null)
    {
        #region Sanity checks
        if (first == null) throw new ArgumentNullException(nameof(first));
        if (second == null) throw new ArgumentNullException(nameof(second));
        #endregion

        if (first.Length != second.Length) return false;
        comparer ??= EqualityComparer<T>.Default;

        for (int i = 0; i < first.Length; i++)
        {
            if (!comparer.Equals(first[i], second[i]))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Converts an <see cref="ArraySegment{T}"/> to an array. Avoids copying the underlying array if possible.
    /// </summary>
    public static T[] AsArray<T>(this ArraySegment<T> segment)
    {
        if (segment.Array == null)
#if NET20 || NET40 || NET45
                return new T[0];
#else
            return Array.Empty<T>();
#endif

        if (segment.Offset == 0 && segment.Count == segment.Array.Length)
            return segment.Array;

        T[] array = new T[segment.Count];
        Array.Copy(segment.Array!, segment.Offset, array, 0, segment.Count);
        return array;
    }
}
