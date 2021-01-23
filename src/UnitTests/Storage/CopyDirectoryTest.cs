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
            string temp1 = CreateCopyTestTempDir();
            string temp2 = FileUtils.GetTempDirectory("unit-tests");
            Directory.Delete(temp2);

            try
            {
                new CopyDirectory(temp1, temp2).Run();
                File.ReadAllBytes(Path.Combine(temp2, "subdir", "file"))
                    .Should().Equal(File.ReadAllBytes(Path.Combine(temp1, "subdir", "file")));
                Directory.GetLastWriteTimeUtc(Path.Combine(temp2, "subdir"))
                         .Should().Be(new DateTime(2000, 1, 1), because: "Last-write time for copied directory");
                File.GetLastWriteTimeUtc(Path.Combine(temp2, "subdir", "file"))
                    .Should().Be(new DateTime(2000, 1, 1), because: "Last-write time for copied file");

                new CopyDirectory(temp1, temp2).Invoking(x => x.Run())
                                               .Should().Throw<IOException>();
            }
            finally
            {
                File.SetAttributes(Path.Combine(temp1, "subdir", "file"), FileAttributes.Normal);
                Directory.Delete(temp1, recursive: true);
                File.SetAttributes(Path.Combine(temp2, "subdir", "file"), FileAttributes.Normal);
                Directory.Delete(temp2, recursive: true);
            }
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
        /// Ensures <see cref="CopyDirectory"/> correctly copies a directories from one location to another without setting directory timestamps.
        /// </summary>
        [Fact]
        public void NoDirTimestamp()
        {
            string temp1 = CreateCopyTestTempDir();
            string temp2 = FileUtils.GetTempDirectory("unit-tests");
            Directory.Delete(temp2);

            try
            {
                new CopyDirectory(temp1, temp2, preserveDirectoryTimestamps: false).Run();
                File.ReadAllBytes(Path.Combine(temp2, "subdir", "file"))
                    .Should().Equal(File.ReadAllBytes(Path.Combine(temp1, "subdir", "file")));
                Directory.GetLastWriteTimeUtc(Path.Combine(temp2, "subdir"))
                         .Should().NotBe(new DateTime(2000, 1, 1), because: "Last-write time for copied directory is invalid");
                File.GetLastWriteTimeUtc(Path.Combine(temp2, "subdir", "file")).Should().Be(
                    new DateTime(2000, 1, 1),
                    because: "Last-write time for copied file is invalid");

                new CopyDirectory(temp1, temp2).Invoking(x => x.Run())
                                               .Should().Throw<IOException>();
            }
            finally
            {
                File.SetAttributes(Path.Combine(temp1, Path.Combine("subdir", "file")), FileAttributes.Normal);
                Directory.Delete(temp1, recursive: true);
                File.SetAttributes(Path.Combine(temp2, Path.Combine("subdir", "file")), FileAttributes.Normal);
                Directory.Delete(temp2, recursive: true);
            }
        }

        /// <summary>
        /// Ensures <see cref="CopyDirectory"/> correctly copies a directory on top another.
        /// </summary>
        [Fact]
        public void Overwrite()
        {
            string temp1 = CreateCopyTestTempDir();

            string temp2 = FileUtils.GetTempDirectory("unit-tests");
            string subdir2 = Path.Combine(temp2, "subdir");
            Directory.CreateDirectory(subdir2);
            File.WriteAllText(Path.Combine(subdir2, "file"), @"B");
            File.SetLastWriteTimeUtc(Path.Combine(subdir2, "file"), new DateTime(2002, 1, 1));
            Directory.SetLastWriteTimeUtc(subdir2, new DateTime(2002, 1, 1));

            try
            {
                new CopyDirectory(temp1, temp2, preserveDirectoryTimestamps: true, overwrite: true).Run();
                File.ReadAllBytes(Path.Combine(temp2, "subdir", "file"))
                    .Should().Equal(File.ReadAllBytes(Path.Combine(temp1, "subdir", "file")));
                Directory.GetLastWriteTimeUtc(Path.Combine(temp2, "subdir")).Should().Be(
                    new DateTime(2000, 1, 1),
                    because: "Last-write time for copied directory is invalid");
                File.GetLastWriteTimeUtc(Path.Combine(temp2, "subdir", "file")).Should().Be(
                    new DateTime(2000, 1, 1),
                    because: "Last-write time for copied file is invalid");
            }
            finally
            {
                File.SetAttributes(Path.Combine(temp1, Path.Combine("subdir", "file")), FileAttributes.Normal);
                Directory.Delete(temp1, recursive: true);
                File.SetAttributes(Path.Combine(temp2, Path.Combine("subdir", "file")), FileAttributes.Normal);
                Directory.Delete(temp2, recursive: true);
            }
        }

        /// <summary>
        /// Ensures <see cref="CopyDirectory"/> correctly copies symlinks.
        /// </summary>
        [SkippableFact]
        public void Symlinks()
        {
            Skip.IfNot(UnixUtils.IsUnix, reason: "Can only test POSIX symlinks on Unixoid system");

            string temp1 = CreateCopyTestTempDir();
            string temp2 = FileUtils.GetTempDirectory("unit-tests");

            try
            {
                FileUtils.CreateSymlink(sourcePath: Path.Combine(temp1, "symlink"), targetPath: "target");

                new CopyDirectory(temp1, temp2).Run();
                FileUtils.IsSymlink(Path.Combine(temp2, "symlink"), out string? symlinkTarget).Should().BeTrue();
                symlinkTarget.Should().Be("target");
            }
            finally
            {
                Directory.Delete(temp1, recursive: true);
                Directory.Delete(temp2, recursive: true);
            }
        }

        private static string CreateCopyTestTempDir()
        {
            string tempPath = FileUtils.GetTempDirectory("unit-tests");
            string subdir1 = Path.Combine(tempPath, "subdir");
            Directory.CreateDirectory(subdir1);
            File.WriteAllText(Path.Combine(subdir1, "file"), @"A");
            File.SetLastWriteTimeUtc(Path.Combine(subdir1, "file"), new DateTime(2000, 1, 1));
            File.SetAttributes(Path.Combine(subdir1, "file"), FileAttributes.ReadOnly);
            Directory.SetLastWriteTimeUtc(subdir1, new DateTime(2000, 1, 1));
            return tempPath;
        }
    }
}
