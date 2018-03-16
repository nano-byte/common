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

using System.IO;
using System.Net;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Contains test methods for <see cref="SimpleTask"/>.
    /// </summary>
    public class SimpleTaskTest
    {
        [Fact]
        public void TestCallback()
        {
            bool called = false;

            var task = new SimpleTask("Test task", () => called = true);
            task.Run();

            called.Should().BeTrue();
        }

        [Fact]
        public void TestExceptionPassing()
        {
            new SimpleTask("Test task", () => throw new IOException("Test exception"))
                .Invoking(x => x.Run()).Should().Throw<IOException>();
            new SimpleTask("Test task", () => throw new WebException("Test exception"))
                .Invoking(x => x.Run()).Should().Throw<WebException>();
        }
    }
}
