// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

/// <summary>
/// Helper methods for <see cref="TimeSpan"/>.
/// </summary>
public static class TimeSpanUtils
{
    /// <summary>
    /// Like <see cref="TimeSpan.FromSeconds(double)"/>, but with higher-than-millisecond accuracy.
    /// </summary>
    [Pure]
    public static TimeSpan FromSecondsAccurate(double seconds)
#if NETFRAMEWORK
        => TimeSpan.FromTicks((long)(seconds * TimeSpan.TicksPerSecond));
#else
        => TimeSpan.FromSeconds(seconds);
#endif

    extension(TimeSpan timeSpan)
    {
        /// <summary>
        /// Multiplies the timespan by a factor.
        /// </summary>
        [Pure]
        public TimeSpan Multiply(double factor)
#if NETFRAMEWORK
        => TimeSpan.FromTicks((long)(timeSpan.Ticks * factor));
#else
            => timeSpan * factor;
#endif

        /// <summary>
        /// Divides the timespan by a factor.
        /// </summary>
        [Pure]
        public TimeSpan Divide(double factor)
#if NETFRAMEWORK
            => TimeSpan.FromTicks((long)(timeSpan.Ticks / factor));
#else
            => timeSpan / factor;
#endif

        /// <summary>
        /// Divides two timespans, returning the quotient.
        /// </summary>
        [Pure]
        public double Divide(TimeSpan other)
#if NETFRAMEWORK
            => timeSpan.Ticks / (double)other.Ticks;
#else
            => timeSpan / other;
#endif
    }
}

