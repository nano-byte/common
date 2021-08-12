// Copyright Bastian Eicher
// Licensed under the MIT License

using System.IO;
using FluentAssertions;
using NanoByte.Common.Storage;
using Xunit;

namespace NanoByte.Common.Native
{
    public class WindowsUtilsTest
    {
        [SkippableFact]
        public void TestCreateSymlinkFile()
        {
            Skip.IfNot(WindowsUtils.IsWindowsVista, reason: "Can only test NTFS symlinks on Windows Vista or newer");

            using var tempDir = new TemporaryDirectory("unit-tests");
            File.WriteAllText(Path.Combine(tempDir, "target"), @"data");
            string sourcePath = Path.Combine(tempDir, "symlink");

            try
            {
                FileUtils.CreateSymlink(sourcePath, "target");
            }
            catch (IOException)
            {
                Skip.IfNot(WindowsUtils.IsAdministrator, reason: "Cannot test NTFS symlinks due to insufficient privileges");
                throw;
            }

            File.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like file");
            WindowsUtils.IsSymlink(sourcePath, out string? target).Should().BeTrue();
            target.Should().Be("target");
        }

        [SkippableFact]
        public void TestCreateSymlinkDirectory()
        {
            Skip.IfNot(WindowsUtils.IsWindowsVista, reason: "Can only test NTFS symlinks on Windows Vista or newer");

            using var tempDir = new TemporaryDirectory("unit-tests");
            Directory.CreateDirectory(Path.Combine(tempDir, "target"));
            string sourcePath = Path.Combine(tempDir, "symlink");

            try
            {
                FileUtils.CreateSymlink(sourcePath, "target");
            }
            catch (IOException)
            {
                Skip.IfNot(WindowsUtils.IsAdministrator, reason: "Cannot test NTFS symlinks due to insufficient privileges");
                throw;
            }

            Directory.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like directory");
            WindowsUtils.IsSymlink(sourcePath, out string? target).Should().BeTrue();
            target.Should().Be("target");
        }

        [SkippableFact]
        public void TestIsNotSymlink()
        {
            Skip.IfNot(WindowsUtils.IsWindowsVista, reason: "Can only test NTFS symlinks on Windows Vista or newer");

            using var tempFile = new TemporaryFile("unit-tests");
            WindowsUtils.IsSymlink(tempFile).Should().BeFalse();
        }
    }
}
