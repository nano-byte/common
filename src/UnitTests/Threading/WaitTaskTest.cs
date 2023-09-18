// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Tasks;

namespace NanoByte.Common.Threading;

/// <summary>
/// Contains test methods for <see cref="WaitTask"/>.
/// </summary>
public class WaitTaskTest
{
    [Fact]
    public async Task TestWait()
    {
        using var waitHandle = new ManualResetEvent(false);
        var task = new WaitTask("Test task", waitHandle);
        var waitTask = Task.Run(() => task.Run());

        waitHandle.Set();
        await waitTask;
        task.State.Should().Be(TaskState.Complete);
    }

    [Fact]
    public async Task TestCancel()
    {
        using var waitHandle = new ManualResetEvent(false);
        var task = new WaitTask("Test task", waitHandle);
        bool exceptionThrown = false;
        var cancellationTokenSource = new CancellationTokenSource();
        var waitTask = Task.Run(() =>
        {
            try
            {
                task.Run(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                exceptionThrown = true;
            }
        });

        // Start and then cancel the download
        Thread.Sleep(100);
        cancellationTokenSource.Cancel();
        await waitTask;

        exceptionThrown.Should().BeTrue(because: task.State.ToString());
    }

    [Fact]
    public async Task TestCancelNoHandle()
    {
        var task = new WaitTask("Test task");
        bool exceptionThrown = false;
        var cancellationTokenSource = new CancellationTokenSource();
        var waitTask = Task.Run(() =>
        {
            try
            {
                task.Run(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                exceptionThrown = true;
            }
        });

        // Start and then cancel the download
        Thread.Sleep(100);
        cancellationTokenSource.Cancel();
        await waitTask;

        exceptionThrown.Should().BeTrue(because: task.State.ToString());
    }
}
