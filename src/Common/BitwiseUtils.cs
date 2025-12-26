// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

/// <summary>
/// Provides utility functions for bitwise operations.
/// </summary>
public static class BitwiseUtils
{
    /// <summary>
    /// Combines two byte arrays via Exclusive Or.
    /// </summary>
    [Pure]
    public static byte[] XOr(byte[] array1, byte[] array2)
    {
        if (array1.Length != array2.Length) throw new ArgumentException("Length of both arrays must be equal.", nameof(array1));

        byte[] result = array1.ToArray();
        for (int i = 0; i < array2.Length; i++) result[i] ^= array2[i];
        return result;
    }

    /// <summary>
    /// Extracts the low-order word (first 16 bits) of a 32-bit integer.
    /// </summary>
    [CLSCompliant(false)]
    [Pure]
    public static short LoWord(this uint l)
    {
        unchecked
        {
            return (short)(l & 0xffff);
        }
    }

    /// <summary>
    /// Extracts the low-order word (first 16 bits) of a 32-bit integer.
    /// </summary>
    [Pure]
    public static short LoWord(this int l)
    {
        unchecked
        {
            return (short)(l & 0xffff);
        }
    }

    /// <summary>
    /// Extracts the high-order word (last 16 bits) of a 32-bit integer.
    /// </summary>
    [CLSCompliant(false)]
    [Pure]
    public static short HiWord(this uint l)
    {
        unchecked
        {
            return (short)(l >> 16);
        }
    }

    /// <summary>
    /// Extracts the high-order word (last 16 bits) of a 32-bit integer.
    /// </summary>
    [Pure]
    public static short HiWord(this int l)
    {
        unchecked
        {
            return (short)(l >> 16);
        }
    }

    /// <summary>
    /// Extracts the high-order nibble (first 4 bits) of a byte.
    /// </summary>
    /// <param name="b">The byte to extract from.</param>
    /// <returns>The high-order nibble as an integer.</returns>
    [Pure]
    public static int HiNibble(this byte b) => b >> 4;

    /// <summary>
    /// Extracts the low-order nibble (last 4 bits) of a byte.
    /// </summary>
    /// <param name="b">The byte to extract from.</param>
    /// <returns>The low-order nibble as an integer.</returns>
    [Pure]
    public static int LoNibble(this byte b) => b & 15;

    /// <summary>
    /// Combines a high nibble and a low nibble (4 bits) into a single byte.
    /// </summary>
    [Pure]
    public static byte CombineHiLoNibble(int high, int low) => (byte)((high << 4) + low);
}
