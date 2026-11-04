// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Diagnostics;
using NanoByte.Common.Native;
using NanoByte.Common.Storage;

namespace NanoByte.Common;

[Collection(nameof(Locations))]
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
    [InlineData(null, new string[0])]
    [InlineData("", new string[0])]
    [InlineData("--foo --bar", new[] {"--foo", "--bar"})]
    [InlineData("  --foo", new[] {"--foo"})]
    [InlineData("--name=\"My App\"", new[] {"--name=My App"})]
    [InlineData("\"my arg\" --foo", new[] {"my arg", "--foo"})]
    [InlineData("--foo \"my arg", new[] {"--foo", "my arg"})]
    [InlineData("--foo \"", new[] {"--foo", ""})]
    public void TestSplitArguments(string? arguments, string[] expected)
        => ProcessUtils.SplitArguments(arguments).Should().Equal(expected);

    [Theory]
    [InlineData(null, new string[0])]
    [InlineData("", new string[0])]
    [InlineData("   ", new string[0])]
    [InlineData("--foo --bar", new[] {"--foo", "--bar"})]
    [InlineData("  --foo   --bar  ", new[] {"--foo", "--bar"})]
    [InlineData("--name=\"My App\" 'a b'", new[] {"--name=My App", "a b"})]
    [InlineData("a\\ b", new[] {"a b"})]
    [InlineData("\"\" ''", new[] {"", ""})]
    [InlineData("\"a\\\"b\\c\" 'a\\b'", new[] {"a\"b\\c", "a\\b"})]
    [InlineData("a\tb\nc", new[] {"a", "b", "c"})]
    [InlineData("a b\rc", new[] {"a b\rc"})]
    [InlineData("a\\", new[] {"a\\"})]
    [InlineData("gdb \\\n--args foo", new[] {"gdb", "--args", "foo"})]
    [InlineData("ab\\\ncd", new[] {"abcd"})]
    [InlineData("\"a\\\nb\"", new[] {"ab"})]
    [InlineData("'a\\\nb'", new[] {"a\\\nb"})]
    [InlineData("\\\n", new string[0])]
    [InlineData("'a b", new[] {"a b"})]
    [InlineData("\"a b", new[] {"a b"})]
    [InlineData("--foo \"", new[] {"--foo", ""})]
    [InlineData("/home/o'brien/app --flag", new[] {"/home/obrien/app --flag"})]
    [InlineData("'a\\", new[] {"a\\"})]
    [InlineData("\"a\\", new[] {"a\\"})]
    [InlineData("\"a\\\"", new[] {"a\""})]
    public void TestSplitArgumentsPosix(string? commandLine, string[] expected)
        => ProcessUtils.SplitArgumentsPosix(commandLine).Should().Equal(expected);

    [Theory]
    [InlineData("", new string[0])]
    [InlineData(" \t ", new string[0])]
    [InlineData("a\tb  c", new[] {"a", "b", "c"})]
    [InlineData("a\nb", new[] {"a\nb"})]
    [InlineData("\"a b\" \"\"", new[] {"a b", ""})]
    [InlineData("a\\b \\\\c", new[] {"a\\b", "\\\\c"})]
    [InlineData("a\\\"b", new[] {"a\"b"})]
    [InlineData("a\\\\\"b c\"", new[] {"a\\b c"})]
    [InlineData("a\\\\\\\"b", new[] {"a\\\"b"})]
    [InlineData("\"a\"\"b\"", new[] {"a\"b"})]
    [InlineData("a\"\"b", new[] {"ab"})]
    [InlineData("'a b'", new[] {"'a", "b'"})]
    [InlineData("\"a b", new[] {"a b"})]
    [InlineData("a\\", new[] {"a\\"})]
    public void TestSplitStartInfoArguments(string arguments, string[] expected)
    {
        ProcessUtils.SplitStartInfoArguments(arguments).Should().Equal(expected);
        if (WindowsUtils.IsWindows)
            ProcessUtils.SplitArguments(arguments).Should().Equal(expected, because: "CommandLineToArgv follows the same rules");
    }

    [Theory]
    [InlineData("executable", "my args", "executable my args")]
    [InlineData("my executable", "my args", "\"my executable\" my args")]
    public void TestToCommandLine(string executable, string args, string commandLine)
        => new ProcessStartInfo(executable, args).ToCommandLine().Should().Be(commandLine);

    [Theory]
    [InlineData("executable", "", "executable")]
    [InlineData("executable", "my args", "executable my args")]
    [InlineData("my executable", "\"my arg\" \"\"", "\"my executable\" \"my arg\" \"\"")]
    [InlineData("/home/o'brien/app", "it's", "\"/home/o'brien/app\" \"it's\"")]
    [InlineData("executable", "C:\\temp \"a\\\\b c\"", "executable \"C:\\\\temp\" \"a\\\\\\\\b c\"")]
    [InlineData("executable", "\"cost $5 `now`\"", "executable \"cost \\$5 \\`now\\`\"")]
    [InlineData("executable", "\"quote \\\" inside\"", "executable \"quote \\\" inside\"")]
    [InlineData("executable", "a;b a&b a|b a>b", "executable \"a;b\" \"a&b\" \"a|b\" \"a>b\"")]
    [InlineData("executable", "*.txt #x ~/x $(id)", "executable \"*.txt\" \"#x\" \"~/x\" \"\\$(id)\"")]
    [InlineData("/opt/a&b/app", "", "\"/opt/a&b/app\"")]
    [InlineData("executable", "\"a\u00A0b\"", "executable \"a\u00A0b\"")]
    [InlineData("executable", "--flag=a,b:c/d.e_f@g%h+i-j", "executable --flag=a,b:c/d.e_f@g%h+i-j")]
    public void TestToCommandLinePosix(string executable, string args, string commandLine)
        => ProcessUtils.ToCommandLinePosix(new(executable, args)).Should().Be(commandLine);

    [Fact]
    public void TestFromCommandLine()
    {
        var startInfo = ProcessUtils.FromCommandLine("executable my args");
        startInfo.FileName.Should().Be("executable");
        startInfo.Arguments.Should().Be("my args");

        startInfo = ProcessUtils.FromCommandLine("\"my executable\" my args");
        startInfo.FileName.Should().Be("my executable");
        startInfo.Arguments.Should().Be("my args");

        startInfo = ProcessUtils.FromCommandLine("\"my executable\"");
        startInfo.FileName.Should().Be("my executable");
        startInfo.Arguments.Should().Be("");

        startInfo = ProcessUtils.FromCommandLine("");
        startInfo.FileName.Should().Be("");
        startInfo.Arguments.Should().Be("");
    }

    [Theory]
    [InlineData("executable", "executable", "")]
    [InlineData("executable my args", "executable", "my args")]
    [InlineData("\"my executable\"", "my executable", "")]
    [InlineData("\"my executable\" my args", "my executable", "my args")]
    [InlineData("'my executable' 'my args'", "my executable", "\"my args\"")]
    [InlineData("my\\ executable arg", "my executable", "arg")]
    [InlineData("", "", "")]
    public void TestFromCommandLinePosix(string commandLine, string fileName, string arguments)
    {
        var startInfo = ProcessUtils.FromCommandLineParts(ProcessUtils.SplitArgumentsPosix(commandLine));
        startInfo.FileName.Should().Be(fileName);
        startInfo.Arguments.Should().Be(arguments);
    }

    [Theory]
    [InlineData("/my dir/executable", "--flag \"/my dir/file\"")]
    [InlineData("/usr/bin/dotnet", "\"/my dir/app.dll\" arg")]
    [InlineData("/usr/bin/executable", "\"quote \\\" inside\"")]
    [InlineData("/home/o'brien/app", "--flag")]
    [InlineData("/usr/bin/echo", "it's two")]
    [InlineData("/usr/bin/grep", "--pattern=\\d+ file")]
    [InlineData("/usr/bin/echo", "C:\\temp\\x")]
    [InlineData("/usr/bin/echo", "\"a\\\\b c\"")]
    [InlineData("/usr/bin/echo", "\"cost \\$5 now\"")]
    [InlineData("/usr/bin/echo", "\"trailing slash\\\\\" \"\"")]
    [InlineData("/usr/bin/echo", "")]
    [InlineData("/opt/a&b/app", "\"a;b c\" *.txt")]
    [InlineData("/usr/bin/echo", "\"a b\"")]
    [InlineData("/usr/bin/echo", "\"a\\\nb\"")]
    public void TestCommandLineRoundTrip(string fileName, string arguments)
    {
        var startInfo = new ProcessStartInfo(fileName, arguments);

        var posixResult = ProcessUtils.FromCommandLineParts(ProcessUtils.SplitArgumentsPosix(ProcessUtils.ToCommandLinePosix(startInfo)));
        posixResult.FileName.Should().Be(fileName);
        posixResult.Arguments.Should().Be(arguments);

        var platformResult = ProcessUtils.FromCommandLine(startInfo.ToCommandLine());
        platformResult.FileName.Should().Be(fileName);
        platformResult.Arguments.Should().Be(arguments);
    }
}
