// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Specifies the equality of objects based on the equality of a key extracted from the objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    /// <typeparam name="TKey">The type of the key to use to determine equality.</typeparam>
    public class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
        where T : notnull
        where TKey : notnull
    {
        private readonly Func<T, TKey> _keySelector;

        /// <summary>
        /// Creates a new key equality comparer.
        /// </summary>
        /// <param name="keySelector">A function mapping objects to their respective equality keys.</param>
        public KeyEqualityComparer(Func<T, TKey> keySelector)
        {
            _keySelector = keySelector;
        }

        public bool Equals(T? x, T? y)
            => Equals(
                x == null ? null : _keySelector(x),
                y == null ? null : _keySelector(y));

        public int GetHashCode(T obj)
            => _keySelector(obj).GetHashCode();
    }
}
