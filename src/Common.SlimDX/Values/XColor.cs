// Copyright Bastian Eicher
// Licensed under the MIT License

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

        [Pure]
        public static XColor FromColorValue(Color4 color) => new XColor(color.Red, color.Green, color.Blue, color.Alpha);

        [Pure]
        public Color4 ToColorValue() => new Color4(Alpha, Red, Green, Blue);
        #endregion

        #region Equality
        public static bool operator ==(XColor color1, XColor color2) => (Color)color1 == (Color)color2;
        public static bool operator !=(XColor color1, XColor color2) => (Color)color1 != (Color)color2;

        public static bool operator ==(XColor color1, Color color2) => (Color)color1 == color2;
        public static bool operator !=(XColor color1, Color color2) => (Color)color1 != color2;

        public static bool operator ==(Color color1, XColor color2) => color1 == (Color)color2;
        public static bool operator !=(Color color1, XColor color2) => color1 != (Color)color2;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is XColor a) return (a == this);
            if (obj is Color b) return (b == this);
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => ((Color)this).GetHashCode();
        #endregion
    }
}
