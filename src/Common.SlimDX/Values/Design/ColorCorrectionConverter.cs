// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace NanoByte.Common.Values.Design
{
    internal class ColorCorrectionConverter : ValueTypeConverter<ColorCorrection>
    {
        /// <inheritdoc/>
        protected override int NoArguments => 4;

        /// <inheritdoc/>
        protected override ConstructorInfo GetConstructor() => typeof(ColorCorrection).GetConstructor(new[] {typeof(float), typeof(float), typeof(float), typeof(float)});

        /// <inheritdoc/>
        protected override object[] GetArguments(ColorCorrection value) => new object[] {value.Brightness, value.Contrast, value.Saturation, value.Hue};

        /// <inheritdoc/>
        protected override string[] GetValues(ColorCorrection value, ITypeDescriptorContext context, CultureInfo culture)
        {
            var floatConverter = TypeDescriptor.GetConverter(typeof(float));
            return new[]
            {
                floatConverter.ConvertToString(context, culture, value.Brightness),
                floatConverter.ConvertToString(context, culture, value.Contrast),
                floatConverter.ConvertToString(context, culture, value.Saturation),
                floatConverter.ConvertToString(context, culture, value.Hue)
            };
        }

        /// <inheritdoc/>
        protected override ColorCorrection GetObject(string[] values, CultureInfo culture)
        {
            #region Sanity checks
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (culture == null) throw new ArgumentNullException(nameof(culture));
            #endregion

            return new ColorCorrection(
                brightness: Convert.ToSingle(values[0], culture),
                contrast: Convert.ToSingle(values[1], culture),
                saturation: Convert.ToSingle(values[2], culture),
                hue: Convert.ToSingle(values[2], culture));
        }

        /// <inheritdoc/>
        protected override ColorCorrection GetObject(IDictionary propertyValues)
        {
            #region Sanity checks
            if (propertyValues == null) throw new ArgumentNullException(nameof(propertyValues));
            #endregion

            return new ColorCorrection(
                brightness: (float)propertyValues["Brightness"],
                contrast: (float)propertyValues["Contrast"],
                saturation: (float)propertyValues["Saturation"],
                hue: (float)propertyValues["Hue"]);
        }
    }
}
