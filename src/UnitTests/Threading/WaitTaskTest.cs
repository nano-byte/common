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
    public void TestWait()
    {
        using var waitHandle = new ManualResetEvent(false);
        var task = new WaitTask("Test task", waitHandle);
        var waitTask = Task.Run(() => task.Run());

        waitHandle.Set();
        waitTask.Wait();
        task.State.Should().Be(TaskState.Complete);
    }

    [Fact]
    public void TestCancel()
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
        waitTask.Wait();

        exceptionThrown.Should().BeTrue(because: task.State.ToString());
    }
}