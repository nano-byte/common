/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Security.Cryptography;
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
        public void TestCountOccurences()
        {
            "abc".CountOccurences('/').Should().Be(0);
            "ab/c".CountOccurences('/').Should().Be(1);
            "ab/c/".CountOccurences('/').Should().Be(2);
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
        public void TestStripCharacters() => "a!b?".StripCharacters("!?").Should().Be("ab");

        [Fact]
        public void TestStripFromEnd() => "abc".StripFromEnd(count: 1).Should().Be("ab");

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
        public void TestBase64Utf8Encode()
        {
            "".Base64Utf8Encode().Should().Be("");
            "test".Base64Utf8Encode().Should().Be("dGVzdA==");
        }

        [Fact]
        public void TestBase64Utf8Decode()
        {
            "".Base64Utf8Decode().Should().Be("");
            "dGVzdA==".Base64Utf8Decode().Should().Be("test");
        }

        [Fact]
        public void TestBase32Encode() => new byte[] {65, 66}.Base32Encode().Should().Be("IFBA");

        [Fact]
        public void TestBase16Encode() => new byte[] {65, 66}.Base16Encode().Should().Be("4142");

        [Fact]
        public void TestBase16Decode() => "4142".Base16Decode().Should().Equal(65, 66);

        [Fact]
        public void TestHash()
        {
            const string sha1ForEmptyString = "da39a3ee5e6b4b0d3255bfef95601890afd80709";
            "".Hash(SHA1.Create()).Should().Be(sha1ForEmptyString);
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
