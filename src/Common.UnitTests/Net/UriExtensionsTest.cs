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
using NanoByte.Common.Native;
using NUnit.Framework;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Contains test methods for <see cref="UriExtensions"/>.
    /// </summary>
    [TestFixture]
    public class UriExtensions
    {
        [Test]
        public void TestToStringRfc()
        {
            Assert.AreEqual(
                expected: "http://test/absolute%20path",
                actual: new Uri("http://test/absolute path").ToStringRfc());
            Assert.AreEqual(
                expected: "relative%20path",
                actual: new Uri("relative%20path", UriKind.Relative).ToStringRfc());


            Assert.AreEqual(
                expected: WindowsUtils.IsWindows ? "file:///C:/absolute%20path" : "file:///absolute%20path",
                actual: new Uri(WindowsUtils.IsWindows ? @"C:\absolute path" : "/absolute path").ToStringRfc());
            Assert.AreEqual(
                expected: "relative path",
                actual: new Uri("relative path", UriKind.Relative).ToStringRfc());
        }

        [Test]
        public void TestEnsureTrailingSlash()
        {
            Assert.AreEqual(
                expected: new Uri("http://test/test/"),
                actual: new Uri("http://test/test").EnsureTrailingSlash());
        }

        [Test]
        public void TestGetLocalFilePath()
        {
            Assert.AreEqual(
                expected: "my file.ext",
                actual: new Uri("http://test/test/my%20file.ext").GetLocalFileName());
            Assert.AreEqual(
                expected: "my file.ext",
                actual: new Uri("file:///test/my%20file.ext").GetLocalFileName());
            Assert.AreEqual(
                expected: "test",
                actual: new Uri("file:///test/").GetLocalFileName());
        }
    }
}