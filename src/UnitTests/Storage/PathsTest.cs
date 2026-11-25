// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Native;

namespace NanoByte.Common.Storage;

/// <summary>
/// Contains test methods for <see cref="Paths"/>.
/// </summary>
[Collection("WorkingDir")]
public class PathsTest
{
    [Fact]
    public void TestCombine()
    {
        Paths.Combine("a", "b").Should().Be(Path.Combine("a", "b"));
        Paths.Combine("a", "b", "c").Should().Be(Path.Combine("a", "b", "c"));
    }

    [Fact]
    public void TestAbsolute()
    {
        Paths.Absolute("test.txt").Should().Be(Path.GetFullPath("test.txt"));
    }

    [Fact]
    public void TestIsAbsolute()
    {
        Paths.IsAbsolute("test.txt").Should().BeFalse();
        if (WindowsUtils.IsWindows)
            Paths.IsAbsolute(@"C:\test.txt").Should().BeTrue();
        else
            Paths.IsAbsolute("/test.txt").Should().BeTrue();
    }

    [Fact]
    public void TestParent()
    {
        Paths.Parent(Path.Combine("a", "b")).Should().Be(Paths.Absolute("a"));
    }

    [Fact]
    public void TestFileName()
    {
        Paths.FileName(Path.Combine("a", "b")).Should().Be("b");
    }

#if NETFRAMEWORK
    [Fact]
    public void TestInvalidCharacters()
    {
        char invalidChar = Path.GetInvalidPathChars()[0];
        string invalidPath = "test" + invalidChar + "test";

        new Action(() => Paths.Combine("a", invalidPath)).Should().Throw<IOException>();
        new Action(() => Paths.Absolute(invalidPath)).Should().Throw<IOException>();
        new Action(() => Paths.IsAbsolute(invalidPath)).Should().Throw<IOException>();
        new Action(() => Paths.Parent(invalidPath)).Should().Throw<IOException>();
        new Action(() => Paths.FileName(invalidPath)).Should().Throw<IOException>();
    }
#endif

    [Theory]
    [InlineData('/')]
    [InlineData('\\')]
    [InlineData(':')]
    [InlineData('*')]
    [InlineData('|')]
    [InlineData('\n')]
    [InlineData('\0')]
    public void TestIsNonPortableFileNameChar(char c)
        => Paths.IsNonPortableFileNameChar(c).Should().BeTrue();

    [Theory]
    [InlineData('a')]
    [InlineData('-')]
    [InlineData('.')]
    [InlineData(' ')]
    [InlineData('é')]
    public void TestIsValidFileNameChar(char c)
        => Paths.IsNonPortableFileNameChar(c).Should().BeFalse();

    [Theory]
    [InlineData("NUL")]
    [InlineData("con")]
    [InlineData("COM1.log")]
    [InlineData("lpt²")]
    [InlineData("AUX .txt")]
    public void TestIsReservedDeviceName(string fileName)
        => Paths.IsReservedDeviceName(fileName).Should().BeTrue();

    [Theory]
    [InlineData("console")]
    [InlineData("auxiliary.c")]
    [InlineData("COM10")]
    [InlineData("my-nul")]
    public void TestIsNotReservedDeviceName(string fileName)
        => Paths.IsReservedDeviceName(fileName).Should().BeFalse();

