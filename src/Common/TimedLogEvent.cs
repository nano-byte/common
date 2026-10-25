// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NanoByte.Common;

/// <summary>
/// Structure that allows you to log timed execution blocks.
/// </summary>
/// <example>
///   <code>using(new LogEvent("Message")) {}</code>
/// </example>
public struct TimedLogEvent : IDisposable
{
    #region Variables
    private readonly Stopwatch _timer;
    private readonly string _entry;
    #endregion

    #region Event control
    /// <summary>
    /// Starts a new log event.
    /// </summary>
    /// <param name="entry">The entry for the log file. Elapsed time will automatically be appended.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimedLogEvent(string entry)
    {
        _entry = entry;
        _timer = Stopwatch.StartNew();
    }

    /// <summary>
    /// Ends the log event.
    /// </summary>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        _timer.Stop();
        Log.Info($"{_entry} => {_timer.Elapsed.TotalSeconds}s");
    }
    #endregion
}
