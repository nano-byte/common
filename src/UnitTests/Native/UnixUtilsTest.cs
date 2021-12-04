// Copyright Bastian Eicher
// Licensed under the MIT License

using System.IO;
using FluentAssertions;
using NanoByte.Common.Storage;
using Xunit;

namespace NanoByte.Common.Native
{
    public class UnixUtilsTest
    {
        public UnixUtilsTest()
        {
            Skip.IfNot(UnixUtils.IsUnix, reason: "Can only test POSIX APIs on Unixoid system");
        }

        [SkippableFact]
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

        [SkippableFact]
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

        [SkippableFact]
        public void TestIsNotSymlink()
        {
            using var tempFile = new TemporaryFile("unit-tests");
            UnixUtils.IsSymlink(tempFile).Should().BeFalse();
        }

        [SkippableFact]
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

        [SkippableFact]
        public void TestIsNotExecutable()
        {
            using var tempFile = new TemporaryFile("unit-tests");
            UnixUtils.IsExecutable(tempFile).Should().BeFalse();
        }
    }
}
