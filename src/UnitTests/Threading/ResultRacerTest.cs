// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
namespace NanoByte.Common.Threading;

public class ResultRacerTest
{
    [Fact]
    public void Sync()
    {
        var racer = ResultRacer.For([1, 2, 3], (i, _) =>
        {
            Thread.Sleep(i * 100);
            return i;
        });
        racer.GetResult().Should().Be(1);
    }

    [Fact]
    public void SyncCancellation()
    {
        var racer = ResultRacer.For([1], (i, _) =>
        {
            Thread.Sleep(100);
            return i;
        }, new CancellationTokenSource(0).Token);
        racer.Invoking(x => x.GetResult())
             .Should().Throw<OperationCanceledException>();
    }

    [Fact]
    public async Task Async()
    {
        var racer = ResultRacer.For([1, 2, 3], async (i, cancellationToken) =>
        {
            await Task.Delay(i * 100, cancellationToken);
            return i;
        });
        (await racer.GetResultAsync()).Should().Be(1);
    }

    [Fact]
    public async Task AsyncCancellation()
    {
        var racer = ResultRacer.For([1], async (i, cancellationToken) =>
        {
            await Task.Delay(100, cancellationToken);
            return i;
        }, new CancellationTokenSource(0).Token);
        await racer.Awaiting(x => x.GetResultAsync())
                   .Should().ThrowAsync<OperationCanceledException>();
    }
}
#endif
