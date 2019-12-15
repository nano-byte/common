// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Tasks
{
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
            var waitThread = new Thread(() => task.Run());
            waitThread.Start();

            Thread.Sleep(100);
            task.State.Should().Be(TaskState.Started);

            waitHandle.Set();
            waitThread.Join();
            task.State.Should().Be(TaskState.Complete);
        }

        [Fact]
        public void TestCancel()
        {
            using var waitHandle = new ManualResetEvent(false);
            // Monitor for a cancellation exception
            var task = new WaitTask("Test task", waitHandle);
            bool exceptionThrown = false;
            var cancellationTokenSource = new CancellationTokenSource();
            var waitThread = new Thread(() =>
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
            waitThread.Start();
            Thread.Sleep(100);
            cancellationTokenSource.Cancel();
            waitThread.Join();

            exceptionThrown.Should().BeTrue(because: task.State.ToString());
        }
    }
}
