﻿// Copyright Bastian Eicher
// Licensed under the MIT License

using System.IO;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Contains test methods for <see cref="SeekBufferStream"/>.
    /// </summary>
    public class SeekBufferStreamTest
    {
        private readonly byte[] _data = {1, 2, 3, 4, 5};

        [Fact]
        public void Normal()
        {
            var stream = new SeekBufferStream(new MemoryStream(_data));

            stream.Read(4).Should().Equal(1, 2, 3, 4);
            stream.Seek(-2, SeekOrigin.Current);
            stream.Read(2).Should().Equal(3, 4);
        }

        [Fact]
        public void SmallBuffer()
        {
            var stream = new SeekBufferStream(new MemoryStream(_data), bufferSize: 2);

            stream.Read(4).Should().Equal(1, 2, 3, 4);
            stream.Seek(-2, SeekOrigin.Current);
            stream.Read(2).Should().Equal(3, 4);
        }

        [Fact]
        public void CannotSeekForward()
        {
            var stream = new SeekBufferStream(new MemoryStream(_data));

            stream.Seek(2, SeekOrigin.Current);
            stream.Invoking(x => x.Read(2))
                  .Should().Throw<IOException>();
        }

        [Fact]
        public void CannotSeekBackTooFar()
        {
            var stream = new SeekBufferStream(new MemoryStream(_data), bufferSize: 2);

            stream.Read(4);
            stream.Seek(-4, SeekOrigin.Current);
            stream.Invoking(x => x.Read(2))
                  .Should().Throw<IOException>();
        }
    }
}