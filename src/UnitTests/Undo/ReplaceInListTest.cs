// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// Contains test methods for <see cref="ReplaceInList{T}"/>.
/// </summary>
public class ReplaceInListTest
{
    /// <summary>
    /// Makes sure <see cref="ReplaceInList{T}"/> correctly performs executions and undos.
    /// </summary>
    [Fact]
    public void TestExecute()
    {
        var list = new List<string> {"a", "b", "c"};
        var command = new ReplaceInList<string>(list, "b", "x");

        command.Execute();
        list.Should().Equal("a", "x", "c");

        command.Undo();
        list.Should().Equal("a", "b", "c");
    }

    /// <summary>
    /// Makes sure <see cref="ReplaceInList{T}"/> undoes and redoes correctly even when the new
    /// element is equal by value to another element already present in the list.
    /// </summary>
    [Fact]
    public void TestUndoRedoWithCollidingValue()
    {
        var list = new List<string> {"a", "b", "c"};
        var command = new ReplaceInList<string>(list, "c", "a");

        command.Execute();
        list.Should().Equal("a", "b", "a");

        command.Undo();
        list.Should().Equal("a", "b", "c");

        command.Execute();
        list.Should().Equal("a", "b", "a");
    }

    /// <summary>
    /// Makes sure <see cref="ReplaceInList{T}"/> targets the specific element instance passed to it,
    /// even when the list contains another element that is equal by value.
    /// </summary>
    [Fact]
    public void TestReplacesSpecificInstance()
    {
        var first = new string('x', 1);
        var second = new string('x', 1);
        var list = new List<string> {first, second};
        var command = new ReplaceInList<string>(list, second, "y");

        command.Execute();
        list.Should().Equal("x", "y");
        list[0].Should().BeSameAs(first);

        command.Undo();
        list.Should().Equal("x", "x");
        list[1].Should().BeSameAs(second);
    }
}