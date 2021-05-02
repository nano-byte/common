// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using Xunit;

namespace NanoByte.Common
{
    public class ProcessUtilsTest
    {
        [Fact]
        public void TestJoinEscapeArguments()
        {
            new[] {"part1"}.JoinEscapeArguments().Should().Be("part1");
            new[] {"part1", "part2"}.JoinEscapeArguments().Should().Be("part1 part2");
            new[] {"part1 \" part2", "part3"}.JoinEscapeArguments().Should().Be("\"part1 \\\" part2\" part3");
        }
    }
}
