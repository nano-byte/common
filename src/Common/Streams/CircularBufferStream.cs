// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// A circular buffer represented as a stream that one producer can write to and one consumer can read from simultaneously.
    /// </summary>
    /// <remarks>Do not use more than one producer or consumer thread simultaneously!</remarks>
    public sealed class CircularBufferStream : Stream
    {
        /// <summary>The byte array used as a circular buffer storage.</summary>
        private readonly byte[] _buffer;

        /// <summary>
        /// The maximum number of bytes the buffer can hold at any time.
        /// </summary>
        public int BufferSize => _buffer.Length;

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

        private long _length = -1;

        /// <summary>
        /// The estimated number of bytes that will run through this buffer in total; -1 for unknown.
        /// </summary>
        public override long Length => _length;

        /// <summary>
        /// Indicates that this stream has been closed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Creates a new circular buffer.
        /// </summary>
        /// <param name="bufferSize">The maximum number of bytes the buffer can hold at any time.</param>
        public CircularBufferStream(int bufferSize)
        {
            _buffer = new byte[bufferSize];
        }

        /// <summary>Synchronization object used to synchronize access across consumer and producer threads.</summary>
        private readonly object _lock = new object();

        /// <summary>The index of the first byte currently store in the <see cref="_buffer"/>.</summary>
        private int _dataStart;

        /// <summary>The number of bytes currently stored in the <see cref="_buffer"/>.</summary>
        private int _dataLength; // Invariant: _positionWrite - _positionRead <= _dataLength <= _buffer.Length

        /// <summary>Indicates that the producer has finished and no new data will be added.</summary>
        private bool _doneWriting;

        /// <summary>Exceptions sent to <see cref="Read"/>ers via <see cref="RelayErrorToReader"/>.</summary>
        private Exception _relayedException;

        /// <summary>A barrier that blocks threads until new data is available in the <see cref="_buffer"/>.</summary>
        private readonly ManualResetEvent _dataAvailable = new ManualResetEvent(initialState: false);

        /// <summary>A barrier that blocks threads until empty space is available in the <see cref="_buffer"/>.</summary>
        private readonly ManualResetEvent _spaceAvailable = new ManualResetEvent(initialState: true);

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            #region Sanity checks
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || offset > buffer.Length) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count + offset > buffer.Length) throw new ArgumentOutOfRangeException(nameof(count));
            #endregion

            // Bytes copied to target buffer so far
            int bytesCopied = 0;

            // Loop until the request number of bytes have been returned
            while (bytesCopied != count)
            {
                if (IsDisposed) throw new ObjectDisposedException("CircularBufferStream");

                lock (_lock)
                {
                    if (_relayedException != null) throw _relayedException;

                    // All data read and no new data coming
                    if (_doneWriting && _dataLength == 0) break;
                }

                // Block while buffer is empty
                _dataAvailable.WaitOne();

                if (IsDisposed) throw new ObjectDisposedException("CircularBufferStream");

                lock (_lock)
                {
                    // The index of the last byte currently stored in the buffer plus one
                    int dataEnd = (_dataStart + _dataLength) % _buffer.Length;

                    // Determine how many bytes can be read in one go
                    int contigousDataBytes;
                    if (_dataLength == 0) contigousDataBytes = 0; // Empty
                    else if (_dataLength == _buffer.Length) contigousDataBytes = _buffer.Length - _dataStart; // Full, potentially wrap around, partial read
                    else if (_dataStart < dataEnd) contigousDataBytes = dataEnd - _dataStart; // No wrap around, read all
                    else contigousDataBytes = _buffer.Length - _dataStart; // Wrap around, partial read
                    Debug.Assert(contigousDataBytes <= _dataLength, "Contigous data length can not exceed total data length");

                    int bytesToCopy = Math.Min(contigousDataBytes, count - bytesCopied);
                    Buffer.BlockCopy(_buffer, _dataStart, buffer, offset + bytesCopied, bytesToCopy);

                    // Update counters
                    bytesCopied += bytesToCopy;
                    _positionRead += bytesToCopy;
                    _dataLength -= bytesToCopy;
                    _dataStart += bytesToCopy;

                    // Roll over start pointer
                    _dataStart %= _buffer.Length;

                    // Start blocking when buffer becomes empty
                    if (_dataLength == 0) _dataAvailable.Reset();

                    // Stop blocking when space becomes available
                    if (_dataLength < _buffer.Length) _spaceAvailable.Set();
                }
            }

            return bytesCopied;
        }

        /// <summary>
        /// Throws an exception from within <see cref="Read"/>.
        /// </summary>
        /// <param name="exception">The exception to throw.</param>
        public void RelayErrorToReader(Exception exception)
        {
            lock (_lock)
                _relayedException = exception;

            // Stop waiting for data that will never come
            _dataAvailable.Set();
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            #region Sanity checks
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || offset > buffer.Length) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count + offset > buffer.Length) throw new ArgumentOutOfRangeException(nameof(count));
            #endregion

            // Bytes copied to internal buffer so far
            int bytesCopied = 0;

            // Loop until the request number of bytes have been written
            while (bytesCopied != count)
            {
                if (IsDisposed) throw new ObjectDisposedException("CircularBufferStream");

                // Block while buffer is full
                _spaceAvailable.WaitOne();

                if (IsDisposed) throw new ObjectDisposedException("CircularBufferStream");

                lock (_lock)
                {
                    // The index of the last byte currently stored in the buffer plus one
                    int dataEnd = (_dataStart + _dataLength) % _buffer.Length;

                    // Determine how many bytes can be written in one go
                    int contigousFreeBytes;
                    if (_dataLength == _buffer.Length) contigousFreeBytes = 0; // Full
                    else if (_dataLength == 0)
                    { // Empty, realign with buffer start
                        _dataStart = 0;
                        dataEnd = 0;
                        contigousFreeBytes = _buffer.Length;
                    }
                    else if (dataEnd < _dataStart) contigousFreeBytes = _dataStart - dataEnd; // No wrap around, full write
                    else contigousFreeBytes = _buffer.Length - dataEnd; // Wrap around, partial write
                    Debug.Assert(contigousFreeBytes <= (_buffer.Length - _dataLength), "Contigous free space can not exceed total free space");

                    int bytesToCopy = Math.Min(contigousFreeBytes, count - bytesCopied);
                    Buffer.BlockCopy(buffer, offset + bytesCopied, _buffer, dataEnd, bytesToCopy);

                    // Update counters
                    bytesCopied += bytesToCopy;
                    PositionWrite += bytesToCopy;
                    _dataLength += bytesToCopy;

                    // Start blocking when buffer becomes full
                    if (_dataLength == _buffer.Length) _spaceAvailable.Reset();

                    // Stop blocking when data becomes available
                    if (_dataLength > 0) _dataAvailable.Set();
                }
            }
        }

        /// <summary>
        /// Signals that no further calls to <see cref="Write"/> are intended and any blocked <see cref="Read"/> calls should return.
        /// </summary>
        public void DoneWriting()
        {
            lock (_lock)
                _doneWriting = true;

            // Stop waiting for data that will never come
            _dataAvailable.Set();
        }

        /// <summary>
        /// Sets the estimated number of bytes that will run through this buffer in total; -1 for unknown.
        /// </summary>
        public override void SetLength(long value) => _length = value;

        #region Unsupported operations
        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void Flush() {}
        #endregion

        #region Dispose
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;

            try
            {
                // Signal all to prevent livelocks
                _dataAvailable.Set();
                _spaceAvailable.Set();

                _dataAvailable.Close();
                _spaceAvailable.Close();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        #endregion
    }
}
