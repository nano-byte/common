// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

public class TemporaryDirectoryTest
{
    [Fact]
    public void CreatesDirectory()
    {
        using var temp = new TemporaryDirectory("unit-tests");
        Directory.Exists(temp).Should().BeTrue();
    }

    [Fact]
    public void CreatesEmptyDirectory()
    {
        using var temp = new TemporaryDirectory("unit-tests");
        Directory.GetFileSystemEntries(temp).Should().BeEmpty();
    }
}
