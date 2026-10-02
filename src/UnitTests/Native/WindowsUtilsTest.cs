// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.Versioning;
using NanoByte.Common.Storage;

namespace NanoByte.Common.Native;

[SupportedOSPlatform("windows6.0")]
public class WindowsUtilsTest
{
    public WindowsUtilsTest()
    {
        Assert.SkipUnless(WindowsUtils.IsWindowsVista, "Can only test NTFS symlinks on Windows Vista or newer");
    }

    [Fact]
    public void TestSplitArgs()
    {
        WindowsUtils.SplitArgs("\"first part\" second \"\\\"third part\\\"\"")
                    .Should().Equal("first part", "second", "\"third part\"");
    }

    [Fact]
    public void TestGetFolderPath()
    {
        WindowsUtils.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                    .Should().Be(@"C:\ProgramData");
    }

    [Fact]
    public void TestCreateSymlinkFile()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string sourcePath = Path.Combine(tempDir, "symlink");

        try
        {
            FileUtils.CreateSymlink(sourcePath, "target");
        }
        catch (IOException) when (!WindowsUtils.IsAdministrator)
        {
            throw new Exception($"{Xunit.v3.DynamicSkipToken.Value}Cannot test NTFS symlinks due to insufficient privileges");
        }

        File.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like file");
        WindowsUtils.IsSymlink(sourcePath, out string? target).Should().BeTrue();
        target.Should().Be("target");
    }

    [Fact]
    public void TestCreateSymlinkDirectory()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        Directory.CreateDirectory(Path.Combine(tempDir, "target"));
        string sourcePath = Path.Combine(tempDir, "symlink");

        try
        {
            FileUtils.CreateSymlink(sourcePath, "target");
        }
        catch (IOException) when (!WindowsUtils.IsAdministrator)
        {
            throw new Exception($"{Xunit.v3.DynamicSkipToken.Value}Cannot test NTFS symlinks due to insufficient privileges");
        }

        Directory.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like directory");
        WindowsUtils.IsSymlink(sourcePath, out string? target).Should().BeTrue();
        target.Should().Be("target");
    }

    [Theory]
    [InlineData(@"C:\Windows")]
    [InlineData(@"\\server\share\dir")]
    [InlineData(@"relative\path")]
    public void TestIsSymlinkReturnsTargetAsSpecified(string target)
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string sourcePath = Path.Combine(tempDir, "symlink");

        try
        {
            FileUtils.CreateSymlink(sourcePath, target);
        }
        catch (IOException) when (!WindowsUtils.IsAdministrator)
        {
            throw new Exception($"{Xunit.v3.DynamicSkipToken.Value}Cannot test NTFS symlinks due to insufficient privileges");
        }

        WindowsUtils.IsSymlink(sourcePath, out string? actualTarget).Should().BeTrue();
        actualTarget.Should().Be(target, because: "absolute targets should not be returned in NT path syntax (e.g., \\??\\C:\\)");
    }

    [Fact]
    public void TestTryGetDirectoryEntryExisting()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string path = Path.Combine(tempDir, "MixedCase.txt");
        File.WriteAllText(path, "test");

        var entry = WindowsUtils.TryGetDirectoryEntry(path.ToUpperInvariant());
        entry.Should().NotBeNull();
        entry.Value.Name.Should().Be("MixedCase.txt");
        entry.Value.Attributes.Should().NotHaveFlag(FileAttributes.Directory);
    }

    [Fact]
    public void TestTryGetDirectoryEntryDirectory()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        WindowsUtils.TryGetDirectoryEntry(tempDir)!.Value.Attributes.Should().HaveFlag(FileAttributes.Directory);
    }

    [Fact]
    public void TestTryGetDirectoryEntryMissing()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        WindowsUtils.TryGetDirectoryEntry(Path.Combine(tempDir, "missing")).Should().BeNull();
        WindowsUtils.TryGetDirectoryEntry(Path.Combine(tempDir, "missing", "nested")).Should().BeNull();
    }

    [Fact]
    public void TestIsNotSymlink()
    {
        using var tempFile = new TemporaryFile("unit-tests");
        WindowsUtils.IsSymlink(tempFile).Should().BeFalse();
    }
}
