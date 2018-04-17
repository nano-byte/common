// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;

namespace NanoByte.Common
{
    /// <summary>
    /// Compares <see cref="INamed{T}"/> objects based on their <see cref="INamed{T}.Name"/> in a case-insensitive way.
    /// </summary>
    public sealed class NamedComparer<T> : IComparer<T>, IEqualityComparer<T> where T : INamed<T>
    {
        /// <summary>A singleton instance of the comparer.</summary>
        public static readonly NamedComparer<T> Instance = new NamedComparer<T>();

        private NamedComparer() {}

        public int Compare(T x, T y) => StringComparer.OrdinalIgnoreCase.Compare(x.Name, y.Name);

        public bool Equals(T x, T y) => StringComparer.OrdinalIgnoreCase.Equals(x.Name, y.Name);

        public int GetHashCode(T obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name);
    }
}
