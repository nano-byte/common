// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Native;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// Informs the user about the progress of tasks and ask questions using ANSI console output.
/// </summary>
/// <remarks>This class is thread-safe.</remarks>
public class AnsiCliTaskHandler : CliTaskHandler
{
    /// <inheritdoc/>
    protected override void LogHandler(LogSeverity severity, string message, Exception? exception)
    {
        void WriteLine(ConsoleColor color)
            => AnsiCli.Error.WriteLine(message, new Style(color));

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

        if (exception != null && Verbosity >= Verbosity.Debug)
            AnsiCli.Error.WriteLine(exception.ToString());
    }

    /// <inheritdoc />
    public override ICredentialProvider? CredentialProvider
        => WindowsUtils.IsWindowsNT
            ? base.CredentialProvider
            : IsInteractive ? new CachedCredentialProvider(new AnsiCliCredentialProvider()) : null;

    private readonly object _progressContextLock = new();
    private AnsiCliProgressContext? _progressContext;

    /// <inheritdoc/>
    public override void RunTask(ITask task)
    {
        #region Sanity checks
        if (task == null) throw new ArgumentNullException(nameof(task));
        #endregion

        Log.Info(task.Name);

        if (!IsInteractive)
        {
            AnsiCli.Error.WriteLine(task.Name);
            task.Run(CancellationToken, CredentialProvider);
            return;
        }

        IProgress<TaskSnapshot> progress;
        lock (_progressContextLock)
        {
            _progressContext ??= new();
            progress = _progressContext.Add(task.Name);
        }

        task.Run(CancellationToken, CredentialProvider, progress);

        lock (_progressContextLock)
        {
            if (_progressContext is {IsFinished: true})
            {
                _progressContext.Dispose();
                _progressContext = null;
            }
        }
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        try
        {
            _progressContext?.Dispose();
        }
        finally
        {
            base.Dispose();
        }
    }

    /// <inheritdoc/>
    protected override bool AskInteractive(string question, bool defaultAnswer)
    {
        if (!AnsiCli.Error.Profile.Capabilities.Interactive)
            return base.AskInteractive(question, defaultAnswer);

        lock (_progressContextLock)
        {
            // Avoid showing progress bars and input prompts at the same time
            _progressContext?.Dispose();
            _progressContext = null;

            return AnsiCli.Prompt(
                       new TextPrompt<char>(question)
                          .AddChoices(new[] {'y', 'n'})
                          .DefaultValue(defaultAnswer ? 'y' : 'n'),
                       CancellationToken)
                == 'y';
        }
    }

    /// <inheritdoc/>
    public override void Output(string title, string message)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (message == null) throw new ArgumentNullException(nameof(message));
        #endregion

        if (Verbosity == Verbosity.Batch || Console.IsOutputRedirected)
        {
            base.Output(title, message);
            return;
        }

        AnsiConsole.Write(AnsiCli.Title(title));
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

        if (Verbosity == Verbosity.Batch || Console.IsOutputRedirected)
        {
            base.Output(title, data);
            return;
        }

        AnsiConsole.Write(AnsiCli.Title(title));
        if (typeof(T) == typeof(string) || typeof(Uri).IsAssignableFrom(typeof(T)))
        {
            foreach (var entry in data)
                AnsiConsole.WriteLine(entry?.ToString() ?? "");
        }
        else
            AnsiConsole.Write(AnsiCli.Table(data));
    }

    /// <inheritdoc/>
    public override void Output<T>(string title, NamedCollection<T> data)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        if (Verbosity == Verbosity.Batch || Console.IsOutputRedirected)
        {
            base.Output(title, data);
            return;
        }

        AnsiConsole.Write(AnsiCli.Title(title));
        AnsiConsole.Write(AnsiCli.Tree(data));
    }
}
