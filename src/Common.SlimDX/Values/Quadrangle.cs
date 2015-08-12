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
using System.Drawing;
using System.Globalization;
using NanoByte.Common.Values.Design;
using SlimDX;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// A 2D polygon consisting of four points.
    /// </summary>
    [TypeConverter(typeof(QuadrangleConverter))]
    public struct Quadrangle : IEquatable<Quadrangle>
    {
        #region Properties
        /// <summary>
        /// The coordinates of the first point; counter-clockwise ordering recommended.
        /// </summary>
        [Description("The coordinates of the first point; counter-clockwise ordering recommended.")]
        public Vector2 P1 { get; set; }

        /// <summary>
        /// The coordinates of the second point; counter-clockwise ordering recommended.
        /// </summary>
        [Description("The coordinates of the second point; counter-clockwise ordering recommended.")]
        public Vector2 P2 { get; set; }

        /// <summary>
        /// The coordinates of the third point; counter-clockwise ordering recommended.
        /// </summary>
        [Description("The coordinates of the third point; counter-clockwise ordering recommended.")]
        public Vector2 P3 { get; set; }

        /// <summary>
        /// The coordinates of the fourth point; counter-clockwise ordering recommended.
        /// </summary>
        [Description("The coordinates of the fourth point; counter-clockwise ordering recommended.")]
        public Vector2 P4 { get; set; }

        /// <summary>
        /// The edge from <see cref="P1"/> to <see cref="P2"/>.
        /// </summary>
        public Vector2Ray Edge1 => new Vector2Ray(P1, P2 - P1);

        /// <summary>
        /// The edge from <see cref="P2"/> to <see cref="P3"/>.
        /// </summary>
        public Vector2Ray Edge2 => new Vector2Ray(P2, P3 - P2);

        /// <summary>
        /// The edge from <see cref="P3"/> to <see cref="P4"/>.
        /// </summary>
        public Vector2Ray Edge3 => new Vector2Ray(P3, P4 - P3);

        /// <summary>
        /// The edge from <see cref="P4"/> to <see cref="P1"/>.
        /// </summary>
        public Vector2Ray Edge4 => new Vector2Ray(P4, P1 - P4);
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new quadrangle. Counter-clockwise ordering is recommended.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1025:ReplaceRepetitiveArgumentsWithParamsArray", Justification = "We always want exactly four points")]
        public Quadrangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) : this()
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
        }

        /// <summary>
        /// Creates a new quadrangle. Counter-clockwise ordering is recommended.
        /// </summary>
        public Quadrangle(float p1X, float p1Y, float p2X, float p2Y, float p3X, float p3Y, float p4X, float p4Y) : this()
        {
            P1 = new Vector2(p1X, p1Y);
            P2 = new Vector2(p2X, p2Y);
            P3 = new Vector2(p3X, p3Y);
            P4 = new Vector2(p4X, p4Y);
        }

        /// <summary>
        /// Creates a new quadrangle from a simple rectangle.
        /// </summary>
        public Quadrangle(RectangleF rectangle) : this()
        {
            P1 = new Vector2(rectangle.Left, rectangle.Top);
            P2 = new Vector2(rectangle.Left, rectangle.Bottom);
            P3 = new Vector2(rectangle.Right, rectangle.Bottom);
            P4 = new Vector2(rectangle.Right, rectangle.Top);
        }
        #endregion

        //--------------------//

        #region Offeset
        /// <summary>
        /// Returns a new <see cref="Quadrangle"/> shifted by <paramref name="distance"/>.
        /// </summary>
        /// <param name="distance">This value is added to each corner position.</param>
        /// <returns>The shifted <see cref="Quadrangle"/>.</returns>
        public Quadrangle Offset(Vector2 distance)
        {
            return new Quadrangle(
                P1 + distance, P2 + distance,
                P3 + distance, P4 + distance);
        }
        #endregion

        #region Rotation
        /// <summary>
        /// Returns a new <see cref="Quadrangle"/> rotated by <paramref name="rotation"/> around the origin.
        /// </summary>
        /// <param name="rotation">The angle to rotate by in degrees.</param>
        /// <returns>The rotated <see cref="Quadrangle"/>.</returns>
        public Quadrangle Rotate(float rotation)
        {
            return new Quadrangle(
                P1.Rotate(rotation), P2.Rotate(rotation),
                P3.Rotate(rotation), P4.Rotate(rotation));
        }
        #endregion

        #region Intersect

        #region Point
        /// <summary>
        /// If the points are stored counter-clockwise and form a convex polygon, this will test if a point lies inside it.
        /// </summary>
        /// <param name="point">The point to test for intersection.</param>
        /// <returns><c>true</c> if <paramref name="point"/> lies within the quadrangle.</returns>
        public bool IntersectWith(Vector2 point)
        {
            // Check if the point lies on the outside of any of the lines
            if ((point.Y - P1.Y) * (P2.X - P1.X) - (point.X - P1.X) * (P2.Y - P1.Y) > 0) return false;
            if ((point.Y - P2.Y) * (P3.X - P2.X) - (point.X - P2.X) * (P3.Y - P2.Y) > 0) return false;
            if ((point.Y - P3.Y) * (P4.X - P3.X) - (point.X - P3.X) * (P4.Y - P3.Y) > 0) return false;
            if ((point.Y - P4.Y) * (P1.X - P4.X) - (point.X - P4.X) * (P1.Y - P4.Y) > 0) return false;

            // If not, it must be inside the quadrangle
            return true;
        }
        #endregion

        #region RectangleF
        /// <summary>
        /// If the points are stored counter-clockwise and form a convex polygon, this will test if a rectangle lies inside it.
        /// </summary>
        /// <param name="rectangle">The rectangle to test for intersection.</param>
        /// <returns><c>true</c> if <paramref name="rectangle"/> lies within the quadrangle.</returns>
        public bool IntersectWith(RectangleF rectangle)
        {
            // Check if any corner of the quadrangle lies within the rectangle
            if (rectangle.Contains(P1.X, P1.Y)) return true;
            if (rectangle.Contains(P2.X, P2.Y)) return true;
            if (rectangle.Contains(P3.X, P3.Y)) return true;
            if (rectangle.Contains(P4.X, P4.Y)) return true;

            // Check if the quadrangle has the size 0
            if (P1 == P2 && P2 == P3 && P3 == P4) return false;

            // Check if any corner of the rectangle lies within the quadrangle
            if (IntersectWith(new Vector2(rectangle.Left, rectangle.Top))) return true;
            if (IntersectWith(new Vector2(rectangle.Right, rectangle.Top))) return true;
            if (IntersectWith(new Vector2(rectangle.Right, rectangle.Bottom))) return true;
            if (IntersectWith(new Vector2(rectangle.Left, rectangle.Bottom))) return true;

            // If neither, it must be outside of the quadrangle
            return false;
        }
        #endregion

        #region Quadrangle
        /// <summary>
        /// This will test if two quadrangles intersect with each other. Only works if both quadrangles are counter-clockwise and form a convex polygon.
        /// </summary>
        /// <param name="quadrangle">The other quadrangle to test for intersection.</param>
        /// <returns><c>true</c> if <paramref name="quadrangle"/> intersects with this quadrangle.</returns>
        public bool IntersectWith(Quadrangle quadrangle)
        {
            // ToDo: Optimize
            return
                // this and quadrangle intersect or quadrangle fully inside this
                IntersectWith(quadrangle.P1) || IntersectWith(quadrangle.P2) || IntersectWith(quadrangle.P3) || IntersectWith(quadrangle.P4) ||
                // this fully inside quadrangle
                quadrangle.IntersectWith(P1) /*|| quadrangle.IntersectWith(_p2) || quadrangle.IntersectWith(_p3) || quadrangle.IntersectWith(_p4)*/;
        }
        #endregion

        #region Circle
        /// <summary>
        /// If the points are stored counter-clockwise and form a convex polygon, this will test if a circle with the origin (0;0) lies inside it.
        /// </summary>
        /// <param name="radius">The rectangle to test for intersection.</param>
        /// <returns><c>true</c> if the circle lies within the quadrangle.</returns>
        public bool IntersectCircle(float radius)
        {
            // Check if a part of the circle lies within the quadrangle
            if (P1.Length() < radius || P2.Length() < radius || P3.Length() < radius || P4.Length() < radius) return true;

            // Check if the quadrangle has the size 0
            if (P1 == P2 && P2 == P3 && P3 == P4) return false;

            // Check if the circle lies within the quadrangle completley
            if (IntersectWith(new Vector2())) return true;

            // If neither, it must be outside of the quadrangle
            return false;
        }
        #endregion

        #endregion

        //--------------------//

        #region Conversion
        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2}, {3})", P1, P2, P3, P4);
        }
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(Quadrangle other)
        {
            return other.P1 == P1 && other.P2 == P2 && other.P3 == P3 && other.P4 == P4;
        }

        /// <inheritdoc/>
        public static bool operator ==(Quadrangle left, Quadrangle right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(Quadrangle left, Quadrangle right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Quadrangle && Equals((Quadrangle)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = P1.GetHashCode();
                result = (result * 397) ^ P2.GetHashCode();
                result = (result * 397) ^ P3.GetHashCode();
                result = (result * 397) ^ P4.GetHashCode();
                return result;
            }
        }
        #endregion
    }
}
