// Copyright Bastian Eicher
// Licensed under the MIT License

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
        protected override int NoArguments => 3;

        /// <inheritdoc/>
        protected override ConstructorInfo GetConstructor() => typeof(DoubleVector3).GetConstructor(new[] {typeof(double), typeof(double), typeof(double)});

        /// <inheritdoc/>
        protected override object[] GetArguments(DoubleVector3 value) => new object[] {value.X, value.Y};

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
