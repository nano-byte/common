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

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;
using JetBrains.Annotations;
using SlimDX;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Stores ARGB-colors as byte values but also surfaces them as float values.
    /// </summary>
    /// <remarks>
    /// This class can be used to serialize ARGB-color values (unlike <see cref="Color"/> all fields are writable).
    /// It provides easy methods for casting to and from <see cref="Color"/> (useful in combination with a <see cref="PropertyGrid"/>).
    /// It also provides easy methods for casting to and from <see cref="Color4"/> (useful for rendering with <see cref="SlimDX"/>).
    /// </remarks>
    [XmlInclude(typeof(Color))]
    [StructLayout(LayoutKind.Sequential)]
    public struct XColor
    {
        [XmlAttribute]
        public byte A { get; set; }

        [XmlAttribute]
        public byte R { get; set; }

        [XmlAttribute]
        public byte G { get; set; }

        [XmlAttribute]
        public byte B { get; set; }

        [XmlIgnore]
        public float Red { get => (float)R / 255; set => R = (byte)(value * 255); }

        [XmlIgnore]
        public float Green { get => (float)G / 255; set => G = (byte)(value * 255); }

        [XmlIgnore]
        public float Blue { get => (float)B / 255; set => B = (byte)(value * 255); }

        [XmlIgnore]
        public float Alpha { get => (float)A / 255; set => A = (byte)(value * 255); }

        public XColor(float red, float green, float blue, float alpha)
        {
            R = (byte)(red * 255);
            G = (byte)(green * 255);
            B = (byte)(blue * 255);
            A = (byte)(alpha * 255);
        }

        public XColor(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        #region Conversion
        /// <inheritdoc/>
        public override string ToString() => ((Color)this).ToString();

        // Convert Drawing.Color into EasyColor
        public static implicit operator XColor(Color color) => new XColor(color.A, color.R, color.G, color.B);

        // Convert EasyColor into Drawing.Color
        public static explicit operator Color(XColor color) => Color.FromArgb(color.A, color.R, color.G, color.B);

        // Convert Direct3D.Color4 into EasyColor
        [Pure]
        public static XColor FromColorValue(Color4 color) => new XColor(color.Red, color.Green, color.Blue, color.Alpha);

        // Convert EasyColor into Direct3D.Color4
        public Color4 ToColorValue() => new Color4(Alpha, Red, Green, Blue);
        #endregion

        #region Equality
        /// <inheritdoc/>
        public static bool operator ==(XColor color1, XColor color2)
        {
            return (Color)color1 == (Color)color2;
        }

        /// <inheritdoc/>
        public static bool operator !=(XColor color1, XColor color2)
        {
            return (Color)color1 != (Color)color2;
        }

        /// <inheritdoc/>
        public static bool operator ==(XColor color1, Color color2)
        {
            return (Color)color1 == color2;
        }

        /// <inheritdoc/>
        public static bool operator !=(XColor color1, Color color2)
        {
            return (Color)color1 != color2;
        }

        /// <inheritdoc/>
        public static bool operator ==(Color color1, XColor color2)
        {
            return color1 == (Color)color2;
        }

        /// <inheritdoc/>
        public static bool operator !=(Color color1, XColor color2)
        {
            return color1 != (Color)color2;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is XColor) return ((XColor)obj == this);
            if (obj is Color) return ((Color)obj == this);
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return ((Color)this).GetHashCode();
        }
        #endregion
    }
}
