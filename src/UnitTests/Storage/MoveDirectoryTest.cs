// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Contains test methods for <see cref="MoveDirectory"/>.
    /// </summary>
    public class MoveDirectoryTest
    {
        /// <summary>
        /// Ensures <see cref="MoveDirectory"/> correctly moves a directories from one location to another.
        /// </summary>
        [Fact]
        public void Normal()
        {
            string temp1 = CreateCopyTestTempDir();
            string temp2 = FileUtils.GetTempDirectory("unit-tests");
            Directory.Delete(temp2);

            try
            {
                new MoveDirectory(temp1, temp2).Run();
                File.Exists(Path.Combine(temp2, "subdir", "file")).Should().BeTrue();
                Directory.GetLastWriteTimeUtc(Path.Combine(temp2, "subdir"))
                         .Should().Be(new DateTime(2000, 1, 1), because: "Last-write time for copied directory");
                File.GetLastWriteTimeUtc(Path.Combine(temp2, "subdir", "file"))
                    .Should().Be(new DateTime(2000, 1, 1), because: "Last-write time for copied file");

                Directory.Exists(temp1).Should().BeFalse(because: "Original directory should be gone after move");
            }
            finally
            {
                File.SetAttributes(Path.Combine(temp2, "subdir", "file"), FileAttributes.Normal);
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
