// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Contains test methods for <see cref="CircularBufferStream"/>.
    /// </summary>
    public class CircularBufferStreamTest
    {
        private readonly CircularBufferStream _stream = new CircularBufferStream(6);

        /// <summary>
        /// Fills the buffer with some data that will fit in a single block and then reads it again.
        /// </summary>
        [Fact]
        public void TestSequential()
        {
            _stream.Write(new byte[] {255, 1, 2, 3, 255}, 1, 3);

            var result = new byte[5];
            _stream.Read(result, 1, 3);
            result.Should().Equal(0, 1, 2, 3, 0);
        }

        /// <summary>
        /// Writes some data into the buffer, reads a part of it, write more data into it and then read all that's remaining.
        /// </summary>
        [Fact]
        public void TestMixed()
        {
            _stream.WriteByte(0);
            _stream.WriteByte(1);
            _stream.WriteByte(2);
            _stream.WriteByte(3);
            _stream.WriteByte(4);
            _stream.WriteByte(5);
            _stream.ReadByte().Should().Be(0);
            _stream.ReadByte().Should().Be(1);
            _stream.ReadByte().Should().Be(2);
            _stream.WriteByte(6);
            _stream.WriteByte(7);
            _stream.WriteByte(8);
            _stream.ReadByte().Should().Be(3);
            _stream.ReadByte().Should().Be(4);
            _stream.ReadByte().Should().Be(5);
            _stream.ReadByte().Should().Be(6);
            _stream.ReadByte().Should().Be(7);
            _stream.ReadByte().Should().Be(8);
        }

        /// <summary>
        /// Read and write data in the buffer in multiple calls, some of which will force the backing byte array storage to wrap around.
        /// </summary>
        [Fact]
        public void TestWrapAround()
        {
            _stream.Write(new byte[] {0, 1, 2, 3}, 0, 4);

            var result = new byte[2];
            _stream.Read(result, 0, 2);
            result.Should().Equal(0, 1);

            _stream.Write(new byte[] {4, 5, 6, 7}, 0, 4);

            result = new byte[6];
            _stream.Read(result, 0, 6);
            result.Should().Equal(2, 3, 4, 5, 6, 7);
        }

        /// <summary>
        /// Creates a producer and a consumer thread, both accessing the buffer simultaneously.
        /// </summary>
        [Fact]
        public void TestThreading()
        {
            var producerData = new byte[128];
            for (byte i = 0; i < producerData.Length; i++)
                producerData[i] = i;
            var producerThread = new Thread(() => _stream.Write(producerData));

            byte[]? consumerData = null;
            var consumerThread = new Thread(() => consumerData = _stream.Read(128));

            producerThread.Start();
            consumerThread.Start();
            consumerThread.Join();

            consumerData.Should().Equal(producerData);
        }

        /// <summary>
        /// Similar to <see cref="TestThreading"/> but uses end-of-data detection via <see cref="CircularBufferStream.DoneWriting"/>
        /// </summary>
        [Fact]
        public void TestThreadingDoneWriting()
        {
            var producerData = new byte[64];
            for (byte i = 0; i < producerData.Length; i++)
                producerData[i] = i;
            var producerThread = new Thread(() =>
            {
                _stream.Write(producerData);
                _stream.DoneWriting();
            });

            int readBytes = 0;
            var consumerData = new byte[128];
            var consumerThread = new Thread(() => readBytes = _stream.Read(consumerData, 0, consumerData.Length));

            producerThread.Start();
            consumerThread.Start();
            consumerThread.Join();

            readBytes.Should().Be(producerData.Length);

            // consumerData was intentionally too large; now trim it down to actual data size for comparison
            var trimmedConsumerData = new byte[producerData.Length];
            Buffer.BlockCopy(consumerData, 0, trimmedConsumerData, 0, trimmedConsumerData.Length);
            trimmedConsumerData.Should().Equal(producerData);
        }

        /// <summary>
        /// Ensures exceptions get passed from <see cref="CircularBufferStream.RelayErrorToReader"/> to <see cref="CircularBufferStream.Read"/>.
        /// </summary>
        [Fact]
        public void TestErrorRelay()
        {
            // Throw exception on producer thread after a short delay
            new Thread(() =>
            {
                Thread.Sleep(50);
                _stream.RelayErrorToReader(new InvalidDataException("Test exception"));
            }).Start();

            // Catch exception on consumer thread
            _stream.Invoking(x => x.Read(2)).Should().Throw<InvalidDataException>();
        }

        /// <summary>
        /// Ensures blocked writers terminate when the stream is disposed.
        /// </summary>
        [Fact]
        public void TestDisposeBeforeComplete()
        {
            var data = new byte[_stream.BufferSize];
            _stream.Write(data);

            // Dispose on consumer thread after a short delay
            new Thread(() =>
            {
                Thread.Sleep(50);
                _stream.Dispose();
            }).Start();

            _stream.Invoking(x => x.Write(data)).Should().Throw<ObjectDisposedException>();
        }
    }
}
