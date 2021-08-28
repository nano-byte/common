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

        [Fact]
        public void TestToCommandLine()
        {
            new ProcessStartInfo("executable", "my args").ToCommandLine().Should().Be("executable my args");
            new ProcessStartInfo("my executable", "my args").ToCommandLine().Should().Be("\"my executable\" my args");
        }

        [Fact]
        public void TestFromCommandLine()
        {
            var startInfo = ProcessUtils.FromCommandLine("executable my args");
            startInfo.FileName.Should().Be("executable");
            startInfo.Arguments.Should().Be("my args");

            startInfo = ProcessUtils.FromCommandLine("\"my executable\" my args");
            startInfo.FileName.Should().Be("my executable");
            startInfo.Arguments.Should().Be("my args");
        }
    }
}
