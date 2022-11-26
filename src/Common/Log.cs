// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Globalization;
using System.Text.RegularExpressions;

namespace NanoByte.Common;

/// <summary>
/// A simple logging system. Writes to an in-memory buffer and a plain text file.
/// Allows additional handlers to be registered (e.g., for console or GUI output).
/// </summary>
public static partial class Log
{
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
    /// Adds a log entry.
    /// </summary>
    private static void AddEntry(LogSeverity severity, string? message, Exception? exception)
    {
        string logLine = GetLogLine(severity, message, exception);

        System.Diagnostics.Debug.Write(logLine);
        lock (_lock)
        {
            AddToBuffer(logLine);
            WriteToFile(logLine);
            _handlers.LastOrDefault()?.Invoke(severity, message, exception);
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

        return $"[{DateTime.Now.ToString("T", CultureInfo.InvariantCulture)}] {_processId} {severity.ToString().ToUpperInvariant()}: {string.Join(Environment.NewLine + "\t", message.Trim().SplitMultilineText())}";
    }

    /// <summary>
    /// Tries to read the last error log line written by another process.
    /// </summary>
    /// <param name="appName">The name of the app to get a log line for.</param>
    /// <param name="processId">The process ID to get a log line for. Leave <c>null</c> to get for any process ID.</param>
    public static string? ReadLastErrorFrom(string appName, int? processId = null)
    {
        if (GetLogFile(appName) is not {Exists: true} file) return null;

        var regEx = new Regex($@"\[\d\d:\d\d:\d\d\] {processId?.ToString(CultureInfo.InvariantCulture) ?? @"\d+"} ERROR: (.+)");
        try
        {
            using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);
            string? result = null;
            while (reader.ReadLine() is {} line)
            {
                if (regEx.Match(line) is {Success: true} match)
                    result = match.Groups[1].Value;
            }
            return result;
        }
        catch
        {
            return null;
        }
    }
}
