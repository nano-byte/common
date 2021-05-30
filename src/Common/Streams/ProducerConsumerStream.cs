// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System;
using System.IO;
using System.Threading;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// A stream that one producer can write to and one consumer can read from simultaneously.
    /// </summary>
    /// <remarks>Uses a circular buffer internally. Do not use more than one producer or consumer thread simultaneously!</remarks>
    public sealed class ProducerConsumerStream : Stream
    {
        /// <summary>
        /// Creates a new producer consumer stream.
        /// </summary>
        /// <param name="bufferSize">The maximum number of written but not read bytes the stream can buffer.</param>
        public ProducerConsumerStream(int bufferSize = 163840)
        {
            _buffer = new byte[bufferSize];
        }

        /// <summary>The byte array used as a circular buffer storage.</summary>
        private readonly Memory<byte> _buffer;

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanWrite => true;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        private long _positionRead;

        /// <summary>
        /// Indicates how many bytes have been read from this buffer so far in total.
        /// </summary>
        public override long Position { get => _positionRead; set => throw new NotSupportedException(); }

        /// <summary>
        /// Indicates how many bytes have been written to this buffer so far in total.
        /// </summary>
        public long PositionWrite { get; private set; }

        private long _length;

        /// <summary>
        /// The estimated number of bytes that will run through this buffer in total; 0 for unknown.
        /// </summary>
        public override long Length => _length;

        /// <summary>
        /// Sets the estimated number of bytes that will run through this buffer in total.
        /// </summary>
        public override void SetLength(long value)
            => _length = value;

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void Flush() {}

        /// <summary>
        /// Reads data from the stream that was previously written.
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
            => Read(new Span<byte>(buffer, offset, count));

        /// <summary>
        /// Reads data from the stream that was previously written.
        /// </summary>
#if NETFRAMEWORK
        public int Read(Span<byte> buffer)
#else
        public override int Read(Span<byte> buffer)
#endif
        {
            #region Sanity checks
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            #endregion

            int count = 0;
            while (!buffer.IsEmpty && TryGetAvailableData(out var availableData))
            {
                if (buffer.Length < availableData.Length) availableData = availableData[..buffer.Length];
                availableData.CopyTo(buffer);
                buffer = buffer[availableData.Length..];

                count += availableData.Length;
                IncrementReadCounters(availableData.Length);
            }

            return count;
        }

#if !NETFRAMEWORK
        /// <inheritdoc/>
        public override void CopyTo(Stream destination, int bufferSize)
        {
            while (TryGetAvailableData(out var availableData))
            {
                destination.Write(availableData);
                IncrementReadCounters(availableData.Length);
            }
        }
#endif

        /// <summary>
        /// Writes data to the stream to be later read.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
            => Write(new ReadOnlySpan<byte>(buffer, offset, count));

        /// <summary>
        /// Writes data to the stream to be later read.
        /// </summary>
#if NETFRAMEWORK
        public void Write(ReadOnlySpan<byte> buffer)
#else
        public override void Write(ReadOnlySpan<byte> buffer)
#endif
        {
            #region Sanity checks
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            #endregion

            // Loop until the requested number of bytes have been written
            while (!buffer.IsEmpty)
            {
                var freeSpace = GetFreeSpace();

                int bytesToCopy = Math.Min(freeSpace.Length, buffer.Length);
                buffer[..bytesToCopy].CopyTo(freeSpace);
                buffer = buffer[bytesToCopy..];

                IncrementWriteCounters(bytesToCopy);
            }
        }

        /// <summary>
        /// Throws an exception from within reader.
        /// </summary>
        /// <param name="exception">The exception to throw.</param>
        public void RelayErrorToReader(Exception exception)
        {
            lock (_lock)
            {
                _relayedException = exception;

                // Stop waiting for data that will never come
                Signal(out _dataAvailable);
            }
        }

        /// <summary>
        /// Signals that no further write calls are intended and any blocked reader calls should return.
        /// </summary>
        public void DoneWriting()
        {
            lock (_lock)
            {
                _doneWriting = true;

                // Stop waiting for data that will never come
                Signal(out _dataAvailable);
            }
        }

        /// <summary>Synchronization object used to synchronize access across consumer and producer threads.</summary>
        private readonly object _lock = new();

        /// <summary>The index of the first byte currently store in the <see cref="_buffer"/>.</summary>
        private int _dataStart;

        /// <summary>The number of bytes currently stored in the <see cref="_buffer"/>.</summary>
        private int _dataLength; // Invariant: _positionWrite - _positionRead <= _dataLength <= _buffer.Length

        /// <summary>Indicates that the producer has finished and no new data will be added.</summary>
        private bool _doneWriting;

        /// <summary>Exceptions sent to reader>ers via <see cref="RelayErrorToReader"/>.</summary>
        private Exception? _relayedException;

        /// <summary>State variable for blocking threads until new data is available in the <see cref="_buffer"/>.</summary>
        private bool _dataAvailable;

        /// <summary>State variable for blocking threads until empty space is available in the <see cref="_buffer"/>.</summary>
        private bool _spaceAvailable = true;

        private bool TryGetAvailableData(out ReadOnlySpan<byte> availableData)
        {
            lock (_lock)
            {
                if (_relayedException != null) throw _relayedException;

                // All data read and no new data coming
                if (_doneWriting && _dataLength == 0)
                {
                    availableData = default;
                    return false;
                }

                // Block while buffer is empty
                WaitFor(ref _dataAvailable);

                // The index of the last byte currently stored in the buffer plus one
                int dataEnd = (_dataStart + _dataLength) % _buffer.Length;

                // Determine how many bytes can be read in one go
                if (_dataLength == 0) // No data
                    availableData = default;
                else if (_dataLength == _buffer.Length) // Data fills entire buffer
                    availableData = _buffer.Span[_dataStart..];
                else if (_dataStart < dataEnd) // Data does not wrap around
                    availableData = _buffer.Span[_dataStart..dataEnd];
                else // Data does wrap around
                    availableData = _buffer.Span[_dataStart..];
                return true;
            }
        }

        private Span<byte> GetFreeSpace()
        {
            lock (_lock)
            {
                // Block while buffer is full
                WaitFor(ref _spaceAvailable);

                // The index of the last byte currently stored in the buffer plus one
                int dataEnd = (_dataStart + _dataLength) % _buffer.Length;

                // Determine how many bytes can be written in one go
                if (_dataLength == _buffer.Length) // No free space available
                    return default;
                else if (_dataLength == 0) // Complete buffer available
                {
                    _dataStart = 0; // Reset start point
                    return _buffer.Span;
                }
                else if (dataEnd < _dataStart) // Data does wrap around
                    return _buffer.Span[dataEnd.._dataStart];
                else // Data does not wrap around
                    return _buffer.Span[dataEnd..];
            }
        }

        private void IncrementReadCounters(int length)
        {
            lock (_lock)
            {
                _positionRead += length;
                _dataLength -= length;
                _dataStart += length;
                _dataStart %= _buffer.Length;

                if (_dataLength == 0) Reset(out _dataAvailable);
                if (_dataLength < _buffer.Length) Signal(out _spaceAvailable);
            }
        }

        private void IncrementWriteCounters(int length)
        {
            lock (_lock)
            {
                PositionWrite += length;
                _dataLength += length;

                if (_dataLength == _buffer.Length) Reset(out _spaceAvailable);
                if (_dataLength > 0) Signal(out _dataAvailable);
            }
        }

        private void Signal(out bool flag)
        {
            flag = true;
            Monitor.PulseAll(_lock);
        }

        private static void Reset(out bool flag)
        {
            flag = false;
        }

        private void WaitFor(ref bool flag)
        {
            while (!flag)
                Monitor.Wait(_lock);

            if (_isDisposed) throw new ObjectDisposedException(nameof(ProducerConsumerStream));
        }

        private bool _isDisposed;

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_lock)
                {
                    _isDisposed = true;

                    // Signal all to prevent live locks
                    Signal(out _dataAvailable);
                    Signal(out _spaceAvailable);
                }
            }
        }
    }
}
#endif
