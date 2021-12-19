// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Specialized;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Contains test methods for <see cref="OSUtils"/>.
    /// </summary>
    public class OSUtilsTest
    {
        [Fact]
        public void TestExpandVariablesCaseSensitive()
        {
            var variables = new Dictionary<string, string>
            {
                {"KEY1", "value1"},
                {"KEY2", "value2"},
                {"LONG KEY", "long value"},
                {"EMPTY", ""}
            };

            OSUtils.ExpandVariables("$KEY1$KEY2/$KEY1 $KEY2 ${LONG KEY} $UNSET", variables).Should().Be("value1value2/value1 value2 long value ");
            OSUtils.ExpandVariables("$KEY1-bla", variables).Should().Be("value1-bla");
            OSUtils.ExpandVariables("$KEY1-bla", variables).Should().Be("value1-bla");
            OSUtils.ExpandVariables("{bla-$KEY1-bla}", variables).Should().Be("{bla-value1-bla}");
            OSUtils.ExpandVariables("{bla-${KEY1}-bla}", variables).Should().Be("{bla-value1-bla}");
            OSUtils.ExpandVariables("$key1", variables).Should().Be("");
            OSUtils.ExpandVariables("${key1:-default}", variables).Should().Be("default");
            OSUtils.ExpandVariables("${key1-default}", variables).Should().Be("default");
            OSUtils.ExpandVariables("${EMPTY:-default}", variables).Should().Be("default");
            OSUtils.ExpandVariables("${EMPTY-default}", variables).Should().Be("");
            OSUtils.ExpandVariables("", variables).Should().Be("");
        }

        [Fact]
        public void TestExpandVariablesCaseInsensitive()
        {
            var variables = new StringDictionary
            {
                {"key1", "value1"},
                {"key2", "value2"},
                {"long key", "long value"},
                {"EMPTY", ""}
            };

            OSUtils.ExpandVariables("$KEY1$KEY2/$KEY1 $KEY2 ${LONG KEY} $UNSET", variables).Should().Be("value1value2/value1 value2 long value ");
            OSUtils.ExpandVariables("$KEY1-bla", variables).Should().Be("value1-bla");
            OSUtils.ExpandVariables("$KEY1{bla}", variables).Should().Be("value1{bla}");
            OSUtils.ExpandVariables("${UNSET:-default}", variables).Should().Be("default");
            OSUtils.ExpandVariables("${UNSET-default}", variables).Should().Be("default");
            OSUtils.ExpandVariables("${EMPTY:-default}", variables).Should().Be("default");
            OSUtils.ExpandVariables("${EMPTY-default}", variables).Should().Be("");
            OSUtils.ExpandVariables("", variables).Should().Be("");
        }

        [Fact]
        public void TestExpandVariablesEscaping()
        {
            var variables = new Dictionary<string, string> {{"KEY", "value"}};

            OSUtils.ExpandVariables("$KEY-$$KEY", variables).Should().Be("value-$KEY");
            OSUtils.ExpandVariables("$$KEY-$KEY", variables).Should().Be("$KEY-value");
            OSUtils.ExpandVariables("${KEY}-$${KEY}", variables).Should().Be("value-${KEY}");
            OSUtils.ExpandVariables("$${KEY}-${KEY}", variables).Should().Be("${KEY}-value");
        }
    }
}
