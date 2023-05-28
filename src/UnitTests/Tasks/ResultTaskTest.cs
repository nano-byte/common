// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// Contains test methods for <see cref="ResultTask{T}"/>.
/// </summary>
public class ResultTaskTest
{
    [Fact]
    public void TestCallback()
    {
        var task = ResultTask.Create("Test task", () => true);
        task.Run();

        task.Result.Should().BeTrue();
    }

    [Fact]
    public void TestExceptionPassing()
    {
        new ResultTask<bool>("Test task", () => throw new IOException("Test exception"))
           .Invoking(x => x.Run()).Should().Throw<IOException>();
        new ResultTask<bool>("Test task", () => throw new WebException("Test exception"))
           .Invoking(x => x.Run()).Should().Throw<WebException>();
    }
}
