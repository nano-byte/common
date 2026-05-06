// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

public class AtomicWriteTest
{
    [Fact]
    public void TestAtomicWrite()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string filePath = Path.Combine(tempDir, "file");
        byte[] fileData = [1, 2, 3];

        using (var atomic = new AtomicWrite(filePath))
        {
            File.WriteAllBytes(atomic.WritePath, fileData);
            atomic.Commit();
        }

        File.ReadAllBytes(filePath).Should().Equal(fileData);
    }

    [Fact]
    public void TestAtomicWriteOverwriteExisting()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string filePath = Path.Combine(tempDir, "file");
        byte[] oldData = [1, 2, 3];
        byte[] newData = [4, 5, 6];
        File.WriteAllBytes(filePath, oldData);

        using (var atomic = new AtomicWrite(filePath))
        {
            File.WriteAllBytes(atomic.WritePath, newData);
            atomic.Commit();
        }

        File.ReadAllBytes(filePath).Should().Equal(newData);
    }

    [Fact]
    public void TestAtomicWriteRollback()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string filePath = Path.Combine(tempDir, "file");
        byte[] oldData = [1, 2, 3];
        byte[] newData = [4, 5, 6];
        File.WriteAllBytes(filePath, oldData);

        using (var atomic = new AtomicWrite(filePath))
            File.WriteAllBytes(atomic.WritePath, newData);

        File.ReadAllBytes(filePath).Should().Equal(oldData);
    }

    [Fact]
    public void TestAtomicWriteRollbackNoExisting()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string filePath = Path.Combine(tempDir, "file");
        byte[] fileData = [1, 2, 3];

        using (var atomic = new AtomicWrite(filePath))
        {
            File.WriteAllBytes(atomic.WritePath, fileData);
            // atomic.Commit(); // No commit
        }

        File.Exists(filePath).Should().BeFalse();
    }

    [Fact]
    public void TestAtomicRead()
    {
        using var tempDir = new TemporaryDirectory("unit-tests");
        string filePath = Path.Combine(tempDir, "file");

        using (new AtomicRead(filePath))
        {
            // Should be able to nest reads
            using (new AtomicRead(filePath))
            {}
        }
    }
}
