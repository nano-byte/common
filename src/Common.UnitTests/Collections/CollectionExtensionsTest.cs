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
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="CollectionExtensions"/>.
    /// </summary>
    [TestFixture]
    public class CollectionExtensionsTest
    {
        /// <summary>
        /// Ensures that <see cref="CollectionExtensions.AddIfNew{T}"/> correctly detects pre-existing entries.
        /// </summary>
        [Test]
        public void TestAddIfNew()
        {
            var list = new List<string> {"a", "b", "c"};

            list.AddIfNew("b").Should().BeFalse();
            list.Should().Equal("a", "b", "c");

            list.AddIfNew("d").Should().BeTrue();
            list.Should().Equal("a", "b", "c", "d");

            list.Invoking(x => x.RemoveLast(-1)).ShouldThrow<ArgumentOutOfRangeException>();
        }

        /// <summary>
        /// Ensures that <see cref="CollectionExtensions.RemoveLast{T}"/> correctly removes the last n elements from a list.
        /// </summary>
        [Test]
        public void TestRemoveLast()
        {
            var list = new List<string> {"a", "b", "c"};
            list.RemoveLast(2);
            list.Should().Equal("a");

            list.Invoking(x => x.RemoveLast(-1)).ShouldThrow<ArgumentOutOfRangeException>();
        }

        /// <summary>
        /// Ensures that <see cref="CollectionExtensions.FindIndex{T}"/> correctly determines the index of an element in a list.
        /// </summary>
        [Test]
        public void TestFindIndex()
        {
            new List<string> {"a", "b", "c"}.FindIndex("b").Should().Be(1);
        }
    }
}
