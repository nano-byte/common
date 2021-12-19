// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Storage;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Contains test methods for <see cref="StreamUtils"/>.
    /// </summary>
    public class StreamUtilsTest
    {
        [Fact]
        public void TestReadArray()
        {
            new MemoryStream(new byte[] {1, 2, 3, 4, 5}).Read(3).Should().Equal(1, 2, 3);
        }

        [Fact]
        public void TestReadSegment()
        {
            var segment = new ArraySegment<byte>(new byte[5], 1, 3);
            new MemoryStream(new byte[] {1, 2, 3, 4, 5}).Read(segment);
            segment.Should().Equal(1, 2, 3);
        }

        [Fact]
        public void TestReadAll()
        {
            var stream = new MemoryStream(new byte[] {1, 2, 3});
            stream.ReadAll().Should().Equal(1, 2, 3);
        }

        [Fact]
        public void TestSkipSeekable()
        {
            var stream = new MemoryStream(new byte[] {1, 2, 3, 4});
            stream.Skip(2);
            stream.Read(2).Should().Equal(3, 4);
        }

        [Fact]
        public void TestSkipNonSeekable()
        {
            var stream = new NonSeekableStream(new MemoryStream(new byte[] {1, 2, 3, 4}));
            stream.Skip(2);
            stream.Read(2).Should().Equal(3, 4);
        }

        [Fact]
        public static void TestWriteArray()
        {
            var stream = new MemoryStream();
            stream.Write(1, 2, 3);
            stream.ReadAll().Should().Equal(1, 2, 3);
        }

        [Fact]
        public static void TestWriteSegment()
        {
            var stream = new MemoryStream();
            stream.Write(new ArraySegment<byte>(new byte[] {1, 2, 3}, 1, 2));
            stream.ReadAll().Should().Equal(2, 3);
        }

        [Fact]
        public void TestAsArrayCopy()
        {
            var stream = new MemoryStream();
            stream.Write(1, 2, 3);
            stream.AsArray().Should().Equal(1, 2, 3);
        }

        [Fact]
        public void TestAsArrayNoCopy()
        {
            byte[] buffer = {1, 2, 3};
            var stream = new MemoryStream(buffer, 0, buffer.Length, true, true);
            stream.AsArray().Should().BeSameAs(buffer);
        }

        [Fact]
        public void TestToMemory()
        {
            var stream = new MemoryStream();
            stream.Write(1,2,3);

            stream = stream.ToMemory();
            stream.AsArray().Should().Equal(1, 2, 3);
            stream.Capacity.Should().Be(3);
        }

        [Fact]
        public void TestCopyToFile()
        {
            using var tempFile = new TemporaryFile("unit-tests");
            "abc".ToStream().CopyToFile(tempFile);
            new FileInfo(tempFile).ReadFirstLine(EncodingUtils.Utf8).Should().Be("abc");
        }

        [Fact]
        public void TestString()
        {
            const string test = "Test";
            using var stream = test.ToStream();
            stream.ReadToString().Should().Be(test);
        }
    }
}
