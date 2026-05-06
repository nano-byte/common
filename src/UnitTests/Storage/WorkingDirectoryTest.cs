// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

[Collection("WorkingDir")]
public class WorkingDirectoryTest
{
    [Fact]
    public void ChangesAndRestoresWorkingDirectory()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string original = Directory.GetCurrentDirectory();

        string changedTo;
        using (new WorkingDirectory(tempDir))
            changedTo = Directory.GetCurrentDirectory();

        changedTo.Should().NotBe(original);
        Directory.GetCurrentDirectory().Should().Be(original);
    }

    [Fact]
    public void TemporaryWorkingDirectoryUsesItsOwnPath()
    {
        string original = Directory.GetCurrentDirectory();

        string changedTo;
        string tempPath;
        using (var temp = new TemporaryWorkingDirectory("unit-tests"))
        {
            changedTo = Directory.GetCurrentDirectory();
            tempPath = temp.Path;
            Directory.Exists(temp.Path).Should().BeTrue();
        }

        changedTo.Should().NotBe(original);
        Directory.GetCurrentDirectory().Should().Be(original);
    }

    [Fact]
    public void TemporaryWorkingDirectoryIsDeletedOnDispose()
    {
        string path;
        using (var temp = new TemporaryWorkingDirectory("unit-tests"))
            path = temp.Path;

        Directory.Exists(path).Should().BeFalse();
    }
}
