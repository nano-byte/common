/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.IO;
using System.Net;
using System.Threading;
using FluentAssertions;
using NanoByte.Common.Storage;
using NanoByte.Common.Tasks;
using Xunit;
using CancellationTokenSource = NanoByte.Common.Tasks.CancellationTokenSource;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Contains test methods for <see cref="DownloadFile"/>.
    /// </summary>
    public class DownloadFileTest : DownloadTestBase
    {
        private readonly TemporaryFile _tempFile = new TemporaryFile("unit-tests");

        public override void Dispose()
        {
            base.Dispose();
            _tempFile.Dispose();
        }

        [Fact]
        public void TestRun()
        {
            // Download the file
            var download = new DownloadFile(Server.FileUri, _tempFile);
            download.Run();

            // Read the file
            var fileContent = File.ReadAllBytes(_tempFile);

            // Ensure the download was successful and the file is identical
            download.State.Should().Be(TaskState.Complete);
            fileContent.Should().Equal(GetTestFileStream().ToArray());
        }

        [Fact]
        public void TestCancel()
        {
            // Prepare a very slow download of the file and monitor for a cancellation exception
            Server.Slow = true;
            var download = new DownloadFile(Server.FileUri, _tempFile);
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
            var download = new DownloadFile(new Uri(Server.ServerUri, "wrong"), _tempFile);
            download.Invoking(x => x.Run()).Should().Throw<WebException>();
        }

        [Fact]
        public void TestIncorrectSize()
        {
            var download = new DownloadFile(Server.FileUri, _tempFile, bytesTotal: 1024);
            download.Invoking(x => x.Run()).Should().Throw<WebException>();
        }
    }
}
