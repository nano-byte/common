// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing;

namespace NanoByte.Common;

/// <summary>
/// Contains test methods for <see cref="MathUtils"/>.
/// </summary>
public class MathUtilsTest
{
    [Theory]
    [InlineData(3, 4, 3)]
    [InlineData(3, 2, 1)]
    [InlineData(-1, 3, 2)]
    public void TestModule(int a, int b, int result)
        => a.Modulo(b).Should().Be(result);

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
    public void TestXOr()
    {
        byte[] array1 = [0x01, 0x10];
        byte[] array2 = [0x10, 0x11];
        MathUtils.XOr(array1, array2)
                 .Should().BeEquivalentTo([0x11, 0x01]);
    }
}
