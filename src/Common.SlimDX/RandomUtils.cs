// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using JetBrains.Annotations;
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
        [Pure]
        public static int GetRandomInt(int min, int max) => _randomGenerator.Next(min, max);

        /// <summary>
        /// Get a random float value between <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float")]
        [Pure]
        public static float GetRandomFloat(float min, float max) => (float)_randomGenerator.NextDouble() * (max - min) + min;

        /// <summary>
        /// Get a random double value between <paramref name="min"/> and <paramref name="max"/> with steps of <paramref name="step"/>
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="step">The step size (all returned values are multiples of this)</param>
        /// <returns>A random multiple of <paramref name="step"/> between <paramref name="min"/> and <paramref name="max"/>.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float")]
        [Pure]
        public static double GetRandomFloatRange(double min, double max, double step) => GetRandomInt((int)(min / step), (int)(max / step)) * step;

        /// <summary>
        /// Get a random double value between <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        [Pure]
        public static double GetRandomDouble(float min, float max) => _randomGenerator.NextDouble() * (max - min) + min;

        /// <summary>
        /// Get a random double value between <paramref name="min"/> and <paramref name="max"/> with steps of <paramref name="step"/>
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="step">The step size (all returned values are multiples of this)</param>
        /// <returns>A random multiple of <paramref name="step"/> between <paramref name="min"/> and <paramref name="max"/>.</returns>
        [Pure]
        public static double GetRandomDouble(double min, double max, double step) => GetRandomInt((int)(min / step), (int)(max / step)) * step;

        /// <summary>
        /// Get a random byte value between <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        [Pure]
        public static byte GetRandomByte(byte min, byte max) => (byte)(_randomGenerator.Next(min, max));
        #endregion

        #region Multi dimensional
        /// <summary>
        /// Get a random Vector2 value between <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">minimum for each component</param>
        /// <param name="max">maximum for each component</param>
        [Pure]
        public static Vector2 GetRandomVector2(Vector2 min, Vector2 max) => new Vector2(GetRandomFloat(min.X, max.X), GetRandomFloat(min.Y, max.Y));

        /// <summary>
        /// Get a random Vector3 value between <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">minimum for each component</param>
        /// <param name="max">maximum for each component</param>
        [Pure]
        public static Vector3 GetRandomVector3(Vector3 min, Vector3 max) => new Vector3(
            GetRandomFloat(min.X, max.X),
            GetRandomFloat(min.Y, max.Y),
            GetRandomFloat(min.Z, max.Z));

        /// <summary>
        /// Get a random color value between <paramref name="limit1"/> and <paramref name="limit2"/>
        /// </summary>
        /// <param name="limit1">One limit for the color values</param>
        /// <param name="limit2">The other limit for the color values</param>
        [Pure]
        public static Color GetRandomColor(Color limit1, Color limit2) => ColorUtils.Interpolate(GetRandomFloat(0, 1), limit1, limit2);
        #endregion
    }
}
