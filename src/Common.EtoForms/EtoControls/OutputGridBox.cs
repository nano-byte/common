// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Reflection;

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Displays tabular data to the user.
/// </summary>
public static class OutputGridBox
{
    /// <summary>
    /// Displays an output dialog with tabular data.
    /// </summary>
    /// <typeparam name="T">The type of the data elements to display.</typeparam>
    /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
    /// <param name="title">The window title to use.</param>
    /// <param name="data">The data to display.</param>
    public static void Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Control? owner, [Localizable(true)] string title, IEnumerable<T> data)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        var grid = new GridView
        {
            DataStore = data.Cast<object>().ToList(),
            GridLines = GridLines.Both
        };
        AddColumns<T>(grid);

        using var dialog = new Dialog
        {
            Title = title,
            Padding = 10,
            Resizable = true,
            Content = grid
        };
        dialog.AddCloseButton();
        dialog.SetInitialClientSize(BoxLayout.TextSize(columns: 100, lines: 22));

        dialog.ShowBox(owner);
    }

    private static void AddColumns<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(GridView grid)
    {
        // Display simple values (strings, URIs, etc.) as a single column
        if (typeof(T) == typeof(string) || typeof(Uri).IsAssignableFrom(typeof(T)))
        {
            grid.Columns.Add(new GridColumn
            {
                HeaderText = "Value",
                Expand = true,
                DataCell = new TextBoxCell {Binding = Binding.Delegate<T, string>(x => x?.ToString() ?? "")}
            });
            return;
        }

        // Otherwise reflect public properties into separate columns
        foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.GetIndexParameters().Length != 0) continue;

            var prop = property;
            grid.Columns.Add(new GridColumn
            {
                HeaderText = prop.Name,
                DataCell = new TextBoxCell {Binding = Binding.Delegate<T, string>(x => prop.GetValue(x)?.ToString() ?? "")}
            });
        }
    }
}
