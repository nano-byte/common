// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Dispatch
{
    /// <summary>
    /// Contains test methods for <see cref="Bucketizer{T}"/> and <see cref="Bucketizer{TElement,TValue}"/>.
    /// </summary>
    public class BucketizerTest
    {
        [Fact]
        public void TestPredicate()
        {
            var even = new List<int>();
            var lessThanThree = new List<int>();
            var rest = new List<int>();
            new[] {1, 2, 3, 4}.Bucketize()
                              .Add(x => x % 2 == 0, even)
                              .Add(x => x < 3, lessThanThree)
                              .Add(x => true, rest)
                              .Run();

            even.Should().Equal(2, 4);
            lessThanThree.Should().Equal(1);
            rest.Should().Equal(3);
        }

        [Fact]
        public void TestValue()
        {
            var a = new List<string>();
            var b = new List<string>();
            new[] {"alfred", "beatrice", "arnold"}.Bucketize(x => x[0])
                                                  .Add('a', a)
                                                  .Add('b', b)
                                                  .Run();

            a.Should().Equal("alfred", "arnold");
            b.Should().Equal("beatrice");
        }
    }
}
