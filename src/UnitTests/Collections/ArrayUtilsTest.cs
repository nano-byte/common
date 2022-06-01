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
        var strings = new[] {"A", "B"};
        strings.Append("C").Should().Equal("A", "B", "C");
    }

    [Fact]
    public void TestPrepend()
    {
        var strings = new[] {"B", "C"};
        strings.Prepend("A").Should().Equal("A", "B", "C");
    }

    [Fact]
    public void TestSequencedEquals()
    {
        new[] {"A", "B", "C"}.SequencedEquals(new[] {"A", "B", "C"}).Should().BeTrue();
        Array.Empty<string>().SequencedEquals(Array.Empty<string>()).Should().BeTrue();
        new[] {"A", "B", "C"}.SequencedEquals(new[] {"C", "B", "A"}).Should().BeFalse();
        new[] {"A", "B", "C"}.SequencedEquals(new[] {"X", "Y", "Z"}).Should().BeFalse();
        new[] {"A", "B", "C"}.SequencedEquals(new[] {"A", "B"}).Should().BeFalse();
        new[] {new object()}.SequencedEquals(new[] {new object()}).Should().BeFalse();
    }
}