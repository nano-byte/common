/*
 * Copyright 2006-2015 Bastian Eicher
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
