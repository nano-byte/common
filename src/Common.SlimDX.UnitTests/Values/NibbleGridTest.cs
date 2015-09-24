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
using FluentAssertions;
using NanoByte.Common.Storage;
using NUnit.Framework;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Contains test methods for <see cref="NibbleGrid"/>.
    /// </summary>
    [TestFixture]
    public class NibbleGridTest
    {
        [Test]
        public void TestSaveLoad()
        {
            using (var tempFile = new TemporaryFile("unit-tests"))
            {
                var grid = new NibbleGrid(new byte[,] {{2, 4}, {5, 10}});
                grid.Save(tempFile);

                using (var stream = File.OpenRead(tempFile))
                    grid = NibbleGrid.Load(stream);

                grid[0, 0].Should().Be(2);
                grid[0, 1].Should().Be(4);
                grid[1, 0].Should().Be(5);
                grid[1, 1].Should().Be(10);
            }
        }
    }
}
