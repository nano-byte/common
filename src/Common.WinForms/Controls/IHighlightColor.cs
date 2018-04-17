// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// An object that can be highlighted with a specific color in list representations.
    /// </summary>
    /// <seealso cref="Controls.FilteredTreeView{T}"/>
    public interface IHighlightColor
    {
        /// <summary>
        /// The color to highlight this object with in list representations. <see cref="Color.Empty"/> for no highlighting.
        /// </summary>
        Color HighlightColor { get; }
    }
}
