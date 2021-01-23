// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using NanoByte.Common.Native;
using Xunit;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Contains test methods for <see cref="ChildProcess"/>.
    /// </summary>
    public class ChildProcessTest
    {
        [Fact]
        public void TestStringOutput()
        {
            var control = new DummyControl();
            string output = control.Execute();
            output.Length.Should().BeGreaterThan(1);
        }

        private class DummyControl : ChildProcess
        {
            protected override string AppBinary => WindowsUtils.IsWindows ? "help" : "ls";
        }
    }
}
