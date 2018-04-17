// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Defines a four component vector with <see cref="byte"/> accuracy.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ByteVector4 : IEquatable<ByteVector4>
    {
        /// <summary>
        /// Gets or sets the X component of the vector.
        /// </summary>
        [XmlAttribute, Description("Gets or sets the X component of the vector.")]
        public byte X { get; set; }

        /// <summary>
        /// Gets or sets the Y component of the vector.
        /// </summary>
        [XmlAttribute, Description("Gets or sets the Y component of the vector.")]
        public byte Y { get; set; }

        /// <summary>
        /// Gets or sets the Z component of the vector.
        /// </summary>
        [XmlAttribute, Description("Gets or sets the Z component of the vector.")]
        public byte Z { get; set; }

        /// <summary>
        /// Gets or sets the W component of the vector.
        /// </summary>
        [XmlAttribute, Description("Gets or sets the W component of the vector.")]
        public byte W { get; set; }

        /// <summary>
        /// Creates a new vector.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="z">The Z component.</param>
        /// <param name="w">The W component.</param>
        public ByteVector4(byte x, byte y, byte z, byte w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        #region Conversion
        /// <inheritdoc/>
        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2}, {3})", X, Y, Z, W);
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(ByteVector4 other) => other.X == X && other.Y == Y && other.Z == Z && other.W == W;

        public static bool operator ==(ByteVector4 left, ByteVector4 right) => left.Equals(right);
        public static bool operator !=(ByteVector4 left, ByteVector4 right) => !left.Equals(right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ByteVector4 vector4 && Equals(vector4);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => X | (Y << 8) | (Z << 16) | (W << 24);
        #endregion
    }
}
