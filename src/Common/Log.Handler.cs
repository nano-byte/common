// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

/// <summary>
/// Describes an event relating to an entry in the <see cref="Log"/>.
/// </summary>
/// <param name="severity">The type/severity of the entry.</param>
/// <param name="message">The message of the entry.</param>
/// <param name="exception">An optional exception associated with the entry.</param>
/// <seealso cref="Log.Handler"/>
public delegate void LogEntryEventHandler(LogSeverity severity, string? message, Exception? exception);

partial class Log
{
    private static readonly List<LogEntryEventHandler> _handlers = new();

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
                ResetBuffer();
                _handlers.Add(value);
            }
        }
        remove
        {
            if (value == null) return;
            lock (_lock) _handlers.Remove(value);
        }
    }
}
