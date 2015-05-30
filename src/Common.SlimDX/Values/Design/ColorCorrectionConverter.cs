/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

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
        protected override int NoArguments { get { return 4; } }

        /// <inheritdoc/>
        protected override ConstructorInfo GetConstructor()
        {
            return typeof(ColorCorrection).GetConstructor(new[] {typeof(float), typeof(float), typeof(float), typeof(float)});
        }

        /// <inheritdoc/>
        protected override object[] GetArguments(ColorCorrection value)
        {
            return new object[] {value.Brightness, value.Contrast, value.Saturation, value.Hue};
        }

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
            if (values == null) throw new ArgumentNullException("values");
            if (culture == null) throw new ArgumentNullException("culture");
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
            if (propertyValues == null) throw new ArgumentNullException("propertyValues");
            #endregion

            return new ColorCorrection(
                brightness: (float)propertyValues["Brightness"],
                contrast: (float)propertyValues["Contrast"],
                saturation: (float)propertyValues["Saturation"],
                hue: (float)propertyValues["Hue"]);
        }
    }
}
