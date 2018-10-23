// Copyright Bastian Eicher
// Licensed under the MIT License

using System.IO;
using System.Text;
using FluentAssertions;
using NanoByte.Common.Storage;
using Xunit;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Contains test methods for <see cref="StreamUtils"/>.
    /// </summary>
    public class StreamUtilsTest
    {
        [Fact]
        public void TestRead()
            => new MemoryStream(new byte[] {1, 2, 3, 4, 5}).Read(3).Should().Equal(1, 2, 3);

        [Fact]
        public void TestToArray()
        {
            Stream stream = new MemoryStream(new byte[] {1, 2, 3});
            stream.ToArray().Should().Equal(1, 2, 3);
        }

        [Fact]
        public static void TestWrite()
        {
            var stream = new MemoryStream();
            stream.Write(new byte[] {1, 2, 3});

            stream.ToArray().Should().Equal(1, 2, 3);
        }

        [Fact]
        public void TestContentEquals()
        {
            "abc".ToStream().ContentEquals("abc".ToStream()).Should().BeTrue();
            "ab".ToStream().ContentEquals("abc".ToStream()).Should().BeFalse();
            "abc".ToStream().ContentEquals("ab".ToStream()).Should().BeFalse();
            "abc".ToStream().ContentEquals("".ToStream()).Should().BeFalse();
        }

        [Fact]
        public void TestString()
        {
            const string test = "Test";
            using (var stream = test.ToStream())
                stream.ReadToString().Should().Be(test);
        }

        [Fact]
        public void TestCopyToFile()
        {
            using (var tempFile = new TemporaryFile("unit-tests"))
            {
                "abc".ToStream().CopyToFile(tempFile);
                new FileInfo(tempFile).ReadFirstLine(Encoding.UTF8).Should().Be("abc");
            }
        }
    }
}
