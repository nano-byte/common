// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls;

/// <summary>
/// Displays tree data to the user.
/// </summary>
/// <typeparam name="T">The type of <see cref="INamed"/> object to list.
/// Special support for types implementing <see cref="IHighlightColor"/> and/or <see cref="IContextMenu"/>.</typeparam>
public sealed class OutputTreeBox<T> : Form
    where T : INamed
{
    internal OutputTreeBox(NamedCollection<T> data, char separator)
    {
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(600, 450);
        Controls.Add(new FilteredTreeView<T>
        {
            Dock = DockStyle.Fill,
            CheckBoxes = false,
            Nodes = data,
            Separator = separator
        });
        ShowIcon = false;
    }
}

/// <summary>
/// Factory methods for <see cref="OutputTreeBox{T}"/>.
/// </summary>
public static class OutputTreeBox
{
    /// <summary>
    /// Displays an output dialog with tree data.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="INamed"/> object to list.
    /// Special support for types implementing <see cref="IHighlightColor"/> and/or <see cref="IContextMenu"/>.</typeparam>
    /// <param name="owner">The parent window for the dialogs; can be <c>null</c>.</param>
    /// <param name="title">A title for the data.</param>
    /// <param name="data">The data to display.</param>
    /// <param name="separator">The character used to separate namespaces in the <see cref="INamed.Name"/>s. This controls how the tree structure is generated.</param>
    public static void Show<T>(IWin32Window? owner, [Localizable(true)] string title, NamedCollection<T> data, char separator = Named.TreeSeparator)
        where T : INamed
    {
        using var dialog = new OutputTreeBox<T>(data ?? throw new ArgumentNullException(nameof(data)), separator)
        {
            Text = title ?? throw new ArgumentNullException(nameof(title))
        };
        dialog.ShowDialog(owner);
    }
}