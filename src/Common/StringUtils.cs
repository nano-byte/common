/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using NanoByte.Common.Collections;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides additional or simplified string functions.
    /// </summary>
    public static class StringUtils
    {
        #region Comparing
        /// <summary>
        /// Compare strings using case-insensitive comparison.
        /// </summary>
        [Pure]
        public static bool EqualsIgnoreCase([CanBeNull] string s1, [CanBeNull] string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compare chars using case-insensitive comparison.
        /// </summary>
        [Pure]
        public static bool EqualsIgnoreCase(char c1, char c2)
        {
            return char.ToLowerInvariant(c1) == char.ToLowerInvariant(c2);
        }

        /// <summary>
        /// Compare strings using case sensitive, invariant culture comparison and considering <c>null</c> and <see cref="string.Empty"/> equal.
        /// </summary>
        [Pure]
        public static bool EqualsEmptyNull([CanBeNull] string s1, [CanBeNull] string s2)
        {
            if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)) return true;
            return s1 == s2;
        }

        /// <summary>
        /// Use case-insensitive compare to check for a contained string.
        /// </summary>
        /// <param name="value">The string to search.</param>
        /// <param name="text">The string to search for in <paramref name="value"/>.</param>
        [Pure]
        public static bool ContainsIgnoreCase([NotNull] this string value, [NotNull] string text)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            if (text == null) throw new ArgumentNullException("text");
            #endregion

            return value.ToUpperInvariant().Contains(text.ToUpperInvariant());
        }

        /// <summary>
        /// Checks whether a string contains any whitespace characters
        /// </summary>
        [Pure]
        public static bool ContainsWhitespace([NotNull] this string value)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            #endregion

            return value.Contains(" ") || value.Contains("\t") || value.Contains("\n") || value.Contains("\r");
        }

        /// <summary>
        /// Counts how many times a character occurs within a string.
        /// </summary>
        /// <param name="value">The string to search within.</param>
        /// <param name="token">The character to search for.</param>
        /// <returns>The number of occurences of <paramref name="token"/> wihin <paramref name="value"/>.</returns>
        [Pure]
        public static int CountOccurences([CanBeNull] this string value, char token)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            return value.Count(t => t == token);
        }

        /// <summary>
        /// Determines whether the beginning of this string matches a specific value case-insensitive comparison.
        /// </summary>
        [Pure]
        public static bool StartsWithIgnoreCase([NotNull] this string text, [NotNull] string value)
        {
            #region Sanity checks
            if (text == null) throw new ArgumentNullException("text");
            if (value == null) throw new ArgumentNullException("value");
            #endregion

            return text.StartsWith(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the end of this string matches a specific value case-insensitive comparison.
        /// </summary>
        [Pure]
        public static bool EndsWithIgnoreCase([NotNull] this string text, [NotNull] string value)
        {
            #region Sanity checks
            if (text == null) throw new ArgumentNullException("text");
            if (value == null) throw new ArgumentNullException("value");
            #endregion

            return text.EndsWith(value, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region Extraction
        /// <summary>
        /// Removes all occurences of a specific set of characters from a string.
        /// </summary>
        [Pure, ContractAnnotation("value:null => null; value:notnull => notnull")]
        public static string StripCharacters([CanBeNull] this string value, [NotNull] IEnumerable<char> characters)
        {
            #region Sanity checks
            if (characters == null) throw new ArgumentNullException("characters");
            #endregion

            if (value == null) return null;
            return new string(value.Except(characters.Contains).ToArray());
        }

        /// <summary>
        /// Returns a string with <paramref name="count"/> characters removed from the end.
        /// </summary>
        [Pure, NotNull]
        public static string StripFromEnd([NotNull] this string value, int count)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            #endregion

            return value.Substring(0, value.Length - count);
        }
        #endregion

        #region Splitting
        /// <summary>
        /// Splits a multiline string to several strings and returns the result as a string array.
        /// </summary>
        [Pure, NotNull, ItemNotNull]
        public static string[] SplitMultilineText([NotNull] this string value)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            #endregion

            var result = new List<string>();
            string[] splitted1 = value.Split('\n');
            string[] splitted2 = value.Split('\r');
            string[] splitted = splitted1.Length >= splitted2.Length ? splitted1 : splitted2;

            foreach (string s in splitted)
            {
                // Never add any \r or \n to the single lines
                if (s.EndsWithIgnoreCase("\r") || s.EndsWithIgnoreCase("\n"))
                    result.Add(s.StripFromEnd(count: 1));
                else if (s.StartsWithIgnoreCase("\n") || s.StartsWithIgnoreCase("\r"))
                    result.Add(s.Substring(1));
                else
                    result.Add(s);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Combines multiple strings into one, placing a <paramref name="separator"/> between the <paramref name="parts"/>.
        /// </summary>
        /// <param name="separator">The separator characters to place between the <paramref name="parts"/>.</param>
        /// <param name="parts">The strings to be combined.</param>
        /// <remarks>Works like <see cref="string.Join(string,string[])"/> but for <see cref="IEnumerable{T}"/>s.</remarks>
        [Pure, NotNull]
        public static string Join([NotNull] string separator, [NotNull, ItemNotNull] IEnumerable<string> parts)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(separator)) throw new ArgumentNullException("separator");
            if (parts == null) throw new ArgumentNullException("parts");
            #endregion

            var output = new StringBuilder();
            bool first = true;
            foreach (var part in parts)
            {
                // No separator before first or after last part
                if (first) first = false;
                else output.Append(separator);

                output.Append(part);
            }

            return output.ToString();
        }

        /// <summary>
        /// Get everything to the left of the first occurrence of a character.
        /// </summary>
        [Pure, NotNull]
        public static string GetLeftPartAtFirstOccurrence([NotNull] this string value, char ch)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            #endregion

            int index = value.IndexOf(ch);
            return (index == -1) ? value : value.Substring(0, index);
        }

        /// <summary>
        /// Get everything to the right of the first occurrence of a character.
        /// </summary>
        [Pure, NotNull]
        public static string GetRightPartAtFirstOccurrence([NotNull] this string value, char ch)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            #endregion

            int index = value.IndexOf(ch);
            return (index == -1) ? "" : value.Substring(index + 1);
        }

        /// <summary>
        /// Get everything to the left of the last occurrence of a character.
        /// </summary>
        [Pure, NotNull]
        public static string GetLeftPartAtLastOccurrence([NotNull] this string value, char ch)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            #endregion

            int index = value.LastIndexOf(ch);
            return (index == -1) ? value : value.Substring(0, index);
        }

        /// <summary>
        /// Get everything to the right of the last occurrence of a character.
        /// </summary>
        [Pure, NotNull]
        public static string GetRightPartAtLastOccurrence([NotNull] this string value, char ch)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            #endregion

            int index = value.LastIndexOf(ch);
            return (index == -1) ? value : value.Substring(index + 1);
        }

        /// <summary>
        /// Get everything to the left of the first occurrence of a string.
        /// </summary>
        [Pure, NotNull]
        public static string GetLeftPartAtFirstOccurrence([NotNull] this string value, [NotNull] string str)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            if (string.IsNullOrEmpty(str)) throw new ArgumentNullException("str");
            #endregion

            int index = value.IndexOf(str, StringComparison.Ordinal);
            return (index == -1) ? value : value.Substring(0, index);
        }

        /// <summary>
        /// Get everything to the right of the first occurrence of a string.
        /// </summary>
        [Pure, NotNull]
        public static string GetRightPartAtFirstOccurrence([NotNull] this string value, [NotNull] string str)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            if (string.IsNullOrEmpty(str)) throw new ArgumentNullException("str");
            #endregion

            int index = value.IndexOf(str, StringComparison.Ordinal);
            return (index == -1) ? "" : value.Substring(index + str.Length);
        }

        /// <summary>
        /// Get everything to the left of the last occurrence of a string.
        /// </summary>
        [Pure, NotNull]
        public static string GetLeftPartAtLastOccurrence([NotNull] this string value, [NotNull] string str)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            if (string.IsNullOrEmpty(str)) throw new ArgumentNullException("str");
            #endregion

            int index = value.LastIndexOf(str, StringComparison.Ordinal);
            return (index == -1) ? value : value.Substring(0, index);
        }

        /// <summary>
        /// Get everything to the right of the last occurrence of a string.
        /// </summary>
        [Pure, NotNull]
        public static string GetRightPartAtLastOccurrence([NotNull] this string value, [NotNull] string str)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            if (string.IsNullOrEmpty(str)) throw new ArgumentNullException("str");
            #endregion

            int index = value.LastIndexOf(str, StringComparison.Ordinal);

            return (index == -1) ? "" : value.Substring(index + str.Length);
        }
        #endregion

        #region Arguments escaping
        /// <summary>
        /// Escapes a string for use as a Windows command-line argument, making sure it is encapsulated within <c>"</c> if it contains whitespace characters.
        /// </summary>
        /// <remarks>
        /// This corresponds to Windows' handling of command-line arguments as specified in:
        /// http://msdn.microsoft.com/library/17w5ykft
        /// </remarks>
        [Pure, NotNull]
        public static string EscapeArgument([NotNull] this string value)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            #endregion

            if (value.Length == 0) return "\"\"";

            // Add leading quotation mark if there are whitespaces
            bool containsWhitespace = ContainsWhitespace(value);
            var result = containsWhitespace ? new StringBuilder("\"", value.Length + 2) : new StringBuilder(value.Length);

            // Split by quotation marks
            string[] parts = value.Split('"');
            for (int i = 0; i < parts.Length; i++)
            {
                // Count slashes preceeding the quotation mark
                int slashesCount = parts[i].Length - parts[i].TrimEnd('\\').Length;

                result.Append(parts[i]);
                if (i < parts.Length - 1)
                { // Not last part
                    for (int j = 0; j < slashesCount; j++) result.Append('\\'); // Double number of slashes
                    result.Append("\\\""); // Escaped quotation mark
                }
                else if (containsWhitespace)
                { // Last part if there are whitespaces
                    for (int j = 0; j < slashesCount; j++) result.Append('\\'); // Double number of slashes
                    result.Append('"'); // Non-escaped quotation mark
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Combines multiple strings into one for use as a Windows command-line argument using <see cref="EscapeArgument"/>.
        /// </summary>
        /// <param name="parts">The strings to be combined.</param>
        /// <remarks>
        /// This coressponds to Windows' handling of command-line arguments as specified in:
        /// http://msdn.microsoft.com/library/17w5ykft
        /// </remarks>
        [Pure, NotNull]
        public static string JoinEscapeArguments([NotNull, ItemNotNull] this IEnumerable<string> parts)
        {
            #region Sanity checks
            if (parts == null) throw new ArgumentNullException("parts");
            #endregion

            var output = new StringBuilder();
            bool first = true;
            foreach (string part in parts)
            {
                // No separator before first or after last part
                if (first) first = false;
                else output.Append(' ');

                output.Append(EscapeArgument(part));
            }

            return output.ToString();
        }
        #endregion

        #region base64
        /// <summary>
        /// Encodes a string as UTF-8 in base64.
        /// </summary>
        [Pure, NotNull]
        public static string Base64Utf8Encode([NotNull] this string value)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            #endregion

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Decodes a UTF-8 in base64 string.
        /// </summary>
        /// <exception cref="FormatException"><paramref name="value"/> is not a valid base 64 string.</exception>
        [Pure, NotNull]
        public static string Base64Utf8Decode([NotNull] this string value)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            #endregion

            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }
        #endregion

        #region base32
        private static readonly char[] _base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();
        private const int NormaleByteSize = 8, Base32ByteSize = 5;

        /// <summary>
        /// Encodes a byte array in base32 without padding.
        /// </summary>
        [Pure, NotNull]
        public static string Base32Encode([NotNull] this byte[] data)
        {
            #region Sanity checks
            if (data == null) throw new ArgumentNullException("data");
            #endregion

            if (data.Length == 0) return "";

            int i = 0, index = 0;
            var result = new StringBuilder((data.Length + 7) * NormaleByteSize / Base32ByteSize);

            while (i < data.Length)
            {
                // ReSharper disable ConditionIsAlwaysTrueOrFalse
                int currentByte = (data[i] >= 0) ? data[i] : (data[i] + 256);
                int digit;

                // Is the current digit going to span a byte boundary?
                if (index > (NormaleByteSize - Base32ByteSize))
                {
                    int nextByte = (i + 1) < data.Length
                        ? ((data[i + 1] >= 0) ? data[i + 1]
                            : (data[i + 1] + 256)) : 0;

                    digit = currentByte & (0xFF >> index);
                    index = (index + Base32ByteSize) % NormaleByteSize;
                    digit <<= index;
                    digit |= nextByte >> (NormaleByteSize - index);
                    i++;
                }
                else
                {
                    digit = (currentByte >> (NormaleByteSize - (index + Base32ByteSize))) & 0x1F;
                    index = (index + Base32ByteSize) % NormaleByteSize;
                    if (index == 0)
                        i++;
                }
                // ReSharper restore ConditionIsAlwaysTrueOrFalse
                result.Append(_base32Alphabet[digit]);
            }

            return result.ToString();
        }
        #endregion

        #region base16
        /// <summary>
        /// Encodes a byte array in base16 (hexadecimal).
        /// </summary>
        [Pure, NotNull]
        public static string Base16Encode([NotNull] this byte[] data)
        {
            #region Sanity checks
            if (data == null) throw new ArgumentNullException("data");
            #endregion

            if (data.Length == 0) return "";

            return BitConverter.ToString(data).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Decodes a base16 (hexadecimal) to a byte array.
        /// </summary>
        [Pure, NotNull]
        public static byte[] Base16Decode([NotNull] this string encoded)
        {
            #region Sanity checks
            if (encoded == null) throw new ArgumentNullException("encoded");
            #endregion

            var result = new byte[encoded.Length / 2];
            for (int i = 0; i < encoded.Length / 2; i++)
                result[i] = Convert.ToByte(encoded.Substring(i * 2, 2), 16);
            return result;
        }
        #endregion

        #region Hash
        /// <summary>
        /// Computes the hash value of a string encoded as UTF-8.
        /// </summary>
        /// <param name="value">The string to hash.</param>
        /// <param name="algorithm">The hashing algorithm to use.</param>
        /// <returns>A hexadecimal string representation of the hash value.</returns>
        [Pure, NotNull]
        public static string Hash([NotNull] this string value, [NotNull] HashAlgorithm algorithm)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException("value");
            if (algorithm == null) throw new ArgumentNullException("algorithm");
            #endregion

            var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
        #endregion

        #region Generate password
        /// <summary>
        /// Returns a string filled with random human-readable ASCII characters based on a cryptographic random number generator.
        /// </summary>
        /// <param name="length">The length of the string to be generated.</param>
        [Pure, NotNull]
        public static string GeneratePassword(int length)
        {
            var generator = RandomNumberGenerator.Create();
            var array = new byte[(int)Math.Round(length * 3 / 4f)];
            generator.GetBytes(array);

            // Use base64 encoding without '=' padding and with '-' instead of 'l'
            return Convert.ToBase64String(array).Substring(0, length).Replace('l', '-');
        }
        #endregion
    }
}
