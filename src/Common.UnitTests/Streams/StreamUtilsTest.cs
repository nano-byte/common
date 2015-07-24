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

using System.IO;
using System.Text;
using NanoByte.Common.Storage;
using NUnit.Framework;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Contains test methods for <see cref="StreamUtils"/>.
    /// </summary>
    [TestFixture]
    public class StreamUtilsTest
    {
        [Test]
        public void TestRead()
        {
            Assert.AreEqual(
                expected: new byte[] {1, 2, 3},
                actual: new MemoryStream(new byte[] {1, 2, 3, 4, 5}).Read(3));
        }

        [Test]
        public void TestToArray()
        {
            var stream = new MemoryStream(new byte[] {1, 2, 3});
            Assert.AreEqual(expected: new byte[] {1, 2, 3}, actual: StreamUtils.ToArray(stream));
        }

        [Test]
        public static void TestWrite()
        {
            var stream = new MemoryStream();
            stream.Write(new byte[] {1, 2, 3});

            Assert.AreEqual(
                expected: new byte[] {1, 2, 3},
                actual: StreamUtils.ToArray(stream));
        }

        [Test]
        public void TestContentEquals()
        {
            Assert.IsTrue("abc".ToStream().ContentEquals("abc".ToStream()));
            Assert.IsFalse("ab".ToStream().ContentEquals("abc".ToStream()));
            Assert.IsFalse("abc".ToStream().ContentEquals("ab".ToStream()));
            Assert.IsFalse("abc".ToStream().ContentEquals("".ToStream()));
        }

        [Test]
        public void TestString()
        {
            const string test = "Test";
            using (var stream = test.ToStream())
                Assert.AreEqual(test, stream.ReadToString());
        }

        [Test]
        public void TestCopyToFile()
        {
            using (var tempFile = new TemporaryFile("unit-tests"))
            {
                "abc".ToStream().CopyToFile(tempFile);
                Assert.AreEqual("abc", new FileInfo(tempFile).ReadFirstLine(Encoding.UTF8));
            }
        }
    }
}
