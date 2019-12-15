// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Continuously reads lines from a <see cref="StreamReader"/> using a background thread while providing them to a foreground thread on demand.
    /// </summary>
    /// <remarks>Useful for processing <see cref="Process.StandardOutput"/> and <see cref="Process.StandardError"/> without risking deadlocks.</remarks>
    public class StreamConsumer
    {
        private readonly object _lock = new object();
        private readonly Queue<string> _queue = new Queue<string>();
        private readonly StreamReader _reader;
        private readonly Thread _thread;

        /// <summary>
        /// Starts reading from the stream in a background thread.
        /// </summary>
        /// <param name="reader">The stream to read from.</param>
        public StreamConsumer(StreamReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _thread = ThreadUtils.StartBackground(ThreadStart, name: "StreamConsumer");
        }

        private void ThreadStart()
        {
            string line;
            while ((line = _reader.ReadLine()) != null)
            {
                lock (_lock)
                    _queue.Enqueue(line);
            }
        }

        /// <summary>
        /// Returns the next pending line; <c>null</c> if there are no pending lines.
        /// </summary>
        public string? ReadLine()
        {
            lock (_lock)
                return (_queue.Count == 0) ? null : _queue.Dequeue();
        }

        /// <summary>
        /// Waits for <see cref="StreamReader.EndOfStream"/>.
        /// </summary>
        public void WaitForEnd() => _thread.Join();

        /// <summary>
        /// Returns all buffered lines that have not been read yet.
        /// </summary>
        public override string ToString()
        {
            lock (_lock)
                return StringUtils.Join(Environment.NewLine, _queue);
        }
    }
}
