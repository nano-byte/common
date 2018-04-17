// Copyright Bastian Eicher
// Licensed under the MIT License

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
            dictionary.Invoking(x => x.Add("key1", "newValue1")).Should().Throw<ArgumentException>();

            // Check for duplicate keys when modifying existing entries
            var entry = new XmlDictionaryEntry("key3", "value3");
            dictionary.Add(entry);
            entry.Invoking(x => x.Key = "key1").Should().Throw<InvalidOperationException>();
        }
    }
}
