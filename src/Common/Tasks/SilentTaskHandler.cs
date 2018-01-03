/*
 * Copyright 2006-2017 Bastian Eicher
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
using System.Linq;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Executes tasks silently and suppresses any questions.
    /// </summary>
    public class SilentTaskHandler : ITaskHandler
    {
        /// <inheritdoc/>
        public virtual void Dispose()
        {}

        /// <inheritdoc/>
        public virtual CancellationToken CancellationToken => default;

        /// <inheritdoc/>
        public virtual ICredentialProvider CredentialProvider => null;

        /// <inheritdoc/>
        public void RunTask(ITask task)
        {
            #region Sanity checks
            if (task == null) throw new ArgumentNullException(nameof(task));
            #endregion

            Log.Debug("Task: " + task.Name);
            task.Run(CancellationToken, CredentialProvider);
        }

        /// <summary>
        /// Always returns <see cref="Tasks.Verbosity.Batch"/>.
        /// </summary>
        public Verbosity Verbosity { get => Verbosity.Batch; set {} }

        /// <summary>
        /// Always returns <c>false</c>.
        /// </summary>
        public bool Ask(string question) => Ask(question, defaultAnswer: false);

        /// <summary>
        /// Always returns <paramref name="defaultAnswer"/>.
        /// </summary>
        public bool Ask(string question, bool defaultAnswer, string alternateMessage = null)
        {
            Log.Info($"Question: {question}\nAutomatic answer: {defaultAnswer}");
            return defaultAnswer;
        }

        /// <inheritdoc/>
        public void Output(string title, string message) => Log.Info($"{title}\n{message}");

        /// <inheritdoc/>
        public void Output<T>(string title, IEnumerable<T> data)
        {
            string message = StringUtils.Join(Environment.NewLine, (data ?? throw new ArgumentNullException(nameof(data))).Select(x => x.ToString()));
            Output(title ?? throw new ArgumentNullException(nameof(title)), message);
        }

        /// <inheritdoc/>
        public void OutputLow(string title, string message) => Log.Debug($"{title}\n{message}");

        /// <inheritdoc/>
        public void OutputLow<T>(string title, IEnumerable<T> data)
        {
            string message = StringUtils.Join(Environment.NewLine, (data ?? throw new ArgumentNullException(nameof(data))).Select(x => x.ToString()));
            OutputLow(title ?? throw new ArgumentNullException(nameof(title)), message);
        }

        /// <inheritdoc/>
        public void Error(Exception exception) => Log.Error(exception);
    }
}
