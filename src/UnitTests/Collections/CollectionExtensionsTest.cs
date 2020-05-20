// Copyright Bastian Eicher
// Licensed under the MIT License

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

            list.Invoking(x => x.RemoveLast(-1)).Should().Throw<ArgumentOutOfRangeException>();
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

        [Fact]
        public void TestPermutate()
        {
            var permutations = new[] {1, 2, 3}.Permutate().ToList();
            permutations[0].Should().Equal(1, 2, 3);
            permutations[1].Should().Equal(1, 3, 2);
            permutations[2].Should().Equal(2, 1, 3);
            permutations[3].Should().Equal(2, 3, 1);
            permutations[4].Should().Equal(3, 2, 1);
            permutations[5].Should().Equal(3, 1, 2);
        }

        [Fact]
        public void TestPermutateEmpty()
        {
            var permutations = new int[0].Permutate().ToList();
            permutations[0].Should().BeEmpty();
        }
    }
}
