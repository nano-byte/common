// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

/// <summary>
/// Shared random number generation.
/// </summary>
public static class RandomShared
{
    /// <summary>
    /// Provides a <see cref="Random"/> instance safe to use on the current thread.
    /// </summary>
    public static Random Instance
#if NET
        => Random.Shared;
#else
        => _local ??= new(GetSeed());

    [ThreadStatic]
    private static Random? _local;

    private static int GetSeed()
    {
#if !NET20
        using
#endif
            var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        byte[] bytes = new byte[4];
        rng.GetBytes(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }
#endif
}
