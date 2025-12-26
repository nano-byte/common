// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// Contains test methods for <see cref="DefaultComparer{T}"/>.
/// </summary>
public class DefaultComparerTest
{
    [Fact]
    public void CompareEqualValues()
    {
        var comparer = DefaultComparer<int>.Instance;
        comparer.Compare(5, 5).Should().Be(0);
    }

    [Fact]
    public void CompareLessThan()
    {
        var comparer = DefaultComparer<int>.Instance;
        comparer.Compare(3, 5).Should().BeLessThan(0);
    }

    [Fact]
    public void CompareGreaterThan()
    {
        var comparer = DefaultComparer<int>.Instance;
        comparer.Compare(7, 5).Should().BeGreaterThan(0);
    }

    [Fact]
    public void CompareStrings()
    {
        var comparer = DefaultComparer<string>.Instance;
        comparer.Compare("apple", "banana").Should().BeLessThan(0);
        comparer.Compare("banana", "apple").Should().BeGreaterThan(0);
        comparer.Compare("test", "test").Should().Be(0);
    }

    [Fact]
    public void CompareNullValues()
    {
        var comparer = DefaultComparer<string>.Instance;
        // This implementation treats null as equal to anything
        comparer.Compare(null, null).Should().Be(0);
        comparer.Compare("test", null).Should().Be(0);
        comparer.Compare(null, "test").Should().Be(0);
    }

    [Fact]
    public void CanBeUsedWithLinqOrderBy()
    {
        var comparer = DefaultComparer<int>.Instance;
        var list = new[] {3, 1, 4, 1, 5, 9, 2, 6};
        var sorted = list.OrderBy(x => x, comparer).ToList();
        sorted.Should().BeInAscendingOrder();
    }
}
