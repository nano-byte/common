// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Provides helper methods for <see cref="Attribute"/>s.
    /// </summary>
    public static class AttributeUtils
    {
        /// <summary>
        /// Uses the type converter for <typeparamref name="TType"/> (set with <see cref="TypeConverterAttribute"/>) to parse a string.
        /// </summary>
        public static TType ConvertFromString<TType>(this string value)
            => (TType)TypeDescriptor
                     .GetConverter(typeof(TType))
                     .ConvertFromInvariantString(value ?? throw new ArgumentNullException(nameof(value)))!;

        /// <summary>
        /// Uses the type converter for <typeparamref name="TType"/> (set with <see cref="TypeConverterAttribute"/>) to generate a string.
        /// </summary>
        [Pure]
        public static string ConvertToString<TType>(this TType value)
            => TypeDescriptor
              .GetConverter(typeof(TType))
              .ConvertToInvariantString(value ?? throw new ArgumentNullException(nameof(value)))
            ?? "";
    }
}
