// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using JetBrains.Annotations;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Represents a tuple of comparable elements.
    /// </summary>
    [Serializable]
    public struct ComparableTuple<T> : IEquatable<ComparableTuple<T>>, IComparable<ComparableTuple<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        /// <summary>
        /// The first element of the tuple.
        /// </summary>
        [NotNull]
        public readonly T Key;

        /// <summary>
        /// The second element of the tuple.
        /// </summary>
        [NotNull]
        public readonly T Value;

        /// <summary>
        /// Creates a new comparable tuple.
        /// </summary>
        /// <param name="key">The first element of the tuple.</param>
        /// <param name="value">The second element of the tuple.</param>
        public ComparableTuple([NotNull] T key, [NotNull] T value)
        {
            #region Sanity checks
            // ReSharper disable CompareNonConstrainedGenericWithNull
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            // ReSharper restore CompareNonConstrainedGenericWithNull
            #endregion

            Key = key;
            Value = value;
        }

        #region Conversion
        /// <summary>
        /// Returns the tuple in the form "Key = Value". Not safe for parsing!
        /// </summary>
        public override string ToString() => Key + " = " + Value;
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(ComparableTuple<T> other) => Equals(Key, other.Key) && Equals(Value, other.Value);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is ComparableTuple<T> tuple && Equals(tuple);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = Key.GetHashCode();
                result = (result * 397) ^ Value.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(ComparableTuple<T> left, ComparableTuple<T> right) => left.Equals(right);
        public static bool operator !=(ComparableTuple<T> left, ComparableTuple<T> right) => !left.Equals(right);
        #endregion

        #region Comparison
        /// <inheritdoc/>
        public int CompareTo(ComparableTuple<T> other)
        {
            // Compare by Key first, then by Value if that was equal
            int keyCompare = Key.CompareTo(other.Key);
            return (keyCompare == 0) ? Value.CompareTo(other.Value) : keyCompare;
        }

        public static bool operator <(ComparableTuple<T> left, ComparableTuple<T> right) => left.CompareTo(right) < 0;
        public static bool operator >(ComparableTuple<T> left, ComparableTuple<T> right) => left.CompareTo(right) > 0;
        public static bool operator <=(ComparableTuple<T> left, ComparableTuple<T> right) => left.CompareTo(right) <= 0;
        public static bool operator >=(ComparableTuple<T> left, ComparableTuple<T> right) => left.CompareTo(right) >= 0;
        #endregion
    }
}
