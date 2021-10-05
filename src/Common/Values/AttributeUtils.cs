// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        /// Gets the first <typeparamref name="TAttribute"/> attribute set on the <typeparamref name="TTarget"/> type.
        /// </summary>
        /// <returns>Falls back to <see cref="object.ToString"/> if the attribute is missing.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [Pure]
        public static IEnumerable<TAttribute> GetAttributes<TAttribute, TTarget>() where TAttribute : Attribute
        {
            var attributes = typeof(TTarget).GetCustomAttributes(typeof(TAttribute), inherit: true);
            return attributes.OfType<TAttribute>();
        }

        /// <summary>
        /// Gets the first <typeparamref name="TAttribute"/> attribute set on the <paramref name="target"/> enum value.
        /// Then retrieves a value from the attribute using <paramref name="valueRetriever"/>.
        /// </summary>
        /// <returns>Falls back to <see cref="object.ToString"/> if the attribute is missing.</returns>
        [Pure]
        public static string GetEnumAttributeValue<TAttribute>(this Enum target, Converter<TAttribute, string> valueRetriever) where TAttribute : Attribute
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (valueRetriever == null) throw new ArgumentNullException(nameof(valueRetriever));

            var fieldInfo = target.GetType().GetField(target.ToString());
            var attributes = (TAttribute[]?)fieldInfo?.GetCustomAttributes(typeof(TAttribute), inherit: true);
            var attribute = attributes?.FirstOrDefault();
            return (attribute == null) ? target.ToString() : valueRetriever(attribute);
        }

        /// <summary>
        /// Uses the type converter for <typeparamref name="TType"/> (set by <see cref="TypeConverterAttribute"/>) to parse a string.
        /// </summary>
        public static TType ConvertFromString<TType>(this string value)
            => (TType)TypeDescriptor
                     .GetConverter(typeof(TType))
                     .ConvertFromInvariantString(value ?? throw new ArgumentNullException(nameof(value)))!;

        /// <summary>
        /// Uses the type converter for <typeparamref name="TType"/> (set by <see cref="TypeConverterAttribute"/>) to generate a string.
        /// </summary>
        [Pure]
        public static string ConvertToString<TType>(this TType value)
            => TypeDescriptor
              .GetConverter(typeof(TType))
              .ConvertToInvariantString(value ?? throw new ArgumentNullException(nameof(value)))
            ?? "";

        /// <summary>
        /// Retrieves a single value from a Custom <see cref="Attribute"/> associated with an <see cref="Assembly"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of Custom <see cref="Attribute"/> associated with the <paramref name="assembly"/> to retrieve.</typeparam>
        /// <typeparam name="TValue">The type of the value to retrieve from the <typeparamref name="TAttribute"/>.</typeparam>
        /// <param name="assembly">The <see cref="Assembly"/> to retrieve the <typeparamref name="TAttribute"/> from.</param>
        /// <param name="valueRetrieval">A callback used to retrieve a <typeparamref name="TValue"/> from a <typeparamref name="TAttribute"/>.</param>
        /// <returns>The retrieved value or <c>null</c> if no <typeparamref name="TAttribute"/> was found.</returns>
        [Pure]
        public static TValue? GetAttributeValue<TAttribute, TValue>(this Assembly assembly, [InstantHandle] Func<TAttribute, TValue?> valueRetrieval)
            where TAttribute : Attribute
            where TValue : class
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (valueRetrieval == null) throw new ArgumentNullException(nameof(valueRetrieval));

            var attributes = assembly.GetCustomAttributes(typeof(TAttribute), inherit: false);
            return (attributes.Length > 0)
                ? valueRetrieval((TAttribute)attributes[0])
                : default;
        }
    }
}
