// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// Contains test methods for <see cref="ForEachTask{T}"/>.
/// </summary>
public class ForEachTaskTest
{
    [Fact]
    public void TestCallback()
    {
        var target = new[] {"element1", "element2", "element2"};
        var calledFor = new List<string>();

        var task = ForEachTask.Create("Test task", target, calledFor.Add);
        task.Run();

        calledFor.Should().Equal(target);
    }

    [Fact]
    public void TestExceptionPassing()
    {
        ForEachTask.Create("Test task", [""], _ => throw new IOException("Test exception"))
                   .Invoking(x => x.Run())
                   .Should().Throw<IOException>();
        ForEachTask.Create("Test task", [""], _ => throw new WebException("Test exception"))
                   .Invoking(x => x.Run())
                   .Should().Throw<WebException>();
    }

    [Fact]
    public void TestRollbackException()
    {
        var applyCalledFor = new List<int>();
        var rollbackCalledFor = new List<int>();
        ForEachTask.Create("Test task", [1, 2, 3],
                        action: value =>
                        {
                            applyCalledFor.Add(value);
                            if (value == 2) throw new ArgumentException("Test exception");
                        },
                        rollback: rollbackCalledFor.Add)
                   .Invoking(x => x.Run())
                   .Should().Throw<ArgumentException>(because: "Exceptions should be passed through after rollback.");

        applyCalledFor.Should().Equal(1, 2);
        rollbackCalledFor.Should().Equal(2, 1);
    }

    [Fact]
    public void TestRollbackCancellation()
    {
        var cts = new CancellationTokenSource();

        var applyCalledFor = new List<int>();
        var rollbackCalledFor = new List<int>();
        ForEachTask.Create("Test task", [1, 2, 3],
                        action: value =>
                        {
                            applyCalledFor.Add(value);
                            if (value == 2) cts.Cancel();
                        },
                        rollback: rollbackCalledFor.Add)
                   .Invoking(x => x.Run(cts.Token))
                   .Should().Throw<OperationCanceledException>();

        applyCalledFor.Should().Equal(1, 2);
        rollbackCalledFor.Should().Equal(2, 1);
    }
}
