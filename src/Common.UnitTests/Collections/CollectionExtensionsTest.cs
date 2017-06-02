/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="CollectionExtensions"/>.
    /// </summary>
    public class CollectionExtensionsTest
    {
        /// <summary>
        /// Ensures that <see cref="CollectionExtensions.AddIfNew{T}"/> correctly detects pre-existing entries.
        /// </summary>
        [Fact]
        public void TestAddIfNew()
        {
            var list = new List<string> {"a", "b", "c"};

            list.AddIfNew("b").Should().BeFalse();
            list.Should().Equal("a", "b", "c");

            list.AddIfNew("d").Should().BeTrue();
            list.Should().Equal("a", "b", "c", "d");

            list.Invoking(x => x.RemoveLast(-1)).ShouldThrow<ArgumentOutOfRangeException>();
        }

        /// <summary>
        /// Ensures that <see cref="CollectionExtensions.RemoveLast{T}"/> correctly removes the last n elements from a list.
        /// </summary>
        [Fact]
        public void TestRemoveLast()
        {
            var list = new List<string> {"a", "b", "c"};
            list.RemoveLast(2);
            list.Should().Equal("a");

            list.Invoking(x => x.RemoveLast(-1)).ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void TestAddOrReplace()
        {
            var list = new List<string> {"a", "bb", "ccc"};
            list.AddOrReplace("xx", keySelector: x => x.Length);
            list.Should().Equal("a", "xx", "ccc");
        }

        [Fact]
        public void TestContainsAny()
        {
            new[] {1, 2}.ContainsAny(new[] {2}).Should().BeTrue();
            new[] {1, 2}.ContainsAny(new[] {2, 3}).Should().BeTrue();
            new[] {1, 2}.ContainsAny(new[] {3}).Should().BeFalse();
            new int[0].ContainsAny(new[] {2}).Should().BeFalse();
            new[] {1, 2}.ContainsAny(new int[0]).Should().BeFalse();
        }

        [Fact]
        public void TestSequencedEqualsList()
        {
            new[] {"A", "B", "C"}.ToList().SequencedEquals(new[] {"A", "B", "C"}).Should().BeTrue();
            new string[0].ToList().SequencedEquals(new string[0]).Should().BeTrue();
            new[] {"A", "B", "C"}.ToList().SequencedEquals(new[] {"C", "B", "A"}).Should().BeFalse();
            new[] {"A", "B", "C"}.ToList().SequencedEquals(new[] {"X", "Y", "Z"}).Should().BeFalse();
            new[] {"A", "B", "C"}.ToList().SequencedEquals(new[] {"A", "B"}).Should().BeFalse();
            new[] {new object()}.ToList().SequencedEquals(new[] {new object()}).Should().BeFalse();
        }

        [Fact]
        public void TestSequencedEqualsArray()
        {
            new[] {"A", "B", "C"}.SequencedEquals(new[] {"A", "B", "C"}).Should().BeTrue();
            new string[0].SequencedEquals(new string[0]).Should().BeTrue();
            new[] {"A", "B", "C"}.SequencedEquals(new[] {"C", "B", "A"}).Should().BeFalse();
            new[] {"A", "B", "C"}.SequencedEquals(new[] {"X", "Y", "Z"}).Should().BeFalse();
            new[] {"A", "B", "C"}.SequencedEquals(new[] {"A", "B"}).Should().BeFalse();
            new[] {new object()}.SequencedEquals(new[] {new object()}).Should().BeFalse();
        }

        [Fact]
        public void TestUnsequencedEquals()
        {
            new[] {"A", "B", "C"}.UnsequencedEquals(new[] {"A", "B", "C"}).Should().BeTrue();
            new string[0].UnsequencedEquals(new string[0]).Should().BeTrue();
            new[] {"A", "B", "C"}.UnsequencedEquals(new[] {"C", "B", "A"}).Should().BeTrue();
            new[] {"A", "B", "C"}.UnsequencedEquals(new[] {"X", "Y", "Z"}).Should().BeFalse();
            new[] {"A", "B", "C"}.UnsequencedEquals(new[] {"A", "B"}).Should().BeFalse();
            new[] {new object()}.UnsequencedEquals(new[] {new object()}).Should().BeFalse();
        }

        [Fact]
        public void TestGetSequencedHashCode()
        {
            new[] {"A", "B", "C"}.GetSequencedHashCode().Should().Be(new[] {"A", "B", "C"}.GetSequencedHashCode());
            new string[0].GetSequencedHashCode().Should().Be(new string[0].GetSequencedHashCode());
            new[] {"C", "B", "A"}.GetSequencedHashCode().Should().NotBe(new[] {"A", "B", "C"}.GetSequencedHashCode());
            new[] {"X", "Y", "Z"}.GetSequencedHashCode().Should().NotBe(new[] {"A", "B", "C"}.GetSequencedHashCode());
            new[] {"A", "B"}.GetSequencedHashCode().Should().NotBe(new[] {"A", "B", "C"}.GetSequencedHashCode());
        }

        [Fact]
        public void TestGetUnsequencedHashCode()
        {
            new[] {"A", "B", "C"}.GetUnsequencedHashCode().Should().Be(new[] {"A", "B", "C"}.GetUnsequencedHashCode());
            new string[0].GetUnsequencedHashCode().Should().Be(new string[0].GetUnsequencedHashCode());
            new[] {"C", "B", "A"}.GetUnsequencedHashCode().Should().Be(new[] {"A", "B", "C"}.GetUnsequencedHashCode());
            new[] {"X", "Y", "Z"}.GetUnsequencedHashCode().Should().NotBe(new[] {"A", "B", "C"}.GetUnsequencedHashCode());
            new[] {"A", "B"}.GetUnsequencedHashCode().Should().NotBe(new[] {"A", "B", "C"}.GetUnsequencedHashCode());
        }
    }
}
