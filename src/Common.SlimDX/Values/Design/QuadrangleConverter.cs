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
    internal class QuadrangleConverter : ValueTypeConverter<Quadrangle>
    {
        /// <inheritdoc/>
        protected override int NoArguments => 8;

        /// <inheritdoc/>
        protected override ConstructorInfo GetConstructor() => typeof(Quadrangle).GetConstructor(new[]
        {
            typeof(float),
            typeof(float),
            typeof(float),
            typeof(float),
            typeof(float),
            typeof(float),
            typeof(float),
            typeof(float)
        });

        /// <inheritdoc/>
        protected override object[] GetArguments(Quadrangle value) => new object[]
        {
            value.P1.X,
            value.P1.Y,
            value.P2.X,
            value.P2.Y,
            value.P3.X,
            value.P3.Y,
            value.P4.X,
            value.P4.Y
        };

        /// <inheritdoc/>
        protected override string[] GetValues(Quadrangle value, ITypeDescriptorContext context, CultureInfo culture)
        {
            var floatConverter = TypeDescriptor.GetConverter(typeof(float));
            return new[]
            {
                floatConverter.ConvertToString(context, culture, value.P1.X),
                floatConverter.ConvertToString(context, culture, value.P1.Y),
                floatConverter.ConvertToString(context, culture, value.P2.X),
                floatConverter.ConvertToString(context, culture, value.P2.Y),
                floatConverter.ConvertToString(context, culture, value.P3.X),
                floatConverter.ConvertToString(context, culture, value.P3.Y),
                floatConverter.ConvertToString(context, culture, value.P4.X),
                floatConverter.ConvertToString(context, culture, value.P4.Y)
            };
        }

        /// <inheritdoc/>
        protected override Quadrangle GetObject(string[] values, CultureInfo culture)
        {
            #region Sanity checks
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (culture == null) throw new ArgumentNullException(nameof(culture));
            #endregion

            return new Quadrangle(
                new Vector2(Convert.ToSingle(values[0], culture), Convert.ToSingle(values[1], culture)),
                new Vector2(Convert.ToSingle(values[2], culture), Convert.ToSingle(values[3], culture)),
                new Vector2(Convert.ToSingle(values[4], culture), Convert.ToSingle(values[5], culture)),
                new Vector2(Convert.ToSingle(values[6], culture), Convert.ToSingle(values[7], culture)));
        }

        /// <inheritdoc/>
        protected override Quadrangle GetObject(IDictionary propertyValues)
        {
            #region Sanity checks
            if (propertyValues == null) throw new ArgumentNullException(nameof(propertyValues));
            #endregion

            return new Quadrangle(
                (Vector2)propertyValues["P1"], (Vector2)propertyValues["P2"],
                (Vector2)propertyValues["P3"], (Vector2)propertyValues["P4"]);
        }
    }
}
