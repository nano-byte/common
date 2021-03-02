// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using NanoByte.Common.Collections;
using NanoByte.Common.Native;
using NanoByte.Common.Net;
using Spectre.Console;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Informs the user about the progress of tasks and ask questions using ANSI console output.
    /// </summary>
    public class AnsiCliTaskHandler : CliTaskHandler
    {
        /// <inheritdoc/>
        protected override void LogHandler(LogSeverity severity, string message)
        {
            void WriteLine(ConsoleColor color)
                => AnsiCli.Stderr.WriteLine(message, new Style(color));

            switch (severity)
            {
                case LogSeverity.Debug when Verbosity >= Verbosity.Debug:
                    WriteLine(ConsoleColor.Blue);
                    break;
                case LogSeverity.Info when Verbosity >= Verbosity.Verbose:
                    WriteLine(ConsoleColor.Green);
                    break;
                case LogSeverity.Warn:
                    WriteLine(ConsoleColor.DarkYellow);
                    break;
                case LogSeverity.Error:
                    WriteLine(ConsoleColor.Red);
                    break;
            }
        }

        /// <inheritdoc />
        public override ICredentialProvider? CredentialProvider
            => WindowsUtils.IsWindowsNT
                ? base.CredentialProvider
                : IsInteractive ? new CachedCredentialProvider(new AnsiCliCredentialProvider()) : null;

        private readonly object _progressContextLock = new();
        private AnsiCliProgressContext? _progressContext;

        /// <inheritdoc/>
        protected override void RunTaskInteractive(ITask task)
        {
            IProgress<TaskSnapshot> progress;
            lock (_progressContextLock)
            {
                _progressContext ??= new();
                progress = _progressContext.Add(task.Name);
            }

            task.Run(CancellationToken, CredentialProvider, progress);

            lock (_progressContextLock)
            {
                if (_progressContext.IsFinished)
                {
                    _progressContext?.Dispose();
                    _progressContext = null;
                }
            }
        }

        /// <inheritdoc/>
        protected override bool AskInteractive(string question, bool? defaultAnswer)
        {
            var prompt = new TextPrompt<char>(question) {Choices = {'y', 'n'}};
            if (defaultAnswer.HasValue)
                prompt.DefaultValue(defaultAnswer.Value ? 'y' : 'n');
            return AnsiCli.Stderr.Prompt(prompt) == 'y';
        }

        /// <inheritdoc/>
        public override void Output(string title, string message)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (message == null) throw new ArgumentNullException(nameof(message));
            #endregion

            AnsiConsole.Render(AnsiCli.Title(title));
            if (message.EndsWith("\n")) AnsiConsole.Write(message);
            else AnsiConsole.WriteLine(message);
        }

        /// <inheritdoc/>
        public override void Output<T>(string title, IEnumerable<T> data)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (data == null) throw new ArgumentNullException(nameof(data));
            #endregion

            AnsiConsole.Render(AnsiCli.Title(title));
            if (typeof(T) == typeof(string) || typeof(Uri).IsAssignableFrom(typeof(T)))
            {
                foreach (var entry in data)
                    AnsiConsole.WriteLine(entry?.ToString() ?? "");
            }
            else
                AnsiConsole.Render(AnsiCli.Table(data));
        }

        /// <inheritdoc/>
        public override void Output<T>(string title, NamedCollection<T> data)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (data == null) throw new ArgumentNullException(nameof(data));
            #endregion

            AnsiConsole.Render(AnsiCli.Title(title));
            AnsiConsole.Render(AnsiCli.Tree(data));
        }
    }
}
