// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace NanoByte.Common;

/// <summary>
/// Represents a point in time as the number of seconds since the Unix epoch (Unix timestamp).
/// </summary>
/// <param name="Seconds">The number of seconds since the Unix epoch (00:00:00 UTC on 1 January 1970).</param>
[Serializable]
public readonly record struct UnixTime(long Seconds) : ISerializable, IComparable<UnixTime>
{
    private UnixTime(SerializationInfo info, StreamingContext context)
        : this(info.GetInt64(nameof(Seconds)))
    {}

    /// <summary>
    /// Converts a number of <paramref name="seconds"/> to a Unix timestamp.
    /// </summary>
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static implicit operator UnixTime(long seconds)
        => new(seconds);

    /// <summary>
    /// Converts a Unix <paramref name="timestamp"/> to a number of seconds.
    /// </summary>
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static implicit operator long(UnixTime timestamp)
        => timestamp.Seconds;

    private const long EpochTicks = 62135596800;

    /// <summary>
    /// Converts a number of <paramref name="dateTime"/> to a Unix timestamp.
    /// </summary>
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static implicit operator UnixTime(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Local) dateTime = dateTime.ToUniversalTime();
        return dateTime.Ticks / TimeSpan.TicksPerSecond - EpochTicks;
    }

    /// <summary>
    /// Converts a Unix <paramref name="timestamp"/> to a <see cref="DateTime"/>.
    /// </summary>
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static implicit operator DateTime(UnixTime timestamp)
        => new((timestamp + EpochTicks) * TimeSpan.TicksPerSecond, DateTimeKind.Utc);

    /// <summary>
    /// Converts a number of <paramref name="dateTime"/> to a Unix timestamp.
    /// </summary>
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static implicit operator UnixTime(DateTimeOffset dateTime)
        => dateTime.UtcDateTime;

    /// <summary>
    /// Converts a Unix <paramref name="timestamp"/> to a <see cref="DateTimeOffset"/>.
    /// </summary>
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static implicit operator DateTimeOffset(UnixTime timestamp)
        => new(timestamp);

    /// <inheritdoc/>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
        => info.AddValue(nameof(Seconds), Seconds);

    /// <inheritdoc/>
    public int CompareTo(UnixTime other)
        => Seconds.CompareTo(other.Seconds);

    /// <inheritdoc/>
    public override int GetHashCode()
        => Seconds.GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
        => Seconds.ToString(CultureInfo.InvariantCulture);
}
