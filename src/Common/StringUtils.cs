// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.CompilerServices;
using System.Security.Cryptography;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsIgnoreCase(string? s1, string? s2)
        => string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Compares chars using case-insensitive comparison.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsIgnoreCase(char c1, char c2)
        => char.ToLowerInvariant(c1) == char.ToLowerInvariant(c2);

    /// <summary>
    /// Compares strings using case-sensitive, ordinal comparison and considering <c>null</c> and <see cref="string.Empty"/> equal.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsEmptyNull(string? s1, string? s2)
        => string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)
        || s1 == s2;

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
    /// Combines multiple strings into one, placing a <paramref name="separator"/> between the <paramref name="parts"/>.
    /// </summary>
    /// <param name="separator">The separator characters to place between the <paramref name="parts"/>.</param>
    /// <param name="parts">The strings to be combined.</param>
    [Pure]
    public static string Join(string separator, [InstantHandle] IEnumerable<string> parts)
        => string.Join(separator, parts);

    /// <param name="value">The string to operate on.</param>
    extension(string value)
    {
        /// <summary>
        /// Determines whether a string contains <paramref name="searchFor"/> using case-insensitive comparison.
        /// </summary>
        /// <param name="searchFor">The string to search for in <paramref name="value"/>.</param>
        [Pure]
        public bool ContainsIgnoreCase(string searchFor)
            => value.ToUpperInvariant().Contains(searchFor.ToUpperInvariant());

        /// <summary>
        /// Determines whether a string contains any whitespace characters.
        /// </summary>
        [Pure]
        public bool ContainsWhitespace()
            => value.Any(char.IsWhiteSpace);

        /// <summary>
        /// Determines whether a string starts with <paramref name="searchFor"/> and, if so, returns the <paramref name="rest"/> that comes after.
        /// </summary>
        [Pure]
        public bool StartsWith(string searchFor, [MaybeNullWhen(false)] out string rest)
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
        public bool EndsWith(string searchFor, [MaybeNullWhen(false)] out string rest)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool StartsWithIgnoreCase(string searchFor)
            => value.StartsWith(searchFor, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Determines whether a string ends with <paramref name="searchFor"/> with case-insensitive comparison.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EndsWithIgnoreCase(string searchFor)
            => value.EndsWith(searchFor, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Cuts off strings longer than <paramref name="maxLength"/> and replaces the rest with ellipsis (...).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string TrimOverflow(int maxLength)
            => (value.Length <= maxLength)
                ? value
                : value[..maxLength] + "...";

        /// <summary>
        /// Splits a multiline string to several strings and returns the result as a string array.
        /// </summary>
        [Pure]
        public IEnumerable<string> SplitMultilineText()
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
        /// Get everything to the left of the first occurrence of a character.
        /// </summary>
        [Pure]
        public string GetLeftPartAtFirstOccurrence(char searchFor)
        {
            int index = value.IndexOf(searchFor);
            return (index == -1) ? value : value[..index];
        }

        /// <summary>
        /// Get everything to the right of the first occurrence of a character.
        /// </summary>
        [Pure]
        public string GetRightPartAtFirstOccurrence(char searchFor)
        {
            int index = value.IndexOf(searchFor);
            return (index == -1) ? "" : value[(index + 1)..];
        }

        /// <summary>
        /// Get everything to the left of the last occurrence of a character.
        /// </summary>
        [Pure]
        public string GetLeftPartAtLastOccurrence(char searchFor)
        {
            int index = value.LastIndexOf(searchFor);
            return (index == -1) ? value : value[..index];
        }

        /// <summary>
        /// Get everything to the right of the last occurrence of a character.
        /// </summary>
        [Pure]
        public string GetRightPartAtLastOccurrence(char searchFor)
        {
            int index = value.LastIndexOf(searchFor);
            return (index == -1) ? value : value[(index + 1)..];
        }

        /// <summary>
        /// Get everything to the left of the first occurrence of a string.
        /// </summary>
        [Pure]
        public string GetLeftPartAtFirstOccurrence(string searchFor)
        {
            int index = value.IndexOf(searchFor, StringComparison.Ordinal);
            return (index == -1) ? value : value[..index];
        }

        /// <summary>
        /// Get everything to the right of the first occurrence of a string.
        /// </summary>
        [Pure]
        public string GetRightPartAtFirstOccurrence(string searchFor)
        {
            int index = value.IndexOf(searchFor, StringComparison.Ordinal);
            return (index == -1) ? "" : value[(index + searchFor.Length)..];
        }

        /// <summary>
        /// Get everything to the left of the last occurrence of a string.
        /// </summary>
        [Pure]
        public string GetLeftPartAtLastOccurrence(string searchFor)
        {
            int index = value.LastIndexOf(searchFor, StringComparison.Ordinal);
            return (index == -1) ? value : value[..index];
        }

        /// <summary>
        /// Get everything to the right of the last occurrence of a string.
        /// </summary>
        [Pure]
        public string GetRightPartAtLastOccurrence(string searchFor)
        {
            int index = value.LastIndexOf(searchFor, StringComparison.Ordinal);
            return (index == -1) ? "" : value[(index + searchFor.Length)..];
        }

        /// <summary>
        /// Turns an English singular noun into its plural form using common heuristics.
        /// </summary>
        [Pure]
        public string Pluralize()
        {
            if (string.IsNullOrEmpty(value)) return value;

            string lower = value.ToLowerInvariant();

            if (lower.EndsWith("y") && value.Length > 1 && !"aeiou".Contains(lower[^2]))
                return value[..^1] + "ies";

            if (lower.EndsWith("s") || lower.EndsWith("x") || lower.EndsWith("z")
             || lower.EndsWith("ch") || lower.EndsWith("sh"))
                return value + "es";

            return value + "s";
        }

        /// <summary>
        /// Returns a hash code for a string that is stable across processes.
        /// </summary>
        [Pure]
        public int GetStableHashCode()
        {
#if NETFRAMEWORK
            return value.GetHashCode();
#else
            // .GetHashCode() is not stable across processes outside of .NET Framework
            uint hash = 2166136261;
            foreach (char c in value)
            {
                hash ^= c;
                hash *= 16777619;
            }
            return unchecked((int)hash);
#endif
        }
    }

    /// <summary>
    /// Maps empty strings to null.
    /// </summary>
    [Pure]
    public static string? EmptyAsNull(this string? value) => string.IsNullOrEmpty(value) ? null : value;

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
        using var generator = RandomNumberGenerator.Create();
        byte[] array = new byte[(int)Math.Round(length * 0.75f)];
        generator.GetBytes(array);

        // Use base64 encoding without '=' padding and with '-' instead of 'l'
        return Convert.ToBase64String(array)[..length].Replace('l', '-');
    }
}
