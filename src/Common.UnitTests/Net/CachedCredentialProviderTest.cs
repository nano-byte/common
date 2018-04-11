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
using System.Net;
using FluentAssertions;
using Moq;
using Xunit;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Contains unit tests for <see cref="CachedCredentialProvider"/>
    /// </summary>
    public class CachedCredentialProviderTest : IDisposable
    {
        private readonly Mock<ICredentialProvider> _innerMock = new Mock<ICredentialProvider>(MockBehavior.Strict);
        private readonly CachedCredentialProvider _provider;

        public CachedCredentialProviderTest() => _provider = new CachedCredentialProvider(_innerMock.Object);

        public void Dispose() => _innerMock.VerifyAll();

        [Fact]
        public void TestGetCredential()
        {
            var uriOuter = new Uri("http://domain/dir/file");
            var uriInner = new Uri("http://domain/dir/");
            var credential = new NetworkCredential("userName", "password");

            _innerMock.Setup(x => x.GetCredential(uriInner, null)).Returns(credential);

            _provider.GetCredential(uriOuter, null).Should().BeSameAs(credential);
            _provider.GetCredential(uriOuter, null).Should().BeSameAs(credential);

            _innerMock.Verify(x => x.GetCredential(uriInner, null), Times.Exactly(1));
        }

        [Fact]
        public void TestReportInvalid()
        {
            var uriOuter = new Uri("http://domain/dir/file");
            var uriInner = new Uri("http://domain/dir/");

            _innerMock.Setup(x => x.ReportInvalid(uriInner));

            _provider.ReportInvalid(uriOuter);
        }
    }
}
