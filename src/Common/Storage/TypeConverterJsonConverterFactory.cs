// Copyright Bastian Eicher
// Licensed under the MIT License

#if NET
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NanoByte.Common.Storage;

/// <summary>
/// Serializes types annotated with a <see cref="TypeConverterAttribute"/> as strings, including when used as dictionary keys.
/// </summary>
/// <remarks>Mimics the behavior of Newtonsoft.Json, which uses <see cref="TypeConverter"/>s for string conversion.</remarks>
[RequiresUnreferencedCode("TypeConverters are resolved via reflection.")]
[RequiresDynamicCode("Converters are constructed for generic types at runtime.")]
internal sealed class TypeConverterJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => !typeToConvert.IsEnum // Newtonsoft.Json treats enums as numbers
        && typeToConvert != typeof(Uri) // Handled natively by System.Text.Json
        && typeToConvert.IsDefined(typeof(TypeConverterAttribute), inherit: false)
        && TypeDescriptor.GetConverter(typeToConvert) is {} converter
        && converter.CanConvertFrom(typeof(string))
        && converter.CanConvertTo(typeof(string));

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => (JsonConverter)Activator.CreateInstance(
            typeof(Converter<>).MakeGenericType(typeToConvert),
            TypeDescriptor.GetConverter(typeToConvert))!;

    private sealed class Converter<T>(TypeConverter converter) : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected a string for {typeToConvert.Name} but found {reader.TokenType}.");
            return FromString(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            => writer.WriteStringValue(ToString(value));

        public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => FromString(reader.GetString()!);

        public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            => writer.WritePropertyName(ToString(value));

        private T FromString(string value)
        {
            try
            {
                return (T)converter.ConvertFromInvariantString(value)!;
            }
            #region Error handling
            catch (Exception ex) when (ex is not JsonException)
            {
                // Convert exception type
                throw new JsonException(ex.Message, ex);
            }
            #endregion
        }

        private string ToString(T value)
            => converter.ConvertToInvariantString(value) ?? "";
    }
}
#endif
