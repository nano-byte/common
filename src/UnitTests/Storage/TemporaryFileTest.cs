// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

public class TemporaryFileTest
{
    [Fact]
    public void CreatesFile()
    {
        using var temp = new TemporaryFile("unit-tests");
        File.Exists(temp).Should().BeTrue();
    }

    [Fact]
    public void CreatesEmptyFile()
    {
        using var temp = new TemporaryFile("unit-tests");
        new FileInfo(temp).Length.Should().Be(0);
    }
}
