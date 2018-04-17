// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing;
using JetBrains.Annotations;

namespace NanoByte.Common
{
    /// <summary>
    /// Convert colors to different formats, interpolate, invert, ...
    /// </summary>
    public static class ColorUtils
    {
        /// <summary>
        /// Compares two colors ignoring the alpha channel and the name
        /// </summary>
        public static bool EqualsIgnoreAlpha(this Color color1, Color color2) => color1.R == color2.R && color1.G == color2.G && color1.B == color2.B;

        /// <summary>
        /// Interpolates between two colors
        /// </summary>
        /// <param name="factor">The proportion of the two colors between 0 (only first color) and 1 (only second color)</param>
        /// <param name="color1">The first color value</param>
        /// <param name="color2">The second color value</param>
        [Pure]
        public static Color Interpolate(float factor, Color color1, Color color2)
        {
            factor = factor.Clamp();
            return Color.FromArgb(
                (byte)(color1.A * (1.0f - factor) + color2.A * factor),
                (byte)(color1.R * (1.0f - factor) + color2.R * factor),
                (byte)(color1.G * (1.0f - factor) + color2.G * factor),
                (byte)(color1.B * (1.0f - factor) + color2.B * factor));
        }
    }
}
