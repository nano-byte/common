// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Native;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// Informs the user about the progress of tasks and ask questions using console output.
/// </summary>
/// <remarks>This class is thread-safe.</remarks>
public class CliTaskHandler : TaskHandlerBase
{
    /// <summary>
    /// Creates a new CLI task handler.
    /// Registers a <see cref="Log.Handler"/> and a <see cref="Console.CancelKeyPress"/> handler.
    /// </summary>
    public CliTaskHandler()
    {
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

    /// <summary>
    /// Unregisters the <see cref="Log.Handler"/> and the <see cref="Console.CancelKeyPress"/> handler.
    /// </summary>
    public override void Dispose()
    {
        try
        {
            Console.CancelKeyPress -= CancelKeyPressHandler;
        }
        catch (IOException)
        {
            // Ignore problems caused by unusual terminal emulators
        }
        finally
        {
            base.Dispose();
        }
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
    /// Prints <see cref="Log"/> entries to the <see cref="Console"/>.
    /// </summary>
    protected override void DisplayLogEntry(LogSeverity severity, string message)
    {
        try
        {
            Console.ForegroundColor = GetLogColor(severity);
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException)
        {}

        Console.Error.WriteLine(message);
        try
        {
            Console.ResetColor();
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException)
        {}
    }

    /// <summary>
    /// Determines the color to use for a log entry based on the <see cref="LogSeverity"/>.
    /// </summary>
    protected static ConsoleColor GetLogColor(LogSeverity severity)
        => severity switch
        {
            LogSeverity.Debug => ConsoleColor.Blue,
            LogSeverity.Info=> ConsoleColor.Green,
            LogSeverity.Warn => ConsoleColor.DarkYellow,
            LogSeverity.Error => ConsoleColor.Red,
            _ => ConsoleColor.Black
        };

    /// <inheritdoc />
    protected override ICredentialProvider CredentialProvider
        => new NetrcCredentialProvider(
            WindowsUtils.IsWindowsNT
                ? IsInteractive ? new WindowsCliCredentialProvider() : new WindowsNonInteractiveCredentialProvider()
                : null);

#if !NET20 && !NET40
    /// <inheritdoc/>
    protected override bool IsInteractive
        => base.IsInteractive && !Console.IsErrorRedirected && !Console.IsInputRedirected;
#endif

    /// <inheritdoc/>
    public override void RunTask(ITask task)
    {
        #region Sanity checks
        if (task == null) throw new ArgumentNullException(nameof(task));
        #endregion

        Log.Info(task.Name);
        Console.Error.WriteLine(task.Name);
        task.Run(CancellationToken, CredentialProvider, IsInteractive ? new CliProgress() : null);
    }

    /// <inheritdoc/>
    protected override bool AskInteractive(string question, bool defaultAnswer)
    {
        Log.Debug($"Question: {question}");
        Console.Error.WriteLine(question);

        // Loop until the user has made a valid choice
        while (true)
        {
            Console.Error.Write(@"[Y/N]" + " ");
            switch ((Console.ReadLine() ?? throw new OperationCanceledException("stdin closed, unable to get answer to question")).ToLower())
            {
                case "y" or "yes":
                    return true;
                case "n" or "no":
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
        => Log.Error(exception);
}
