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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using NanoByte.Common.Values.Design;
using SlimDX;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Factors describing the attenuation of light intensity over distance.
    /// </summary>
    [TypeConverter(typeof(AttenuationConverter))]
    public struct Attenuation : IEquatable<Attenuation>
    {
        #region Constants
        /// <summary>
        /// Value for no attenuation over distance.
        /// </summary>
        public static readonly Attenuation None = new Attenuation(1, 0, 0);
        #endregion

        /// <summary>
        /// A constant factor multiplied with the color.
        /// </summary>
        [XmlAttribute, Description("A constant factor multiplied with the color.")]
        public float Constant { get; }

        /// <summary>
        /// A constant factor multiplied with the color and the inverse distance.
        /// </summary>
        [XmlAttribute, Description("A constant factor multiplied with the color and the inverse distance.")]
        public float Linear { get; }

        /// <summary>
        /// A constant factor multiplied with the color and the inverse distance squared.
        /// </summary>
        [XmlAttribute, Description("A constant factor multiplied with the color and the inverse distance squared.")]
        public float Quadratic { get; }

        /// <summary>
        /// Creates a new attenuation structure
        /// </summary>
        /// <param name="constant">A constant factor multiplied with the color.</param>
        /// <param name="linear">A constant factor multiplied with the color and the inverse distance.</param>
        /// <param name="quadratic">A constant factor multiplied with the color and the inverse distance squared.</param>
        public Attenuation(float constant, float linear, float quadratic) : this()
        {
            Constant = constant;
            Linear = linear;
            Quadratic = quadratic;
        }

        #region Conversion
        /// <inheritdoc/>
        public override string ToString() => $"(Constant: {Constant}, Linear: {Linear}, Quadratic: {Quadratic})";

        /// <summary>Convert <see cref="Attenuation"/> into <see cref="Vector4"/></summary>
        public static explicit operator Vector4(Attenuation attenuation) => new Vector4(attenuation.Constant, attenuation.Linear, attenuation.Quadratic, 0);

        /// <summary>Convert <see cref="Vector4"/> into <see cref="Attenuation"/></summary>
        public static explicit operator Attenuation(Vector4 vector) => new Attenuation(vector.X, vector.Y, vector.Z);
        #endregion

        #region Equality
        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public bool Equals(Attenuation other)
        {
            return other.Constant == Constant && other.Linear == Linear && other.Quadratic == Quadratic;
        }

        public static bool operator ==(Attenuation left, Attenuation right) => left.Equals(right);
        public static bool operator !=(Attenuation left, Attenuation right) => !left.Equals(right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Attenuation attenuation && Equals(attenuation);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 7;
                hash = 97 * hash + ((int)Constant ^ ((int)Constant >> 32));
                hash = 97 * hash + ((int)Linear ^ ((int)Linear >> 32));
                hash = 97 * hash + ((int)Quadratic ^ ((int)Quadratic >> 32));
                return hash;
            }
        }
        #endregion
    }
}
