/*
 * Copyright 2006-2015 Bastian Eicher, Simon E. Silva Lauinger
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
using FluentAssertions;
using NanoByte.Common.Native;
using Xunit;

namespace NanoByte.Common
{
    /// <summary>
    /// Contains test methods for <see cref="AppMutex"/>.
    /// </summary>
    public class AppMutexTest
    {
        /// <summary>
        /// Ensures the methods <see cref="AppMutex.Probe"/>, <see cref="AppMutex.Create(string,out AppMutex)"/> and <see cref="AppMutex.Close"/> work correctly together.
        /// </summary>
        [SkippableFact]
        public void TestProbeCreateClose()
        {
            Skip.IfNot(WindowsUtils.IsWindowsNT, reason: "AppMutexes are only available on the Windows NT platform.");

            string mutexName = "unit-tests-" + Path.GetRandomFileName();
            AppMutex.Probe(mutexName).Should().BeFalse();
            AppMutex.Create(mutexName, out AppMutex mutex);
            AppMutex.Probe(mutexName).Should().BeTrue();
            mutex.Close();
            AppMutex.Probe(mutexName).Should().BeFalse();
        }
    }
}
