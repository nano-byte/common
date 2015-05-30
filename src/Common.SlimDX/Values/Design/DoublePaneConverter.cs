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
using SlimDX;

namespace NanoByte.Common.Values.Design
{
    internal class DoublePlaneConverter : ValueTypeConverter<DoublePlane>
    {
        /// <inheritdoc/>
        protected override int NoArguments { get { return 6; } }

        /// <inheritdoc/>
        protected override ConstructorInfo GetConstructor()
        {
            return typeof(DoublePlane).GetConstructor(new[]
            {
                typeof(double), typeof(double), typeof(double),
                typeof(float), typeof(float), typeof(float)
            });
        }

        /// <inheritdoc/>
        protected override object[] GetArguments(DoublePlane value)
        {
            return new object[]
            {
                value.Point.X, value.Point.Y, value.Point.Z,
                value.Normal.X, value.Normal.Y, value.Normal.Z
            };
        }

        /// <inheritdoc/>
        protected override string[] GetValues(DoublePlane value, ITypeDescriptorContext context, CultureInfo culture)
        {
            var doubleConverter = TypeDescriptor.GetConverter(typeof(double));
            var floatConverter = TypeDescriptor.GetConverter(typeof(float));
            return new[]
            {
                doubleConverter.ConvertToString(context, culture, value.Point.X),
                doubleConverter.ConvertToString(context, culture, value.Point.Y),
                doubleConverter.ConvertToString(context, culture, value.Point.Z),
                floatConverter.ConvertToString(context, culture, value.Normal.X),
                floatConverter.ConvertToString(context, culture, value.Normal.Y),
                floatConverter.ConvertToString(context, culture, value.Normal.Z)
            };
        }

        /// <inheritdoc/>
        protected override DoublePlane GetObject(string[] values, CultureInfo culture)
        {
            #region Sanity checks
            if (values == null) throw new ArgumentNullException("values");
            if (culture == null) throw new ArgumentNullException("culture");
            #endregion

            return new DoublePlane(
                new DoubleVector3(Convert.ToDouble(values[0], culture), Convert.ToDouble(values[1], culture), Convert.ToDouble(values[2], culture)),
                new Vector3(Convert.ToSingle(values[3], culture), Convert.ToSingle(values[4], culture), Convert.ToSingle(values[5], culture)));
        }

        /// <inheritdoc/>
        protected override DoublePlane GetObject(IDictionary propertyValues)
        {
            #region Sanity checks
            if (propertyValues == null) throw new ArgumentNullException("propertyValues");
            #endregion

            return new DoublePlane((DoubleVector3)propertyValues["Point"], (Vector3)propertyValues["Normal"]);
        }
    }
}
