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
            Skip.IfNot(WindowsUtils.IsAdministrator, reason: "Can only test NTFS symlinks with Administrator privileges");

            using var tempDir = new TemporaryDirectory("unit-tests");
            File.WriteAllText(Path.Combine(tempDir, "target"), @"data");
            string sourcePath = Path.Combine(tempDir, "symlink");
            FileUtils.CreateSymlink(sourcePath, "target");

            File.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like file");
            File.ReadAllText(sourcePath).Should().Be("data", because: "Symlinked file contents should be equal");
        }

        [SkippableFact]
        public void TestCreateSymlinkDirectory()
        {
            Skip.IfNot(WindowsUtils.IsWindowsVista, reason: "Can only test NTFS symlinks on Windows Vista or newer");
            Skip.IfNot(WindowsUtils.IsAdministrator, reason: "Can only test NTFS symlinks with Administrator privileges");

            using var tempDir = new TemporaryDirectory("unit-tests");
            Directory.CreateDirectory(Path.Combine(tempDir, "target"));
            string sourcePath = Path.Combine(tempDir, "symlink");
            FileUtils.CreateSymlink(sourcePath, "target");

            Directory.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like directory");
        }
    }
}
