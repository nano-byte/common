// Copyright Bastian Eicher
// Licensed under the MIT License

using System.IO;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Contains test methods for <see cref="OffsetStream"/>.
    /// </summary>
    public class OffsetStreamTest
    {
        private readonly MemoryStream _underlyingStream = new(new byte[] {0, 1, 2, 3, 4});

        [Fact]
        public void TestPosition()
        {
            var stream = new OffsetStream(_underlyingStream);
            stream.ApplyOffset(2);
            stream.Position = 2;
            _underlyingStream.Position.Should().Be(4);
        }

        [Fact]
        public void TestSeekBegin()
        {
            var stream = new OffsetStream(_underlyingStream);
            stream.ApplyOffset(2);
            stream.Position = stream.Length - 1;
            stream.Seek(2, SeekOrigin.Begin).Should().Be(2);
            _underlyingStream.Position.Should().Be(4);
        }

        [Fact]
        public void TestSeekCurrent()
        {
            var stream = new OffsetStream(_underlyingStream);
            stream.ApplyOffset(2);
            stream.Position = 2;
            stream.Seek(1, SeekOrigin.Current).Should().Be(3);
            _underlyingStream.Position.Should().Be(5);
        }

        [Fact]
        public void TestReadAll()
        {
            var stream = new OffsetStream(_underlyingStream);
            stream.ApplyOffset(2);
            stream.ReadAll()
                   .Should().Equal(2, 3, 4);
        }

        [Fact]
        public void TestReadAllWithoutSeek()
        {
            var stream = new OffsetStream(new NonSeekableStream(_underlyingStream));
            stream.ApplyOffset(2);

            stream.ReadAll()
                  .Should().Equal(2, 3, 4);
        }

        private class NonSeekableStream : DelegatingStream
        {
            public NonSeekableStream(Stream underlyingStream) : base(underlyingStream)
            {}

            public override bool CanSeek => false;
        }
    }
}
