// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Values;

/// <summary>
/// Contains test methods for <see cref="KeyEqualityComparer{T,TKey}"/>.
/// </summary>
public class KeyEqualityComparerTest
{
    private record TestRecord(string Name, int Age);

    [Fact]
    public void EqualsByKey()
    {
        var comparer = new KeyEqualityComparer<TestRecord, string>(x => x.Name);
        var obj1 = new TestRecord("John", 30);
        var obj2 = new TestRecord("John", 40);
        
        comparer.Equals(obj1, obj2).Should().BeTrue();
    }

    [Fact]
    public void NotEqualsByDifferentKey()
    {
        var comparer = new KeyEqualityComparer<TestRecord, string>(x => x.Name);
        var obj1 = new TestRecord("John", 30);
        var obj2 = new TestRecord("Jane", 30);
        
        comparer.Equals(obj1, obj2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCodeFromKey()
    {
        var comparer = new KeyEqualityComparer<TestRecord, string>(x => x.Name);
        var obj1 = new TestRecord("John", 30);
        var obj2 = new TestRecord("John", 40);
        
        comparer.GetHashCode(obj1).Should().Be(comparer.GetHashCode(obj2));
    }

    [Fact]
    public void HandlesNullValues()
    {
        var comparer = new KeyEqualityComparer<TestRecord, string>(x => x.Name);
        
        comparer.Equals(null, null).Should().BeTrue();
        comparer.Equals(new TestRecord("John", 30), null).Should().BeFalse();
        comparer.Equals(null, new TestRecord("John", 30)).Should().BeFalse();
    }

    [Fact]
    public void CanBeUsedWithLinqDistinct()
    {
        var comparer = new KeyEqualityComparer<TestRecord, string>(x => x.Name);
        var list = new[]
        {
            new TestRecord("John", 30),
            new TestRecord("John", 40),
            new TestRecord("Jane", 25)
        };

        var distinct = list.Distinct(comparer).ToList();
        distinct.Should().HaveCount(2);
        distinct.Should().Contain(x => x.Name == "John");
        distinct.Should().Contain(x => x.Name == "Jane");
    }
}
