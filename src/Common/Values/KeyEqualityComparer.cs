/*
 * Copyright 2010-2014 Bastian Eicher
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser Public License for more details.
 *
 * You should have received a copy of the GNU Lesser Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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
    {
        private readonly Func<T, TKey> _keySelector;

        /// <summary>
        /// Creates a new key equality comparer.
        /// </summary>
        /// <param name="keySelector">A function mapping objects to their respective equality keys.</param>
        public KeyEqualityComparer(Func<T, TKey> keySelector) => _keySelector = keySelector;

        public bool Equals(T x, T y) => Equals(_keySelector(x), _keySelector(y));

        public int GetHashCode(T obj)
        {
            var key = _keySelector(obj);
            return (key == null) ? 0 : _keySelector(obj).GetHashCode();
        }
    }
}
