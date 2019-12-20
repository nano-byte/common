// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using Xunit;

namespace NanoByte.Common
{
    public class PropertyPointerTest
    {
        private class Sample
        {
            public string? Data { get; set; }
        }

        [Fact]
        public void Conventional()
        {
            var sample = new Sample {Data = "a"};
            var pointer = PropertyPointer.For(() => sample.Data, value => sample.Data = value, defaultValue: "a");

            pointer.Value.Should().Be("a");
            pointer.IsDefaultValue.Should().BeTrue();

            pointer.Value = "b";
            sample.Data.Should().Be("b");
            pointer.IsDefaultValue.Should().BeFalse();
        }

        [Fact]
        public void ExpressionTree()
        {
            var sample = new Sample {Data = "a"};
            var pointer = PropertyPointer.For(() => sample.Data, defaultValue: "a");

            pointer.Value.Should().Be("a");
            pointer.IsDefaultValue.Should().BeTrue();

            pointer.Value = "b";
            sample.Data.Should().Be("b");
            pointer.IsDefaultValue.Should().BeFalse();
        }
    }
}
