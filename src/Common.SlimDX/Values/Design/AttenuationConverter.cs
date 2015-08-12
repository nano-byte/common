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
    internal class AttenuationConverter : ValueTypeConverter<Attenuation>
    {
        /// <inheritdoc/>
        protected override int NoArguments { get { return 3; } }

        /// <inheritdoc/>
        protected override ConstructorInfo GetConstructor()
        {
            return typeof(Attenuation).GetConstructor(new[] {typeof(float), typeof(float), typeof(float)});
        }

        /// <inheritdoc/>
        protected override object[] GetArguments(Attenuation value)
        {
            return new object[] {value.Constant, value.Linear, value.Quadratic};
        }

        /// <inheritdoc/>
        protected override string[] GetValues(Attenuation value, ITypeDescriptorContext context, CultureInfo culture)
        {
            var floatConverter = TypeDescriptor.GetConverter(typeof(float));
            return new[]
            {
                floatConverter.ConvertToString(context, culture, value.Constant),
                floatConverter.ConvertToString(context, culture, value.Linear),
                floatConverter.ConvertToString(context, culture, value.Quadratic)
            };
        }

        /// <inheritdoc/>
        protected override Attenuation GetObject(string[] values, CultureInfo culture)
        {
            #region Sanity checks
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (culture == null) throw new ArgumentNullException(nameof(culture));
            #endregion

            return new Attenuation(Convert.ToSingle(values[0], culture), Convert.ToSingle(values[1], culture), Convert.ToSingle(values[2], culture));
        }

        /// <inheritdoc/>
        protected override Attenuation GetObject(IDictionary propertyValues)
        {
            #region Sanity checks
            if (propertyValues == null) throw new ArgumentNullException(nameof(propertyValues));
            #endregion

            return new Attenuation((float)propertyValues["Constant"], (float)propertyValues["Linear"], (float)propertyValues["Quadratic"]);
        }
    }
}
