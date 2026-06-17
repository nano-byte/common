// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Displays tree data to the user.
/// </summary>
public static class OutputTreeBox
{
    /// <summary>
    /// Displays an output dialog with tree data.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="INamed"/> object to list.
    /// Special support for types implementing <see cref="IHighlightColor"/> and/or <see cref="IContextMenu"/>.</typeparam>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="title">A title for the data.</param>
    /// <param name="data">The data to display.</param>
    /// <param name="separator">The character used to separate namespaces in the <see cref="INamed.Name"/>s. This controls how the tree structure is generated.</param>
    public static void Show<T>(Control? owner, [Localizable(true)] string title, NamedCollection<T> data, char separator = Named.TreeSeparator)
        where T : INamed
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        using var dialog = new Dialog
        {
            Title = title,
            Padding = 10,
            Width = 400,
            Height = 400,
            Content = new FilteredTreeView<T>
            {
                CheckBoxes = false,
                Nodes = data,
                Separator = separator
            }
        };

        var closeButton = new Button {Text = Resources.OK};
        closeButton.Click += delegate { dialog.Close(); };
        dialog.PositiveButtons.Add(closeButton);
        dialog.DefaultButton = closeButton;
        dialog.AbortButton = closeButton;

        if (owner == null) dialog.ShowModal();
        else dialog.ShowModal(owner);
    }
}
