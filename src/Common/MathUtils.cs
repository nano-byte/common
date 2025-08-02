// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing;
using static System.Math;

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
    [Pure]
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
    [Pure]
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
    [Pure]
    public static bool EqualsTolerance(this float a, float b, float tolerance = 0.00001f)
        => Abs(a - b) <= tolerance;

    /// <summary>
    /// Compares two floating-point values for equality, allowing for a certain <paramref name="tolerance"/>.
    /// </summary>
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    [Pure]
    public static bool EqualsTolerance(this double a, double b, double tolerance = 0.00001)
        => Abs(a - b) <= tolerance;

    /// <summary>
    /// Multiplies a <see cref="Size"/> with a <see cref="SizeF"/> and then rounds the components to integer values.
    /// </summary>
    [Pure]
    public static Size MultiplyAndRound(this Size size, SizeF factor)
        => new(
            (int)Round(size.Width * factor.Width),
            (int)Round(size.Height * factor.Height));

    /// <summary>
    /// Combines two byte arrays via Exclusive Or.
    /// </summary>
    [Pure]
    public static byte[] XOr(byte[] array1, byte[] array2)
    {
        if (array1.Length != array2.Length) throw new ArgumentException("Length of both arrays must be equal.", nameof(array1));

        byte[] result = array1.ToArray();
        for (int i = 0; i < array2.Length; i++) result[i] ^= array2[i];
        return result;
    }
}
