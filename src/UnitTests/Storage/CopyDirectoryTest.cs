// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using FluentAssertions;
using NanoByte.Common.Native;
using Xunit;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Contains test methods for <see cref="CopyDirectory"/>.
    /// </summary>
    public class CopyDirectoryTest
    {
        /// <summary>
        /// Ensures <see cref="CopyDirectory"/> correctly copies a directories from one location to another.
        /// </summary>
        [Fact]
        public void Normal()
        {
            using var source = CreateTestDir();

            using var destination = new TemporaryDirectory("unit-tests");
            Directory.Delete(destination);

            new CopyDirectory(source, destination).Run();

            File.ReadAllBytes(Path.Combine(destination, "subdir", "file"))
                .Should().Equal(File.ReadAllBytes(Path.Combine(source, "subdir", "file")));
            File.GetLastWriteTimeUtc(Path.Combine(destination, "subdir", "file"))
                .Should().Be(new DateTime(2000, 1, 1), because: "Last-write time for copied file");
        }

        /// <summary>
        /// Ensures <see cref="CopyDirectory"/> correctly detects usage errors.
        /// </summary>
        [Fact]
        public void ErrorHandling()
        {
            Assert.Throws<ArgumentException>(() => new CopyDirectory("a", "a"));

            string temp = FileUtils.GetTempDirectory("unit-tests");
            Directory.Delete(temp);

            new CopyDirectory(temp, "a").Invoking(x => x.Run())
                                        .Should().Throw<DirectoryNotFoundException>();
        }

        /// <summary>
        /// Ensures <see cref="CopyDirectory"/> correctly rejects overwriting existing directories.
        /// </summary>
        [Fact]
        public void NoOverwrite()
        {
            using var source = CreateTestDir();
            using var destination = CreateTestDir();

            new CopyDirectory(source, destination).Invoking(x => x.Run())
                                                  .Should().Throw<IOException>("Should not overwrite existing directory.");
        }

        /// <summary>
        /// Ensures <see cref="CopyDirectory"/> correctly copies a directory on top another.
        /// </summary>
        [Fact]
        public void Overwrite()
        {
            using var source = CreateTestDir();

            using var destination = new TemporaryDirectory("unit-tests");
            string subdir2 = Path.Combine(destination, "subdir");
            Directory.CreateDirectory(subdir2);
            File.WriteAllText(Path.Combine(subdir2, "file"), @"B");
            File.SetLastWriteTimeUtc(Path.Combine(subdir2, "file"), new DateTime(2002, 1, 1));

            new CopyDirectory(source, destination) {Overwrite = true}.Run();

            File.ReadAllBytes(Path.Combine(destination, "subdir", "file"))
                .Should().Equal(File.ReadAllBytes(Path.Combine(source, "subdir", "file")));
            File.GetLastWriteTimeUtc(Path.Combine(destination, "subdir", "file")).Should().Be(
                new DateTime(2000, 1, 1),
                because: "Last-write time for copied file is invalid");
        }

        /// <summary>
        /// Ensures <see cref="CopyDirectory"/> correctly copies symlinks.
        /// </summary>
        [SkippableFact]
        public void CopySymlinks()
        {
            Skip.IfNot(UnixUtils.IsUnix, reason: "Can only test POSIX symlinks on Unixoid system");

            using var source = CreateTestDir();
            FileUtils.CreateSymlink(
                sourcePath: Path.Combine(source, "dir-symlink"),
                targetPath: "subdir");
            FileUtils.CreateSymlink(
                sourcePath: Path.Combine(source, "file-symlink"),
                targetPath: Path.Combine("subdir", "file"));

            using var destination = new TemporaryDirectory("unit-tests");

            new CopyDirectory(source, destination).Run();

            FileUtils.IsSymlink(Path.Combine(destination, "dir-symlink"), out string? symlinkTarget).Should().BeTrue();
            symlinkTarget.Should().Be("subdir");

            FileUtils.IsSymlink(Path.Combine(destination, "file-symlink"), out symlinkTarget).Should().BeTrue();
            symlinkTarget.Should().Be(Path.Combine("subdir", "file"));
        }

        /// <summary>
        /// Ensures <see cref="CopyDirectory"/> correctly copies hardlinks.
        /// </summary>
        [SkippableFact]
        public void CopyHardlinks()
        {
            using var source = CreateTestDir();
            FileUtils.CreateHardlink(
                sourcePath: Path.Combine(source, "hardlink"),
                targetPath: Path.Combine(source, "subdir", "file"));

            using var destination = new TemporaryDirectory("unit-tests");

            new CopyDirectory(source, destination).Run();

            FileUtils.AreHardlinked(
                          Path.Combine(destination, "hardlink"),
                          Path.Combine(destination, "subdir", "file"))
                     .Should().BeTrue();
        }

        /// <summary>
        /// Ensures <see cref="CopyDirectory"/> correctly follows symlinks.
        /// </summary>
        [SkippableFact]
        public void FollowSymlinks()
        {
            Skip.IfNot(UnixUtils.IsUnix, reason: "Can only test POSIX symlinks on Unixoid system");

            using var source = CreateTestDir();
            FileUtils.CreateSymlink(sourcePath: Path.Combine(source, "dir-symlink"), targetPath: "subdir");
            FileUtils.CreateSymlink(sourcePath: Path.Combine(source, "file-symlink"), targetPath: Path.Combine("subdir", "file"));

            using var destination = new TemporaryDirectory("unit-tests");

            new CopyDirectory(source, destination) {FollowSymlinks = true}.Run();

            FileUtils.IsSymlink(Path.Combine(destination, "dir-symlink"), out _).Should().BeFalse();
            File.ReadAllBytes(Path.Combine(destination, "dir-symlink", "file"))
                .Should().Equal(File.ReadAllBytes(Path.Combine(destination, "subdir", "file")));

            FileUtils.IsSymlink(Path.Combine(destination, "file-symlink"), out _).Should().BeFalse();
            File.ReadAllBytes(Path.Combine(destination, "file-symlink"))
                .Should().Equal(File.ReadAllBytes(Path.Combine(destination, "subdir", "file")));
        }

        internal static TemporaryDirectory CreateTestDir()
        {
            var tempDir = new TemporaryDirectory("unit-tests");
            string subdir1 = Path.Combine(tempDir, "subdir");
            Directory.CreateDirectory(subdir1);
            File.WriteAllText(Path.Combine(subdir1, "file"), @"A");
            File.SetLastWriteTimeUtc(Path.Combine(subdir1, "file"), new DateTime(2000, 1, 1));
            File.SetAttributes(Path.Combine(subdir1, "file"), FileAttributes.ReadOnly);
            return tempDir;
        }
    }
}
