// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20

namespace NanoByte.Common.Collections;

/// <summary>
/// Contains test methods for <see cref="ConcurrentSet{T}"/>.
/// </summary>
public class ConcurrentSetTest
{
    [Fact]
    public void EmptySetHasZeroCount()
    {
        var set = new ConcurrentSet<int>();
        set.Count.Should().Be(0);
    }

    [Fact]
    public void AddIncreasesCount()
    {
        var set = new ConcurrentSet<int>();
        set.Add(1);
        set.Count.Should().Be(1);
        set.Add(2);
        set.Count.Should().Be(2);
    }

    [Fact]
    public void AddDuplicateDoesNotIncreaseCount()
    {
        var set = new ConcurrentSet<int> {1, 1};
        set.Count.Should().Be(1);
    }

    [Fact]
    public void ContainsReturnsTrueForAddedItems()
    {
        var set = new ConcurrentSet<string> {"test"};
        set.Contains("test").Should().BeTrue();
        set.Contains("missing").Should().BeFalse();
    }

    [Fact]
    public void RemoveDeletesItem()
    {
        var set = new ConcurrentSet<int> {1, 2, 3};
        set.Remove(2).Should().BeTrue();
        set.Contains(2).Should().BeFalse();
        set.Count.Should().Be(2);
    }

    [Fact]
    public void RemoveReturnsFalseForMissingItem()
    {
        var set = new ConcurrentSet<int> {1};
        set.Remove(2).Should().BeFalse();
    }

    [Fact]
    public void ClearRemovesAllItems()
    {
        var set = new ConcurrentSet<int> {1, 2, 3};
        set.Clear();
        set.Count.Should().Be(0);
    }

    [Fact]
    public void ConstructorWithCollection()
    {
        var set = new ConcurrentSet<int>(new[] {1, 2, 3});
        set.Count.Should().Be(3);
        set.Contains(2).Should().BeTrue();
    }

    [Fact]
    public void ConstructorWithCollectionAndComparer()
    {
        var set = new ConcurrentSet<string>(StringComparer.OrdinalIgnoreCase) {"Test"};
        set.Add("test");
        set.Count.Should().Be(1);
    }

    [Fact]
    public void CopyTo()
    {
        var set = new ConcurrentSet<int> {1, 2, 3};
        var array = new int[3];
        set.CopyTo(array, 0);
        array.Should().BeEquivalentTo(new[] {1, 2, 3});
    }

    [Fact]
    public void GetEnumerator()
    {
        var set = new ConcurrentSet<int> {1, 2, 3};
        var items = set.ToList();
        items.Should().BeEquivalentTo(new[] {1, 2, 3});
    }

    [Fact]
    public void IsReadOnlyReturnsFalse()
    {
        var set = new ConcurrentSet<int>();
        set.IsReadOnly.Should().BeFalse();
    }

    [Fact]
    public void AddNull()
    {
        var set = new ConcurrentSet<string>();
        set.Invoking(s => s.Add(null!))
           .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ContainsNull()
    {
        var set = new ConcurrentSet<string>();
        set.Invoking(s => s.Contains(null!))
           .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RemoveNull()
    {
        var set = new ConcurrentSet<string>();
        set.Invoking(s => s.Remove(null!))
           .Should().Throw<ArgumentNullException>();
    }
}

#endif
