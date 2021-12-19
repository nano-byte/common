// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Security.Cryptography;
using System.Text;

namespace NanoByte.Common
{
    /// <summary>
    /// Helper methods for encoding strings, decoding byte arrays, calculating hashes, etc..
    /// </summary>
    public static class EncodingUtils
    {
        /// <summary>
        /// UTF-8 encoding without BOM (byte order marker).
        /// </summary>
        public static readonly Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        /// <summary>
        /// Computes the hash value of a string encoded as UTF-8.
        /// </summary>
        /// <param name="value">The string to hash.</param>
        /// <param name="algorithm">The hashing algorithm to use.</param>
        /// <returns>A hexadecimal string representation of the hash value.</returns>
        [Pure]
        public static string Hash(this string value, HashAlgorithm algorithm)
        {
            #region Sanity checks
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (algorithm == null) throw new ArgumentNullException(nameof(algorithm));
            #endregion

            var hash = algorithm.ComputeHash(Utf8.GetBytes(value));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Encodes a string as UTF-8 in base64.
        /// </summary>
        [Pure]
        public static string Base64Utf8Encode(this string value)
            => Convert.ToBase64String(Utf8.GetBytes(value ?? throw new ArgumentNullException(nameof(value))));

        /// <summary>
        /// Decodes a UTF-8 in base64 string.
        /// </summary>
        /// <exception cref="FormatException"><paramref name="value"/> is not a valid base 64 string.</exception>
        [Pure]
        public static string Base64Utf8Decode(this string value)
            => Utf8.GetString(Convert.FromBase64String(value ?? throw new ArgumentNullException(nameof(value))));

        private static readonly char[] _base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

        private const int NormalByteSize = 8, Base32ByteSize = 5;

        /// <summary>
        /// Encodes a byte array in base32 without padding.
        /// </summary>
        [Pure]
        public static string Base32Encode(this byte[] data)
        {
            #region Sanity checks
            if (data == null) throw new ArgumentNullException(nameof(data));
            #endregion

            if (data.Length == 0) return "";

            int i = 0, index = 0;
            var result = new StringBuilder((data.Length + 7) * NormalByteSize / Base32ByteSize);

            while (i < data.Length)
            {
                // ReSharper disable ConditionIsAlwaysTrueOrFalse
                int currentByte = (data[i] >= 0) ? data[i] : (data[i] + 256);
                int digit;

                // Is the current digit going to span a byte boundary?
                if (index > (NormalByteSize - Base32ByteSize))
                {
                    int nextByte = ((i + 1) < data.Length)
                        ? ((data[i + 1] >= 0) ? data[i + 1] : (data[i + 1] + 256))
                        : 0;

                    digit = currentByte & (0xFF >> index);
                    index = (index + Base32ByteSize) % NormalByteSize;
                    digit <<= index;
                    digit |= nextByte >> (NormalByteSize - index);
                    i++;
                }
                else
                {
                    digit = (currentByte >> (NormalByteSize - (index + Base32ByteSize))) & 0x1F;
                    index = (index + Base32ByteSize) % NormalByteSize;
                    if (index == 0)
                        i++;
                }
                // ReSharper restore ConditionIsAlwaysTrueOrFalse
                result.Append(_base32Alphabet[digit]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Encodes a byte array in base16 (hexadecimal).
        /// </summary>
        [Pure]
        public static string Base16Encode(this byte[] data)
        {
            #region Sanity checks
            if (data == null) throw new ArgumentNullException(nameof(data));
            #endregion

            if (data.Length == 0) return "";

            return BitConverter.ToString(data).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Decodes a base16 (hexadecimal) to a byte array.
        /// </summary>
        [Pure]
        public static byte[] Base16Decode(this string encoded)
        {
            #region Sanity checks
            if (encoded == null) throw new ArgumentNullException(nameof(encoded));
            #endregion

            var result = new byte[encoded.Length / 2];
            for (int i = 0; i < encoded.Length / 2; i++)
                result[i] = Convert.ToByte(encoded.Substring(i * 2, 2), 16);
            return result;
        }
    }
}
