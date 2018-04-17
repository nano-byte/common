// Copyright Bastian Eicher
// Licensed under the MIT License

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
        public DoublePlane(DoubleVector3 point, Vector3 normal)
            : this()
        {
            Point = point;
            Normal = normal;
        }

        /// <summary>
        /// Returns a single-precision standard <see cref="Plane"/> after subtracting an offset value.
        /// </summary>
        /// <param name="offset">This value is subtracted from the double-precision data before it is casted to single-precision.</param>
        /// <returns>The newly positioned <see cref="Plane"/>.</returns>
        public Plane ApplyOffset(DoubleVector3 offset) => new Plane(Point.ApplyOffset(offset), _normal);

        #region Conversion
        /// <inheritdoc/>
        public override string ToString() => $"({Point} => {Normal})";
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(DoublePlane other) => other.Point == Point && other.Normal == Normal;

        public static bool operator ==(DoublePlane left, DoublePlane right) => left.Equals(right);
        public static bool operator !=(DoublePlane left, DoublePlane right) => !left.Equals(right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DoublePlane plane && Equals(plane);
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
