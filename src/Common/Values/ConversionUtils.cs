// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Values;

/// <summary>
/// Helpers for working with type converters.
/// </summary>
[RequiresUnreferencedCode("Uses reflection to discover the converter type.")]
public static class ConversionUtils
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
