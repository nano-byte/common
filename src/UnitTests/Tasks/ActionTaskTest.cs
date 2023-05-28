// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// Contains test methods for <see cref="ActionTask"/>.
/// </summary>
public class ActionTaskTest
{
    [Fact]
    public void TestCallback()
    {
        bool called = false;

        var task = new ActionTask("Test task", () => called = true);
        task.Run();

        called.Should().BeTrue();
    }

    [Fact]
    public void TestExceptionPassing()
    {
        new ActionTask("Test task", () => throw new IOException("Test exception"))
           .Invoking(x => x.Run()).Should().Throw<IOException>();
        new ActionTask("Test task", () => throw new WebException("Test exception"))
           .Invoking(x => x.Run()).Should().Throw<WebException>();
    }
}
