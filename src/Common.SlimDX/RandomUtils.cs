/*
 * Copyright 2006-2014 Bastian Eicher
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using SlimDX;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides helper methods for creating different types of variables with random content.
    /// </summary>
    public static class RandomUtils
    {
        #region Variables
        /// <summary>
        /// Global random generator
        /// </summary>
        private static readonly Random _randomGenerator = new Random();
        #endregion

        #region One dimensional
        /// <summary>
        /// Get random a integer value
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
        public static int GetRandomInt(int min, int max)
        {
            return _randomGenerator.Next(min, max);
        }

        /// <summary>
        /// Get a random float value between <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float")]
        public static float GetRandomFloat(float min, float max)
        {
            return (float)_randomGenerator.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Get a random double value between <paramref name="min"/> and <paramref name="max"/> with steps of <paramref name="step"/>
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="step">The step size (all returned values are multiples of this)</param>
        /// <returns>A random multiple of <paramref name="step"/> between <paramref name="min"/> and <paramref name="max"/>.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float")]
        public static double GetRandomFloatRange(double min, double max, double step)
        {
            return GetRandomInt((int)(min / step), (int)(max / step)) * step;
        }

        /// <summary>
        /// Get a random double value between <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        public static double GetRandomDouble(float min, float max)
        {
            return _randomGenerator.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Get a random double value between <paramref name="min"/> and <paramref name="max"/> with steps of <paramref name="step"/>
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="step">The step size (all returned values are multiples of this)</param>
        /// <returns>A random multiple of <paramref name="step"/> between <paramref name="min"/> and <paramref name="max"/>.</returns>
        public static double GetRandomDouble(double min, double max, double step)
        {
            return GetRandomInt((int)(min / step), (int)(max / step)) * step;
        }

        /// <summary>
        /// Get a random byte value between <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        public static byte GetRandomByte(byte min, byte max)
        {
            return (byte)(_randomGenerator.Next(min, max));
        }
        #endregion

        #region Multi dimensional
        /// <summary>
        /// Get a random Vector2 value between <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">minimum for each component</param>
        /// <param name="max">maximum for each component</param>
        public static Vector2 GetRandomVector2(Vector2 min, Vector2 max)
        {
            return new Vector2(GetRandomFloat(min.X, max.X), GetRandomFloat(min.Y, max.Y));
        }

        /// <summary>
        /// Get a random Vector3 value between <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">minimum for each component</param>
        /// <param name="max">maximum for each component</param>
        public static Vector3 GetRandomVector3(Vector3 min, Vector3 max)
        {
            return new Vector3(
                GetRandomFloat(min.X, max.X),
                GetRandomFloat(min.Y, max.Y),
                GetRandomFloat(min.Z, max.Z));
        }

        /// <summary>
        /// Get a random color value between <paramref name="limit1"/> and <paramref name="limit2"/>
        /// </summary>
        /// <param name="limit1">One limit for the color values</param>
        /// <param name="limit2">The other limit for the color values</param>
        public static Color GetRandomColor(Color limit1, Color limit2)
        {
            return ColorUtils.Interpolate(GetRandomFloat(0, 1), limit1, limit2);
        }
        #endregion
    }
}
