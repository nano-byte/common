// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.EtoControls;

/// <summary>
/// An object that can provide its own context menu.
/// </summary>
/// <seealso cref="FilteredTreeView{T}"/>
public interface IContextMenu
{
    /// <summary>
    /// Returns the context menu for this object; can be <c>null</c>.
    /// </summary>
    ContextMenu? GetContextMenu();
}
