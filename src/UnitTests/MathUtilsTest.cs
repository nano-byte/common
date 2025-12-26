// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing;

namespace NanoByte.Common;

/// <summary>
/// Contains test methods for <see cref="MathUtils"/>.
/// </summary>
public class MathUtilsTest
{
    [Fact]
    public void TestModuloInt()
    {
        5.Modulo(3).Should().Be(2);
        4.Modulo(3).Should().Be(1);
        3.Modulo(3).Should().Be(0);
        2.Modulo(3).Should().Be(2);
        1.Modulo(3).Should().Be(1);
        0.Modulo(3).Should().Be(0);
        (-1).Modulo(3).Should().Be(2);
        (-2).Modulo(3).Should().Be(1);
        (-3).Modulo(3).Should().Be(0);
    }

    [Fact]
    public void TestModuloFloat()
    {
        5f.Modulo(3).Should().Be(2);
        4f.Modulo(3).Should().Be(1);
        3f.Modulo(3).Should().Be(0);
        2f.Modulo(3).Should().Be(2);
        1f.Modulo(3).Should().Be(1);
        0f.Modulo(3).Should().Be(0);
        (-1f).Modulo(3).Should().Be(2);
        (-2f).Modulo(3).Should().Be(1);
        (-3f).Modulo(3).Should().Be(0);
    }

    [Fact]
    public void TestModuloDouble()
    {
        5.0.Modulo(3).Should().Be(2);
        4.0.Modulo(3).Should().Be(1);
        3.0.Modulo(3).Should().Be(0);
        2.0.Modulo(3).Should().Be(2);
        1.0.Modulo(3).Should().Be(1);
        0.0.Modulo(3).Should().Be(0);
        (-1.0).Modulo(3).Should().Be(2);
        (-2.0).Modulo(3).Should().Be(1);
        (-3.0).Modulo(3).Should().Be(0);
    }

    [Fact]
    public void TestEqualsTolerance()
    {
        1f.EqualsTolerance(1.000009f).Should().BeTrue();
        1f.EqualsTolerance(2f).Should().BeFalse();
    }

    [Fact]
    public void TestSizeMultiply()
    {
        new Size(2, 3).MultiplyAndRound(new SizeF(2, 3))
                      .Should().Be(new Size(4, 9));
    }

    [Fact]
    public void Ease_Sinusoidal_AtBounds()
    {
        MathUtils.EaseIn(0).Should().Be(0);
        MathUtils.EaseIn(1).Should().Be(1);

        MathUtils.EaseOut(0).Should().Be(0);
        MathUtils.EaseOut(1).Should().Be(1);

        MathUtils.EaseInOut(0).Should().Be(0);
        MathUtils.EaseInOut(1).Should().Be(1);
    }

    [Fact]
    public void Ease_Cubic_KnownValues()
    {
        MathUtils.EaseIn(0.5, EasingFunction.Cubic).Should().BeApproximately(0.125, precision: 1e-6);
        MathUtils.EaseOut(0.5, EasingFunction.Cubic).Should().BeApproximately(0.875, precision: 1e-6);
        MathUtils.EaseInOut(0.5, EasingFunction.Cubic).Should().BeApproximately(0.5, precision: 1e-6);
    }

    [Fact]
    public void Ease_Quintic_KnownValues()
    {
        MathUtils.EaseIn(0.5, EasingFunction.Quintic).Should().BeApproximately(0.03125, precision: 1e-6);
        MathUtils.EaseOut(0.5, EasingFunction.Quintic).Should().BeApproximately(0.96875, precision: 1e-6);
        MathUtils.EaseInOut(0.5, EasingFunction.Quintic).Should().BeApproximately(0.5, precision: 1e-6);
    }

    [Theory]
    [InlineData(EasingFunction.Sinusoidal)]
    [InlineData(EasingFunction.Cubic)]
    [InlineData(EasingFunction.Quintic)]
    public void Ease_IsMonotonic(EasingFunction function)
    {
        AssertMonotonic(v => MathUtils.EaseIn(v, function));
        AssertMonotonic(v => MathUtils.EaseOut(v, function));
        AssertMonotonic(v => MathUtils.EaseInOut(v, function));
    }

    private static void AssertMonotonic(Func<double, double> easingFunc)
    {
        double last = 0;

        for (int i = 1; i <= 100; i++)
        {
            double value = i / 100.0;
            double eased = easingFunc(value);

            eased.Should().BeGreaterOrEqualTo(last);
            last = eased;
        }
    }
}
