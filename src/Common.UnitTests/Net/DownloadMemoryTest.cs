// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Net;
using System.Threading;
using FluentAssertions;
using NanoByte.Common.Tasks;
using Xunit;
using CancellationTokenSource = NanoByte.Common.Tasks.CancellationTokenSource;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Contains test methods for <see cref="DownloadMemory"/>.
    /// </summary>
    public class DownloadMemoryTest : DownloadTestBase
    {
        [Fact]
        public void TestRun()
        {
            // Download the file
            var download = new DownloadMemory(Server.FileUri);
            download.Run();

            // Ensure the download was successful and the file is identical
            download.State.Should().Be(TaskState.Complete);
            download.GetData().Should().Equal(GetTestFileStream().ToArray());
        }

        [Fact]
        public void TestCancel()
        {
            // Prepare a very slow download of the file and monitor for a cancellation exception
            Server.Slow = true;
            var download = new DownloadMemory(Server.FileUri);
            bool exceptionThrown = false;
            var cancellationTokenSource = new CancellationTokenSource();
            var downloadThread = new Thread(() =>
            {
                try
                {
                    download.Run(cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    exceptionThrown = true;
                }
            });

            // Start and then cancel the download
            downloadThread.Start();
            Thread.Sleep(100);
            cancellationTokenSource.Cancel();
            downloadThread.Join();

            exceptionThrown.Should().BeTrue(because: "Should throw OperationCanceledException");
        }

        [Fact]
        public void TestFileMissing()
        {
            var download = new DownloadMemory(new Uri(Server.ServerUri, "wrong"));
            download.Invoking(x => x.Run()).Should().Throw<WebException>();
        }

        [Fact]
        public void TestIncorrectSize()
        {
            var download = new DownloadMemory(Server.FileUri, bytesTotal: 1024);
            download.Invoking(x => x.Run()).Should().Throw<WebException>();
        }
    }
}
