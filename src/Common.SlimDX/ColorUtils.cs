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
