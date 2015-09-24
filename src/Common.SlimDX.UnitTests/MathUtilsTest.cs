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

using FluentAssertions;
using NUnit.Framework;

namespace NanoByte.Common
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
            5.0.Modulo(3).Should().Be(2);
            4.0.Modulo(3).Should().Be(1);
            3.0.Modulo(3).Should().Be(0);
            2.0.Modulo(3).Should().Be(2);
            1.0.Modulo(3).Should().Be(1);
            0.0.Modulo(3).Should().Be(0);
            (-1.0).Modulo(3).Should().Be(2);
            (-2.0).Modulo(3).Should().Be(1);
            (-3.0).Modulo(3).Should().Be(0);
        }

        [Test]
        public void TestModuloFloat()
        {
            5f.Modulo(3).Should().Be(2);
            4f.Modulo(3).Should().Be(1);
            3f.Modulo(3).Should().Be(0);
            2f.Modulo(3).Should().Be(2);
            1f.Modulo(3).Should().Be(1);
            0f.Modulo(3).Should().Be(0);
            (-1f).Modulo(3).Should().Be(2);
            (-2f).Modulo(3).Should().Be(1);
            (-3f).Modulo(3).Should().Be(0);
        }

        [Test]
        public void TestModuloInt()
        {
            5.Modulo(3).Should().Be(2);
            4.Modulo(3).Should().Be(1);
            3.Modulo(3).Should().Be(0);
            2.Modulo(3).Should().Be(2);
            1.Modulo(3).Should().Be(1);
            0.Modulo(3).Should().Be(0);
            (-1).Modulo(3).Should().Be(2);
            (-2).Modulo(3).Should().Be(1);
            (-3).Modulo(3).Should().Be(0);
        }
    }
}
