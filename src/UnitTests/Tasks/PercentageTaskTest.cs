// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// Contains test methods for <see cref="PercentageTask"/>.
/// </summary>
public class PercentageTaskTest
{
    [Fact]
    public void TestCallback()
    {
        bool called = false;

        var task = new PercentageTask("Test task", _ => called = true);
        task.Run(TestContext.Current.CancellationToken);

        called.Should().BeTrue();
    }

    [Fact]
    public void TestExceptionPassing()
    {
        new PercentageTask("Test task", _ => throw new IOException("Test exception"))
           .Invoking(x => x.Run(TestContext.Current.CancellationToken)).Should().Throw<IOException>();
        new PercentageTask("Test task", _ => throw new WebException("Test exception"))
           .Invoking(x => x.Run(TestContext.Current.CancellationToken)).Should().Throw<WebException>();
    }
}
