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
using FluentAssertions;
using NanoByte.Common.Storage;
using Xunit;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="XmlDictionary"/>.
    /// </summary>
    public class XmlDictionaryTest
    {
        [Fact]
        public void TestSaveLoad()
        {
            var dictionary1 = new XmlDictionary
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            // Serialize and deserialize data
            string data = dictionary1.ToXmlString();
            var dictionary2 = XmlStorage.FromXmlString<XmlDictionary>(data);

            // Ensure data stayed the same
            dictionary2.Should().Equal(dictionary1, because: "Serialized objects should be equal.");
            ReferenceEquals(dictionary1, dictionary2).Should().BeFalse(because: "Serialized objects should not return the same reference.");
        }

        [Fact]
        public void TestContainsKey()
        {
            var dictionary = new XmlDictionary
            {
                {"key1", "value1"}
            };
            dictionary.ContainsKey("key1").Should().BeTrue();
            dictionary.ContainsKey("key2").Should().BeFalse();
        }

        [Fact]
        public void TestContainsValue()
        {
            var dictionary = new XmlDictionary
            {
                {"key1", "value1"}
            };
            dictionary.ContainsValue("value1").Should().BeTrue();
            dictionary.ContainsValue("value2").Should().BeFalse();
        }

        [Fact]
        public void TestGetValue()
        {
            var dictionary = new XmlDictionary
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };
            dictionary.GetValue("key1").Should().Be("value1");
        }

        [Fact]
        public void ShouldRejectDuplicateKeys()
        {
            var dictionary = new XmlDictionary
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            // Check for duplicate keys when adding new entries
            dictionary.Invoking(x => x.Add("key1", "newValue1")).ShouldThrow<ArgumentException>();

            // Check for duplicate keys when modifying existing entries
            var entry = new XmlDictionaryEntry("key3", "value3");
            dictionary.Add(entry);
            entry.Invoking(x => x.Key = "key1").ShouldThrow<InvalidOperationException>();
        }
    }
}
