// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Diagnostics;

#if !NET20 && !NET40
using System.Runtime.CompilerServices;
#endif

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
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public TimedLogEvent(string entry)
    {
        _entry = entry;
        _timer = Stopwatch.StartNew();
    }

    /// <summary>
    /// Ends the log event.
    /// </summary>

#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void Dispose()
    {
        _timer.Stop();
        Log.Info(_entry + " => " + (float)_timer.Elapsed.TotalSeconds + "s");
    }
    #endregion
}
