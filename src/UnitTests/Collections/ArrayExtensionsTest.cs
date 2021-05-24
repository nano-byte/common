// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="ArrayExtensions"/>.
    /// </summary>
    public class ArrayExtensionsTest
    {
        [Fact]
        public void TestAppend()
        {
            var strings = new[] {"A", "B"};
            strings.Append("C").Should().Equal("A", "B", "C");
        }

        [Fact]
        public void TestPrepend()
        {
            var strings = new[] {"B", "C"};
            strings.Prepend("A").Should().Equal("A", "B", "C");
        }

        [Fact]
        public void TestSequencedEquals()
        {
            new[] {"A", "B", "C"}.SequencedEquals(new[] {"A", "B", "C"}).Should().BeTrue();
            new string[0].SequencedEquals(new string[0]).Should().BeTrue();
            new[] {"A", "B", "C"}.SequencedEquals(new[] {"C", "B", "A"}).Should().BeFalse();
            new[] {"A", "B", "C"}.SequencedEquals(new[] {"X", "Y", "Z"}).Should().BeFalse();
            new[] {"A", "B", "C"}.SequencedEquals(new[] {"A", "B"}).Should().BeFalse();
            new[] {new object()}.SequencedEquals(new[] {new object()}).Should().BeFalse();
        }
    }
}
