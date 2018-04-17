// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using NanoByte.Common.Properties;
using NanoByte.Common.Values.Design;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Stores the mimimum and maximum values allowed for a float field or property.
    /// Controls the behaviour of <see cref="AngleEditor"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FloatRangeAttribute : Attribute
    {
        /// <summary>
        /// The minimum value the field or property may have.
        /// </summary>
        public float Minimum { get; }

        /// <summary>
        /// The maximum value the field or property may have.
        /// </summary>
        public float Maximum { get; }

        /// <summary>
        /// Creates a new float range attribute.
        /// </summary>
        /// <param name="minimum">The minimum value the field or property may have.</param>
        /// <param name="maximum">The maximum value the field or property may have.</param>
        public FloatRangeAttribute(float minimum, float maximum)
        {
            #region Sanity checks
            if (minimum > maximum) throw new ArgumentException(Resources.MinLargerMax, nameof(minimum));
            #endregion

            Minimum = minimum;
            Maximum = maximum;
        }
    }
}
