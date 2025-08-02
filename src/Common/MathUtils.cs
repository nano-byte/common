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
    /// Performs smooth (trigonometric) interpolation between two or more values.
    /// </summary>
    /// <param name="factor">A factor between 0 and <paramref name="values"/>.Length.</param>
    /// <param name="values">The value checkpoints.</param>
    [Pure]
    public static float InterpolateTrigonometric(this float factor, params float[] values)
    {
        if (values.Length < 2) throw new ArgumentException("You need to pass at least two values.", nameof(values));

        // Handle value overflows
        if (factor <= 0) return values[0];
        if (factor >= values.Length - 1) return values[^1];

        // Isolate index shift from factor
        int index = (int)factor;

        // Remove index shift from factor
        factor -= index;

        // Apply sinus smoothing to factor
        factor = (float)(-0.5 * Cos(factor * PI) + 0.5);

        return values[index] + factor * (values[index + 1] - values[index]);
    }

    /// <summary>
    /// Performs smooth (trigonometric) interpolation between two or more values.
    /// </summary>
    /// <param name="factor">A factor between 0 and <paramref name="values"/>.Length.</param>
    /// <param name="values">The value checkpoints.</param>
    [Pure]
    public static double InterpolateTrigonometric(double factor, params double[] values)
    {
        if (values.Length < 2) throw new ArgumentException("You need to pass at least two values.", nameof(values));

        // Handle value overflows
        if (factor <= 0) return values[0];
        if (factor >= values.Length - 1) return values[^1];

        // Isolate index shift from factor
        int index = (int)Floor(factor);

        // Remove index shift from factor
        factor -= index;

        // Apply sinus smoothing to factor
        factor = -0.5 * Cos(factor * PI) + 0.5;

        return values[index] + factor * (values[index + 1] - values[index]);
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
