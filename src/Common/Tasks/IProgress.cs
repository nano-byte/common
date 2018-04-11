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

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Defines a provider for progress updates.
    /// </summary>
    /// <remarks>Implementations should derive from <see cref="MarshalByRefObject"/>.</remarks>
    /// <remarks>Unlike the built-in Progress type of .NET implementations of the NanoByte.Common variant should derive from <see cref="MarshalByRefObject"/> to support remoting.</remarks>
    /// <typeparam name="T">The type of progress update value.</typeparam>
    public interface IProgress
#if NET40
        <in T>
#else
        <T>
#endif
    {
        /// <summary>
        /// Reports a progress update.
        /// </summary>
        /// <param name="value">The value of the updated progress.</param>
        /// <remarks>May be called from background/worker threads. Callee must perform thread marshaling as needed.</remarks>
        void Report(T value);
    }
}
