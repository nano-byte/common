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

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Ignores progress reports.
    /// </summary>
    public class SilentTaskHandler : MarshalNoTimeout, ITaskHandler
    {
        /// <summary>
        /// Used to signal the <see cref="CancellationToken"/>.
        /// </summary>
        protected readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        /// <inheritdoc/>
        public CancellationToken CancellationToken { get { return CancellationTokenSource.Token; } }

        /// <inheritdoc/>
        public void RunTask(ITask task)
        {
            #region Sanity checks
            if (task == null) throw new ArgumentNullException("task");
            #endregion

            Log.Debug("Task: " + task.Name);
            task.Run(CancellationToken);
        }

        /// <summary>
        /// Always returns <seealso cref="Tasks.Verbosity.Batch"/>.
        /// </summary>
        public virtual Verbosity Verbosity { get { return Verbosity.Batch; } set { } }

        /// <summary>
        /// Always returns <see langword="false"/>.
        /// </summary>
        public virtual bool Ask(string question)
        {
            Log.Debug("Question: " + question);
            Log.Debug("Silent answer: No");
            return false;
        }

        /// <inheritdoc/>
        public virtual void Output(string title, string message)
        {
            // No UI, so nothing to do
        }

        /// <inheritdoc/>
        public virtual void Output<T>(string title, IEnumerable<T> data)
        {
            // No UI, so nothing to do
        }

        #region Dispose
        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) CancellationTokenSource.Dispose();
        }
        #endregion
    }
}
