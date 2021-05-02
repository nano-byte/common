// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using Xunit;

namespace NanoByte.Common
{
    /// <summary>
    /// Contains test methods for <see cref="StringUtils"/>.
    /// </summary>
    public class StringUtilsTest
    {
        [Fact]
        public void TestCompare()
        {
            StringUtils.EqualsIgnoreCase("abc", "abc").Should().BeTrue();
            StringUtils.EqualsIgnoreCase("abc", "ABC").Should().BeTrue();

            StringUtils.EqualsIgnoreCase("abc", "123").Should().BeFalse();
            StringUtils.EqualsIgnoreCase("abc", "abc ").Should().BeFalse();
        }

        [Fact]
        public void TestContains()
        {
            "This is a test.".ContainsIgnoreCase("TEST").Should().BeTrue();
            "This is a test.".ContainsIgnoreCase("test").Should().BeTrue();

            "abc".ContainsIgnoreCase("123").Should().BeFalse();
            "test".ContainsIgnoreCase("This is a test.").Should().BeFalse();
        }

        [Fact]
        public void TestSplitMultilineText()
        {
            "123\nabc".SplitMultilineText().Should().Equal(new[] {"123", "abc"}, because: "Should split Linux-stlye linebreaks");
            "123\rabc".SplitMultilineText().Should().Equal(new[] {"123", "abc"}, because: "Should split old Mac-stlye linebreaks");
            "123\r\nabc".SplitMultilineText().Should().Equal(new[] {"123", "abc"}, because: "Should split Windows-stlye linebreaks");
        }

        [Fact]
        public void TestJoin()
        {
            StringUtils.Join(" ", new[] {"part1"}).Should().Be("part1");
            StringUtils.Join(" ", new[] {"part1", "part2"}).Should().Be("part1 part2");
            new[] {"part1 part2", "part3"}.JoinEscapeArguments().Should().Be("\"part1 part2\" part3");
        }

        [Fact]
        public void TestJoinEscapeArguments()
        {
            new[] {"part1"}.JoinEscapeArguments().Should().Be("part1");
            new[] {"part1", "part2"}.JoinEscapeArguments().Should().Be("part1 part2");
            new[] {"part1 \" part2", "part3"}.JoinEscapeArguments().Should().Be("\"part1 \\\" part2\" part3");
        }

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
        public void TestRemoveCharacters() => "a!b?".RemoveCharacters("!?").Should().Be("ab");

        [Fact]
        public void TestTrimOverflow()
        {
            "abc".TrimOverflow(3).Should().Be("abc");
            "abc".TrimOverflow(4).Should().Be("abc");
            "abc".TrimOverflow(2).Should().Be("ab...");
        }

        [Fact]
        public void TestEscapeArgument()
        {
            "".EscapeArgument().Should().Be("\"\"", because: "Empty strings need to be escaped in order not to vanish");
            "test".EscapeArgument().Should().Be("test", because: "Simple strings shouldn't be modified");
            "test1 test2".EscapeArgument().Should().Be("\"test1 test2\"", because: "Strings with whitespaces should be encapsulated");
            "test1 test2\\".EscapeArgument().Should().Be("\"test1 test2\\\\\"", because: "Trailing backslashes should be escaped");
            "test1\"test2".EscapeArgument().Should().Be("test1\\\"test2", because: "Quotation marks should be escaped");
            "test1\\\\test2".EscapeArgument().Should().Be("test1\\\\test2", because: "Consecutive slashes without quotation marks should not be escaped");
            "test1\\\"test2".EscapeArgument().Should().Be("test1\\\\\\\"test2", because: "Slashes with quotation marks should be escaped");
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
}
