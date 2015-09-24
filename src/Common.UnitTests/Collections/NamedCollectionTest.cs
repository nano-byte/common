/*
 * Copyright 2010-2014 Bastian Eicher
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser Public License for more details.
 *
 * You should have received a copy of the GNU Lesser Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="NamedCollection{T}"/>.
    /// </summary>
    [TestFixture]
    public class NamedCollectionTest
    {
        private class TestElement : INamed<TestElement>
        {
            public string Name { get; set; }

            public int CompareTo(TestElement other)
            {
                return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Test]
        public void TestSort()
        {
            var collection = new NamedCollection<TestElement> {new TestElement {Name = "c"}, new TestElement {Name = "b"}, new TestElement {Name = "a"}};

            collection.Select(x => x.Name).Should().Equal("a", "b", "c");
        }

        [Test]
        public void TestRename()
        {
            var element = new TestElement {Name = "Name1"};

            var collection = new NamedCollection<TestElement> {element};
            collection["Name1"].Should().BeSameAs(element);

            collection.Rename(element, "Name2");
            element.Name.Should().Be("Name2");
            collection["Name2"].Should().BeSameAs(element);
        }
    }
}
