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

using System.IO;
using NanoByte.Common.Storage;
using NUnit.Framework;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Contains test methods for <see cref="ByteGrid"/>.
    /// </summary>
    [TestFixture]
    public class ByteGridTest
    {
        [Test]
        public void TestSaveLoad()
        {
            using (var tempFile = new TemporaryFile("unit-tests"))
            {
                var grid = new ByteGrid(new byte[,] {{10, 20}, {100, 200}});
                grid.Save(tempFile);

                using (var stream = File.OpenRead(tempFile))
                    grid = ByteGrid.Load(stream);

                Assert.AreEqual(10, grid[0, 0]);
                Assert.AreEqual(20, grid[0, 1]);
                Assert.AreEqual(100, grid[1, 0]);
                Assert.AreEqual(200, grid[1, 1]);
            }
        }

        [Test]
        public void TestSampledRead()
        {
            Assert.AreEqual(5, new ByteGrid(new byte[,] {{10}, {0}}).SampledRead(0.5f, 0));
            Assert.AreEqual(5, new ByteGrid(new byte[,] {{10, 0}}).SampledRead(0.5f, 0.5f));
            Assert.AreEqual(3.75f, new ByteGrid(new byte[,] {{10, 5}, {0, 0}}).SampledRead(0.5f, 0.5f));
        }
    }
}
