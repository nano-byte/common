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
    internal class DoubleVector3Converter : ValueTypeConverter<DoubleVector3>
    {
        /// <inheritdoc/>
        protected override int NoArguments { get { return 3; } }

        /// <inheritdoc/>
        protected override ConstructorInfo GetConstructor()
        {
            return typeof(DoubleVector3).GetConstructor(new[] {typeof(double), typeof(double), typeof(double)});
        }

        /// <inheritdoc/>
        protected override object[] GetArguments(DoubleVector3 value)
        {
            return new object[] {value.X, value.Y};
        }

        /// <inheritdoc/>
        protected override string[] GetValues(DoubleVector3 value, ITypeDescriptorContext context, CultureInfo culture)
        {
            var doubleConverter = TypeDescriptor.GetConverter(typeof(double));
            return new[]
            {
                doubleConverter.ConvertToString(context, culture, value.X),
                doubleConverter.ConvertToString(context, culture, value.Y),
                doubleConverter.ConvertToString(context, culture, value.Z)
            };
        }

        /// <inheritdoc/>
        protected override DoubleVector3 GetObject(string[] values, CultureInfo culture)
        {
            #region Sanity checks
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (culture == null) throw new ArgumentNullException(nameof(culture));
            #endregion

            return new DoubleVector3(Convert.ToDouble(values[0], culture), Convert.ToDouble(values[1], culture), Convert.ToDouble(values[2], culture));
        }

        /// <inheritdoc/>
        protected override DoubleVector3 GetObject(IDictionary propertyValues)
        {
            #region Sanity checks
            if (propertyValues == null) throw new ArgumentNullException(nameof(propertyValues));
            #endregion

            return new DoubleVector3((double)propertyValues["X"], (double)propertyValues["Y"], (double)propertyValues["Z"]);
        }
    }
}
