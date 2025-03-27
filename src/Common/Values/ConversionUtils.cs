// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Values;

/// <summary>
/// Helpers for working with type converters.
/// </summary>
[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Getting converters is trim compatible when avoiding Nullable<T>.")]
public static class ConversionUtils
{
    /// <summary>
    /// Uses the type converter for <typeparamref name="TType"/> (set with <see cref="TypeConverterAttribute"/>) to parse a string.
    /// </summary>
    public static TType ConvertFromString<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TType>(this string value)
        where TType : notnull
        => (TType)TypeDescriptor
                 .GetConverter(typeof(TType))
                 .ConvertFromInvariantString(value ?? throw new ArgumentNullException(nameof(value)))!;

    /// <summary>
    /// Uses the type converter for <typeparamref name="TType"/> (set with <see cref="TypeConverterAttribute"/>) to generate a string.
    /// </summary>
    [Pure]
    public static string ConvertToString<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TType>(this TType value)
        where TType : notnull
        => TypeDescriptor
          .GetConverter(typeof(TType))
          .ConvertToInvariantString(value)
        ?? "";
}
