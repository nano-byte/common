// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using NanoByte.Common.Native;
using Xunit;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Contains test methods for <see cref="SubProcess"/>.
    /// </summary>
    public class SubProcessTest
    {
        [Fact]
        public void TestStringOutput()
        {
            string output = new SubProcess(WindowsUtils.IsWindows ? "attrib" : "ls").Run();
            output.Length.Should().BeGreaterThan(1);
        }
    }
}
