/*
 * Copyright 2006-2017 Bastian Eicher
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

using System.Collections.Generic;
using System.Collections.Specialized;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Contains test methods for <see cref="UnixUtils"/>.
    /// </summary>
    public class UnixUtilsTest
    {
        [Fact]
        public void TestExpandUnixVariablesCaseSensitive()
        {
            // Environment variables are case-insensitive on some OSes
            var variables = new Dictionary<string, string> {{"KEY1", "value1"}, {"KEY2", "value2"}, {"LONG KEY", "long value"}};

            UnixUtils.ExpandVariables("$KEY1$KEY2/$KEY1 $KEY2 ${LONG KEY} $UNSET ${UNSET:-default}", variables).Should().Be("value1value2/value1 value2 long value  default");
            UnixUtils.ExpandVariables("$KEY1-bla", variables).Should().Be("value1-bla");
            UnixUtils.ExpandVariables("$key1", variables).Should().Be("");
            UnixUtils.ExpandVariables("{bla-$KEY1-bla}", variables).Should().Be("{bla-value1-bla}");
            UnixUtils.ExpandVariables("{bla-${KEY1}-bla}", variables).Should().Be("{bla-value1-bla}");
            UnixUtils.ExpandVariables("", variables).Should().Be("");
        }

        [Fact]
        public void TestExpandUnixVariablesCaseInsensitive()
        {
            // StringDictionary is case-insensitive
            var variables = new StringDictionary
            {
                {"key1", "value1"}, {"key2", "value2"}, {"long key", "long value"}
            };

            UnixUtils.ExpandVariables("$KEY1$KEY2/$KEY1 $KEY2 ${LONG KEY} $UNSET ${UNSET:-default}", variables).Should().Be("value1value2/value1 value2 long value  default");
            UnixUtils.ExpandVariables("$KEY1-bla", variables).Should().Be("value1-bla");
            UnixUtils.ExpandVariables("", variables).Should().Be("");
        }
    }
}
