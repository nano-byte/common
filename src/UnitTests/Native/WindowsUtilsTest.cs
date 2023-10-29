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
        Skip.IfNot(WindowsUtils.IsWindowsVista, "Can only test NTFS symlinks on Windows Vista or newer");
    }

    [SkippableFact]
    public void TestSplitArgs()
    {
        WindowsUtils.SplitArgs("\"first part\" second \"\\\"third part\\\"\"")
                    .Should().Equal("first part", "second", "\"third part\"");
    }

    [SkippableFact]
    public void TestGetFolderPath()
    {
        WindowsUtils.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                    .Should().Be(@"C:\ProgramData");
    }

    [SkippableFact]
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
            throw new SkipException("Cannot test NTFS symlinks due to insufficient privileges");
        }

        File.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like file");
        WindowsUtils.IsSymlink(sourcePath, out string? target).Should().BeTrue();
        target.Should().Be("target");
    }

    [SkippableFact]
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
            throw new SkipException("Cannot test NTFS symlinks due to insufficient privileges");
        }

        Directory.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like directory");
        WindowsUtils.IsSymlink(sourcePath, out string? target).Should().BeTrue();
        target.Should().Be("target");
    }

    [SkippableFact]
    public void TestIsNotSymlink()
    {
        using var tempFile = new TemporaryFile("unit-tests");
        WindowsUtils.IsSymlink(tempFile).Should().BeFalse();
    }
}
