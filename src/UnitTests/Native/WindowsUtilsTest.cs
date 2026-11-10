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
        catch (NotAdminException)
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
        catch (NotAdminException)
        {
            throw new Exception($"{Xunit.v3.DynamicSkipToken.Value}Cannot test NTFS symlinks due to insufficient privileges");
        }

        Directory.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like directory");
        WindowsUtils.IsSymlink(sourcePath, out string? target).Should().BeTrue();
        target.Should().Be("target");
    }

    [Fact]
    public void TestCreateJunction()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string targetPath = Path.Combine(tempDir, "target");
        Directory.CreateDirectory(targetPath);
        File.WriteAllText(Path.Combine(targetPath, "file"), "data");
        string sourcePath = Path.Combine(tempDir, "junction");

        WindowsUtils.CreateJunction(sourcePath, "target");

        Directory.Exists(sourcePath).Should().BeTrue(because: "Junction should look like directory");
        File.ReadAllText(Path.Combine(sourcePath, "file")).Should().Be("data");
        WindowsUtils.IsJunction(sourcePath, out string? target).Should().BeTrue();
        target.Should().Be(targetPath, because: "junctions store absolute paths");
        WindowsUtils.IsSymlink(sourcePath).Should().BeFalse();
    }

    [Fact]
    public void TestCreateJunctionTrailingSeparator()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string targetPath = Path.Combine(tempDir, "target");
        Directory.CreateDirectory(targetPath);
        string sourcePath = Path.Combine(tempDir, "junction");

        WindowsUtils.CreateJunction(sourcePath + Path.DirectorySeparatorChar, "target");

        WindowsUtils.IsJunction(sourcePath, out string? target).Should().BeTrue();
        target.Should().Be(targetPath, because: "relative targets should be resolved against the directory containing the junction");
    }

    [Fact]
    public void TestCreateJunctionExisting()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        Directory.CreateDirectory(Path.Combine(tempDir, "target"));
        string sourcePath = Path.Combine(tempDir, "junction");
        Directory.CreateDirectory(sourcePath);

        Assert.Throws<IOException>(() => WindowsUtils.CreateJunction(sourcePath, "target"));
        WindowsUtils.IsJunction(sourcePath).Should().BeFalse(because: "existing directory should be left untouched");
    }

    [Fact]
    public void TestCreateJunctionMissingParent()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        Directory.CreateDirectory(Path.Combine(tempDir, "target"));

        Assert.Throws<DirectoryNotFoundException>(() => WindowsUtils.CreateJunction(Path.Combine(tempDir, "missing", "junction"), Path.Combine(tempDir, "target")));
        Directory.Exists(Path.Combine(tempDir, "missing")).Should().BeFalse(because: "should not create parent directories");
    }

    [Fact]
    public void TestCreateJunctionExtendedPath()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string targetPath = Path.Combine(tempDir, "target");
        Directory.CreateDirectory(targetPath);
        string sourcePath = Path.Combine(tempDir, "junction");

        WindowsUtils.CreateJunction(sourcePath, @"\\?\" + targetPath);

        WindowsUtils.IsJunction(sourcePath, out string? target).Should().BeTrue();
        target.Should().Be(targetPath);
    }

    [Fact]
    public void TestCreateJunctionToNetworkPath()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string sourcePath = Path.Combine(tempDir, "junction");

        Assert.Throws<IOException>(() => WindowsUtils.CreateJunction(sourcePath, @"\\server\share\dir"));
        Directory.Exists(sourcePath).Should().BeFalse();
    }

    [Fact]
    public void TestCreateJunctionToFile()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        File.WriteAllText(Path.Combine(tempDir, "target"), "data");
        string sourcePath = Path.Combine(tempDir, "junction");

        Assert.Throws<IOException>(() => WindowsUtils.CreateJunction(sourcePath, "target"));
        Directory.Exists(sourcePath).Should().BeFalse();
    }

    [Fact]
    public void TestIsNotJunction()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        WindowsUtils.IsJunction(tempDir).Should().BeFalse();
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
        catch (NotAdminException)
        {
            throw new Exception($"{Xunit.v3.DynamicSkipToken.Value}Cannot test NTFS symlinks due to insufficient privileges");
        }

        WindowsUtils.IsSymlink(sourcePath, out string? actualTarget).Should().BeTrue();
        actualTarget.Should().Be(target, because: "absolute targets should not be returned in NT path syntax (e.g., \\??\\C:\\)");
    }

    [Fact]
    public void TestIsSymlinkReturnsNormalizedAbsoluteTarget()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string sourcePath = Path.Combine(tempDir, "symlink");

        try
        {
            FileUtils.CreateSymlink(sourcePath, @"C:\Windows\System32\..");
        }
        catch (NotAdminException)
        {
            throw new Exception($"{Xunit.v3.DynamicSkipToken.Value}Cannot test NTFS symlinks due to insufficient privileges");
        }

        WindowsUtils.IsSymlink(sourcePath, out string? actualTarget).Should().BeTrue();
        actualTarget.Should().Be(@"C:\Windows", because: "should return the path the file system follows rather than the display name");
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
    public void TestIsSymlinkOrJunctionMissing()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string missing = Path.Combine(tempDir, "missing");
        string missingNested = Path.Combine(missing, "nested");

        Assert.Throws<FileNotFoundException>(() => WindowsUtils.IsSymlink(missing));
        Assert.Throws<FileNotFoundException>(() => WindowsUtils.IsJunction(missing));
        Assert.Throws<DirectoryNotFoundException>(() => WindowsUtils.IsSymlink(missingNested));
        Assert.Throws<DirectoryNotFoundException>(() => WindowsUtils.IsJunction(missingNested));
    }

    [Fact]
    public void TestIsNotSymlink()
    {
        using var tempFile = new TemporaryFile("unit-tests");
        WindowsUtils.IsSymlink(tempFile).Should().BeFalse();
    }
}
