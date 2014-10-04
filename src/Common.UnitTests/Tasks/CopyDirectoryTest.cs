/*
 * Copyright 2006-2014 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.IO;
using NanoByte.Common.Native;
using NanoByte.Common.Storage;
using NUnit.Framework;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Contains test methods for <see cref="CopyDirectory"/>.
    /// </summary>
    [TestFixture]
    public class CopyDirectoryTest
    {
        /// <summary>
        /// Ensures <see cref="CopyDirectory"/> correctly copies a directories from one location to another.
        /// </summary>
        [Test]
        public void Normal()
        {
            string temp1 = CreateCopyTestTempDir();
            string temp2 = FileUtils.GetTempDirectory("unit-tests");
            Directory.Delete(temp2);

            try
            {
                new CopyDirectory(temp1, temp2).Run();
                FileAssert.AreEqual(
                    expected: Path.Combine(Path.Combine(temp1, "subdir"), "file"),
                    actual: Path.Combine(Path.Combine(temp2, "subdir"), "file"));
                Assert.AreEqual(
                    expected: new DateTime(2000, 1, 1),
                    actual: Directory.GetLastWriteTimeUtc(Path.Combine(temp2, "subdir")),
                    message: "Last-write time for copied directory is invalid");
                Assert.AreEqual(
                    expected: new DateTime(2000, 1, 1),
                    actual: File.GetLastWriteTimeUtc(Path.Combine(Path.Combine(temp2, "subdir"), "file")),
                    message: "Last-write time for copied file is invalid");

                Assert.Throws<IOException>(() => new CopyDirectory(temp1, temp2).Run());
            }
            finally
            {
                File.SetAttributes(Path.Combine(Path.Combine(temp1, "subdir"), "file"), FileAttributes.Normal);
                Directory.Delete(temp1, recursive: true);
                File.SetAttributes(Path.Combine(Path.Combine(temp2, "subdir"), "file"), FileAttributes.Normal);
                Directory.Delete(temp2, recursive: true);
            }
        }

        /// <summary>
        /// Ensures <see cref="CopyDirectory"/> correctly detects usage errors.
        /// </summary>
        [Test]
        public void ErrorHandling()
        {
            Assert.Throws<ArgumentException>(() => new CopyDirectory("a", "a"));

            string temp = FileUtils.GetTempDirectory("unit-tests");
            Directory.Delete(temp);
            Assert.Throws<DirectoryNotFoundException>(() => new CopyDirectory(temp, "a").Run());
        }

        /// <summary>
        /// Ensures <see cref="CopyDirectory"/> correctly copies a directories from one location to another without setting directory timestamps.
        /// </summary>
        [Test]
        public void NoDirTimestamp()
        {
            string temp1 = CreateCopyTestTempDir();
            string temp2 = FileUtils.GetTempDirectory("unit-tests");
            Directory.Delete(temp2);

            try
            {
                new CopyDirectory(temp1, temp2, preserveDirectoryTimestamps: false).Run();
                FileAssert.AreEqual(
                    expected: Path.Combine(Path.Combine(temp1, "subdir"), "file"),
                    actual: Path.Combine(Path.Combine(temp2, "subdir"), "file"));
                Assert.AreNotEqual(
                    expected: new DateTime(2000, 1, 1),
                    actual: Directory.GetLastWriteTimeUtc(Path.Combine(temp2, "subdir")),
                    message: "Last-write time for copied directory is invalid");
                Assert.AreEqual(
                    expected: new DateTime(2000, 1, 1),
                    actual: File.GetLastWriteTimeUtc(Path.Combine(Path.Combine(temp2, "subdir"), "file")),
                    message: "Last-write time for copied file is invalid");

                Assert.Throws<IOException>(() => new CopyDirectory(temp1, temp2).Run());
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
        [Test]
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
                FileAssert.AreEqual(
                    expected: Path.Combine(Path.Combine(temp1, "subdir"), "file"),
                    actual: Path.Combine(Path.Combine(temp2, "subdir"), "file"));
                Assert.AreEqual(
                    expected: new DateTime(2000, 1, 1),
                    actual: Directory.GetLastWriteTimeUtc(Path.Combine(temp2, "subdir")),
                    message: "Last-write time for copied directory is invalid");
                Assert.AreEqual(
                    expected: new DateTime(2000, 1, 1),
                    actual: File.GetLastWriteTimeUtc(Path.Combine(Path.Combine(temp2, "subdir"), "file")),
                    message: "Last-write time for copied file is invalid");
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
        [Test]
        public void Symlinks()
        {
            if (!UnixUtils.IsUnix) Assert.Ignore("Can only test POSIX symlinks on Unixoid system");

            string temp1 = CreateCopyTestTempDir();
            string temp2 = FileUtils.GetTempDirectory("unit-tests");

            try
            {
                FileUtils.CreateSymlink(source: Path.Combine(temp1, "symlink"), target: "target");

                new CopyDirectory(temp1, temp2).Run();
                string symlinkTarget;
                Assert.IsTrue(FileUtils.IsSymlink(Path.Combine(temp2, "symlink"), out symlinkTarget));
                Assert.AreEqual(expected: "target", actual: symlinkTarget);
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
