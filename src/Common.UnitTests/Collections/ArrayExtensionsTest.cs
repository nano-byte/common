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
        public void TestGetAddedElements()
        {
            new[] {"A", "B", "C", "E", "G", "H"}.GetAddedElements(new[] {"A", "C", "E", "G"}).Should().Equal("B", "H");
            new[] {"C", "D"}.GetAddedElements(new[] {"A", "D"}).Should().Equal("C");
        }
    }
}
