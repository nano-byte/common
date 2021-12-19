// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

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
