// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Diagnostics;
using System.IO;
using FluentAssertions;
using NanoByte.Common.Storage;
using Xunit;

#if NETFRAMEWORK
using NanoByte.Common.Native;
#endif

namespace NanoByte.Common
{
    public class ProcessUtilsTest
    {
        [Fact]
        public void TestAssembly()
        {
            string assemblyPath = Path.Combine(Locations.InstallBase,
#if NETFRAMEWORK
                "my assembly.exe"
#else
                "my assembly.dll"
#endif
            );

            FileUtils.Touch(assemblyPath);
            ProcessStartInfo startInfo;
            try
            {
                startInfo = ProcessUtils.Assembly("my assembly", "my", "args");
            }
            finally
            {
                File.Delete(assemblyPath);
            }

#if NETFRAMEWORK
            if (WindowsUtils.IsWindows)
            {
                startInfo.FileName.Should().Be(assemblyPath);
                startInfo.Arguments.Should().Be("my args");
            }
            else
            {
                startInfo.FileName.Should().EndWith("mono");
                startInfo.Arguments.Should().Be(assemblyPath.EscapeArgument() + " my args");
            }
#else
            startInfo.FileName.Should().EndWith("dotnet");
            startInfo.Arguments.Should().Be(assemblyPath.EscapeArgument() + " my args");
#endif
        }

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
