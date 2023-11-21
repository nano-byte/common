// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Values;

/// <summary>
/// Specifies the equality of objects based on the equality of a key extracted from the objects.
/// </summary>
/// <param name="keySelector">A function mapping objects to their respective equality keys.</param>
/// <typeparam name="T">The type of objects to compare.</typeparam>
/// <typeparam name="TKey">The type of the key to use to determine equality.</typeparam>
public class KeyEqualityComparer<T, TKey>(Func<T, TKey> keySelector) : IEqualityComparer<T>
    where T : notnull
    where TKey : notnull
{
    public bool Equals(T? x, T? y)
        => Equals(
            x == null ? null : keySelector(x),
            y == null ? null : keySelector(y));

    public int GetHashCode(T obj)
        => keySelector(obj).GetHashCode();
}
