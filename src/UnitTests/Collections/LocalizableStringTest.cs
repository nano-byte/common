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
        var str = new LocalizableString {Language = new("de-DE")};
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
    public void LanguageStringEmpty()
    {
        var str = new LocalizableString {LanguageString = ""};
        str.Language.Should().Be(LocalizableString.DefaultLanguage);
    }

    [Fact]
    public void EqualsWithSameValueAndLanguage()
    {
        var str1 = new LocalizableString {Value = "test", Language = new("en")};
        var str2 = new LocalizableString {Value = "test", Language = new("en")};
        str1.Equals(str2).Should().BeTrue();
    }

    [Fact]
    public void NotEqualsWithDifferentValue()
    {
        var str1 = new LocalizableString {Value = "test1", Language = new("en")};
        var str2 = new LocalizableString {Value = "test2", Language = new("en")};
        str1.Equals(str2).Should().BeFalse();
    }

    [Fact]
    public void NotEqualsWithDifferentLanguage()
    {
        var str1 = new LocalizableString {Value = "test", Language = new("en")};
        var str2 = new LocalizableString {Value = "test", Language = new("de")};
        str1.Equals(str2).Should().BeFalse();
    }

    [Fact]
    public void EqualityOperators()
    {
        var str1 = new LocalizableString {Value = "test", Language = new("en")};
        var str2 = new LocalizableString {Value = "test", Language = new("en")};
        var str3 = new LocalizableString {Value = "other", Language = new("en")};

        (str1 == str2).Should().BeTrue();
        (str1 != str3).Should().BeTrue();
    }

    [Fact]
    public void GetHashCodeConsistent()
    {
        var str1 = new LocalizableString {Value = "test", Language = new("en")};
        var str2 = new LocalizableString {Value = "test", Language = new("en")};
        str1.GetHashCode().Should().Be(str2.GetHashCode());
    }

    [Fact]
    public void Clone()
    {
        var str = new LocalizableString {Value = "test", Language = new("de-DE")};
        var clone = str.Clone();
        clone.Should().NotBeSameAs(str);
        clone.Value.Should().Be(str.Value);
        clone.Language.Should().Be(str.Language);
    }
}
