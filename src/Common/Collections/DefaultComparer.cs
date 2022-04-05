// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// Compares objects using their <see cref="IComparable"/> implementation.
/// </summary>
public sealed class DefaultComparer<T> : IComparer<T>
    where T : IComparable<T>
{
    /// <summary>A singleton instance of the comparer.</summary>
    public static readonly DefaultComparer<T> Instance = new();

    private DefaultComparer() {}

    public int Compare(T? x, T? y)
        => (x == null || y == null)
            ? 0
            : x.CompareTo(y);
}
