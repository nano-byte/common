// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Drawing;

#if !NET20 && !NET40
using System.Runtime.CompilerServices;
#endif

namespace NanoByte.Common
{
    /// <summary>
    /// Provides math-related utility functions.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Compares two floating-point values for equality, allowing for a certain <paramref name="tolerance"/>.
        /// </summary>
#if !NET20 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool EqualsTolerance(this float a, float b, float tolerance = 0.00001f)
            => Math.Abs(a - b) <= tolerance;

        /// <summary>
        /// Compares two floating-point values for equality, allowing for a certain <paramref name="tolerance"/>.
        /// </summary>
#if !NET20 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool EqualsTolerance(this double a, double b, double tolerance = 0.00001)
            => Math.Abs(a - b) <= tolerance;

        /// <summary>
        /// Multiplies a size with a linear <paramref name="factor"/>.
        /// </summary>
        public static Size Multiply(this Size size, float factor)
            => new((int)(size.Width * factor), (int)(size.Height * factor));
    }
}
