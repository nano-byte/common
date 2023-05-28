// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Diagnostics;
using NanoByte.Common.Storage;

#if NETFRAMEWORK
using NanoByte.Common.Native;
#endif

namespace NanoByte.Common;

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
            startInfo.Arguments.Should().Be($"{assemblyPath.EscapeArgument()} my args");
        }
#else
        startInfo.FileName.Should().Contain("dotnet");
        startInfo.Arguments.Should().Be($"{assemblyPath.EscapeArgument()} my args");
#endif
    }

    [Theory]
    [InlineData(new[] {"part1"}, "part1")]
    [InlineData(new[] {"part1", "part2"}, "part1 part2")]
    [InlineData(new[] {"part1 \" part2", "part3"}, "\"part1 \\\" part2\" part3")]
    public void TestJoinEscapeArguments(string[] parts, string result)
        => parts.JoinEscapeArguments().Should().Be(result);

    [Theory]
    [InlineData("", "\"\"", "Empty strings need to be escaped in order not to vanish")]
    [InlineData("test", "test", "Simple strings shouldn't be modified")]
    [InlineData("test1 test2", "\"test1 test2\"", "Strings with whitespaces should be encapsulated")]
    [InlineData("test1 test2\\", "\"test1 test2\\\\\"", "Trailing backslashes should be escaped")]
    [InlineData("test1\"test2", "test1\\\"test2", "Quotation marks should be escaped")]
    [InlineData("test1\\\\test2", "test1\\\\test2", "Consecutive slashes without quotation marks should not be escaped")]
    [InlineData("test1\\\"test2", "test1\\\\\\\"test2", "Slashes with quotation marks should be escaped")]
    public void TestEscapeArgument(string unescaped, string escaped, string because)
        => unescaped.EscapeArgument().Should().Be(escaped, because);

    [Theory]
    [InlineData("executable", "my args", "executable my args")]
    [InlineData("my executable", "my args", "\"my executable\" my args")]
    public void TestToCommandLine(string executable, string args, string commandLine)
        => new ProcessStartInfo(executable, args).ToCommandLine().Should().Be(commandLine);

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
