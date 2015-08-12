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
