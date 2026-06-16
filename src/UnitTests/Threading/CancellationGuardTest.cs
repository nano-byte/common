// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Threading;

public class CancellationGuardTest
{
    [Fact]
    public async Task CancelBlocksUntilDispose()
    {
        using var cts = new CancellationTokenSource();
        var guard = new CancellationGuard(cts.Token);

        var cancelTask = Task.Run(cts.Cancel, TestContext.Current.CancellationToken);

        await Task.Delay(100, TestContext.Current.CancellationToken);
        cancelTask.IsCompleted.Should().BeFalse(because: "Cancel should be blocked while guard is alive");

        guard.Dispose();
        await cancelTask;
        cancelTask.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task CancelTimesOutEvenWithoutDispose()
    {
        using var cts = new CancellationTokenSource();
        using var guard = new CancellationGuard(cts.Token, TimeSpan.FromMilliseconds(100));

        var cancelTask = Task.Run(cts.Cancel, TestContext.Current.CancellationToken);

#if NET
        await cancelTask.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
#else
        await cancelTask;
#endif

        cancelTask.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void DisposeWithoutCancelDoesNotThrow()
    {
        using var cts = new CancellationTokenSource();
        // ReSharper disable once NotDisposedResource
        var guard = new CancellationGuard(cts.Token);

        guard.Invoking(g => g.Dispose()).Should().NotThrow();
    }
}
