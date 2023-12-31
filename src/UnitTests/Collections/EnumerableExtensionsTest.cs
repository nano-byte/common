// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// Contains test methods for <see cref="EnumerableExtensions"/>.
/// </summary>
public class EnumerableExtensionsTest
{
    [Fact]
    public void TestContainsAny()
    {
        new[] {1, 2}.ContainsAny(new[] {2}).Should().BeTrue();
        new[] {1, 2}.ContainsAny(new[] {2, 3}).Should().BeTrue();
        new[] {1, 2}.ContainsAny(new[] {3}).Should().BeFalse();
        Array.Empty<int>().ContainsAny(new[] {2}).Should().BeFalse();
        new[] {1, 2}.ContainsAny(Array.Empty<int>()).Should().BeFalse();
    }

    [Fact]
    public void TestSequencedEquals()
    {
        new[] {"A", "B", "C"}.ToList().SequencedEquals(new[] {"A", "B", "C"}).Should().BeTrue();
        Array.Empty<string>().ToList().SequencedEquals(Array.Empty<string>()).Should().BeTrue();
        new[] {"A", "B", "C"}.ToList().SequencedEquals(new[] {"C", "B", "A"}).Should().BeFalse();
        new[] {"A", "B", "C"}.ToList().SequencedEquals(new[] {"X", "Y", "Z"}).Should().BeFalse();
        new[] {"A", "B", "C"}.ToList().SequencedEquals(new[] {"A", "B"}).Should().BeFalse();
        new[] {new object()}.ToList().SequencedEquals(new[] {new object()}).Should().BeFalse();
    }

    [Fact]
    public void TestUnsequencedEquals()
    {
        new[] {"A", "B", "C"}.UnsequencedEquals(new[] {"A", "B", "C"}).Should().BeTrue();
        Array.Empty<string>().UnsequencedEquals(Array.Empty<string>()).Should().BeTrue();
        new[] {"A", "B", "C"}.UnsequencedEquals(new[] {"C", "B", "A"}).Should().BeTrue();
        new[] {"A", "B", "C"}.UnsequencedEquals(new[] {"X", "Y", "Z"}).Should().BeFalse();
        new[] {"A", "B", "C"}.UnsequencedEquals(new[] {"A", "B"}).Should().BeFalse();
        new[] {new object()}.UnsequencedEquals(new[] {new object()}).Should().BeFalse();
    }

    [Fact]
    public void TestGetSequencedHashCode()
    {
        new[] {"A", "B", "C"}.GetSequencedHashCode().Should().Be(new[] {"A", "B", "C"}.GetSequencedHashCode());
        Array.Empty<string>().GetSequencedHashCode().Should().Be(Array.Empty<string>().GetSequencedHashCode());
        new[] {"C", "B", "A"}.GetSequencedHashCode().Should().NotBe(new[] {"A", "B", "C"}.GetSequencedHashCode());
        new[] {"X", "Y", "Z"}.GetSequencedHashCode().Should().NotBe(new[] {"A", "B", "C"}.GetSequencedHashCode());
        new[] {"A", "B"}.GetSequencedHashCode().Should().NotBe(new[] {"A", "B", "C"}.GetSequencedHashCode());
    }

    [Fact]
    public void TestGetUnsequencedHashCode()
    {
        new[] {"A", "B", "C"}.GetUnsequencedHashCode().Should().Be(new[] {"A", "B", "C"}.GetUnsequencedHashCode());
        Array.Empty<string>().GetUnsequencedHashCode().Should().Be(Array.Empty<string>().GetUnsequencedHashCode());
        new[] {"C", "B", "A"}.GetUnsequencedHashCode().Should().Be(new[] {"A", "B", "C"}.GetUnsequencedHashCode());
        new[] {"X", "Y", "Z"}.GetUnsequencedHashCode().Should().NotBe(new[] {"A", "B", "C"}.GetUnsequencedHashCode());
        new[] {"A", "B"}.GetUnsequencedHashCode().Should().NotBe(new[] {"A", "B", "C"}.GetUnsequencedHashCode());
    }

    [Fact]
    public void TestMaxBy()
        => new[] {"abc", "a", "abcd", "ab"}
          .MaxBy(x => x.Length)
          .Should().Be("abcd");

    [Fact]
    public void TestMinBy()
        => new[] {"abc", "a", "abcd", "ab"}
          .MinBy(x => x.Length)
          .Should().Be("a");

    [Fact]
    public void TestDistinctBy()
        => new[] {"a123", "a234", "b123"}
          .DistinctBy(x => x[0])
          .Should().BeEquivalentTo("a123", "b123");

    [Fact]
    public void TestTrySelectPassException()
    {
        string[] strings = ["1", "2", "c", "4"];
        bool callbackCalled = false;
        strings.Invoking(x => x.TrySelect(int.Parse, (ArgumentException _) => callbackCalled = true).ToList())
               .Should().Throw<FormatException>();
        callbackCalled.Should().BeFalse();
    }

    [Fact]
    public void TestTrySelectCatchException()
    {
        string[] strings = ["1", "2", "c", "4"];
        bool callbackCalled = false;
        strings.TrySelect(int.Parse, (FormatException _) => callbackCalled = true)
               .Should().Equal(1, 2, 4);
        callbackCalled.Should().BeTrue();
    }

    [Fact]
    public void TestAppend()
    {
        var strings = new List<string> {"A", "B"};
        strings.Append("C").Should().Equal("A", "B", "C");
    }

    [Fact]
    public void TestPrepend()
    {
        var strings = new List<string> {"B", "C"};
        strings.Prepend("A").Should().Equal("A", "B", "C");
    }

    private class Element
    {
        public readonly List<Element> Children = [];
    }

    [Fact]
    public void TestTopologicalSort()
    {
        var b = new Element();
        var c = new Element {Children = {b}};
        var a = new Element {Children = {c}};
        var list = new List<Element> {a, b, c};

        list.TopologicalSort(getDependencies: x => x.Children)
            .Should().Equal(b, c, a);
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
        var permutations = Array.Empty<int>().Permutate().ToList();
        permutations[0].Should().BeEmpty();
    }
}
