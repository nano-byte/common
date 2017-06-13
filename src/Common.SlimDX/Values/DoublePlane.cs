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
using NanoByte.Common.Values.Design;
using SlimDX;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Defines a plane in three dimensions with <see cref="double"/> distance accuracy.
    /// </summary>
    [TypeConverter(typeof(DoublePlaneConverter))]
    public struct DoublePlane : IEquatable<DoublePlane>
    {
        /// <summary>
        /// A point that lies along the plane.
        /// </summary>
        [Description("A point that lies along the plane.")]
        public DoubleVector3 Point { get; }

        private Vector3 _normal;

        /// <summary>
        /// The normal vector of the plane.
        /// </summary>
        [Description("The normal vector of the plane.")]
        public Vector3 Normal { get => _normal; set => _normal = Vector3.Normalize(value); }

        /// <summary>
        /// Creates a new plane.
        /// </summary>
        /// <param name="point">A point that lies along the plane.</param>
        /// <param name="normal">The normal vector of the plane.</param>
        public DoublePlane(DoubleVector3 point, Vector3 normal) : this()
        {
            Point = point;
            Normal = normal;
        }

        /// <summary>
        /// Returns a single-precision standard <see cref="Plane"/> after subtracting an offset value.
        /// </summary>
        /// <param name="offset">This value is subtracted from the double-precision data before it is casted to single-precision.</param>
        /// <returns>The newly positioned <see cref="Plane"/>.</returns>
        public Plane ApplyOffset(DoubleVector3 offset)
        {
            return new Plane(Point.ApplyOffset(offset), _normal);
        }

        #region Conversion
        /// <inheritdoc/>
        public override string ToString() => $"({Point} => {Normal})";
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(DoublePlane other)
        {
            return other.Point == Point && other.Normal == Normal;
        }

        /// <inheritdoc/>
        public static bool operator ==(DoublePlane left, DoublePlane right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(DoublePlane left, DoublePlane right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DoublePlane && Equals((DoublePlane)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Point.GetHashCode() * 397) ^ Normal.GetHashCode();
            }
        }
        #endregion
    }
}
