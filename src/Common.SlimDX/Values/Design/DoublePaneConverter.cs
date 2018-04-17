// Copyright Bastian Eicher
// Licensed under the MIT License

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
        protected override int NoArguments => 6;

        /// <inheritdoc/>
        protected override ConstructorInfo GetConstructor() => typeof(DoublePlane).GetConstructor(new[]
        {
            typeof(double),
            typeof(double),
            typeof(double),
            typeof(float),
            typeof(float),
            typeof(float)
        });

        /// <inheritdoc/>
        protected override object[] GetArguments(DoublePlane value) => new object[]
        {
            value.Point.X,
            value.Point.Y,
            value.Point.Z,
            value.Normal.X,
            value.Normal.Y,
            value.Normal.Z
        };

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
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (culture == null) throw new ArgumentNullException(nameof(culture));
            #endregion

            return new DoublePlane(
                new DoubleVector3(Convert.ToDouble(values[0], culture), Convert.ToDouble(values[1], culture), Convert.ToDouble(values[2], culture)),
                new Vector3(Convert.ToSingle(values[3], culture), Convert.ToSingle(values[4], culture), Convert.ToSingle(values[5], culture)));
        }

        /// <inheritdoc/>
        protected override DoublePlane GetObject(IDictionary propertyValues)
        {
            #region Sanity checks
            if (propertyValues == null) throw new ArgumentNullException(nameof(propertyValues));
            #endregion

            return new DoublePlane((DoubleVector3)propertyValues["Point"], (Vector3)propertyValues["Normal"]);
        }
    }
}
