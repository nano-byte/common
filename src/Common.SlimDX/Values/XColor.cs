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
        #region Properties
        private byte _a, _r, _g, _b;

        [XmlAttribute]
        public byte A { get { return _a; } set { _a = value; } }

        [XmlAttribute]
        public byte R { get { return _r; } set { _r = value; } }

        [XmlAttribute]
        public byte G { get { return _g; } set { _g = value; } }

        [XmlAttribute]
        public byte B { get { return _b; } set { _b = value; } }

        [XmlIgnore]
        public float Red { get { return (float)_r / 255; } set { _r = (byte)(value * 255); } }

        [XmlIgnore]
        public float Green { get { return (float)_g / 255; } set { _g = (byte)(value * 255); } }

        [XmlIgnore]
        public float Blue { get { return (float)_b / 255; } set { _b = (byte)(value * 255); } }

        [XmlIgnore]
        public float Alpha { get { return (float)_a / 255; } set { _a = (byte)(value * 255); } }
        #endregion

        #region Constructor
        public XColor(float red, float green, float blue, float alpha)
        {
            _r = (byte)(red * 255);
            _g = (byte)(green * 255);
            _b = (byte)(blue * 255);
            _a = (byte)(alpha * 255);
        }

        public XColor(byte a, byte r, byte g, byte b)
        {
            _a = a;
            _r = r;
            _g = g;
            _b = b;
        }
        #endregion

        //--------------------//

        #region Conversion
        /// <inheritdoc/>
        public override string ToString()
        {
            return ((Color)this).ToString();
        }

        // Convert Drawing.Color into EasyColor
        public static implicit operator XColor(Color color)
        {
            return new XColor(color.A, color.R, color.G, color.B);
        }

        // Convert EasyColor into Drawing.Color
        public static explicit operator Color(XColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        // Convert Direct3D.Color4 into EasyColor
        [Pure]
        public static XColor FromColorValue(Color4 color)
        {
            return new XColor(color.Red, color.Green, color.Blue, color.Alpha);
        }

        // Convert EasyColor into Direct3D.Color4
        public Color4 ToColorValue()
        {
            return new Color4(Alpha, Red, Green, Blue);
        }
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
