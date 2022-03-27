// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing;

#if !NET20 && !NET40
using System.Runtime.CompilerServices;
#endif

namespace NanoByte.Common;

/// <summary>
/// Provides math-related utility functions.
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Calculates the mathematical modulo of a value.
    /// </summary>
    public static int Modulo(this int value, int modulo)
    {
        int remainder = value % modulo;
        return remainder < 0
            ? remainder + modulo
            : remainder;
    }

    /// <summary>
    /// Calculates the mathematical modulo of a value.
    /// </summary>
    public static long Modulo(this long value, long modulo)
    {
        long remainder = value % modulo;
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        return remainder < 0
            ? remainder + modulo
            : remainder;
    }

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
    /// Multiplies a <see cref="Size"/> with a <see cref="SizeF"/> and then rounds the components to integer values.
    /// </summary>
    public static Size MultiplyAndRound(this Size size, SizeF factor)
        => new(
            (int)Math.Round(size.Width * factor.Width),
            (int)Math.Round(size.Height * factor.Height));
}
