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
    internal class Vector2RayConverter : ValueTypeConverter<Vector2Ray>
    {
        /// <inheritdoc/>
        protected override int NoArguments => 4;

        /// <inheritdoc/>
        protected override ConstructorInfo GetConstructor() => typeof(Vector2Ray).GetConstructor(new[]
        {
            typeof(float),
            typeof(float),
            typeof(float),
            typeof(float)
        });

        /// <inheritdoc/>
        protected override object[] GetArguments(Vector2Ray value) => new object[]
        {
            value.Position.X,
            value.Position.Y,
            value.Direction.X,
            value.Direction.Y
        };

        /// <inheritdoc/>
        protected override string[] GetValues(Vector2Ray value, ITypeDescriptorContext context, CultureInfo culture)
        {
            var floatConverter = TypeDescriptor.GetConverter(typeof(float));
            return new[]
            {
                floatConverter.ConvertToString(context, culture, value.Position.X),
                floatConverter.ConvertToString(context, culture, value.Position.Y),
                floatConverter.ConvertToString(context, culture, value.Direction.X),
                floatConverter.ConvertToString(context, culture, value.Direction.Y)
            };
        }

        /// <inheritdoc/>
        protected override Vector2Ray GetObject(string[] values, CultureInfo culture)
        {
            #region Sanity checks
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (culture == null) throw new ArgumentNullException(nameof(culture));
            #endregion

            return new Vector2Ray(
                new Vector2(Convert.ToSingle(values[0], culture), Convert.ToSingle(values[1], culture)),
                new Vector2(Convert.ToSingle(values[3], culture), Convert.ToSingle(values[4], culture)));
        }

        /// <inheritdoc/>
        protected override Vector2Ray GetObject(IDictionary propertyValues)
        {
            #region Sanity checks
            if (propertyValues == null) throw new ArgumentNullException(nameof(propertyValues));
            #endregion

            return new Vector2Ray((Vector2)propertyValues["Point"], (Vector2)propertyValues["Direction"]);
        }
    }
}
