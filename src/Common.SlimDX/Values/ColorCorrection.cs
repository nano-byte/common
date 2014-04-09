/*
 * Copyright 2006-2014 Bastian Eicher
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
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using NanoByte.Common.Utils;
using NanoByte.Common.Values.Design;
using SlimDX;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Color correction values for use in post-processing.
    /// </summary>
    [TypeConverter(typeof(ColorCorrectionConverter))]
    public struct ColorCorrection : IEquatable<ColorCorrection>
    {
        #region Variables
        /// <summary>
        /// No color change.
        /// </summary>
        public static readonly ColorCorrection Default = new ColorCorrection(brightness: 1);
        #endregion

        #region Properties
        private float _brightness;

        /// <summary>
        /// How bright the picture should be - values between 0 (black) and 5 (5x normal).
        /// </summary>
        [XmlAttribute, Description("How bright the picture should be - values between 0 (black) and 5 (5x normal).")]
        public float Brightness { get { return _brightness; } set { _brightness = value.Clamp(0, 5); } }

        private float _contrast;

        /// <summary>
        /// The contrast level of the picture - values between -5 and 5.
        /// </summary>
        [XmlAttribute, Description("The contrast level of the picture - values between -5 and 5.")]
        public float Contrast { get { return _contrast; } set { _contrast = value.Clamp(-5, 5); } }

        private float _saturation;

        /// <summary>
        /// The color saturation level of the picture - values between -5 and 5.
        /// </summary>
        [XmlAttribute, Description("The color saturation level of the picture - values between -5 and 5.")]
        public float Saturation { get { return _saturation; } set { _saturation = value.Clamp(-5, 5); } }

        private float _hue;

        /// <summary>
        /// The color hue rotation of the picture - values between 0 and 360.
        /// </summary>
        [XmlAttribute, DefaultValue(0f), Description("The color hue rotation of the picture - values between 0 and 360.")]
        public float Hue { get { return _hue; } set { _hue = value.Clamp(0, 360); } }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new color correction structure.
        /// </summary>
        /// <param name="brightness">How bright the picture should be - values between 0 (black) and 5 (5x normal).</param>
        /// <param name="contrast">The contrast level of the picture - values between -5 and 5.</param>
        /// <param name="saturation">The color saturation level of the picture - values between -5 and 5.</param>
        /// <param name="hue">The color hue rotation of the picture - values between 0 and 360.</param>
        public ColorCorrection(float brightness = 1, float contrast = 1, float saturation = 1, float hue = 0) : this()
        {
            Brightness = brightness;
            Contrast = contrast;
            Saturation = saturation;
            Hue = hue;
        }
        #endregion

        //--------------------//

        #region Interpolate
        /// <summary>
        /// Performs smooth (sinus-based) interpolation between two or more value sets.
        /// </summary>
        /// <param name="factor">A factor between 0 and <paramref name="values"/>.Length.</param>
        /// <param name="values">The value checkpoints.</param>
        public static ColorCorrection SinusInterpolate(float factor, params ColorCorrection[] values)
        {
            #region Sanity checks
            if (values == null) throw new ArgumentNullException("values");
            #endregion

            // Cast to Vector4 for interpolating...
            var vectorValues = new Vector4[values.Length];
            for (int i = 0; i < values.Length; i++)
                vectorValues[i] = (Vector4)values[i];

            // ... and then cast back
            return (ColorCorrection)MathUtils.InterpolateTrigonometric(factor, vectorValues);
        }
        #endregion

        //--------------------//

        #region Conversion
        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "(Brightness: {0}, Contrast: {1}, Saturation: {2}, Hue: {3})", Brightness, Contrast, Saturation, Hue);
        }

        /// <summary>Convert <see cref="ColorCorrection"/> into <see cref="Vector4"/></summary>
        public static explicit operator Vector4(ColorCorrection correction)
        {
            return new Vector4(correction._brightness, correction._contrast, correction._saturation, correction._hue);
        }

        /// <summary>Convert <see cref="Vector4"/> into see <see cref="ColorCorrection"/></summary>
        public static explicit operator ColorCorrection(Vector4 vector)
        {
            return new ColorCorrection(vector.X, vector.Y, vector.Z, vector.W);
        }
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(ColorCorrection other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return other.Brightness == Brightness && other.Contrast == Contrast && other.Saturation == Saturation && other.Hue == Hue;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <inheritdoc/>
        public static bool operator ==(ColorCorrection left, ColorCorrection right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(ColorCorrection left, ColorCorrection right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return (obj.GetType() != typeof(ColorCorrection)) && Equals((ColorCorrection)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = _brightness.GetHashCode();
                result = (result * 397) ^ _contrast.GetHashCode();
                result = (result * 397) ^ _saturation.GetHashCode();
                result = (result * 397) ^ _hue.GetHashCode();
                return result;
            }
        }
        #endregion
    }
}
