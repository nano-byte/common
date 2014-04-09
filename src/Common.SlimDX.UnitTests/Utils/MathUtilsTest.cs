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

using NUnit.Framework;

namespace NanoByte.Common.Utils
{
    /// <summary>
    /// Contains test methods for <see cref="MathUtils"/>.
    /// </summary>
    [TestFixture]
    public class MathUtilsTest
    {
        [Test]
        public void TestModuloDouble()
        {
            Assert.AreEqual(expected: 2, actual: 5.0.Modulo(3));
            Assert.AreEqual(expected: 1, actual: 4.0.Modulo(3));
            Assert.AreEqual(expected: 0, actual: 3.0.Modulo(3));
            Assert.AreEqual(expected: 2, actual: 2.0.Modulo(3));
            Assert.AreEqual(expected: 1, actual: 1.0.Modulo(3));
            Assert.AreEqual(expected: 0, actual: 0.0.Modulo(3));
            Assert.AreEqual(expected: 2, actual: (-1.0).Modulo(3));
            Assert.AreEqual(expected: 1, actual: (-2.0).Modulo(3));
            Assert.AreEqual(expected: 0, actual: (-3.0).Modulo(3));
        }

        [Test]
        public void TestModuloFloat()
        {
            Assert.AreEqual(expected: 2, actual: 5f.Modulo(3));
            Assert.AreEqual(expected: 1, actual: 4f.Modulo(3));
            Assert.AreEqual(expected: 0, actual: 3f.Modulo(3));
            Assert.AreEqual(expected: 2, actual: 2f.Modulo(3));
            Assert.AreEqual(expected: 1, actual: 1f.Modulo(3));
            Assert.AreEqual(expected: 0, actual: 0f.Modulo(3));
            Assert.AreEqual(expected: 2, actual: (-1f).Modulo(3));
            Assert.AreEqual(expected: 1, actual: (-2f).Modulo(3));
            Assert.AreEqual(expected: 0, actual: (-3f).Modulo(3));
        }

        [Test]
        public void TestModuloInt()
        {
            Assert.AreEqual(expected: 2, actual: 5.Modulo(3));
            Assert.AreEqual(expected: 1, actual: 4.Modulo(3));
            Assert.AreEqual(expected: 0, actual: 3.Modulo(3));
            Assert.AreEqual(expected: 2, actual: 2.Modulo(3));
            Assert.AreEqual(expected: 1, actual: 1.Modulo(3));
            Assert.AreEqual(expected: 0, actual: 0.Modulo(3));
            Assert.AreEqual(expected: 2, actual: (-1).Modulo(3));
            Assert.AreEqual(expected: 1, actual: (-2).Modulo(3));
            Assert.AreEqual(expected: 0, actual: (-3).Modulo(3));
        }
    }
}
