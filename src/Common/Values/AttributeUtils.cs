// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Provides helper methods for <see cref="Attribute"/>s.
    /// </summary>
    public static class AttributeUtils
    {
        /// <summary>
        /// Gets the first <typeparamref name="TAttribute"/> attribute set on the <paramref name="target"/> enum value.
        /// </summary>
        [Pure]
        public static TAttribute? GetEnumAttribute<TAttribute>(this Enum target) where TAttribute : Attribute
            => target.GetType()
                     .GetField((target ?? throw new ArgumentNullException(nameof(target))).ToString())
                    ?.GetCustomAttribute<TAttribute>();

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
