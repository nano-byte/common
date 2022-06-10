// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Text;
using NanoByte.Common.Native;

namespace NanoByte.Common.Storage;

/// <summary>
/// Contains test methods for <see cref="FileUtils"/>.
/// </summary>
public class FileUtilsTest
{
    [Fact]
    public void TestToNativePath()
        => "a/b".ToNativePath()
                .Should().Be(Path.Combine("a", "b"));

    [Fact]
    public void TestToUnixPath()
        => Path.Combine("a", "b").ToUnixPath()
               .Should().Be("a/b");

    [Fact]
    public void TestIsBreakoutPath()
    {
        FileUtils.IsBreakoutPath(WindowsUtils.IsWindows ? @"C:\test" : "/test").Should().BeTrue(because: "Should detect absolute paths");

        foreach (string path in new[] {"..", "/..", "../", "/../", "a/../b", "../a", "a/.."})
            FileUtils.IsBreakoutPath(path).Should().BeTrue(because: "Should detect parent directory references");

        foreach (string path in new[] {"..a", "a/..a", "a..", "a/a.."})
            FileUtils.IsBreakoutPath(path).Should().BeFalse(because: "Should not trip on '..' as a part of file/directory names");

        FileUtils.IsBreakoutPath("").Should().BeFalse();
    }

    [Fact]
    public void TestRelativeTo()
    {
        if (WindowsUtils.IsWindows)
        {
            new FileInfo(@"C:\test\a\b").RelativeTo(new DirectoryInfo(@"C:\test")).Should().Be(@"a\b");
            new FileInfo(@"C:\test\a\b").RelativeTo(new FileInfo(@"C:\file")).Should().Be(@"test\a\b");
            new DirectoryInfo(@"C:\test1").RelativeTo(new DirectoryInfo(@"C:\test2")).Should().Be(@"..\test1");
        }
        else
        {
            new FileInfo("/test/a/b").RelativeTo(new DirectoryInfo("/test")).Should().Be("a/b");
            new FileInfo("/test/a/b").RelativeTo(new FileInfo("/file")).Should().Be("test/a/b");
            new DirectoryInfo("/test1").RelativeTo(new DirectoryInfo("/test2")).Should().Be("../test1");
        }
    }

