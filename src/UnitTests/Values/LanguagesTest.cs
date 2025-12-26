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
    public void AllKnown()
        => Languages.AllKnown.ToList().Should().NotBeEmpty();

    [Theory]
    [InlineData("en-US", "en-US")]
    [InlineData("en_US", "en-US")]
    [InlineData("de-DE", "de-DE")]
    [InlineData("de_DE", "de-DE")]
    public void FromString(string input, string expected)
        => Languages.FromString(input).Name.Should().Be(expected);
}
