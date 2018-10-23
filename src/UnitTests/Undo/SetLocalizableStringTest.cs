// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Globalization;
using FluentAssertions;
using NanoByte.Common.Collections;
using Xunit;

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// Contains test methods for <see cref="SetLocalizableString"/>.
    /// </summary>
    public class SetLocalizableStringTest
    {
        /// <summary>
        /// Makes sure <see cref="SetLocalizableString"/> correctly performs executions and undos.
        /// </summary>
        [Fact]
        public void TestExecute()
        {
            var collection = new LocalizableStringCollection
            {
                "neutralValue1",
                {"en-US", "americaValue1"}
            };

            var neutralCommand = new SetLocalizableString(collection, new LocalizableString {Value = "neutralValue2"});
            var americanCommand = new SetLocalizableString(collection, new LocalizableString {Language = new CultureInfo("en-US"), Value = "americaValue2"});

            neutralCommand.Execute();
            collection.GetExactLanguage(LocalizableString.DefaultLanguage)
                      .Should().Be("neutralValue2", because: "Unspecified language should default to English generic");

            neutralCommand.Undo();
            collection.GetExactLanguage(LocalizableString.DefaultLanguage)
                      .Should().Be("neutralValue1", because: "Unspecified language should default to English generic");

            americanCommand.Execute();
            collection.GetExactLanguage(new CultureInfo("en-US"))
                      .Should().Be("americaValue2");

            americanCommand.Undo();
            collection.GetExactLanguage(new CultureInfo("en-US"))
                      .Should().Be("americaValue1");
        }
    }
}