    /// <summary>
    /// Ensures <see cref="FileUtils.ExistsCaseSensitive"/> correctly detects case mismatches.
    /// </summary>
    [Fact]
    public void TestExistsCaseSensitive()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        File.WriteAllText(Path.Combine(tempDir, "test"), "");
        FileUtils.ExistsCaseSensitive(Path.Combine(tempDir, "test")).Should().BeTrue();
        FileUtils.ExistsCaseSensitive(Path.Combine(tempDir, "Test")).Should().BeFalse();
    }

    [Fact]
    public void TestTouchNew()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string testFile = Path.Combine(tempDir, "test");
        FileUtils.Touch(testFile);
        File.Exists(testFile).Should().BeTrue();
        File.GetLastWriteTimeUtc(testFile).Should().BeOnOrAfter(DateTime.UtcNow - TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void TestTouchExisting()
    {
        using var tempFile = new TemporaryFile("unit-tests");
        File.SetLastWriteTimeUtc(tempFile, new DateTime(2000, 1, 1));
        FileUtils.Touch(tempFile);
        File.GetLastWriteTimeUtc(tempFile).Should().BeOnOrAfter(DateTime.UtcNow - TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void TestCreate()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string testFile = Path.Combine(tempDir, "test");
        FileUtils.Create(testFile, expectedSize: 8).Dispose();
        File.Exists(testFile).Should().BeTrue();
    }

    /// <summary>
    /// Ensures <see cref="FileUtils.Replace"/> correctly replaces the content of one file with that of another.
    /// </summary>
    [Fact]
    public void TestReplace()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string sourcePath = Path.Combine(tempDir, "source");
        string targetPath = Path.Combine(tempDir, "target");

        File.WriteAllText(sourcePath, @"source");
        File.WriteAllText(targetPath, @"target");
        FileUtils.Replace(sourcePath, targetPath);
        File.ReadAllText(targetPath).Should().Be("source");
    }

    /// <summary>
    /// Ensures <see cref="FileUtils.Replace"/> correctly handles a missing destination file (simply move).
    /// </summary>
    [Fact]
    public void TestReplaceMissing()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string sourcePath = Path.Combine(tempDir, "source");
        string targetPath = Path.Combine(tempDir, "target");

        File.WriteAllText(sourcePath, @"source");
        FileUtils.Replace(sourcePath, targetPath);
        File.ReadAllText(targetPath).Should().Be("source");
    }

    [Fact]
    public void TestReadFirstLine()
    {
        using var tempFile = new TemporaryFile("unit-tests");
        File.WriteAllText(tempFile, "line1\nline2");
        new FileInfo(tempFile).ReadFirstLine(Encoding.ASCII).Should().Be("line1");
    }

    // Interfaces used for mocking delegates
    public interface IActionSimulator<in T>
    {
        void Execute(T obj);
    }

    [Fact]
    public void TestWalk()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string subDirPath = Path.Combine(tempDir, "subdir");
        Directory.CreateDirectory(subDirPath);
        string filePath = Path.Combine(subDirPath, "file");
        File.WriteAllText(filePath, "");
        string symlinkPath = Path.Combine(tempDir, "symlink");
        if (UnixUtils.IsUnix) UnixUtils.CreateSymlink(symlinkPath, ".");
        else Directory.CreateDirectory(symlinkPath);

        // Set up delegate mocks
        var dirCallbackMock = new Mock<IActionSimulator<string>>(MockBehavior.Strict);
        // ReSharper disable once AccessToDisposedClosure
        dirCallbackMock.Setup(x => x.Execute(tempDir)).Verifiable();
        dirCallbackMock.Setup(x => x.Execute(subDirPath)).Verifiable();
        dirCallbackMock.Setup(x => x.Execute(symlinkPath)).Verifiable();
        var fileCallbackMock = new Mock<IActionSimulator<string>>(MockBehavior.Strict);
        fileCallbackMock.Setup(x => x.Execute(filePath)).Verifiable();

        new DirectoryInfo(tempDir).Walk(
            dir => dirCallbackMock.Object.Execute(dir.FullName),
            file => fileCallbackMock.Object.Execute(file.FullName));

        dirCallbackMock.Verify();
        fileCallbackMock.Verify();
    }

    [Fact]
    public void TestWalkSingleFile()
    {
        using var tempFile = new TemporaryFile("unit-tests");

        // Set up delegate mocks
        var dirCallbackMock = new Mock<IActionSimulator<string>>(MockBehavior.Strict);
        var fileCallbackMock = new Mock<IActionSimulator<string>>(MockBehavior.Strict);
        // ReSharper disable once AccessToDisposedClosure
        fileCallbackMock.Setup(x => x.Execute(tempFile)).Verifiable();

        new FileInfo(tempFile).Walk(
            dir => dirCallbackMock.Object.Execute(dir.FullName),
            file => fileCallbackMock.Object.Execute(file.FullName));

        dirCallbackMock.Verify();
        fileCallbackMock.Verify();
    }

    [Fact]
    public void TestWalkThroughPrefix()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string dotFile = Path.Combine(tempDir, ".something");
        string prefix1 = Path.Combine(tempDir, "prefix1");
        string prefix2 = Path.Combine(prefix1, "prefix2");
        string sub = Path.Combine(prefix2, "sub");
        string file = Path.Combine(prefix2, "file");

        FileUtils.Touch(dotFile);
        Directory.CreateDirectory(prefix1);
        Directory.CreateDirectory(prefix2);
        Directory.CreateDirectory(sub);
        FileUtils.Touch(file);

        new DirectoryInfo(tempDir).WalkThroughPrefix().FullName.Should().Be(prefix2);
    }

    [Fact]
    public void TestGetFilesRecursive()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string file1 = Path.Combine(tempDir, "file1");
        string sub = Path.Combine(tempDir, "sub");
        string file2 = Path.Combine(sub, "file2");

        FileUtils.Touch(file1);
        Directory.CreateDirectory(sub);
        FileUtils.Touch(file2);

        FileUtils.GetFilesRecursive(tempDir).Should().Equal(file1, file2);
    }

    [Fact]
    public void TestWriteProtection()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        FileUtils.Touch(Path.Combine(tempDir, "file"));
        if (UnixUtils.IsUnix || (WindowsUtils.IsWindows && WindowsUtils.IsAdministrator))
            FileUtils.CreateSymlink(Path.Combine(tempDir, "symlink"), targetPath: "file");

        FileUtils.EnableWriteProtection(tempDir);
        Assert.Throws<UnauthorizedAccessException>(() => File.Delete(Path.Combine(tempDir, "file")));
        FileUtils.DisableWriteProtection(tempDir);
    }

    [Fact]
    public void TestCreateHardlink()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string sourcePath = Path.Combine(tempDir, "hardlink");
        string destinationPath = Path.Combine(tempDir, "target");
        string copyPath = Path.Combine(tempDir, "copy");

        File.WriteAllText(destinationPath, @"data");
        FileUtils.CreateHardlink(sourcePath, destinationPath);
        File.Exists(sourcePath).Should().BeTrue(because: "Hardlink should look like regular file");
        File.ReadAllText(sourcePath).Should().Be("data", because: "Hardlinked file contents should be equal");
        FileUtils.AreHardlinked(sourcePath, destinationPath).Should().BeTrue();

        File.Copy(sourcePath, copyPath);
        FileUtils.AreHardlinked(sourcePath, copyPath).Should().BeFalse();
    }

    [Fact]
    public void TestIsRegularFile()
    {
        using var tempFile = new TemporaryFile("unit-tests");
        FileUtils.IsRegularFile(tempFile).Should().BeTrue(because: "Regular file should be detected as such");
    }

    [Fact]
    public void TestIsUnixFS()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        if (UnixUtils.IsUnix) FileUtils.IsUnixFS(tempDir).Should().BeTrue(because: "Temp dir should be on Unixoid filesystem on Unixoid OS");
        else FileUtils.IsUnixFS(tempDir).Should().BeFalse(because: "No directory should be Unixoid on a non-Unixoid OS");
    }

    [Fact]
    public void TestWriteReadExtendedMetadata()
    {
        var data = new byte[] {0, 1, 2, 3, 4, 5, 6, 7};
        using var tempFile = new TemporaryFile("unit-tests");
        FileUtils.ReadExtendedMetadata(tempFile, "test-stream").Should().BeNull();

        FileUtils.WriteExtendedMetadata(tempFile, "test-stream", data);
        FileUtils.ReadExtendedMetadata(tempFile, "test-stream").Should().Equal(data);
    }

    [Fact]
    public void TestReadExtendedMetadataExceptionOnMissingBaseFile()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        Assert.Throws<FileNotFoundException>(() => FileUtils.ReadExtendedMetadata(Path.Combine(tempDir, "invalid"), "test-stream"));
    }
}
