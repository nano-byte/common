// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Globalization;

namespace NanoByte.Common.Values;

/// <summary>
/// Contains test methods for <see cref="Languages"/>.
/// </summary>
public class LanguagesTest
{
    [Fact]
    public void AllKnownReturnsMultipleCultures()
    {
        var cultures = Languages.AllKnown.ToList();
        cultures.Should().NotBeEmpty();
        cultures.Should().Contain(x => x.Name == "en");
        cultures.Should().Contain(x => x.Name == "de");
    }

    [Theory]
    [InlineData("en-US", "en-US")]
    [InlineData("en_US", "en-US")]
    [InlineData("de-DE", "de-DE")]
    [InlineData("de_DE", "de-DE")]
    public void FromString(string input, string expected)
    {
        var culture = Languages.FromString(input);
        culture.Name.Should().Be(expected);
    }

    [Fact]
    public void FromStringNull()
        => Assert.Throws<ArgumentNullException>(() => Languages.FromString(null!));

    [Fact]
    public void FromStringEmpty()
        => Assert.Throws<ArgumentNullException>(() => Languages.FromString(""));

    [Fact]
    public void SetUI()
    {
        var originalCulture = Thread.CurrentThread.CurrentUICulture;
        var testCulture = new CultureInfo("de-DE");

        Languages.SetUI(testCulture);
        Thread.CurrentThread.CurrentUICulture.Should().Be(testCulture);

        // Restore original culture
        Languages.SetUI(originalCulture);
    }

    [Fact]
    public void SetUINull()
        => Assert.Throws<ArgumentNullException>(() => Languages.SetUI(null!));
}
