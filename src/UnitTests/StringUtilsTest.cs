// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

/// <summary>
/// Contains test methods for <see cref="StringUtils"/>.
/// </summary>
public class StringUtilsTest
{
    [Theory]
    [InlineData("abc", "abc")]
    [InlineData("abc", "ABC")]
    public void EqualsIgnoreCase(string a, string b)
        => StringUtils.EqualsIgnoreCase(a, b).Should().BeTrue();

    [Theory]
    [InlineData("abc", "123")]
    [InlineData("abc", "abc ")]
    public void NotEqualsIgnoreCase(string a, string b)
        => StringUtils.EqualsIgnoreCase(a, b).Should().BeFalse();

    [Theory]
    [InlineData("This is a test.", "TEST")]
    [InlineData("This is a test.", "test")]
    public void ContainsIgnoreCase(string value, string searchFor)
        => value.ContainsIgnoreCase(searchFor).Should().BeTrue();

    [Theory]
    [InlineData("This is a test.", "123")]
    [InlineData("test", "This is a test.")]
    public void NotContainsIgnoreCase(string value, string searchFor)
        => value.ContainsIgnoreCase(searchFor).Should().BeFalse();

    [Fact]
    public void TestStartsWith()
    {
        "prefix: rest".StartsWith("prefix", out string? rest).Should().BeTrue();
        rest.Should().Be(": rest");
    }

    [Fact]
    public void TestEndsWith()
    {
        "rest: suffix".EndsWith("suffix", out string? rest).Should().BeTrue();
        rest.Should().Be("rest: ");
    }

    [Theory]
    [InlineData("123\nabc", new[] {"123", "abc"}, "Should split Linux-style linebreaks")]
    [InlineData("123\rabc", new[] {"123", "abc"}, "Should split Linux-style linebreaks")]
    [InlineData("123\r\nabc", new[] {"123", "abc"}, "Should split Linux-style linebreaks")]
    [InlineData("123", new[] {"123"}, "Should work with single lines")]
    [InlineData("", new[] {""}, "Should work with empty strings")]
    public void TestSplitMultilineText(string value, string[] result, string because)
        => value.SplitMultilineText().Should().Equal(result, because);

    [Theory]
    [InlineData(new string[0], "")]
    [InlineData(new[] {"part1"}, "part1")]
    [InlineData(new[] {"part1", "part2"}, "part1 part2")]
    public void TestJoin(string[] parts, string result)
        => StringUtils.Join(" ", parts).Should().Be(result);

    [Fact]
    public void TestGetLeftRightPartChar()
    {
        const string testString = "text1 text2 text3";
        testString.GetLeftPartAtFirstOccurrence(' ').Should().Be("text1");
        testString.GetRightPartAtFirstOccurrence(' ').Should().Be("text2 text3");
        testString.GetLeftPartAtLastOccurrence(' ').Should().Be("text1 text2");
        testString.GetRightPartAtLastOccurrence(' ').Should().Be("text3");
    }

    [Fact]
    public void TestGetLeftRightPartString()
    {
        const string testString = "text1 - text2 - text3";
        testString.GetLeftPartAtFirstOccurrence(" - ").Should().Be("text1");
        testString.GetRightPartAtFirstOccurrence(" - ").Should().Be("text2 - text3");
        testString.GetLeftPartAtLastOccurrence(" - ").Should().Be("text1 - text2");
        testString.GetRightPartAtLastOccurrence(" - ").Should().Be("text3");
    }

    [Fact]
    public void TestRemoveCharacters()
        => "a!b?".RemoveCharacters("!?").Should().Be("ab");

    [Fact]
    public void TestTrimOverflow()
    {
        "abc".TrimOverflow(3).Should().Be("abc");
        "abc".TrimOverflow(4).Should().Be("abc");
        "abc".TrimOverflow(2).Should().Be("ab...");
    }

    [Fact]
    public void TestGeneratePassword()
    {
        for (int i = 0; i < 128; i++)
        {
            string result = StringUtils.GeneratePassword(i);
            result.Should().NotContain("=").And.NotContain("l");
            result.Length.Should().Be(i);
        }
    }
}
