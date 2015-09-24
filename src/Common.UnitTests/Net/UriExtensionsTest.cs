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

using System;
using FluentAssertions;
using NanoByte.Common.Native;
using NUnit.Framework;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Contains test methods for <see cref="UriExtensions"/>.
    /// </summary>
    [TestFixture]
    public class UriExtensionsTest
    {
        [Test]
        public void TestToStringRfc()
        {
            new Uri("http://test/absolute path").ToStringRfc().Should().Be("http://test/absolute%20path");
            new Uri("relative%20path", UriKind.Relative).ToStringRfc().Should().Be("relative%20path");

            new Uri(WindowsUtils.IsWindows ? @"C:\absolute path" : "/absolute path").ToStringRfc().Should().Be(
                WindowsUtils.IsWindows ? "file:///C:/absolute%20path" : "file:///absolute%20path");
            new Uri("relative path", UriKind.Relative).ToStringRfc().Should().Be(
                "relative path");
        }

        [Test]
        public void TestEnsureTrailingSlashAbsolute()
        {
            new Uri("http://test/test", UriKind.Absolute).EnsureTrailingSlash().Should().Be(
                new Uri("http://test/test/", UriKind.Absolute));
        }

        [Test]
        public void TestEnsureTrailingSlashRelative()
        {
            new Uri("test", UriKind.Relative).EnsureTrailingSlash().Should().Be(
                new Uri("test/", UriKind.Relative));
        }

        [Test]
        public void TestGetLocalFilePath()
        {
            new Uri("http://test/test/my%20file.ext").GetLocalFileName().Should().Be("my file.ext");
            new Uri("file:///test/my%20file.ext").GetLocalFileName().Should().Be("my file.ext");
            new Uri("file:///test/").GetLocalFileName().Should().Be("test");
        }
    }
}
