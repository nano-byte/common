// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Streams;

/// <summary>
/// Contains test methods for <see cref="SeekBufferStream"/>.
/// </summary>
public class SeekBufferStreamTest
{
    private static readonly byte[] _data = [1, 2, 3, 4, 5];

    [Fact]
    public void SeekBackwards()
    {
        var stream = new SeekBufferStream(new MemoryStream(_data));

        stream.Read(4).Should().Equal(1, 2, 3, 4);
        stream.Seek(-2, SeekOrigin.Current);
        stream.Read(2).Should().Equal(3, 4);
    }

    [Fact]
    public void SeekForwardsAndBackwards()
    {
        var stream = new SeekBufferStream(new MemoryStream(_data));

        stream.Seek(2, SeekOrigin.Current);
        stream.Read(2).Should().Equal(3, 4);
        stream.Seek(0, SeekOrigin.Begin);
        stream.Read(2).Should().Equal(1, 2);
    }

    [Fact]
    public void SmallBuffer()
    {
        var stream = new SeekBufferStream(new MemoryStream(_data), bufferSize: 2);

        stream.Read(4).Should().Equal(1, 2, 3, 4);
        stream.Seek(-2, SeekOrigin.Current);
        stream.Read(2).Should().Equal(3, 4);
    }

    [Fact]
    public void CannotSeekBackwardsTooFar()
    {
        var stream = new SeekBufferStream(new MemoryStream(_data), bufferSize: 2);

        stream.Read(4);
        stream.Seek(-4, SeekOrigin.Current);
        stream.Invoking(x => x.Read(2))
              .Should().Throw<IOException>();
    }

    [Fact]
    public void CanSekForwardsBeyondBuffer()
    {
        var stream = new SeekBufferStream(new MemoryStream(_data), bufferSize: 2);

        stream.Seek(3, SeekOrigin.Current);
        stream.Read(2).Should().Equal(4, 5);
    }

    [Fact]
    public void CanSekForwardsWithZeroBuffer()
    {
        var stream = new SeekBufferStream(new MemoryStream(_data), bufferSize: 0);

        stream.Seek(3, SeekOrigin.Current);
        stream.Read(2).Should().Equal(4, 5);
    }
}
