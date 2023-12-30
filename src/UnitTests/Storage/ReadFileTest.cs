// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Streams;

namespace NanoByte.Common.Storage;

/// <summary>
/// Contains test methods for <see cref="ReadFile"/>.
/// </summary>
public class ReadFileTest
{
    [Fact]
    public void TestRun()
    {
        using var tempFile = new TemporaryFile("unit-tests");
        File.WriteAllBytes(tempFile, [1, 2, 3]);

        ArraySegment<byte> data = default;
        new ReadFile(tempFile, stream => data = stream.ReadAll()).Run();
        data.Should().Equal(1, 2, 3);
    }
}