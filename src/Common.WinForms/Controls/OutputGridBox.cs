// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Native;

namespace NanoByte.Common.Controls;

/// <summary>
/// Displays tabular data to the user.
/// </summary>
public sealed partial class OutputGridBox : Form
{
    private OutputGridBox()
    {
        InitializeComponent();
        Font = DefaultFonts.Modern;
        HandleCreated += delegate { WindowsTaskbar.PreventPinning(Handle); };
    }

    private void Form_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape) Close();
    }

    /// <summary>
    /// Displays an output dialog with tabular data.
    /// </summary>
    /// <typeparam name="T">The type of the data elements to display.</typeparam>
    /// <param name="owner">The parent window for the dialogs; can be <c>null</c>.</param>
    /// <param name="title">A title for the data.</param>
    /// <param name="data">The data to display.</param>
    public static void Show<T>(IWin32Window? owner, [Localizable(true)] string title, IEnumerable<T> data)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        using var dialog = new OutputGridBox
        {
            Text = title,
            ShowInTaskbar = owner == null
        };
        dialog.SetData(data);

        dialog.ShowDialog(owner);
    }

    /// <summary>
    /// Simple wrapper around objects to make them appear as a single column in a <see cref="DataGridView"/>.
    /// </summary>
    private class SimpleEntry<T>
    {
        [UsedImplicitly]
        public T Value { get; }

        public SimpleEntry(T value)
        {
            Value = value;
        }
    }

    private void SetData<T>(IEnumerable<T> data)
    {
        if (typeof(T) == typeof(string) || typeof(Uri).IsAssignableFrom(typeof(T)))
            dataGrid.DataSource = data.Select(x => new SimpleEntry<T>(x)).ToList();
        else dataGrid.DataSource = data.ToList();
    }
}
