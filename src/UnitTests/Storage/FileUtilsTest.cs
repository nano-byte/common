// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Moq;
using NanoByte.Common.Native;
using Xunit;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Contains test methods for <see cref="FileUtils"/>.
    /// </summary>
    public class FileUtilsTest
    {
        #region Paths
        [Fact]
        public void TestUnifySlashes()
            => FileUtils.UnifySlashes("a/b").Should().Be("a" + Path.DirectorySeparatorChar + "b");

        [Fact]
        public void TestIsBreakoutPath()
        {
            FileUtils.IsBreakoutPath(WindowsUtils.IsWindows ? @"C:\test" : "/test").Should().BeTrue(because: "Should detect absolute paths");

            foreach (string path in new[] {"..", "/..", "../", "/../", "a/../b", "../a", "a/.."})
                FileUtils.IsBreakoutPath(path).Should().BeTrue(because: "Should detect parent directory references");

            foreach (string path in new[] {"..a", "a/..a", "a..", "a/a.."})
                FileUtils.IsBreakoutPath(path).Should().BeFalse(because: "Should not trip on '..' as a part of file/directory names");

            FileUtils.IsBreakoutPath("").Should().BeFalse();
        }

        [Fact]
        public void TestRelativeTo()
        {
            if (WindowsUtils.IsWindows)
            {
                new FileInfo(@"C:\test\a\b").RelativeTo(new DirectoryInfo(@"C:\test")).Should().Be("a/b");
                new FileInfo(@"C:\test\a\b").RelativeTo(new FileInfo(@"C:\file")).Should().Be("test/a/b");
                new DirectoryInfo(@"C:\test1").RelativeTo(new DirectoryInfo(@"C:\test2")).Should().Be("../test1");
            }
            else
            {
                new FileInfo("/test/a/b").RelativeTo(new DirectoryInfo("/test")).Should().Be("a/b");
                new FileInfo("/test/a/b").RelativeTo(new FileInfo("/file")).Should().Be("test/a/b");
                new DirectoryInfo("/test1").RelativeTo(new DirectoryInfo("/test2")).Should().Be("../test1");
            }
        }
        #endregion

        #region Time
        /// <summary>
        /// Ensures <see cref="FileUtils.ToUnixTime"/> correctly converts a <see cref="DateTime"/> value to a Unix epoch value.
        /// </summary>
        [Fact]
        public void TestToUnixTime()
            => new DateTime(2004, 09, 16, 0, 0, 0, DateTimeKind.Utc)
              .ToUnixTime()
              .Should().Be(12677 /*days*/ * 86400 /*seconds*/);

        /// <summary>
        /// Ensures <see cref="FileUtils.FromUnixTime"/> correctly converts a Unix epoch value to a <see cref="DateTime"/> value.
        /// </summary>
        [Fact]
        public void TestFromUnixTime()
            => FileUtils.FromUnixTime(12677 /*days*/ * 86400 /*seconds*/)
                        .Should().Be(new DateTime(2004, 09, 16));
        #endregion

        #region Exists
        /// <summary>
        /// Ensures <see cref="FileUtils.ExistsCaseSensitive"/> correctly detects case mismatches.
        /// </summary>
        [Fact]
        public void TestExistsCaseSensitive()
        {
            using var tempDir = new TemporaryDirectory("unit-tests");
            File.WriteAllText(Path.Combine(tempDir, "test"), "");
            FileUtils.ExistsCaseSensitive(Path.Combine(tempDir, "test")).Should().BeTrue();
            FileUtils.ExistsCaseSensitive(Path.Combine(tempDir, "Test")).Should().BeFalse();
        }
        #endregion

        #region Touch
        [Fact]
        public void TestTouchNew()
        {
            using var tempDir = new TemporaryDirectory("unit-tests");
            string testFile = Path.Combine(tempDir, "test");
            FileUtils.Touch(testFile);
            File.Exists(testFile).Should().BeTrue();
            File.GetLastWriteTimeUtc(testFile).Should().BeOnOrAfter(DateTime.UtcNow - TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void TestTouchExisting()
        {
            using var tempFile = new TemporaryFile("unit-tests");
            File.SetLastWriteTimeUtc(tempFile, new DateTime(2000, 1, 1));
            FileUtils.Touch(tempFile);
            File.GetLastWriteTimeUtc(tempFile).Should().BeOnOrAfter(DateTime.UtcNow - TimeSpan.FromSeconds(2));
        }
        #endregion

        #region Temp
        /// <summary>
        /// Creates a temporary file using <see cref="FileUtils.GetTempFile"/>, ensures it is empty and deletes it again.
        /// </summary>
        [Fact]
        public void TestGetTempFile()
        {
            string path = FileUtils.GetTempFile("unit-tests");
            path.Should().NotBeNullOrEmpty();
            File.Exists(path).Should().BeTrue();
            File.ReadAllText(path).Should().Be("");
            File.Delete(path);
        }

        /// <summary>
        /// Creates a temporary directory using <see cref="FileUtils.GetTempDirectory"/>, ensures it is empty and deletes it again.
        /// </summary>
        [Fact]
        public void TestGetTempDirectory()
        {
            string path = FileUtils.GetTempDirectory("unit-tests");
            path.Should().NotBeNullOrEmpty();
            Directory.Exists(path).Should().BeTrue();
            Directory.GetFileSystemEntries(path).Should().BeEmpty();
            Directory.Delete(path);
        }
        #endregion

        #region Replace
        /// <summary>
        /// Ensures <see cref="FileUtils.Replace"/> correctly replaces the content of one file with that of another.
        /// </summary>
        [Fact]
        public void TestReplace()
        {
            using var tempDir = new TemporaryDirectory("unit-tests");
            string sourcePath = Path.Combine(tempDir, "source");
            string targetPath = Path.Combine(tempDir, "target");

            File.WriteAllText(sourcePath, @"source");
            File.WriteAllText(targetPath, @"target");
            FileUtils.Replace(sourcePath, targetPath);
            File.ReadAllText(targetPath).Should().Be("source");
        }

        /// <summary>
        /// Ensures <see cref="FileUtils.Replace"/> correctly handles a missing destination file (simply move).
        /// </summary>
        [Fact]
        public void TestReplaceMissing()
        {
            using var tempDir = new TemporaryDirectory("unit-tests");
            string sourcePath = Path.Combine(tempDir, "source");
            string targetPath = Path.Combine(tempDir, "target");

            File.WriteAllText(sourcePath, @"source");
            FileUtils.Replace(sourcePath, targetPath);
            File.ReadAllText(targetPath).Should().Be("source");
        }
        #endregion

        #region Read
        [Fact]
        public void TestReadFirstline()
        {
            using var tempFile = new TemporaryFile("unit-tests");
            File.WriteAllText(tempFile, "line1\nline2");
            new FileInfo(tempFile).ReadFirstLine(Encoding.ASCII).Should().Be("line1");
        }
        #endregion

        #region Directory
        // Interfaces used for mocking delegates
        // ReSharper disable once MemberCanBePrivate.Global
        public interface IActionSimulator<in T>
        {
            void Execute(T obj);
        }

        [Fact]
        public void TestWalk()
        {
            using var tempDir = new TemporaryDirectory("unit-tests");
            string subDirPath = Path.Combine(tempDir, "subdir");
            Directory.CreateDirectory(subDirPath);
            string filePath = Path.Combine(subDirPath, "file");
            File.WriteAllText(filePath, "");
            string symlinkPath = Path.Combine(tempDir, "symlink");
            if (UnixUtils.IsUnix) UnixUtils.CreateSymlink(symlinkPath, ".");
            else Directory.CreateDirectory(symlinkPath);

            // Set up delegate mocks
            var dirCallbackMock = new Mock<IActionSimulator<string>>(MockBehavior.Strict);
            // ReSharper disable once AccessToDisposedClosure
            dirCallbackMock.Setup(x => x.Execute(tempDir)).Verifiable();
            dirCallbackMock.Setup(x => x.Execute(subDirPath)).Verifiable();
            dirCallbackMock.Setup(x => x.Execute(symlinkPath)).Verifiable();
            var fileCallbackMock = new Mock<IActionSimulator<string>>(MockBehavior.Strict);
            fileCallbackMock.Setup(x => x.Execute(filePath)).Verifiable();

            new DirectoryInfo(tempDir).Walk(
                dir => dirCallbackMock.Object.Execute(dir.FullName),
                file => fileCallbackMock.Object.Execute(file.FullName));

            dirCallbackMock.Verify();
            fileCallbackMock.Verify();
        }

        [Fact]
        public void TestWalkThroughPrefix()
        {
            using var tempDir = new TemporaryDirectory("unit-tests");
            string dotFile = Path.Combine(tempDir, ".something");
            string prefix1 = Path.Combine(tempDir, "prefix1");
            string prefix2 = Path.Combine(prefix1, "prefix2");
            string sub = Path.Combine(prefix2, "sub");
            string file = Path.Combine(prefix2, "file");

            FileUtils.Touch(dotFile);
            Directory.CreateDirectory(prefix1);
            Directory.CreateDirectory(prefix2);
            Directory.CreateDirectory(sub);
            FileUtils.Touch(file);

            new DirectoryInfo(tempDir).WalkThroughPrefix().FullName.Should().Be(prefix2);
        }

        [Fact]
        public void TestGetFilesRecursive()
        {
            using var tempDir = new TemporaryDirectory("unit-tests");
            string file1 = Path.Combine(tempDir, "file1");
            string sub = Path.Combine(tempDir, "sub");
            string file2 = Path.Combine(sub, "file2");

            FileUtils.Touch(file1);
            Directory.CreateDirectory(sub);
            FileUtils.Touch(file2);

            FileUtils.GetFilesRecursive(tempDir).Should().Equal(file1, file2);
        }
        #endregion

        #region Write protection
        [SkippableFact]
        public void TestWriteProtection()
        {
            Skip.If(WindowsUtils.IsWindowsVista && !WindowsUtils.IsAdministrator, "Must be Admin to create symlinks on Windows");

            using var tempDir = new TemporaryDirectory("unit-tests");
            File.WriteAllText(Path.Combine(tempDir, "file"), @"contents");
            FileUtils.CreateSymlink(Path.Combine(tempDir, "symlink"), targetPath: "file");

            FileUtils.EnableWriteProtection(tempDir);
            try
            {
                Assert.Throws<UnauthorizedAccessException>(() => File.Delete(Path.Combine(tempDir, "file")));
            }
            finally
            {
                FileUtils.DisableWriteProtection(tempDir);
            }
        }
        #endregion

        #region Links
        [SkippableFact]
        public void TestCreateSymlinkPosixFile()
        {
            Skip.IfNot(UnixUtils.IsUnix, reason: "Can only test POSIX symlinks on Unixoid system");

            using var tempDir = new TemporaryDirectory("unit-tests");
            File.WriteAllText(Path.Combine(tempDir, "target"), @"data");
            string sourcePath = Path.Combine(tempDir, "symlink");
            FileUtils.CreateSymlink(sourcePath, "target");

            File.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like file");
            File.ReadAllText(sourcePath).Should().Be("data", because: "Symlinked file contents should be equal");

            FileUtils.IsSymlink(sourcePath, out string target).Should().BeTrue(because: "Should detect symlink as such");
            "target".Should().Be(target, because: "Should retrieve relative link target");
            FileUtils.IsRegularFile(sourcePath).Should().BeFalse(because: "Should not detect symlink as regular file");
        }

        [SkippableFact]
        public void TestCreateSymlinkPosixDirectory()
        {
            Skip.IfNot(UnixUtils.IsUnix, reason: "Can only test POSIX symlinks on Unixoid system");

            using var tempDir = new TemporaryDirectory("unit-tests");
            Directory.CreateDirectory(Path.Combine(tempDir, "target"));
            string sourcePath = Path.Combine(tempDir, "symlink");
            FileUtils.CreateSymlink(sourcePath, "target");

            Directory.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like directory");

            FileUtils.IsSymlink(sourcePath, out string contents).Should().BeTrue(because: "Should detect symlink as such");
            "target".Should().Be(contents, because: "Should retrieve relative link target");
        }

        [SkippableFact]
        public void TestCreateSymlinkNtfsFile()
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
        public void TestCreateSymlinkNtfsDirectory()
        {
            Skip.IfNot(WindowsUtils.IsWindowsVista, reason: "Can only test NTFS symlinks on Windows Vista or newer");
            Skip.IfNot(WindowsUtils.IsAdministrator, reason: "Can only test NTFS symlinks with Administrator privileges");

            using var tempDir = new TemporaryDirectory("unit-tests");
            Directory.CreateDirectory(Path.Combine(tempDir, "target"));
            string sourcePath = Path.Combine(tempDir, "symlink");
            FileUtils.CreateSymlink(sourcePath, "target");

            Directory.Exists(sourcePath).Should().BeTrue(because: "Symlink should look like directory");
        }

        [Fact]
        public void TestCreateHardlink()
        {
            using var tempDir = new TemporaryDirectory("unit-tests");
            string sourcePath = Path.Combine(tempDir, "hardlink");
            string destinationPath = Path.Combine(tempDir, "target");
            string copyPath = Path.Combine(tempDir, "copy");

            File.WriteAllText(destinationPath, @"data");
            FileUtils.CreateHardlink(sourcePath, destinationPath);
            File.Exists(sourcePath).Should().BeTrue(because: "Hardlink should look like regular file");
            File.ReadAllText(sourcePath).Should().Be("data", because: "Hardlinked file contents should be equal");
            FileUtils.AreHardlinked(sourcePath, destinationPath).Should().BeTrue();

            File.Copy(sourcePath, copyPath);
            FileUtils.AreHardlinked(sourcePath, copyPath).Should().BeFalse();
        }
        #endregion

        #region Unix
        [Fact]
        public void TestIsRegularFile()
        {
            using var tempFile = new TemporaryFile("unit-tests");
            FileUtils.IsRegularFile(tempFile).Should().BeTrue(because: "Regular file should be detected as such");
        }

        [Fact]
        public void TestIsSymlink()
        {
            using var tempFile = new TemporaryFile("unit-tests");
            FileUtils.IsSymlink(tempFile, out string contents).Should().BeFalse(because: "File was incorrectly identified as symlink");
            contents.Should().BeNull();
        }

        [Fact]
        public void TestIsExecutable()
        {
            using var tempFile = new TemporaryFile("unit-tests");
            FileUtils.IsExecutable(tempFile).Should().BeFalse(because: "File was incorrectly identified as executable");
        }

        [SkippableFact]
        public void TestSetExecutable()
        {
            Skip.IfNot(UnixUtils.IsUnix, reason: "Can only test executable bits on Unixoid system");

            using var tempFile = new TemporaryFile("unit-tests");
            FileUtils.IsExecutable(tempFile).Should().BeFalse(because: "File should not be executable yet");

            FileUtils.SetExecutable(tempFile, true);
            FileUtils.IsExecutable(tempFile).Should().BeTrue(because: "File should now be executable");
            FileUtils.IsRegularFile(tempFile).Should().BeTrue(because: "File should still be considered a regular file");

            FileUtils.SetExecutable(tempFile, false);
            FileUtils.IsExecutable(tempFile).Should().BeFalse(because: "File should no longer be executable");
        }

        [Fact]
        public void TestIsUnixFS()
        {
            using var tempDir = new TemporaryDirectory("unit-tests");
            if (UnixUtils.IsUnix) FileUtils.IsUnixFS(tempDir).Should().BeTrue(because: "Temp dir should be on Unixoid filesystem on Unixoid OS");
            else FileUtils.IsUnixFS(tempDir).Should().BeFalse(because: "No directory should be Unixoid on a non-Unixoid OS");
        }
        #endregion

        #region Extended metadata
        [Fact]
        public void TestWriteReadExtendedMetadata()
        {
            var data = new byte[] {0, 1, 2, 3, 4, 5, 6, 7};
            using var tempFile = new TemporaryFile("unit-tests");
            FileUtils.ReadExtendedMetadata(tempFile, "test-stream").Should().BeNull();

            FileUtils.WriteExtendedMetadata(tempFile, "test-stream", data);
            FileUtils.ReadExtendedMetadata(tempFile, "test-stream").Should().Equal(data);
        }

        [Fact]
        public void TestReadExtendedMetadataExceptionOnMissingBaseFile()
        {
            using var tempDir = new TemporaryDirectory("unit-tests");
            Assert.Throws<FileNotFoundException>(() => FileUtils.ReadExtendedMetadata(Path.Combine(tempDir, "invalid"), "test-stream"));
        }
        #endregion
    }
}
