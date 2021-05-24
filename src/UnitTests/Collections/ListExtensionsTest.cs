// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="ListExtensions"/>.
    /// </summary>
    public class ListExtensionsTest
    {
        /// <summary>
        /// Ensures that <see cref="ListExtensions.RemoveLast{T}"/> correctly removes the last n elements from a list.
        /// </summary>
        [Fact]
        public void TestRemoveLast()
        {
            var list = new List<string> {"a", "b", "c"};
            list.RemoveLast(2);
            list.Should().Equal("a");

            list.Invoking(x => x.RemoveLast(-1)).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void TestAddOrReplace()
        {
            var list = new List<string> {"a", "bb", "ccc"};
            list.AddOrReplace("xx", keySelector: x => x.Length);
            list.Should().Equal("a", "xx", "ccc");
        }

        [Fact]
        public void TestGetAddedElements()
        {
            new[] {"A", "B", "C", "E", "G", "H"}.GetAddedElements(new[] {"A", "C", "E", "G"}).Should().Equal("B", "H");
            new[] {"C", "D"}.GetAddedElements(new[] {"A", "D"}).Should().Equal("C");
        }
    }
}
