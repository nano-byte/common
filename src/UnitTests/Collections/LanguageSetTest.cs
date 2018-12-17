// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Globalization;
using FluentAssertions;
using NanoByte.Common.Native;
using Xunit;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Contains test methods for <see cref="LanguageSet"/>.
    /// </summary>
    public class LanguageSetTest
    {
        [Fact]
        public void TestToString()
        {
            var collection = new LanguageSet {"en-US", "de"};
            collection.ToString().Should().Be("de en_US");
        }

        [Fact]
        public void TestFromString()
        {
            new LanguageSet("en_US de ").Should().BeEquivalentTo(new LanguageSet {"de", "en-US"});
        }

        [Fact]
        public void TestDuplicateDetection()
        {
            var collection = new LanguageSet("en_US");
            collection.Add("en-US").Should().BeFalse();
            collection.Should().BeEquivalentTo(new LanguageSet {"en-US"});
        }

        [Fact]
        public void TestContainsAny()
        {
            new LanguageSet {"de", "en"}.ContainsAny(new LanguageSet {"en", "fr"}).Should().BeTrue();
            new LanguageSet {"en", "fr"}.ContainsAny(new LanguageSet {"de", "en"}).Should().BeTrue();

            new LanguageSet().ContainsAny(new LanguageSet {"de"}).Should().BeFalse();
            new LanguageSet {"de"}.ContainsAny(new LanguageSet()).Should().BeFalse();

            new LanguageSet {"de", "en-US"}.ContainsAny(new LanguageSet {"en", "fr"}).Should().BeFalse();
            new LanguageSet {"en", "fr"}.ContainsAny(new LanguageSet {"de", "en-US"}).Should().BeFalse();

            new LanguageSet {"de", "en"}.ContainsAny(new LanguageSet {"fr"}).Should().BeFalse();
            new LanguageSet {"fr"}.ContainsAny(new LanguageSet {"de", "en"}).Should().BeFalse();
        }

        [Fact]
        public void TestContainsAnyIgnoreCountry()
        {
            new LanguageSet {"de", "en"}.ContainsAny(new LanguageSet {"en", "fr"}, ignoreCountry: true).Should().BeTrue();
            new LanguageSet {"en", "fr"}.ContainsAny(new LanguageSet {"de", "en"}, ignoreCountry: true).Should().BeTrue();

            new LanguageSet().ContainsAny(new LanguageSet {"de"}, ignoreCountry: true).Should().BeFalse();
            new LanguageSet {"de"}.ContainsAny(new LanguageSet(), ignoreCountry: true).Should().BeFalse();

            new LanguageSet {"de", "en-US"}.ContainsAny(new LanguageSet {"en", "fr"}, ignoreCountry: true).Should().BeTrue();
            new LanguageSet {"en", "fr"}.ContainsAny(new LanguageSet {"de", "en-US"}, ignoreCountry: true).Should().BeTrue();

            new LanguageSet {"de", "en"}.ContainsAny(new LanguageSet {"fr"}, ignoreCountry: true).Should().BeFalse();
            new LanguageSet {"fr"}.ContainsAny(new LanguageSet {"de", "en"}, ignoreCountry: true).Should().BeFalse();
        }

        [SkippableFact]
        public void TestInvalidCultureInSet()
        {
            Skip.IfNot(WindowsUtils.IsWindows, reason: "Non-Windows systems may not have a .NET language/culture database installed.");

            new LanguageSet("invalid en").Should().Equal(new LanguageSet {CultureInfo.InvariantCulture, new CultureInfo("en")});
        }
    }
}
