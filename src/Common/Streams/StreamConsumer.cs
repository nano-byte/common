/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using JetBrains.Annotations;

namespace NanoByte.Common.Streams
{
    /// <summary>
    /// Continously reads lines from a <see cref="StreamReader"/> using a background thread while providing them to a foreground thread on demand.
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
        public StreamConsumer([NotNull] StreamReader reader)
        {
            #region Sanity checks
            if (reader == null) throw new ArgumentNullException("reader");
            #endregion

            _reader = reader;
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
        [CanBeNull]
        public string ReadLine()
        {
            lock (_lock)
                return (_queue.Count == 0) ? null : _queue.Dequeue();
        }

        /// <summary>
        /// Waits for <see cref="StreamReader.EndOfStream"/>.
        /// </summary>
        public void WaitForEnd()
        {
            _thread.Join();
        }

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
