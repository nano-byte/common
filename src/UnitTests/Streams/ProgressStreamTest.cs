// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Streams;

/// <summary>
/// Contains test methods for <see cref="ProgressStream"/>.
/// </summary>
public class ProgressStreamTest
{
    [Fact]
    public void TestReadProgress()
    {
        var progressMock = new Mock<IProgress<long>>();
        var stream = new ProgressStream(new MemoryStream(new byte[5]), progressMock.Object);
        byte[] buffer = new byte[3];

        _ = stream.Read(buffer, 0, 3);
        progressMock.Verify(x => x.Report(3));

        _ = stream.Read(buffer, 0, 2);
        progressMock.Verify(x => x.Report(5));
    }

    [Fact]
    public void TestWriteProgress()
    {
        var progressMock = new Mock<IProgress<long>>();
        var stream = new ProgressStream(new MemoryStream(new byte[5]), progressMock.Object);

        stream.Write(new byte[3]);
        progressMock.Verify(x => x.Report(3));

        stream.Write(new byte[2]);
        progressMock.Verify(x => x.Report(5));
    }

    [Fact]
    public void TestReadCancellation()
    {
        var cts = new CancellationTokenSource();
        var stream = new ProgressStream(new MemoryStream(new byte[5]), cancellationToken: cts.Token);
        byte[] buffer = new byte[2];

        _ = stream.Read(buffer, 0, 2);

        cts.Cancel();
        stream.Invoking(x => x.Read(buffer, 0, 2))
              .Should().Throw<OperationCanceledException>();
    }

    [Fact]
    public void TestWriteCancellation()
    {
        var cts = new CancellationTokenSource();
        var stream = new ProgressStream(new MemoryStream(), cancellationToken: cts.Token);

        stream.Write(new byte[2]);

        cts.Cancel();
        stream.Invoking(x => x.Write(new byte[2]))
              .Should().Throw<OperationCanceledException>();
    }
}
