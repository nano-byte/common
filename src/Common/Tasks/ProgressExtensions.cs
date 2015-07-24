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
using JetBrains.Annotations;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Provides extension methods for <see cref="IProgress{T}"/>.
    /// </summary>
    public static class ProgressExtensions
    {
        /// <summary>
        /// Limits the rate at which progress reports are passed on.
        /// </summary>
        /// <param name="progress">The progress report target to wrap.</param>
        /// <param name="interval">The minimum interval between two progress reports in milliseconds.</param>
        public static IProgress<T> LimitRate<T>([NotNull] this IProgress<T> progress, int interval = 250)
        {
            #region Sanity checks
            if (progress == null) throw new ArgumentNullException("progress");
            #endregion

            return new RateLimitedProgress<T>(progress, new TimeSpan(0, 0, 0, 0, interval));
        }

        private sealed class RateLimitedProgress<T> : MarshalByRefObject, IProgress<T>
        {
            private readonly IProgress<T> _progress;
            private readonly TimeSpan _interval;

            public RateLimitedProgress([NotNull] IProgress<T> progress, TimeSpan interval)
            {
                _progress = progress;
                _interval = interval;
            }

            private DateTime _lastReport;

            public void Report(T value)
            {
                if (DateTime.UtcNow - _lastReport >= _interval)
                {
                    _progress.Report(value);
                    _lastReport = DateTime.UtcNow;
                }
            }
        }
    }
}