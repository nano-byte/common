// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Threading;

public class LatestBackgroundResultTest
{
    [Fact]
    public async Task ResultNullBeforeRun()
    {
        var subject = new LatestBackgroundResult<string>();
        subject.Result.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact]
    public async Task ResultAvailableAfterRun()
    {
        var subject = new LatestBackgroundResult<string>();
        subject.Run(_ => "hello");

        await Task.Delay(100, TestContext.Current.CancellationToken);

        subject.Result.Should().Be("hello");
    }

    [Fact]
    public async Task ResultPersistsAfterReading()
    {
        var subject = new LatestBackgroundResult<string>();
        subject.Run(_ => "hello");

        await Task.Delay(100, TestContext.Current.CancellationToken);

        _ = subject.Result;
        subject.Result.Should().Be("hello");
    }

    [Fact]
    public async Task ConsumeResultClearsResult()
    {
        var subject = new LatestBackgroundResult<string>();
        subject.Run(_ => "hello");

        await Task.Delay(100, TestContext.Current.CancellationToken);

        subject.ConsumeResult().Should().Be("hello");
        subject.Result.Should().BeNull();
        subject.ConsumeResult().Should().BeNull();
    }

    [Fact]
    public async Task RunCancelsInProgressComputation()
    {
        var subject = new LatestBackgroundResult<string>();
        bool firstCanceled = false;

        var gate = new TaskCompletionSource<object?>();
        subject.Run(ct =>
        {
            gate.SetResult(null);
            try { Task.Delay(10_000, ct).Wait(ct); }
            catch (OperationCanceledException) { firstCanceled = true; }
            return "first";
        });

        await gate.Task.WaitAsync(TestContext.Current.CancellationToken);
        subject.Run(_ => "second");

        await Task.Delay(200, TestContext.Current.CancellationToken);

        firstCanceled.Should().BeTrue();
        subject.Result.Should().Be("second");
    }

    [Fact]
    public async Task CancelClearsResult()
    {
        var subject = new LatestBackgroundResult<string>();
        subject.Run(_ => "hello");

        await Task.Delay(100, TestContext.Current.CancellationToken);

        subject.Cancel();

        subject.Result.Should().BeNull();
    }

    [Fact]
    public async Task CancelStopsInProgressComputation()
    {
        var subject = new LatestBackgroundResult<string>();
        bool canceled = false;

        var gate = new TaskCompletionSource<object?>();
        subject.Run(ct =>
        {
            gate.SetResult(null);
            try { Task.Delay(10_000, ct).Wait(ct); }
            catch (OperationCanceledException) { canceled = true; }
            return "value";
        });

        await gate.Task.WaitAsync(TestContext.Current.CancellationToken);
        subject.Cancel();

        await Task.Delay(200, TestContext.Current.CancellationToken);

        canceled.Should().BeTrue();
        subject.Result.Should().BeNull();
    }
}
