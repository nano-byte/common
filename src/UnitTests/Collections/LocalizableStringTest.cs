// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Globalization;

namespace NanoByte.Common.Collections;

/// <summary>
/// Contains test methods for <see cref="LocalizableString"/>.
/// </summary>
public class LocalizableStringTest
{
    [Fact]
    public void DefaultLanguageIsEnglish()
    {
        var str = new LocalizableString();
        str.Language.Should().Be(LocalizableString.DefaultLanguage);
        str.Language.Name.Should().Be("en");
    }

    [Fact]
    public void SetAndGetValue()
    {
        var str = new LocalizableString {Value = "test"};
        str.Value.Should().Be("test");
    }

    [Fact]
    public void SetAndGetLanguage()
    {
        var str = new LocalizableString {Language = new CultureInfo("de-DE")};
        str.Language.Name.Should().Be("de-DE");
    }

    [Fact]
    public void InvariantCultureConvertsToDefault()
    {
        var str = new LocalizableString {Language = CultureInfo.InvariantCulture};
        str.Language.Should().Be(LocalizableString.DefaultLanguage);
    }

    [Fact]
    public void LanguageStringRoundtrip()
    {
        var str = new LocalizableString {LanguageString = "de-DE"};
        str.LanguageString.Should().Be("de-DE");
    }

    [Fact]
    public void LanguageStringUnixFormat()
    {
        var str = new LocalizableString {LanguageString = "de_DE"};
        str.Language.Name.Should().Be("de-DE");
    }

    [Fact]
    public void LanguageStringEmptyDefaultsToEnglish()
    {
        var str = new LocalizableString {LanguageString = ""};
        str.Language.Should().Be(LocalizableString.DefaultLanguage);
    }

    [Fact]
    public void ToStringFormat()
    {
        var str = new LocalizableString {Value = "test", Language = new CultureInfo("de-DE")};
        str.ToString().Should().Be("test (de-DE)");
    }

    [Fact]
    public void EqualsWithSameValueAndLanguage()
    {
        var str1 = new LocalizableString {Value = "test", Language = new CultureInfo("en")};
        var str2 = new LocalizableString {Value = "test", Language = new CultureInfo("en")};
        str1.Equals(str2).Should().BeTrue();
    }

    [Fact]
    public void NotEqualsWithDifferentValue()
    {
        var str1 = new LocalizableString {Value = "test1", Language = new CultureInfo("en")};
        var str2 = new LocalizableString {Value = "test2", Language = new CultureInfo("en")};
        str1.Equals(str2).Should().BeFalse();
    }

    [Fact]
    public void NotEqualsWithDifferentLanguage()
    {
        var str1 = new LocalizableString {Value = "test", Language = new CultureInfo("en")};
        var str2 = new LocalizableString {Value = "test", Language = new CultureInfo("de")};
        str1.Equals(str2).Should().BeFalse();
    }

    [Fact]
    public void EqualityOperators()
    {
        var str1 = new LocalizableString {Value = "test", Language = new CultureInfo("en")};
        var str2 = new LocalizableString {Value = "test", Language = new CultureInfo("en")};
        var str3 = new LocalizableString {Value = "other", Language = new CultureInfo("en")};

        (str1 == str2).Should().BeTrue();
        (str1 != str3).Should().BeTrue();
    }

    [Fact]
    public void GetHashCodeConsistent()
    {
        var str1 = new LocalizableString {Value = "test", Language = new CultureInfo("en")};
        var str2 = new LocalizableString {Value = "test", Language = new CultureInfo("en")};
        str1.GetHashCode().Should().Be(str2.GetHashCode());
    }

    [Fact]
    public void Clone()
    {
        var str = new LocalizableString {Value = "test", Language = new CultureInfo("de-DE")};
        var clone = str.Clone();
        clone.Should().NotBeSameAs(str);
        clone.Value.Should().Be(str.Value);
        clone.Language.Should().Be(str.Language);
    }

    [Fact]
    public void SetLanguageNull()
    {
        var str = new LocalizableString();
        str.Invoking(s => s.Language = null!)
           .Should().Throw<ArgumentNullException>();
    }
}
