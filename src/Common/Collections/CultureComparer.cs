// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Globalization;

namespace NanoByte.Common.Collections;

/// <summary>
/// This compares two <see cref="CultureInfo"/>s by alphabetically comparing their string representations.
/// </summary>
public sealed class CultureComparer : IComparer<CultureInfo>
{
    /// <summary>A singleton instance of the comparer.</summary>
    public static readonly CultureComparer Instance = new();

    private CultureComparer() {}

    /// <inheritdoc/>
    public int Compare(CultureInfo? x, CultureInfo? y)
        => ReferenceEquals(x, y)
            ? 0
            : StringComparer.Ordinal.Compare(x?.ToString(), y?.ToString());
}
