// Copyright Bastian Eicher
// Licensed under the MIT License

using System.IO;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Contains test methods for <see cref="AppMutex"/>.
    /// </summary>
    public class AppMutexTest
    {
        /// <summary>
        /// Ensures the methods <see cref="AppMutex.Probe"/>, <see cref="AppMutex.Create"/> and <see cref="AppMutex.Close"/> work correctly together.
        /// </summary>
        [SkippableFact]
        public void TestProbeCreateClose()
        {
            Skip.IfNot(WindowsUtils.IsWindowsNT, reason: "AppMutexes are only available on the Windows NT platform.");

            string mutexName = "unit-tests-" + Path.GetRandomFileName();
            AppMutex.Probe(mutexName).Should().BeFalse();
            var mutex = AppMutex.Create(mutexName);
            AppMutex.Probe(mutexName).Should().BeTrue();
            mutex.Close();
            AppMutex.Probe(mutexName).Should().BeFalse();
        }
    }
}
