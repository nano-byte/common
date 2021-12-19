// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Globalization;
using System.Xml.Serialization;

namespace NanoByte.Common.Values.Design
{
    /// <summary>
    /// Type converter for <see cref="Enum"/>s annotated with <see cref="XmlEnumAttribute"/>s.
    /// </summary>
    /// <typeparam name="T">The type the converter is used for.</typeparam>
    /// <example>
    ///   Add this attribute to the <see cref="Enum"/>:
    ///   <code>[TypeConverter(typeof(XmlEnumConverter&lt;NameOfEnum&gt;))]</code>
    /// </example>
    /// <remarks><see cref="XmlEnumAttribute.Name"/> is used as the case-insensitive string representation (falls back to element name).</remarks>
    public class EnumXmlConverter<T> : TypeConverter
        where T : struct
    {
        private static object GetEnumFromString(string stringValue)
        {
            foreach (var field in typeof(T).GetFields())
            {
                var attributes = (XmlEnumAttribute[])field.GetCustomAttributes(typeof(XmlEnumAttribute), inherit: false);
                if (attributes.Length > 0 && StringUtils.EqualsIgnoreCase(attributes[0].Name, stringValue))
                    return field.GetValue(field.Name)!;
            }
            return Enum.Parse(typeof(T), stringValue, ignoreCase: true);
        }

        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
            => value is string stringValue
                ? GetEnumFromString(stringValue)
                : base.ConvertFrom(context!, culture!, value)!;

        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            => value is Enum enumValue && destinationType == typeof(string)
                ? enumValue.GetEnumAttribute<XmlEnumAttribute>()?.Name ?? enumValue.ToString()!
                : base.ConvertTo(context, culture, value, destinationType)!;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => true;

        private static readonly string[] _values = (from T value in Enum.GetValues(typeof(T)) select value.ConvertToString()).ToArray();

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context) => new(_values);
    }
}
