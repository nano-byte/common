// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
namespace NanoByte.Common.Threading;

public class ResultRacerTest
{
    [Fact(Skip = "Slow")]
    public void Sync()
    {
        var racer = ResultRacer.For([1, 2, 3], (i, _) =>
        {
            Thread.Sleep(i * 100);
            return i;
        }, TestContext.Current.CancellationToken);
        racer.GetResult().Should().Be(1);
    }

    [Fact(Skip = "Slow")]
    public void SyncCancellation()
    {
        using var alreadyCancelled = new CancellationTokenSource(millisecondsDelay: 0);
        var racer = ResultRacer.For([1], (i, _) =>
        {
            Thread.Sleep(100);
            return i;
        }, alreadyCancelled.Token);
        racer.Invoking(x => x.GetResult())
             .Should().Throw<OperationCanceledException>();
    }

    [Fact(Skip = "Slow")]
    public async Task Async()
    {
        var racer = ResultRacer.For([1, 2, 3], async (i, cancellationToken) =>
        {
            await Task.Delay(i * 100, cancellationToken);
            return i;
        }, TestContext.Current.CancellationToken);
        (await racer.GetResultAsync()).Should().Be(1);
    }

    [Fact(Skip = "Slow")]
    public async Task AsyncCancellation()
    {
        using var alreadyCancelled = new CancellationTokenSource(millisecondsDelay: 0);
        var racer = ResultRacer.For([1], async (i, cancellationToken) =>
        {
            await Task.Delay(100, cancellationToken);
            return i;
        }, alreadyCancelled.Token);
        await racer.Awaiting(x => x.GetResultAsync())
                   .Should().ThrowAsync<OperationCanceledException>();
    }
}
#endif
