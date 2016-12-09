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
using System.IO;
using NanoByte.Common.Cli;
using NanoByte.Common.Native;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Uses the stderr stream to inform the user about the progress of tasks.
    /// </summary>
    public class CliTaskHandler : TaskHandlerBase
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

        protected override void Dispose(bool disposing)
        {
            try
            {
                Console.CancelKeyPress -= CancelKeyPressHandler;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Prints <see cref="Log"/> messages to the <see cref="Console"/> based on their <see cref="LogSeverity"/> and the current <see cref="Verbosity"/> level.
        /// </summary>
        /// <param name="severity">The type/severity of the entry.</param>
        /// <param name="message">The message text of the entry.</param>
        protected override void LogHandler(LogSeverity severity, string message)
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
        protected override ICredentialProvider BuildCrendentialProvider()
        {
            if (WindowsUtils.IsWindowsNT) return new WindowsCliCredentialProvider(interactive: Verbosity >= Verbosity.Normal);
            else return new CliCredentialProvider(interactive: Verbosity >= Verbosity.Normal);
        }

        /// <inheritdoc/>
        public override void RunTask(ITask task)
        {
            #region Sanity checks
            if (task == null) throw new ArgumentNullException(nameof(task));
            #endregion

            if (Verbosity <= Verbosity.Batch)
                task.Run(CancellationToken, CredentialProvider);
            else
            {
                Log.Debug("Task: " + task.Name);
                Console.Error.WriteLine(task.Name + @"...");
                using (var progressBar = new TaskProgressBar())
                    task.Run(CancellationToken, CredentialProvider, progressBar);
            }
        }

        /// <inheritdoc/>
        protected override bool Ask(string question, MsgSeverity severity)
        {
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
        public override void Output(string title, string message)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (message == null) throw new ArgumentNullException(nameof(message));
            #endregion

            if (message.EndsWith("\n")) Console.Write(message);
            else Console.WriteLine(message);
        }

        /// <inheritdoc/>
        public override void Error(Exception exception)
        {
            Log.Error(exception);
        }
    }
}
