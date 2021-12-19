// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Threading;

public class JobQueueTest
{
    [Fact]
    public async Task RunsWork()
    {
        bool called = false;

        var workQueue = new JobQueue();
        workQueue.Enqueue(() => called = true);

        await Task.Delay(100);

        called.Should().BeTrue();
    }
}