    [Theory]
    [InlineData("hello")]
    [InlineData("my file.txt")]
    [InlineData("café")]
    [InlineData(".hidden")]
    [InlineData("-x")]
    [InlineData("a=b;c'd")]
    [InlineData("console")]
    public void TestIsPortableFileName(string fileName)
        => Paths.IsPortableFileName(fileName).Should().BeTrue();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(".")]
    [InlineData("..")]
    [InlineData("foo.")]
    [InlineData("foo ")]
    [InlineData("a/b")]
    [InlineData(@"a\b")]
    [InlineData("a:b")]
    [InlineData("a\"b")]
    [InlineData("a\nb")]
    [InlineData("NUL")]
    [InlineData("COM1.txt")]
    public void TestIsNotPortableFileName(string? fileName)
        => Paths.IsPortableFileName(fileName).Should().BeFalse();

    [Fact]
    public void TestAreEqualAbsolutePath()
    {
        string path = WindowsUtils.IsWindows ? @"C:\some\path" : "/some/path";
        Paths.AreEqual(path, path).Should().BeTrue();
    }

    [Fact]
    public void TestAreEqualRelativePath()
        => Paths.AreEqual("abc", Path.Combine("xyz", "..", "abc")).Should().BeTrue();

    [Fact]
    public void TestAreEqualWorkingDirectory()
        => Paths.AreEqual("abc", Path.Combine(Directory.GetCurrentDirectory(), "abc")).Should().BeTrue();

    [Fact]
    public void TestAreEqualDirectorySeparator()
        => Paths.AreEqual("abc/def", Path.Combine("abc", "def")).Should().BeTrue();

    [Fact]
    public void TestAreEqualTrailingSLash()
        => Paths.AreEqual(
                         WindowsUtils.IsWindows ? @"C:\some\path" : "/some/path",
                         WindowsUtils.IsWindows ? @"C:\some\path\" : "/some/path/")
                    .Should().BeTrue();

    [Fact]
    public void TestAreEqualCaseSensitive()
    {
        Assert.SkipWhen(WindowsUtils.IsWindows || UnixUtils.IsMacOSX, "Windows and macOS use case-insensitive paths");
        Paths.AreEqual("abc", "ABC").Should().BeFalse();
    }

    [Fact]
    public void TestAreEqualCaseInsensitive()
    {
        Assert.SkipUnless(WindowsUtils.IsWindows || UnixUtils.IsMacOSX, "Only Windows and macOS use case-insensitive paths");
        Paths.AreEqual("abc", "ABC").Should().BeTrue();
    }

    [Fact]
    public void TestAreEqualFalse()
        => Paths.AreEqual("abc", "xyz").Should().BeFalse();

    [Fact]
    public void TestAreEqualUnnormalizable()
        => Paths.AreEqual("", "").Should().BeTrue();

    [Fact]
    public void TestAreEqualUnnormalizableFalse()
        => Paths.AreEqual("", "abc").Should().BeFalse();

    [Fact]
    public void TestToNative()
        => Paths.ToNative("a/b")
                .Should().Be(Path.Combine("a", "b"));

    [Fact]
    public void TestToUnix()
        => Paths.ToUnix(Path.Combine("a", "b"))
                .Should().Be("a/b");

    [Fact]
    public void TestIsBreakout()
    {
        Paths.IsBreakout(WindowsUtils.IsWindows ? @"C:\test" : "/test").Should().BeTrue(because: "Should detect absolute paths");

        foreach (string path in new[] {"..", "/..", "../", "/../", "a/../b", "../a", "a/.."})
            Paths.IsBreakout(path).Should().BeTrue(because: "Should detect parent directory references");

        foreach (string path in new[] {"..a", "a/..a", "a..", "a/a.."})
            Paths.IsBreakout(path).Should().BeFalse(because: "Should not trip on '..' as a part of file/directory names");

        Paths.IsBreakout("").Should().BeFalse();
    }

    [Fact]
    public void TestRelativeTo()
    {
        if (WindowsUtils.IsWindows)
        {
            new FileInfo(@"C:\test\a\b").RelativeTo(new DirectoryInfo(@"C:\test")).Should().Be(@"a\b");
            new FileInfo(@"C:\test\a\b").RelativeTo(new FileInfo(@"C:\file")).Should().Be(@"test\a\b");
            new DirectoryInfo(@"C:\test1").RelativeTo(new DirectoryInfo(@"C:\test2")).Should().Be(@"..\test1");
        }
        else
        {
            new FileInfo("/test/a/b").RelativeTo(new DirectoryInfo("/test")).Should().Be("a/b");
            new FileInfo("/test/a/b").RelativeTo(new FileInfo("/file")).Should().Be("test/a/b");
            new DirectoryInfo("/test1").RelativeTo(new DirectoryInfo("/test2")).Should().Be("../test1");
        }
    }

    [Fact]
    public void TestGetFilesAbsolute()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        File.WriteAllText(Path.Combine(tempDir, "a.txt"), @"a");
        File.WriteAllText(Path.Combine(tempDir, "b.txt"), @"b");
        File.WriteAllText(Path.Combine(tempDir, "c.inf"), @"c");
        File.WriteAllText(Path.Combine(tempDir, "d.nfo"), @"d");

        string subdirPath = Path.Combine(tempDir, "dir");
        Directory.CreateDirectory(subdirPath);
        File.WriteAllText(Path.Combine(subdirPath, "1.txt"), @"1");
        File.WriteAllText(Path.Combine(subdirPath, "2.inf"), @"a");

        Paths.ResolveFiles([
            Path.Combine(tempDir, "*.txt"), // Wildcard
            Path.Combine(tempDir, "d.nfo"), // Specific file
            subdirPath // Directory with implicit default wildcard
        ], "*.txt").Select(x => x.FullName).Should().BeEquivalentTo(
            Path.Combine(tempDir, "a.txt"),
            Path.Combine(tempDir, "b.txt"),
            Path.Combine(tempDir, "d.nfo"),
            Path.Combine(subdirPath, "1.txt"));
    }

    [Fact]
    public void TestGetFilesRelative()
    {
        using var tempDir = new TemporaryWorkingDirectory("unit-tests");
        File.WriteAllText("a.txt", @"a");
        File.WriteAllText("b.txt", @"b");
        File.WriteAllText("c.inf", @"c");
        File.WriteAllText("d.nfo", @"d");

        const string subdirPath = "dir";
        Directory.CreateDirectory(subdirPath);
        File.WriteAllText(Path.Combine(subdirPath, "1.txt"), @"1");
        File.WriteAllText(Path.Combine(subdirPath, "2.inf"), @"a");

        Paths.ResolveFiles([
            "*.txt", // Wildcard
            "d.nfo", // Specific file
            subdirPath // Directory with implicit default wildcard
        ], "*.txt").Select(x => x.FullName).Should().BeEquivalentTo(
            Path.Combine(Environment.CurrentDirectory, "a.txt"),
            Path.Combine(Environment.CurrentDirectory, "b.txt"),
            Path.Combine(Environment.CurrentDirectory, "d.nfo"),
            Path.Combine(Environment.CurrentDirectory, subdirPath, "1.txt"));
    }
}
