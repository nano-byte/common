// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls;

/// <summary>
/// A custom drop-down that can contain arbitrary controls instead of just a menu.
/// </summary>
public class DropDownContainer : UserControl
{
    public DropDownContainer()
    {
        BorderStyle = BorderStyle.FixedSingle;
    }

    /// <summary>
    /// Shows the drop-down below the specified <paramref name="control"/>.
    /// </summary>
    public void Show(Control control)
        => new Helper(this).Show(control, new(0, control.Height));

    /// <summary>
    /// Closes the drop down.
    /// </summary>
    public void Close()
        => Parent?.Dispose();

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if (keyData.HasFlag(Keys.Enter) && ActiveControl is Button button)
        {
            button.PerformClick();
            return true;
        }

        return base.ProcessDialogKey(keyData);
    }

    private sealed class Helper : ToolStripDropDown
    {
        public Helper(DropDownContainer dropDown)
        {
            Padding = Padding.Empty;
            Items.Add(new ToolStripControlHost(dropDown) {AutoSize = false, Margin = Padding.Empty});
            Opened += delegate { dropDown.Focus(); };
        }

        // Prevent ALT button from closing popup to allow ALT+Mnemonic to work
        protected override bool ProcessDialogKey(Keys keyData)
            => !keyData.HasFlag(Keys.Alt) && base.ProcessDialogKey(keyData);
    }
}
