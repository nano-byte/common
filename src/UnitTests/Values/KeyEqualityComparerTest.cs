// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Values;

/// <summary>
/// Contains test methods for <see cref="KeyEqualityComparer{T,TKey}"/>.
/// </summary>
public class KeyEqualityComparerTest
{
    private record TestRecord(string Name);

    [Fact]
    public void EqualsByKey()
    {
        var comparer = new KeyEqualityComparer<TestRecord, string>(x => x.Name);
        var obj1 = new TestRecord("X");
        var obj2 = new TestRecord("X");

        comparer.Equals(obj1, obj2).Should().BeTrue();
    }

    [Fact]
    public void NotEqualsByDifferentKey()
    {
        var comparer = new KeyEqualityComparer<TestRecord, string>(x => x.Name);
        var obj1 = new TestRecord("X");
        var obj2 = new TestRecord("Y");

        comparer.Equals(obj1, obj2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCodeFromKey()
    {
        var comparer = new KeyEqualityComparer<TestRecord, string>(x => x.Name);
        var obj1 = new TestRecord("X");
        var obj2 = new TestRecord("X");

        comparer.GetHashCode(obj1).Should().Be(comparer.GetHashCode(obj2));
    }

    [Fact]
    public void HandlesNullValues()
    {
        var comparer = new KeyEqualityComparer<TestRecord, string>(x => x.Name);

        comparer.Equals(null, null).Should().BeTrue();
        comparer.Equals(new TestRecord("X"), null).Should().BeFalse();
        comparer.Equals(null, new TestRecord("X")).Should().BeFalse();
    }

    [Fact]
    public void CanBeUsedWithLinqDistinct()
    {
        var comparer = new KeyEqualityComparer<TestRecord, string>(x => x.Name);
        var list = new[]
        {
            new TestRecord("X"),
            new TestRecord("X"),
            new TestRecord("Y")
        };

        var distinct = list.Distinct(comparer).ToList();
        distinct.Should().HaveCount(2);
        distinct.Should().Contain(x => x.Name == "X");
        distinct.Should().Contain(x => x.Name == "Y");
    }
}
