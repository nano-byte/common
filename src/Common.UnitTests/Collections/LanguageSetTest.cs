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
        public void TestFromString() => new LanguageSet("en_US de").Should().BeEquivalentTo(new LanguageSet {"de", "en-US"});

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

            new LanguageSet("invalid en").Should().Equal(new LanguageSet{CultureInfo.InvariantCulture, new CultureInfo("en")});
        }
    }
}
