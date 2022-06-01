// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Streams;

/// <summary>
/// Contains test methods for <see cref="ShadowingStream"/>.
/// </summary>
public class ShadowingStreamTest
{
    private readonly MemoryStream _underlyingStream = new(new byte[] {0, 1, 2, 3, 4});
    private readonly MemoryStream _shadowStream = new();
    private readonly ShadowingStream _stream;

    public ShadowingStreamTest()
    {
        _stream = new(_underlyingStream, _shadowStream);
    }

    [Fact]
    public void TestRead()
    {
        var buffer = new byte[3];
        _ = _stream.Read(buffer, 0, buffer.Length);
        buffer.Should().Equal(0, 1, 2);
        _shadowStream.ReadAll().Should().Equal(0, 1, 2);
    }
}
