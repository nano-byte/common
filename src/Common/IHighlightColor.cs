// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing;

namespace NanoByte.Common;

/// <summary>
/// An object that can be highlighted with a specific color in graphical representations.
/// </summary>
public interface IHighlightColor
{
    /// <summary>
    /// The color to highlight this object with in graphical representations. <see cref="Color.Empty"/> for no highlighting.
    /// </summary>
    Color HighlightColor { get; }
}