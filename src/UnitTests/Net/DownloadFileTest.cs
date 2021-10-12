// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NanoByte.Common.Storage;
using NanoByte.Common.Streams;
using NanoByte.Common.Tasks;
using Xunit;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Contains test methods for <see cref="DownloadFile"/>.
    /// </summary>
    public class DownloadFileTest : IDisposable
    {
        private static MemoryStream GetTestFileStream() => "abc".ToStream();

        private readonly MicroServer _server = new("file", GetTestFileStream());

        public void Dispose()
        {
            _server.Dispose();
        }

        [Fact]
        public void TestRun()
        {
            // Download the file
            ArraySegment<byte> data = default;
            var download = new DownloadFile(_server.FileUri, stream => data = stream.ReadAll());
            download.Run();

            // Ensure the download was successful and the file is identical
            download.State.Should().Be(TaskState.Complete);
            data.Should().Equal(GetTestFileStream().ReadAll());
        }

        [Fact]
        public void TestRunFile()
        {
            using var tempFile = new TemporaryFile("unit-tests");

            // Download the file
            var download = new DownloadFile(_server.FileUri, tempFile);
            download.Run();

            // Ensure the download was successful and the file is identical
            download.State.Should().Be(TaskState.Complete);
            File.ReadAllBytes(tempFile).Should().Equal(GetTestFileStream().ReadAll());
        }

        [Fact]
        public void TestCancel()
        {
            // Prepare a very slow download of the file and monitor for a cancellation exception
            _server.Slow = true;
            var download = new DownloadFile(_server.FileUri, stream => stream.ReadAll());
            bool exceptionThrown = false;
            var cancellationTokenSource = new CancellationTokenSource();
            var downloadTask = Task.Run(() =>
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
            Thread.Sleep(100);
            cancellationTokenSource.Cancel();
            downloadTask.Wait();

            exceptionThrown.Should().BeTrue(because: "Should throw OperationCanceledException");
        }

        [Fact]
        public void TestFileMissing()
        {
            var download = new DownloadFile(new Uri(_server.ServerUri, "wrong"), stream => stream.ReadAll());
            download.Invoking(x => x.Run()).Should().Throw<WebException>();
        }

        [Fact]
        public void TestIncorrectSize()
        {
            var download = new DownloadFile(_server.FileUri, stream => stream.ReadAll(), bytesTotal: 1024);
            download.Invoking(x => x.Run()).Should().Throw<WebException>();
        }
    }
}
