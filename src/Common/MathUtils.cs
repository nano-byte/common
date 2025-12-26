// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing;
using static System.Math;
using static NanoByte.Common.EasingFunction;

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
        return remainder < 0 ? remainder + modulo : remainder;
    }

    /// <summary>
    /// Calculates the mathematical modulo of a value.
    /// </summary>
    [Pure]
    public static long Modulo(this long value, long modulo)
    {
        long remainder = value % modulo;
        return remainder < 0 ? remainder + modulo : remainder;
    }

    /// <summary>
    /// Calculates the mathematical modulo of a value.
    /// </summary>
    [Pure]
    public static float Modulo(this float value, float modulo)
    {
        float remainder = value % modulo;
        return remainder < 0 ? remainder + modulo : remainder;
    }

    /// <summary>
    /// Calculates the mathematical modulo of a value.
    /// </summary>
    [Pure]
    public static double Modulo(this double value, double modulo)
    {
        double remainder = value % modulo;
        return remainder < 0 ? remainder + modulo : remainder;
    }

    /// <summary>
    /// Makes a value stay within a certain range.
    /// </summary>
    /// <param name="value">The number to clamp.</param>
    /// <param name="min">The minimum number to return.</param>
    /// <param name="max">The maximum number to return.</param>
    /// <returns>The <paramref name="value"/> if it was in range, otherwise <paramref name="min"/> or <paramref name="max"/>.</returns>
    [Pure]
    public static int Clamp(this int value, int min = 0, int max = 1)
    {
        if (value < min) return min;
        if (value > max) return max;
        if (min > max) throw new ArgumentException("The min value may not be larger than the max value.", nameof(min));

        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    /// <summary>
    /// Makes a value stay within a certain range.
    /// </summary>
    /// <param name="value">The number to clamp.</param>
    /// <param name="min">The minimum number to return.</param>
    /// <param name="max">The maximum number to return.</param>
    /// <returns>The <paramref name="value"/> if it was in range, otherwise <paramref name="min"/> or <paramref name="max"/>.</returns>
    [Pure]
    public static long Clamp(this long value, long min = 0, long max = 1)
    {
        if (value < min) return min;
        if (value > max) return max;
        if (min > max) throw new ArgumentException("The min value may not be larger than the max value.", nameof(min));

        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    /// <summary>
    /// Makes a value stay within a certain range.
    /// </summary>
    /// <param name="value">The number to clamp.</param>
    /// <param name="min">The minimum number to return.</param>
    /// <param name="max">The maximum number to return.</param>
    /// <returns>The <paramref name="value"/> if it was in range, otherwise <paramref name="min"/> or <paramref name="max"/>.</returns>
    [Pure]
    public static float Clamp(this float value, float min = 0, float max = 1)
    {
        if (value < min) return min;
        if (value > max) return max;
        if (min > max) throw new ArgumentException("The min value may not be larger than the max value.", nameof(min));

        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    /// <summary>
    /// Makes a value stay within a certain range.
    /// </summary>
    /// <param name="value">The number to clamp.</param>
    /// <param name="min">The minimum number to return.</param>
    /// <param name="max">The maximum number to return.</param>
    /// <returns>The <paramref name="value"/> if it was in range, otherwise <paramref name="min"/> or <paramref name="max"/>.</returns>
    [Pure]
    public static double Clamp(this double value, double min = 0, double max = 1)
    {
        if (value < min) return min;
        if (value > max) return max;
        if (min > max) throw new ArgumentException("The min value may not be larger than the max value.", nameof(min));

        if (value < min) return min;
        if (value > max) return max;
        return value;
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
    /// Converts an angle in degrees to radians.
    /// </summary>
    /// <param name="value">The angle in degrees.</param>
    /// <returns>The angle in radians.</returns>
    [Pure]
    public static float DegreeToRadian(this float value) => value * ((float)PI / 180);

    /// <summary>
    /// Converts an angle in degrees to radians.
    /// </summary>
    /// <param name="value">The angle in degrees.</param>
    /// <returns>The angle in radians.</returns>
    [Pure]
    public static double DegreeToRadian(this double value) => value * (PI / 180);

    /// <summary>
    /// Converts an angle in radians to degrees.
    /// </summary>
    /// <param name="value">The angle in radians.</param>
    /// <returns>The angle in degrees.</returns>
    [Pure]
    public static float RadianToDegree(this float value) => value * (180 / (float)PI);

    /// <summary>
    /// Converts an angle in radians to degrees.
    /// </summary>
    /// <param name="value">The angle in radians.</param>
    /// <returns>The angle in degrees.</returns>
    [Pure]
    public static double RadianToDegree(this double value) => value * (180 / PI);

    /// <summary>
    /// Applies an ease-in function to the given normalized value (0–1).
    /// </summary>
    public static double EaseIn(double value, EasingFunction function = Sinusoidal)
    {
        value = value.Clamp();

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (value == 1) return 1;

        return function switch
        {
            Sinusoidal => 1 - Cos(value * PI * 0.5),
            Cubic => value * value * value,
            Quintic => value * value * value * value * value,
            _ => throw new ArgumentOutOfRangeException(nameof(function))
        };
    }

    /// <summary>
    /// Applies an ease-out function to the given normalized value (0–1).
    /// </summary>
    public static double EaseOut(double value, EasingFunction function = Sinusoidal)
    {
        value = value.Clamp();

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (value == 1) return 1;

        return function switch
        {
            Sinusoidal => Sin(value * PI * 0.5),
            Cubic => 1 - Pow(1d - value, 3),
            Quintic => 1 - Pow(1d - value, 5),
            _ => throw new ArgumentOutOfRangeException(nameof(function))
        };
    }

    /// <summary>
    /// Applies an ease-in-out function to the given normalized value (0–1).
    /// </summary>
    public static double EaseInOut(double value, EasingFunction function = Sinusoidal)
    {
        value = value.Clamp();

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (value == 1) return 1;

        return function switch
        {
            Sinusoidal => 0.5 - Cos(value * PI) * 0.5,
            Cubic => value < 0.5
                ? 4 * value * value * value
                : 1 - Pow(-2 * value + 2, 3) * 0.5,
            Quintic => value < 0.5d
                ? 16 * value * value * value * value * value
                : 1 - Pow(-2 * value + 2, 5) * 0.5,
            _ => throw new ArgumentOutOfRangeException(nameof(function))
        };
    }

    /// <summary>
    /// Generates a Gaussian kernel.
    /// </summary>
    /// <param name="sigma">The standard deviation of the Gaussian distribution.</param>
    /// <param name="kernelSize">The size of the kernel. Should be an uneven number.</param>
    [Pure]
    public static double[] GaussKernel(double sigma, int kernelSize)
    {
        var kernel = new double[kernelSize];
        double sum = 0;
        for (int i = 0; i < kernel.Length; i++)
        {
            // ReSharper disable once PossibleLossOfFraction
            double x = i - kernelSize / 2;
            sum += kernel[i] = Exp(-x * x / (2 * sigma * sigma));
        }

        // Normalize
        for (int i = 0; i < kernel.Length; i++)
            kernel[i] /= sum;

        return kernel;
    }
}
