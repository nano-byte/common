// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// Contains test methods for <see cref="ListExtensions"/>.
/// </summary>
public class ListExtensionsTest
{
    /// <summary>
    /// Ensures that <see cref="ListExtensions.RemoveLast{T}"/> correctly removes the last n elements from a list.
    /// </summary>
    [Fact]
    public void TestRemoveLast()
    {
        var list = new List<string> {"a", "b", "c"};
        list.RemoveLast(2);
        list.Should().Equal("a");

        list.Invoking(x => x.RemoveLast(-1)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void TestAddOrReplace()
    {
        var list = new List<string> {"a", "bb", "ccc"};
        list.AddOrReplace("xx", keySelector: x => x.Length);
        list.Should().Equal("a", "xx", "ccc");
    }

    [Fact]
    public void TestGetAddedElements()
    {
        new[] {"A", "B", "C", "E", "G", "H"}.GetAddedElements(["A", "C", "E", "G"]).Should().Equal("B", "H");
        new[] {"C", "D"}.GetAddedElements(["A", "D"]).Should().Equal("C");
    }

    /// <summary>
    /// Ensures <see cref="ListExtensions.IndexOfByReference{T}"/> locates the exact instance and ignores other elements that are merely equal by value.
    /// </summary>
    [Fact]
    public void TestIndexOfByReference()
    {
        var first = new string('x', 1);
        var second = new string('x', 1);
        var list = new List<string> {first, second};

        list.IndexOfByReference(first).Should().Be(0);
        list.IndexOfByReference(second).Should().Be(1);
        list.IndexOfByReference(new string('x', 1)).Should().Be(-1, because: "an instance only equal by value must not match");
    }

    /// <summary>
    /// Ensures <see cref="ListExtensions.IndexOfByReference{T}"/> works for value types.
    /// </summary>
    [Fact]
    public void TestIndexOfByReferenceValueType()
    {
        var list = new List<int> {1, 2, 2, 3};

        list.IndexOfByReference(2).Should().Be(1);
        list.IndexOfByReference(9).Should().Be(-1);
    }
}
