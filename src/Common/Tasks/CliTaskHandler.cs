/*
 * Copyright 2006-2014 Bastian Eicher
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
using NanoByte.Common.Cli;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Uses the stderr stream to inform the user about the progress of tasks.
    /// </summary>
    public class CliTaskHandler : MarshalNoTimeout, ITaskHandler
    {
        /// <inheritdoc/>
        public int Verbosity { get; set; }

        /// <summary>
        /// Used to signal the <see cref="CancellationToken"/>.
        /// </summary>
        protected readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        /// <inheritdoc/>
        public CancellationToken CancellationToken { get { return CancellationTokenSource.Token; } }

        /// <inheritdoc/>
        public virtual void RunTask(ITask task)
        {
            #region Sanity checks
            if (task == null) throw new ArgumentNullException("task");
            #endregion

            Log.Info(task.Name + "...");
            using (var progressBar = new ProgressBar())
                task.Run(CancellationToken, progressBar);
        }

        /// <inheritdoc/>
        public bool Batch { get; set; }

        /// <inheritdoc/>
        public virtual bool AskQuestion(string question, string batchInformation = null)
        {
            #region Sanity checks
            if (question == null) throw new ArgumentNullException("question");
            #endregion

            if (Batch)
            {
                if (!string.IsNullOrEmpty(batchInformation)) Log.Warn(batchInformation);
                return false;
            }

            Log.Info(question);

            // Loop until the user has made a valid choice
            while (true)
            {
                string input = CliUtils.ReadString("[Y/N]");
                if (input == null) throw new OperationCanceledException();
                switch (input.ToLower())
                {
                    case "y":
                    case "yes":
                        return true;
                    case "n":
                    case "no":
                        return false;
                }
            }
        }

        /// <inheritdoc/>
        public virtual void Output(string title, string message)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException("title");
            if (message == null) throw new ArgumentNullException("message");
            #endregion

            Console.WriteLine(message);
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
