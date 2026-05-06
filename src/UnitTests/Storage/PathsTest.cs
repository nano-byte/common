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
