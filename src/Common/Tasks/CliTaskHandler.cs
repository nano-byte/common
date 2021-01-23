// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using NanoByte.Common.Cli;
using NanoByte.Common.Native;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Uses the console (stderr stream) to inform the user about the progress of tasks and ask questions.
    /// </summary>
    public class CliTaskHandler : TaskHandlerBase
    {
        public CliTaskHandler()
        {
            if (WindowsUtils.IsWindowsNT)
                CredentialProvider = new CachedCredentialProvider(new WindowsCliCredentialProvider(this));

            try
            {
                Console.CancelKeyPress += CancelKeyPressHandler;
            }
            #region Error handling
            catch (IOException)
            {
                // Ignore problems caused by unusual terminal emulators
            }
            #endregion
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            try
            {
                Console.CancelKeyPress -= CancelKeyPressHandler;
            }
            #region Error handling
            catch (IOException)
            {
                // Ignore problems caused by unusual terminal emulators
            }
            #endregion

            base.Dispose();
        }

        /// <summary>
        /// Handles Ctrl+C key presses.
        /// </summary>
        private void CancelKeyPressHandler(object? sender, ConsoleCancelEventArgs e)
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
        protected override void LogHandler(LogSeverity severity, string message)
        {
            if (severity == LogSeverity.Debug && Verbosity >= Verbosity.Debug
             || severity == LogSeverity.Info && Verbosity >= Verbosity.Verbose
             || severity == LogSeverity.Warn
             || severity == LogSeverity.Error)
                Log.PrintToConsole(severity, message);
        }

        /// <inheritdoc/>
        public override ICredentialProvider? CredentialProvider { get; }

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
                using var progressBar = new TaskProgressBar();
                task.Run(CancellationToken, CredentialProvider, progressBar);
            }
        }

        /// <inheritdoc />
        public override bool Ask(string question, bool? defaultAnswer = null, string? alternateMessage = null)
        {
            #region Sanity checks
            if (question == null) throw new ArgumentNullException(nameof(question));
            #endregion

            if (Verbosity <= Verbosity.Batch && defaultAnswer.HasValue)
            {
                if (!string.IsNullOrEmpty(alternateMessage)) Log.Warn(alternateMessage);
                return defaultAnswer.Value;
            }

            Log.Debug($"Question: {question}");
            Console.Error.WriteLine(question);

            // Loop until the user has made a valid choice
            while (true)
            {
                switch (CliUtils.ReadString(@"[Y/N]").ToLower())
                {
                    case "y" or "yes":
                        Log.Debug("Answer: Yes");
                        return true;
                    case "n" or "no":
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
        public override void Error(Exception exception) => Log.Error(exception);
    }
}
