// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common
{
    /// <summary>
    /// Contains test methods for <see cref="MathUtils"/>.
    /// </summary>
    public class MathUtilsTest
    {
        [Fact]
        public void TestModule()
        {
            3.Modulo(4).Should().Be(3);
            3.Modulo(2).Should().Be(1);
            (-1).Modulo(3).Should().Be(2);
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
            new Size(2, 3).Multiply(2).Should().Be(new Size(4, 6));
        }
    }
}
