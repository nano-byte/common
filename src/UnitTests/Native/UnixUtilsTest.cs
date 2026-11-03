// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.Versioning;
using NanoByte.Common.Storage;

namespace NanoByte.Common.Native;

[SupportedOSPlatform("linux"), SupportedOSPlatform("freebsd"), SupportedOSPlatform("macos")]
public class UnixUtilsTest
{
    public UnixUtilsTest()
    {
        Assert.SkipUnless(UnixUtils.IsUnix, reason: "Can only test POSIX APIs on Unixoid system");
    }

    [Fact]
    public void TestCreateSymlinkFile()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        File.WriteAllText(Path.Combine(tempDir, "target"), @"data");
        string sourcePath = Path.Combine(tempDir, "symlink");
        UnixUtils.CreateSymlink(sourcePath, "target");

        File.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like file");
        File.ReadAllText(sourcePath).Should().Be("data", because: "Symlinked file contents should be equal");

        UnixUtils.IsSymlink(sourcePath, out string? target).Should().BeTrue(because: "Should detect symlink as such");
        "target".Should().Be(target, because: "Should retrieve relative link target");
        UnixUtils.IsRegularFile(sourcePath).Should().BeFalse(because: "Should not detect symlink as regular file");
    }

    [Fact]
    public void TestCreateSymlinkDirectory()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        Directory.CreateDirectory(Path.Combine(tempDir, "target"));
        string sourcePath = Path.Combine(tempDir, "symlink");
        UnixUtils.CreateSymlink(sourcePath, "target");

        Directory.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like directory");

        UnixUtils.IsSymlink(sourcePath, out string? contents).Should().BeTrue(because: "Should detect symlink as such");
        "target".Should().Be(contents, because: "Should retrieve relative link target");
    }

    [Fact]
    public void TestIsNotSymlink()
    {
        using var tempFile = new TemporaryFile("unit-tests");
        UnixUtils.IsSymlink(tempFile).Should().BeFalse();
    }

    [Fact]
    public void TestSetExecutable()
    {
        using var tempFile = new TemporaryFile("unit-tests");
        UnixUtils.IsExecutable(tempFile).Should().BeFalse(because: "File should not be executable yet");

        UnixUtils.SetExecutable(tempFile, true);
        UnixUtils.IsExecutable(tempFile).Should().BeTrue(because: "File should now be executable");
        UnixUtils.IsRegularFile(tempFile).Should().BeTrue(because: "File should still be considered a regular file");

        UnixUtils.SetExecutable(tempFile, false);
        UnixUtils.IsExecutable(tempFile).Should().BeFalse(because: "File should no longer be executable");
    }

    [Fact]
    public void TestIsNotExecutable()
    {
        using var tempFile = new TemporaryFile("unit-tests");
        UnixUtils.IsExecutable(tempFile).Should().BeFalse();
    }

    [Fact]
    public void TestUname()
    {
        UnixUtils.OSName.Should().NotBeNullOrEmpty().And.NotBe("unknown");
        UnixUtils.CpuType.Should().NotBeNullOrEmpty().And.NotBe("unknown");
    }

    [Fact]
    public void TestCreateHardlink()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string targetPath = Path.Combine(tempDir, "target");
        File.WriteAllText(targetPath, @"data");
        string sourcePath = Path.Combine(tempDir, "hardlink");
        UnixUtils.CreateHardlink(sourcePath, targetPath);

        File.ReadAllText(sourcePath).Should().Be("data", because: "Hardlinked file contents should be equal");
        UnixUtils.GetInode(sourcePath).Should().Be(UnixUtils.GetInode(targetPath), because: "Hardlinks should share an inode");
        UnixUtils.IsRegularFile(sourcePath).Should().BeTrue(because: "Hardlinks are regular files");
    }

    [Fact]
    public void TestGetInodeSymlink()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        File.WriteAllText(Path.Combine(tempDir, "target"), @"data");
        string sourcePath = Path.Combine(tempDir, "symlink");
        UnixUtils.CreateSymlink(sourcePath, "target");

        UnixUtils.GetInode(sourcePath).Should().NotBe(UnixUtils.GetInode(Path.Combine(tempDir, "target")), because: "Should not follow symlinks");
    }

    [Fact]
    public void TestIsRegularFileDirectory()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        UnixUtils.IsRegularFile(tempDir).Should().BeFalse();
    }

    [Fact]
    public void TestNonExistingFile()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string path = Path.Combine(tempDir, "missing");

        Assert.Throws<FileNotFoundException>(() => UnixUtils.IsRegularFile(path));
        Assert.Throws<FileNotFoundException>(() => UnixUtils.GetInode(path));
        Assert.Throws<FileNotFoundException>(() => UnixUtils.SetExecutable(path, true));
    }

    [Fact]
    public void TestRename()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string source = Path.Combine(tempDir, "source"), destination = Path.Combine(tempDir, "destination");
        File.WriteAllText(source, @"new");
        File.WriteAllText(destination, @"old");

        UnixUtils.Rename(source, destination);

        File.Exists(source).Should().BeFalse();
        File.ReadAllText(destination).Should().Be("new");
    }

#if NET
    [Fact]
    public void TestMakeReadOnlyWritable()
    {
        using var tempFile = new TemporaryFile("unit-tests");

        UnixUtils.MakeReadOnly(tempFile);
        File.GetUnixFileMode(tempFile).Should().NotHaveFlag(UnixFileMode.UserWrite);

        UnixUtils.MakeWritable(tempFile);
        File.GetUnixFileMode(tempFile).Should().HaveFlag(UnixFileMode.UserWrite);
    }
#endif

    [Fact]
    public void TestXattr()
    {
        using var tempFile = new TemporaryFile("unit-tests");
        try
        {
            UnixUtils.SetXattr(tempFile, "user.test", [1, 2, 3]);
        }
        catch (Exception ex) when (ex is IOException or PlatformNotSupportedException)
        {
            Assert.Skip("Extended file attributes not supported: " + ex.Message);
        }

        UnixUtils.GetXattr(tempFile, "user.test").Should().Equal(1, 2, 3);
        UnixUtils.GetXattr(tempFile, "user.missing").Should().BeNull();
    }

#if NET
    [Fact]
    public void TestGetFstabFileSystem()
    {
        const string fstab =
            "# <file system> <mount point> <type> <options> <dump> <pass>\n" +
            "UUID=1234   /           ext4    defaults    0 1\n" +
            "\n" +
            "/dev/sdb1\t/mnt/data\tntfs-3g\tdefaults\t0 0\n" +
            "/dev/sdc1   /mnt/my\\040disk exfat defaults 0 0\n";

        UnixUtils.GetFstabFileSystem(new StringReader(fstab), "/").Should().Be("ext4");
        UnixUtils.GetFstabFileSystem(new StringReader(fstab), "/mnt/data").Should().Be("ntfs-3g");
        UnixUtils.GetFstabFileSystem(new StringReader(fstab), "/mnt/my disk").Should().Be("exfat");
        UnixUtils.GetFstabFileSystem(new StringReader(fstab), "/mnt/other").Should().BeNull();
    }
#endif
}