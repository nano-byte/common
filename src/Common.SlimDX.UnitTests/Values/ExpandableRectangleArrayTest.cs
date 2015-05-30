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

using System.Drawing;
using NUnit.Framework;

namespace NanoByte.Common.Values
{
    /// <summary>
    /// Contains test methods for <see cref="ExpandableRectangleArray{T}"/>.
    /// </summary>
    [TestFixture]
    public class ExpandableRectangleArrayTest
    {
        [Test]
        public void TestGetArrayBase()
        {
            var expandableRectangleArray = new ExpandableRectangleArray<int>();
            expandableRectangleArray.AddLast(new Point(1, 1), new[,] {{5, 6}, {7, 8}});
            expandableRectangleArray.AddFirst(new Point(2, 2), new[,] {{1, 2}, {3, 4}});

            var result = expandableRectangleArray.GetArray(new[,] {{-1, -2, -3, -4}, {-5, -6, -7, -8}, {-9, -10, -11, -12}, {-13, -14, -15, -16}});
            Assert.AreEqual(new[,] {{5, 6, -8}, {7, 8, 2}, {-14, 3, 4}}, result);
            Assert.AreEqual(new Rectangle(1, 1, 3, 3), expandableRectangleArray.TotalArea);
        }

        [Test]
        public void TestGetArraySmallBase()
        {
            var expandableRectangleArray = new ExpandableRectangleArray<int>();
            expandableRectangleArray.AddLast(new Point(1, 1), new[,] {{5, 6}, {7, 8}});
            expandableRectangleArray.AddFirst(new Point(2, 2), new[,] {{1, 2}, {3, 4}});

            var result = expandableRectangleArray.GetArray(new[,] {{-1, -2, -3}, {-5, -6, -7}, {-9, -10, -11}, {-13, -14, -15}});
            Assert.AreEqual(new[,] {{5, 6}, {7, 8}, {-14, 3}}, result);
            Assert.AreEqual(new Rectangle(1, 1, 3, 3), expandableRectangleArray.TotalArea);
        }

        [Test]
        public void TestGetArrayNoBase()
        {
            var expandableRectangleArray = new ExpandableRectangleArray<int>();
            expandableRectangleArray.AddLast(new Point(1, 1), new[,] {{5, 6}, {7, 8}});
            expandableRectangleArray.AddFirst(new Point(2, 2), new[,] {{1, 2}, {3, 4}});

            var result = expandableRectangleArray.GetArray();
            Assert.AreEqual(new[,] {{5, 6, 0}, {7, 8, 2}, {0, 3, 4}}, result);
            Assert.AreEqual(new Rectangle(1, 1, 3, 3), expandableRectangleArray.TotalArea);
        }

        [Test]
        public void TestNegativeArea()
        {
            var expandableRectangleArray = new ExpandableRectangleArray<int>();
            expandableRectangleArray.AddLast(new Point(-1, -1), new[,] {{1, 2}, {1, 2}});

            var result = expandableRectangleArray.GetArray();
            Assert.AreEqual(new[,] {{2}}, result);
            Assert.AreEqual(new Rectangle(0, 0, 1, 1), expandableRectangleArray.TotalArea);
        }
    }
}
