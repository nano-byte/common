// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Threading;

/// <summary>
/// Contains test methods for <see cref="WaitHandleExtensions"/>.
/// </summary>
public class WaitHandleExtensionsTest
{
    private static string RandomName() => $"{nameof(WaitHandleExtensionsTest)}-{Guid.NewGuid()}";

    [Fact]
    public void AcquiresAvailableMutex()
    {
        using var mutex = new Mutex(false, RandomName());

        mutex.WaitOne(TestContext.Current.CancellationToken, millisecondsTimeout: 1000);

        mutex.ReleaseMutex(); // Throws if not owned by this thread
    }

    [Fact]
    public void AcquiresAvailableMutexEvenIfCanceled()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        using var mutex = new Mutex(false, RandomName());

        mutex.WaitOne(cancellation.Token, millisecondsTimeout: 1000);

        mutex.ReleaseMutex(); // Throws if not owned by this thread
    }

    [Fact]
    public void AcquiresMutexOnceReleasedElsewhere()
    {
        string name = RandomName();
        using var owner = new Mutex(true, name);

        var waiter = StartWaiter(name, CancellationToken.None, Timeout.Infinite);
        Thread.Sleep(250);
        owner.ReleaseMutex();

        waiter().Should().BeNull();
    }

    [Fact]
    public void ThrowsOnTimeoutForMutexHeldElsewhere()
    {
        string name = RandomName();
        using var owner = new Mutex(true, name);

        var waiter = StartWaiter(name, CancellationToken.None, millisecondsTimeout: 250);
        var exception = waiter();

        owner.ReleaseMutex();

        exception.Should().BeOfType<TimeoutException>();
    }

    [Fact]
    public void ThrowsOnCancelForMutexHeldElsewhere()
    {
        string name = RandomName();
        using var owner = new Mutex(true, name);
        using var cancellation = new CancellationTokenSource();

        var waiter = StartWaiter(name, cancellation.Token, Timeout.Infinite);
        Thread.Sleep(250);
        cancellation.Cancel();
        var exception = waiter();

        owner.ReleaseMutex();

        exception.Should().BeOfType<OperationCanceledException>();
    }

    /// <summary>
    /// Waits for a named mutex on a separate thread, because mutexes are re-entrant within the same thread.
    /// </summary>
    /// <returns>A function that waits for the thread to finish and returns the exception thrown by the wait, if any.</returns>
    private static Func<Exception?> StartWaiter(string name, CancellationToken cancellationToken, int millisecondsTimeout)
    {
        Exception? exception = null;

        var thread = new Thread(() =>
        {
            using var mutex = new Mutex(false, name);
            try
            {
                mutex.WaitOne(cancellationToken, millisecondsTimeout);
                mutex.ReleaseMutex();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }) {IsBackground = true};
        thread.Start();

        return () =>
        {
            thread.Join(10000).Should().BeTrue(because: "The wait should finish.");
            return exception;
        };
    }

    [Fact]
    public void ThrowsOnTimeoutForEvent()
    {
        using var waitHandle = new ManualResetEvent(false);

        Assert.Throws<TimeoutException>(() => waitHandle.WaitOne(TestContext.Current.CancellationToken, millisecondsTimeout: 100));
    }
}
