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
using System.Globalization;
using NanoByte.Common.Values.Design;
using SlimDX;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Defines a ray in two dimensions, specified by a starting position and a direction.
    /// </summary>
    [TypeConverter(typeof(Vector2RayConverter))]
    public struct Vector2Ray : IEquatable<Vector2Ray>
    {
        /// <summary>
        /// Specifies the location of the ray's origin.
        /// </summary>
        [Description("Specifies the location of the ray's origin.")]
        public Vector2 Position { get; }

        private Vector2 _direction;

        /// <summary>
        /// A vector pointing along the ray - automatically normalized when set
        /// </summary>
        [Description("A unit vector specifying the direction in which the ray is pointing.")]
        public Vector2 Direction { get => _direction; set => _direction = Vector2.Normalize(value); }

        /// <summary>
        /// Creates a new ray
        /// </summary>
        /// <param name="point">A point along the ray</param>
        /// <param name="direction">A vector pointing along the ray - automatically normalized when set</param>
        public Vector2Ray(Vector2 point, Vector2 direction) : this()
        {
            Position = point;
            Direction = direction;
        }

        #region Conversion
        /// <inheritdoc/>
        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "({0} => {1})", Position, Direction);
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(Vector2Ray other) => other.Direction == Direction && other.Position == Position;

        public static bool operator ==(Vector2Ray left, Vector2Ray right) => left.Equals(right);
        public static bool operator !=(Vector2Ray left, Vector2Ray right) => !left.Equals(right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector2Ray ray && Equals(ray);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Direction.GetHashCode() * 397) ^ Position.GetHashCode();
            }
        }
        #endregion
    }
}
