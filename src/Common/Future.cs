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
using System.Threading;

namespace NanoByte.Common
{
    /// <summary>
    /// Combines an <see cref="EventWaitHandle"/> with a result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public sealed class Future<T> : IDisposable
    {
        private T _result;
        private readonly EventWaitHandle _waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        /// <summary>
        /// Creates a future waiting for a result.
        /// </summary>
        public Future()
        {}

        /// <summary>
        /// Creates a future with the result already set.
        /// </summary>
        public Future(T result)
        {
            Set(result);
        }

        /// <summary>
        /// Sets the result and signals anyone waiting for it.
        /// </summary>
        public void Set(T result)
        {
            _result = result;
            _waitHandle.Set();
        }

        /// <summary>
        /// Waits for the result and returns it when it is ready.
        /// </summary>
        public T Get()
        {
            _waitHandle.WaitOne();
            return _result;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _waitHandle.Close();
        }
    }
}
