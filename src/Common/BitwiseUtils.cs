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
}
