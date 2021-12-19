// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Contains test methods for <see cref="MoveDirectory"/>.
    /// </summary>
    public class MoveDirectoryTest
    {
        /// <summary>
        /// Ensures <see cref="MoveDirectory"/> correctly moves a directories from one location to another.
        /// </summary>
        [Fact]
        public void Normal()
        {
            using var source = CopyDirectoryTest.CreateTestDir();

            using var destination = new TemporaryDirectory("unit-tests");
            Directory.Delete(destination);

            new MoveDirectory(source, destination).Run();
            File.Exists(Path.Combine(destination, "subdir", "file")).Should().BeTrue();
            File.GetLastWriteTimeUtc(Path.Combine(destination, "subdir", "file"))
                .Should().Be(new DateTime(2000, 1, 1), because: "Last-write time for copied file");

            Directory.Exists(source).Should().BeFalse(because: "Original directory should be gone after move");
        }
    }
}
