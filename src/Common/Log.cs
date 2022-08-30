// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Globalization;
using System.Text;
using NanoByte.Common.Info;
using NanoByte.Common.Storage;

namespace NanoByte.Common;

/// <summary>
/// Describes an event relating to an entry in the <see cref="Log"/>.
/// </summary>
/// <param name="severity">The type/severity of the entry.</param>
/// <param name="message">The message of the entry.</param>
/// <param name="exception">An optional exception associated with the entry.</param>
/// <seealso cref="Log.Handler"/>
public delegate void LogEntryEventHandler(LogSeverity severity, string? message, Exception? exception);

/// <summary>
/// Sends log messages to custom handlers or the console.
/// Additionally writes to <see cref="System.Diagnostics.Debug"/>, an in-memory buffer and a plain text file.
/// </summary>
public static class Log
{
    #region File Writer
    private static StreamWriter? _fileWriter;

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "The static constructor is used to add an identification header to the log file")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any kind of problems writing the log file should be ignored")]
    static Log()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"{AppInfo.Current.Name} {Environment.UserName} Log.txt");

        // Try to open the file for writing but give up right away if there are any problems
        FileStream file;
        try
        {
            file = new(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        }
        catch
        {
            return;
        }

        // Clear cache file once it reaches 1MB
        if (file.Length > 1024 * 1024)
        {
            file.Dispose();
            file = new(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        }

        // When writing to a new file use UTF-8 with BOM, otherwise keep existing encoding
        _fileWriter = (file.Length == 0 ? new StreamWriter(file, Encoding.UTF8) : new StreamWriter(file));
        _fileWriter.AutoFlush = true;

        // Go to end of file
        _fileWriter.BaseStream.Seek(0, SeekOrigin.End);

        // Add session identification block to the file
        _fileWriter.WriteLine("");
        _fileWriter.WriteLine("/// " + AppInfo.Current.NameVersion);
        _fileWriter.WriteLine("/// Install base: " + Locations.InstallBase);
        _fileWriter.WriteLine("/// Command-line args: " + Environment.GetCommandLineArgs().JoinEscapeArguments());
        _fileWriter.WriteLine("/// Log session started at: " + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        _fileWriter.WriteLine("");
    }
    #endregion

    private static readonly List<LogEntryEventHandler> _handlers = new()
    {
        // Default handler
        (severity, message, _) =>
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
                case LogSeverity.Warn:
                    WriteLine(ConsoleColor.DarkYellow);
                    break;
                case LogSeverity.Error:
                    WriteLine(ConsoleColor.Red);
                    break;
            }
        }
    };

    /// <summary>
    /// Invoked when a new entry is added to the log.
    /// Only the newest (last) registered handler is invoked.
    /// <see cref="Console"/> output is used as a fallback if no handlers are registered.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
    public static event LogEntryEventHandler? Handler
    {
        add
        {
            lock (_lock)
            {
                if (value == null) return;
                _sessionContent = new(); // Reset per session (indicated by new handler)
                _handlers.Add(value);
            }
        }
        remove
        {
            if (value == null) return;
            lock (_lock) _handlers.Remove(value);
        }
    }

    private static StringBuilder _sessionContent = new();

    /// <summary>
    /// Collects all log entries from this application session.
    /// </summary>
    public static string Content
    {
        get
        {
            lock (_lock)
                return _sessionContent.ToString();
        }
    }

    /// <summary>
    /// Writes information to help developers diagnose problems to the log.
    /// </summary>
    public static void Debug(string message, Exception? exception = null) => AddEntry(LogSeverity.Debug, message, exception);

    /// <summary>
    /// Writes information to help developers diagnose problems to the log.
    /// </summary>
    public static void Debug(Exception exception) => AddEntry(LogSeverity.Debug, null, exception);

    /// <summary>
    /// Writes nice-to-know information to the log.
    /// </summary>
    public static void Info(string message, Exception? exception = null) => AddEntry(LogSeverity.Info, message, exception);

    /// <summary>
    /// Writes nice-to-know information to the log.
    /// </summary>
    public static void Info(Exception exception) => AddEntry(LogSeverity.Info, null, exception);

    /// <summary>
    /// Writes a warning that doesn't have to be acted upon immediately to the log.
    /// </summary>
    public static void Warn(string message, Exception? exception = null) => AddEntry(LogSeverity.Warn, message, exception);

    /// <summary>
    /// Writes a warning that doesn't have to be acted upon immediately to the log.
    /// </summary>
    public static void Warn(Exception exception) => AddEntry(LogSeverity.Warn, null, exception);

    /// <summary>
    /// Writes a critical error that should be attended to to the log.
    /// </summary>
    public static void Error(string message, Exception? exception = null) => AddEntry(LogSeverity.Error, message, exception);

    /// <summary>
    /// Writes a critical error that should be attended to to the log.
    /// </summary>
    public static void Error(Exception exception) => AddEntry(LogSeverity.Error, null, exception);

    private static readonly object _lock = new();

    /// <summary>
    /// Adds a log entry to the log file and sends it to the last entry in <see cref="_handlers"/>.
    /// </summary>
    private static void AddEntry(LogSeverity severity, string? message, Exception? exception)
    {
        string logLine = GetLogLine(severity, message, exception);

        System.Diagnostics.Debug.Write(logLine);
        lock (_lock)
        {
            _sessionContent.AppendLine(logLine);
            WriteToFile(logLine);
            _handlers.Last()(severity, message, exception);
        }
    }

    /// <summary>
    /// Builds a log line containing timestamp, severity, message and exception information.
    /// </summary>
    private static string GetLogLine(LogSeverity severity, string? message, Exception? exception)
    {
        if (exception == null) message ??= "";
        else if (message == null) message = exception.ToString();
        else message += Environment.NewLine + exception;

        return "[" + DateTime.Now.ToString("T", CultureInfo.InvariantCulture) + "] "
             + severity.ToString().ToUpperInvariant() + ": "
             + string.Join(Environment.NewLine + "\t", message.Trim().SplitMultilineText());
    }

    /// <summary>
    /// Appends a line to the log file.
    /// </summary>
    private static void WriteToFile(string logLine)
    {
        try
        {
            _fileWriter?.WriteLine(logLine);
        }
        #region Error handling
        catch (Exception ex)
        {
            _fileWriter = null;
            Console.Error.WriteLine("Error writing to log file:");
            Console.Error.WriteLine(ex);
        }
        #endregion
    }
}
