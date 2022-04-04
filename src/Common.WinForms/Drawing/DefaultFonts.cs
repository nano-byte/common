// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Drawing;

/// <summary>
/// Common fonts used in WinForms.
/// </summary>
public static class DefaultFonts
{
    /// <summary>
    /// The classic WinForms default font (Microsoft Sans Serif).
    /// </summary>
    public static Font Classic { get; } = new("Microsoft Sans Serif", 8.25f);

    /// <summary>
    /// The modern WinForms default font (Segoe UI) but with the classic font size (8.25).
    /// </summary>
    public static Font Modern { get; } = new("Segoe UI", Classic.Size);
}
