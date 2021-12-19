// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Globalization;
using NanoByte.Common.Storage;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="LocalizableStringCollection"/>.
    /// </summary>
    public class LocalizableStringCollectionTest
    {
        [Fact]
        public void TestSaveLoad()
        {
            var collection1 = new LocalizableStringCollection
            {
                "neutralValue",
                {"en-US", "americaValue"},
                {"en-GB", "gbValue"},
                {"de", "germanValue"},
                {"de-DE", "germanyValue"}
            };

            // Serialize and deserialize data
            string data = collection1.ToXmlString();
            var collection2 = XmlStorage.FromXmlString<LocalizableStringCollection>(data);

            // Ensure data stayed the same
            collection2.Should().Equal(collection1, because: "Serialized objects should be equal.");
            collection2.GetSequencedHashCode().Should().Be(collection1.GetSequencedHashCode(), because: "Serialized objects' hashes should be equal.");
            ReferenceEquals(collection1, collection2).Should().BeFalse(because: "Serialized objects should not return the same reference.");
        }

        [Fact]
        public void TestContainsExactLanguage()
        {
            var dictionary = new LocalizableStringCollection
            {
                "neutralValue",
                {"de-DE", "germanyValue"}
            };

            dictionary.ContainsExactLanguage(LocalizableString.DefaultLanguage).Should().BeTrue(because: "Unspecified language should default to English generic");
            dictionary.ContainsExactLanguage(new CultureInfo("de-DE")).Should().BeTrue();
            dictionary.ContainsExactLanguage(new CultureInfo("de")).Should().BeFalse();
            dictionary.ContainsExactLanguage(new CultureInfo("de-AT")).Should().BeFalse();
            dictionary.ContainsExactLanguage(new CultureInfo("en-US")).Should().BeFalse();
        }

        [Fact]
        public void TestRemoveRange()
        {
            var dictionary = new LocalizableStringCollection
            {
                "neutralValue",
                {"de-DE", "germanyValue"},
                // Intentional duplicates (should be ignored)
                "neutralValue",
                {"de-DE", "germanyValue"}
            };

            dictionary.Set(LocalizableString.DefaultLanguage, null);
            dictionary.ContainsExactLanguage(LocalizableString.DefaultLanguage).Should().BeFalse(because: "Unspecified language should default to English generic");
            dictionary.Set(new CultureInfo("de-DE"), null);
            dictionary.ContainsExactLanguage(new CultureInfo("de-DE")).Should().BeFalse();
        }

        [Fact]
        public void TestSet()
        {
            var dictionary = new LocalizableStringCollection
            {
                "neutralValue",
                {"de-DE", "germanyValue"},
                // Intentional duplicates (should be removed)
                "neutralValue",
                {"de-DE", "germanyValue"}
            };

            dictionary.Set(LocalizableString.DefaultLanguage, "neutralValue2");
            dictionary.Set(new CultureInfo("de-DE"), "germanyValue2");

            dictionary.GetExactLanguage(LocalizableString.DefaultLanguage).Should().Be("neutralValue2");
            dictionary.GetExactLanguage(new CultureInfo("de-DE")).Should().Be("germanyValue2");
        }

        [Fact]
        public void TestGetExactLanguage()
        {
            var dictionary = new LocalizableStringCollection
            {
                "neutralValue",
                {"en-US", "americaValue"},
                {"en-GB", "gbValue"},
                {"de", "germanValue"},
                {"de-DE", "germanyValue"}
            };

            dictionary.GetExactLanguage(LocalizableString.DefaultLanguage).Should().Be("neutralValue", because: "Unspecified language should default to English generic");
            dictionary.GetExactLanguage(new CultureInfo("en-US")).Should().Be("americaValue");
            dictionary.GetExactLanguage(new CultureInfo("en-CA")).Should().BeNull();
            dictionary.GetExactLanguage(new CultureInfo("en-GB")).Should().Be("gbValue");
            dictionary.GetExactLanguage(new CultureInfo("de")).Should().Be("germanValue");
            dictionary.GetExactLanguage(new CultureInfo("de-DE")).Should().Be("germanyValue");
            dictionary.GetExactLanguage(new CultureInfo("de-AT")).Should().BeNull();
        }

        [Fact]
        public void TestGetBestLanguage()
        {
            var dictionary = new LocalizableStringCollection
            {
                {"de", "germanValue"},
                {"de-DE", "germanyValue"},
                {"en-US", "americaValue"},
                {"en-GB", "gbValue"},
                "neutralValue"
            };

            dictionary.GetBestLanguage(LocalizableString.DefaultLanguage).Should().Be("neutralValue", because: "Unspecified language should default to English generic");
            dictionary.GetBestLanguage(new CultureInfo("en-US")).Should().Be("americaValue");
            dictionary.GetBestLanguage(new CultureInfo("en-CA")).Should().Be("neutralValue", because: "No exact match, should fall back to English generic");
            dictionary.GetBestLanguage(new CultureInfo("en-GB")).Should().Be("gbValue");
            dictionary.GetBestLanguage(new CultureInfo("de")).Should().Be("germanValue");
            dictionary.GetBestLanguage(new CultureInfo("de-DE")).Should().Be("germanyValue", because: "No exact match, should fall back to German generic");
            dictionary.GetBestLanguage(new CultureInfo("de-AT")).Should().Be("germanValue", because: "No exact match, should fall back to German generic");
            dictionary.GetBestLanguage(new CultureInfo("es-ES")).Should().Be("neutralValue", because: "No match, should fall back to English generic");

            dictionary.Set(LocalizableString.DefaultLanguage, null);
            dictionary.GetBestLanguage(new CultureInfo("es-ES")).Should().Be("americaValue", because: "No English generic, should fall back to English US");

            dictionary.Set(new CultureInfo("en-US"), null);
            dictionary.GetBestLanguage(new CultureInfo("es-ES")).Should().Be("germanValue", because: "No English US, should fall back to first entry in collection");
        }
    }
}
