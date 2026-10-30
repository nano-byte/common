// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

/// <summary>
/// Ways in which a file system may resolve different names to the same directory entry.
/// </summary>
/// <seealso cref="FileUtils.DetectNameAliasing"/>
[Flags]
public enum NameAliasing
{
    /// <summary>Names are resolved exactly.</summary>
    None = 0,

    /// <summary>Names that differ only in upper/lower case resolve to the same entry (e.g., NTFS, APFS by default, Linux casefold directories).</summary>
    Case = 1,

    /// <summary>Names that differ only in Unicode normalization form (NFC vs. NFD) resolve to the same entry or are rewritten on creation (e.g., APFS, HFS+).</summary>
    Normalization = 2
}
