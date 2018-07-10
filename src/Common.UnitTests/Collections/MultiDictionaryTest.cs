// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="MultiDictionary{TKey,TValue}"/>.
    /// </summary>
    public class MultiDictionaryTest
    {
        [Fact]
        public void TestAdd()
        {
            new MultiDictionary<string, int>
            {
                {"A", 0},
                {"B", 0},
                {"A", 1},
                {"B", 0}
            }.Should().BeEquivalentTo(new Dictionary<string, IEnumerable<int>>
            {
                ["A"] = new[] {0, 1},
                ["B"] = new[] {0}
            });
        }

        [Fact]
        public void TestValues()
        {
            new MultiDictionary<string, int>
            {
                {"A", 0},
                {"B", 1},
                {"C", 1}
            }.Values.Should().BeEquivalentTo(0, 1, 1);
        }

        [Fact]
        public void TestRemove()
        {
            var dict = new MultiDictionary<string, int>
            {
                {"A", 0},
                {"A", 1},
                {"B", 1}
            };

            dict.Remove("A", 1).Should().BeTrue();
            dict.Remove("B", 1).Should().BeTrue();

            dict.Remove("A", 2).Should().BeFalse();
            dict.Remove("C", 0).Should().BeFalse();

            dict.Should().BeEquivalentTo(new Dictionary<string, IEnumerable<int>> {["A"] = new[] {0}});
        }
    }
}
