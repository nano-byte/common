// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// Contains test methods for <see cref="ArrayUtils"/>.
/// </summary>
public class ArrayUtilsTest
{
    [Fact]
    public void TestAppend()
    {
        new[] {"A", "B"}.Append("C")
                        .Should().Equal("A", "B", "C");
    }

    [Fact]
    public void TestPrepend()
    {
        new[] {"B", "C"}.Prepend("A")
                        .Should().Equal("A", "B", "C");
    }

    [Fact]
    public void TestConcat()
    {
        new[] {"A", "B"}.Concat(new []{"C", "D"})
                        .Should().Equal("A", "B", "C", "D");
    }

    [Fact]
    public void TestConcatParams()
    {
        ArrayUtils.Concat(new[] { "A", "B" }, new[] { "C", "D" }, new[] { "E", "F" })
                  .Should().Equal("A", "B", "C", "D", "E", "F");
    }

    [Fact]
    public void TestSequencedEquals()
    {
        new[] {"A", "B", "C"}.SequencedEquals(["A", "B", "C"]).Should().BeTrue();
        Array.Empty<string>().SequencedEquals([]).Should().BeTrue();
        new[] {"A", "B", "C"}.SequencedEquals(["C", "B", "A"]).Should().BeFalse();
        new[] {"A", "B", "C"}.SequencedEquals(["X", "Y", "Z"]).Should().BeFalse();
        new[] {"A", "B", "C"}.SequencedEquals(["A", "B"]).Should().BeFalse();
        new[] {new object()}.SequencedEquals([new object()]).Should().BeFalse();
    }
}
