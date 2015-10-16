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
using System.IO;
using System.Linq;
using NanoByte.Common.Cli;
using NanoByte.Common.Native;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Uses the stderr stream to inform the user about the progress of tasks.
    /// </summary>
    public class CliTaskHandler : MarshalNoTimeout, ITaskHandler
    {
        /// <summary>
        /// Sets up Ctrl+C handling and console <see cref="Log"/> output.
        /// </summary>
        public CliTaskHandler()
        {
            try
            {
                Console.CancelKeyPress += CancelKeyPressHandler;
            }
                #region Error handler
            catch (IOException)
            {
                // Ignore failures caused by non-standard terminal emulators
            }
            #endregion

            Log.Handler += LogHandler;
        }

        /// <summary>
        /// Handles Ctrl+C key presses.
        /// </summary>
        private void CancelKeyPressHandler(object sender, ConsoleCancelEventArgs e)
        {
            CancellationTokenSource.Cancel();

            // Allow the application to finish cleanup rather than terminating immediately
            e.Cancel = true;
        }

        /// <summary>
        /// Prints <see cref="Log"/> messages to the <see cref="Console"/> based on their <see cref="LogSeverity"/> and the current <see cref="Verbosity"/> level.
        /// </summary>
        /// <param name="severity">The type/severity of the entry.</param>
        /// <param name="message">The message text of the entry.</param>
        protected virtual void LogHandler(LogSeverity severity, string message)
        {
            switch (severity)
            {
                case LogSeverity.Debug:
                    if (Verbosity >= Verbosity.Debug) Log.PrintToConsole(severity, message);
                    break;
                case LogSeverity.Info:
                    if (Verbosity >= Verbosity.Verbose) Log.PrintToConsole(severity, message);
                    break;
                case LogSeverity.Warn:
                case LogSeverity.Error:
                    Log.PrintToConsole(severity, message);
                    break;
            }
        }

        /// <inheritdoc/>
        public Verbosity Verbosity { get; set; }

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

            if (Verbosity <= Verbosity.Batch)
                task.Run(CancellationToken, credentialProvider: BuildCredentialProvider());
            else
            {
                Log.Debug("Task: " + task.Name);
                Console.Error.WriteLine(task.Name + @"...");
                using (var progressBar = new TaskProgressBar())
                    task.Run(CancellationToken, progressBar, BuildCredentialProvider());
            }
        }

        /// <inheritdoc/>
        public virtual bool Ask(string question)
        {
            #region Sanity checks
            if (question == null) throw new ArgumentNullException("question");
            #endregion

            Log.Debug("Question: " + question);
            Console.Error.WriteLine(question);

            // Loop until the user has made a valid choice
            while (true)
            {
                switch (CliUtils.ReadString(@"[Y/N]").ToLower())
                {
                    case "y":
                    case "yes":
                        Log.Debug("Answer: Yes");
                        return true;
                    case "n":
                    case "no":
                        Log.Debug("Answer: No");
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

        /// <inheritdoc/>
        public void Output<T>(string title, IEnumerable<T> data)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException("title");
            if (data == null) throw new ArgumentNullException("data");
            #endregion

            foreach (string entry in data.Select(x => x.ToString()))
            {
                Console.WriteLine(entry);
                if (entry.Contains("\n")) Console.WriteLine();
            }
        }

        /// <inheritdoc/>
        public virtual ICredentialProvider BuildCredentialProvider()
        {
            bool silent = (Verbosity == Verbosity.Batch);
            if (WindowsUtils.IsWindowsNT) return new WindowsCliCredentialProvider(silent);
            else return (silent ? null : new CliCredentialProvider());
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
            Log.Handler -= LogHandler;
            Console.CancelKeyPress -= CancelKeyPressHandler;
            if (disposing) CancellationTokenSource.Dispose();
        }
        #endregion
    }
}
