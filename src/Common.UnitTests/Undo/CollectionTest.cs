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

using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// Contains test methods for <see cref="AddToCollection{T}"/> and <see cref="RemoveFromCollection{T}"/>.
    /// </summary>
    [TestFixture]
    public class CollectionTest
    {
        [Test]
        public void TestAddToCollection()
        {
            var collection = new List<string> {"a", "b", "c"};
            var command = new AddToCollection<string>(collection, "d");

            command.Execute();
            collection.Should().Contain("d");

            command.Undo();
            collection.Should().NotContain("d");
        }

        [Test]
        public void TestRemoveFromCollection()
        {
            var collection = new List<string> {"a", "b", "c"};
            var command = new RemoveFromCollection<string>(collection, "b");

            command.Execute();
            collection.Should().Contain("a");
            collection.Should().NotContain("b");
            collection.Should().Contain("c");

            command.Undo();
            collection.Should().Contain("a");
            collection.Should().Contain("b");
            collection.Should().Contain("c");
        }
    }
}
