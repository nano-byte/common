// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// An object that can provide its own context menu.
    /// </summary>
    /// <seealso cref="Controls.FilteredTreeView{T}"/>
    public interface IContextMenu
    {
        /// <summary>
        /// Returns the context menu for this object; can be <c>null</c>.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Creates a new object on each call")]
        ContextMenuStrip? GetContextMenu();
    }
}
