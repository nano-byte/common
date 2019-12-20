// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="NamedCollection{T}"/>.
    /// </summary>
    public class NamedCollectionTest
    {
        private class TestElement : INamed<TestElement>
        {
            public string Name { get; set; }

            public TestElement(string name)
            {
                Name = name;
            }

            public int CompareTo(TestElement other)
                => string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void TestSort()
        {
            var collection = new NamedCollection<TestElement> {new TestElement("c"), new TestElement("b"), new TestElement("a")};

            collection.Select(x => x.Name).Should().Equal("a", "b", "c");
        }

        [Fact]
        public void TestRename()
        {
            var element = new TestElement("Name1");

            var collection = new NamedCollection<TestElement> {element};
            collection["Name1"].Should().BeSameAs(element);

            collection.Rename(element, "Name2");
            element.Name.Should().Be("Name2");
            collection["Name2"].Should().BeSameAs(element);
        }
    }
}
