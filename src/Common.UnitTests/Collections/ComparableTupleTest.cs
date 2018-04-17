// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="ComparableTuple{T}"/>.
    /// </summary>
    public class ComparableTupleTest
    {
        [Fact]
        public void TestCompareTo()
        {
            var tuple1 = new ComparableTuple<int>(1, 1);
            var tuple2 = new ComparableTuple<int>(1, 2);
            var tuple3 = new ComparableTuple<int>(2, 1);

            tuple1.Should().Be(tuple1);

            tuple1.Should().BeLessThan(tuple2);
            tuple2.Should().BeLessThan(tuple3);
            tuple3.Should().BeGreaterThan(tuple2);
            tuple2.Should().BeGreaterThan(tuple1);
        }
    }
}
