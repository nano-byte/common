// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Security.Cryptography;

#if NET20
using System.Text;
#endif

#if !NET20 && !NET40
using System.Runtime.CompilerServices;
#endif

namespace NanoByte.Common;

/// <summary>
/// Provides additional or simplified string functions.
/// </summary>
public static class StringUtils
{
    /// <summary>
    /// Compares strings using case-insensitive comparison.
    /// </summary>
    [Pure]
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static bool EqualsIgnoreCase(string? s1, string? s2)
        => string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Compares chars using case-insensitive comparison.
    /// </summary>
    [Pure]
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static bool EqualsIgnoreCase(char c1, char c2)
        => char.ToLowerInvariant(c1) == char.ToLowerInvariant(c2);

    /// <summary>
    /// Compares strings using case sensitive, invariant culture comparison and considering <c>null</c> and <see cref="string.Empty"/> equal.
    /// </summary>
    [Pure]
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static bool EqualsEmptyNull(string? s1, string? s2)
        => string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)
        || s1 == s2;

    /// <summary>
    /// Determines whether a string contains <paramref name="searchFor"/> using case-insensitive comparison.
    /// </summary>
    /// <param name="value">The string to search.</param>
    /// <param name="searchFor">The string to search for in <paramref name="value"/>.</param>
    [Pure]
    public static bool ContainsIgnoreCase(this string value, string searchFor)
        => value.ToUpperInvariant().Contains(searchFor.ToUpperInvariant());

    /// <summary>
    /// Determines whether a string contains any whitespace characters.
    /// </summary>
    [Pure]
    public static bool ContainsWhitespace(this string value)
        => value.Any(char.IsWhiteSpace);

    /// <summary>
    /// Determines whether a string starts with <paramref name="searchFor"/> and, if so, returns the <paramref name="rest"/> that comes after.
    /// </summary>
    [Pure]
    public static bool StartsWith(this string value, string searchFor, [MaybeNullWhen(false)] out string rest)
    {
        if (value.StartsWith(searchFor))
        {
            rest = value[searchFor.Length..];
            return true;
        }
        else
        {
            rest = null;
            return false;
        }
    }

    /// <summary>
    /// Determines whether a string starts with <paramref name="searchFor"/> and, if so, returns the <paramref name="rest"/> that comes before.
    /// </summary>
    [Pure]
    public static bool EndsWith(this string value, string searchFor, [MaybeNullWhen(false)] out string rest)
    {
        if (value.EndsWith(searchFor))
        {
            rest = value[..^searchFor.Length];
            return true;
        }
        else
        {
            rest = null;
            return false;
        }
    }

    /// <summary>
    /// Determines whether a string starts with <paramref name="searchFor"/> with case-insensitive comparison.
    /// </summary>
    [Pure]
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static bool StartsWithIgnoreCase(this string value, string searchFor)
        => value.StartsWith(searchFor, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Determines whether a string ends with <paramref name="searchFor"/> with case-insensitive comparison.
    /// </summary>
    [Pure]
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static bool EndsWithIgnoreCase(this string value, string searchFor)
        => value.EndsWith(searchFor, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Removes all occurrences of a specific set of characters from a string.
    /// </summary>
    [Pure]
    [return: NotNullIfNotNull("value")]
    public static string? RemoveCharacters(this string? value, [InstantHandle] IEnumerable<char> characters)
        => value == null
            ? null
            : new string(value.Except(characters.Contains).ToArray());

    /// <summary>
    /// Cuts off strings longer than <paramref name="maxLength"/> and replaces the rest with ellipsis (...).
    /// </summary>
#if !NET20 && !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static string TrimOverflow(this string value, int maxLength)
        => (value.Length <= maxLength)
            ? value
            : value[..maxLength] + "...";

    /// <summary>
    /// Splits a multiline string to several strings and returns the result as a string array.
    /// </summary>
    [Pure]
    public static IEnumerable<string> SplitMultilineText(this string value)
    {
        string[] split1 = value.Split('\n');
        string[] split2 = value.Split('\r');
        string[] split = split1.Length >= split2.Length ? split1 : split2;

        foreach (string line in split)
        {
            // Never add any \r or \n to the single lines
            if (line.EndsWithIgnoreCase("\r") || line.EndsWithIgnoreCase("\n"))
                yield return line[..^1];
            else if (line.StartsWithIgnoreCase("\n") || line.StartsWithIgnoreCase("\r"))
                yield return line[1..];
            else
                yield return line;
        }
    }

    /// <summary>
    /// Combines multiple strings into one, placing a <paramref name="separator"/> between the <paramref name="parts"/>.
    /// </summary>
    /// <param name="separator">The separator characters to place between the <paramref name="parts"/>.</param>
    /// <param name="parts">The strings to be combined.</param>
    [Pure]
    public static string Join(string separator, [InstantHandle] IEnumerable<string> parts)
#if NET20
    {
        var output = new StringBuilder();
        bool first = true;
        foreach (string part in parts)
        {
            // No separator before first or after last part
            if (first) first = false;
            else output.Append(separator);

            output.Append(part);
        }

        return output.ToString();
    }
#else
        => string.Join(separator, parts);
#endif

    /// <summary>
    /// Get everything to the left of the first occurrence of a character.
    /// </summary>
    [Pure]
    public static string GetLeftPartAtFirstOccurrence(this string value, char searchFor)
    {
        int index = value.IndexOf(searchFor);
        return (index == -1) ? value : value[..index];
    }

    /// <summary>
    /// Get everything to the right of the first occurrence of a character.
    /// </summary>
    [Pure]
    public static string GetRightPartAtFirstOccurrence(this string value, char searchFor)
    {
        int index = value.IndexOf(searchFor);
        return (index == -1) ? "" : value[(index + 1)..];
    }

    /// <summary>
    /// Get everything to the left of the last occurrence of a character.
    /// </summary>
    [Pure]
    public static string GetLeftPartAtLastOccurrence(this string value, char searchFor)
    {
        int index = value.LastIndexOf(searchFor);
        return (index == -1) ? value : value[..index];
    }

    /// <summary>
    /// Get everything to the right of the last occurrence of a character.
    /// </summary>
    [Pure]
    public static string GetRightPartAtLastOccurrence(this string value, char searchFor)
    {
        int index = value.LastIndexOf(searchFor);
        return (index == -1) ? value : value[(index + 1)..];
    }

    /// <summary>
    /// Get everything to the left of the first occurrence of a string.
    /// </summary>
    [Pure]
    public static string GetLeftPartAtFirstOccurrence(this string value, string searchFor)
    {
        int index = value.IndexOf(searchFor, StringComparison.Ordinal);
        return (index == -1) ? value : value[..index];
    }

    /// <summary>
    /// Get everything to the right of the first occurrence of a string.
    /// </summary>
    [Pure]
    public static string GetRightPartAtFirstOccurrence(this string value, string searchFor)
    {
        int index = value.IndexOf(searchFor, StringComparison.Ordinal);
        return (index == -1) ? "" : value[(index + searchFor.Length)..];
    }

    /// <summary>
    /// Get everything to the left of the last occurrence of a string.
    /// </summary>
    [Pure]
    public static string GetLeftPartAtLastOccurrence(this string value, string searchFor)
    {
        int index = value.LastIndexOf(searchFor, StringComparison.Ordinal);
        return (index == -1) ? value : value[..index];
    }

    /// <summary>
    /// Get everything to the right of the last occurrence of a string.
    /// </summary>
    [Pure]
    public static string GetRightPartAtLastOccurrence(this string value, string searchFor)
    {
        int index = value.LastIndexOf(searchFor, StringComparison.Ordinal);
        return (index == -1) ? "" : value[(index + searchFor.Length)..];
    }

    /// <summary>
    /// Formats a byte number in human-readable form (KB, MB, GB).
    /// </summary>
    /// <param name="value">The value in bytes.</param>
    /// <param name="provider">Provides culture-specific formatting information.</param>
    public static string FormatBytes(this long value, IFormatProvider? provider = null)
        => value switch
        {
            >= 1024L*1024L*1024L*1024L => string.Format(provider, Resources.BytesTebi, value / 1024f / 1024f / 1024f / 1024f),
            >= 1024L*1024L*1024L => string.Format(provider, Resources.BytesGibi, value / 1024f / 1024f / 1024f),
            >= 1024L*1024L => string.Format(provider, Resources.BytesMebi, value / 1024f / 1024f),
            >= 1024L =>string.Format(provider, Resources.BytesKibi, value / 1024f),
            _ => string.Format(provider, Resources.Bytes, value),
        };

    /// <summary>
    /// Returns a string filled with random human-readable ASCII characters based on a cryptographic random number generator.
    /// </summary>
    /// <param name="length">The length of the string to be generated.</param>
    [Pure]
    public static string GeneratePassword(int length)
    {
        var generator = RandomNumberGenerator.Create();
        byte[] array = new byte[(int)Math.Round(length * 3 / 4f)];
        generator.GetBytes(array);

        // Use base64 encoding without '=' padding and with '-' instead of 'l'
        return Convert.ToBase64String(array)[..length].Replace('l', '-');
    }
}
