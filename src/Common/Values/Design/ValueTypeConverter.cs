// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace NanoByte.Common.Values.Design;

/// <summary>
/// Abstract base-class for easily creating a <see cref="TypeConverter"/> for a struct (value type).
/// </summary>
/// <typeparam name="T">The struct to create the <see cref="TypeConverter"/> for.</typeparam>
/// <remarks>Providing a <see cref="TypeConverter"/> for a struct improves the runtime experience with PropertyGrids.</remarks>
/// <example>
///   Add this attribute to the struct:
///   <code>[TypeConverter(typeof(ClassDerivedFromThisOne))]</code>
/// </example>
public abstract class ValueTypeConverter<T> : TypeConverter where T : struct
{
    #region Capabilities
    /// <inheritdoc/>
    public override bool GetCreateInstanceSupported(ITypeDescriptorContext? context) => true;

    /// <inheritdoc/>
    public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;

    /// <inheritdoc/>
    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes) =>
        TypeDescriptor.GetProperties(value, attributes);

    /// <inheritdoc/>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) =>
        destinationType == typeof(InstanceDescriptor) || base.CanConvertFrom(context, destinationType!);

    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    #endregion

    #region Convert to
    /// <inheritdoc/>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (culture == null) throw new ArgumentNullException(nameof(culture));

        if (destinationType == typeof(InstanceDescriptor))
            return new InstanceDescriptor(GetConstructor(), GetArguments((T)value!));

        if (destinationType == typeof(string))
            return string.Join(GetElementSeparator(culture), GetValues((T)value!, context, culture));

        return base.ConvertTo(context, culture, value, destinationType);
    }
    #endregion

    #region Convert from
    /// <inheritdoc/>
    // ReSharper disable once NullnessAnnotationConflictWithJetBrainsAnnotations
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (culture == null) throw new ArgumentNullException(nameof(culture));

        if (value is not string sValue) return base.ConvertFrom(context!, culture, value);

        sValue = sValue.Trim();
        if (sValue.Length == 0) return null;

        var arguments = sValue.Split(GetElementSeparator(culture)[0]);
        if (arguments.Length != NoArguments) return null;
        return GetObject(arguments, culture);
    }
    #endregion

    #region Create instance
    /// <inheritdoc/>
    public override object CreateInstance(ITypeDescriptorContext? context, IDictionary propertyValues) => GetObject(propertyValues);
    #endregion

    //--------------------//

    #region Hooks
    /// <summary>The separator to place between individual elements.</summary>
    protected virtual string GetElementSeparator(CultureInfo culture)
    {
        #region Sanity checks
        if (culture == null) throw new ArgumentNullException(nameof(culture));
        #endregion

        return culture.TextInfo.ListSeparator;
    }

    /// <summary>The number of arguments the constructor of <typeparamref name="T"/> has.</summary>
    protected abstract int NoArguments { get; }

    /// <returns>The constructor used to create new instances of <typeparamref name="T"/> (deserialization).</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    protected abstract ConstructorInfo GetConstructor();

    /// <returns>The arguments for the constructor of <typeparamref name="T"/>.</returns>
    protected abstract object[] GetArguments(T value);

    /// <returns>The elements of <typeparamref name="T"/> converted to strings.</returns>
    protected abstract string[] GetValues(T value, ITypeDescriptorContext? context, CultureInfo culture);

    /// <returns>A new instance of <typeparamref name="T"/>.</returns>
    protected abstract T GetObject(string[] values, CultureInfo culture);

    /// <returns>A new instance of <typeparamref name="T"/>.</returns>
    protected abstract T GetObject(IDictionary propertyValues);
    #endregion
}
