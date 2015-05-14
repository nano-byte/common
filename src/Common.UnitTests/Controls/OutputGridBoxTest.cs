/*
 * Copyright 2006-2014 Bastian Eicher
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

using System;
using System.ComponentModel;
using System.Linq;
using NUnit.Framework;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// Contains test methods for <see cref="OutputGridBox"/>.
    /// </summary>
    [TestFixture, Ignore("These tests are interactive.")]
    public class OutputGridBoxTest
    {
        public class TestData
        {
            public string Name { get; set; }

            public Uri Uri { get; set; }

            [Browsable(false)]
            public string Internal { get; set; }
        }

        [Test]
        public void TestOutputObjects()
        {
            var data = new[]
            {
                new TestData {Name = "A", Uri = new Uri("http://test/1"), Internal = "abc"},
                new TestData {Name = "B", Uri = new Uri("http://test/2"), Internal = "xyz"}
            };
            OutputGridBox.Show(null, "Title", data.Select(x => x));
        }

        [Test]
        public void TestOutputStrings()
        {
            var data = new[] {"a", "b", "c"};
            OutputGridBox.Show(null, "Title", data.Select(x => x));
        }

        [Test]
        public void TestOutputUris()
        {
            var data = new[] {new Uri("http://a/"), new Uri("http://b/")};
            OutputGridBox.Show(null, "Title", data.Select(x => x));
        }
    }
}
