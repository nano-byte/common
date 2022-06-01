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

        Log.Handler += LogHandler;
    }

    /// <summary>
    /// Unregisters the <see cref="Log.Handler"/> and the <see cref="Console.CancelKeyPress"/> handler.
    /// </summary>
    public override void Dispose()
    {
        try
        {
            Log.Handler -= LogHandler;
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
    /// Prints <see cref="Log"/> messages to the <see cref="Console"/> based on their <see cref="LogSeverity"/> and the current <see cref="Verbosity"/> level.
    /// </summary>
    /// <param name="severity">The type/severity of the entry.</param>
    /// <param name="message">The message text of the entry.</param>
    /// <param name="exception">An optional exception associated with the entry.</param>
    protected virtual void LogHandler(LogSeverity severity, string message, Exception? exception)
    {
        void WriteLine(ConsoleColor color)
        {
            try
            {
                Console.ForegroundColor = color;
            }
            catch (InvalidOperationException)
            {}
            catch (IOException)
            {}

            Console.Error.WriteLine(message);
            try
            {
                Console.ResetColor();
            }
            catch (InvalidOperationException)
            {}
            catch (IOException)
            {}
        }

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
            Console.Error.WriteLine(exception.ToString());
    }

    /// <inheritdoc />
    public override ICredentialProvider? CredentialProvider
        => WindowsUtils.IsWindowsNT
            ? IsInteractive ? new WindowsCliCredentialProvider() : new WindowsNonInteractiveCredentialProvider()
            : null;

#if !NET20 && !NET40
    /// <inheritdoc/>
    protected override bool IsInteractive
        => base.IsInteractive && !Console.IsErrorRedirected;
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
            switch ((Console.ReadLine() ?? throw new IOException("input stream closed, unable to get user input")).ToLower())
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
        => Log.Error(exception.Message, exception);
}
