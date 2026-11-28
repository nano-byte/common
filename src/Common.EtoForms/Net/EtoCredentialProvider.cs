// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Net;

/// <summary>
/// Asks the user for <see cref="NetworkCredential"/>s using an Eto.Forms dialog.
/// </summary>
/// <param name="owner">The parent window the dialog is modal to; <c>null</c> to show it parentless.</param>
/// <param name="offerRemember">Show a checkbox that lets the user ask to remember the credentials. If set, returns <see cref="PromptedCredential"/>s.</param>
/// <remarks>Can be called from any thread. Marshals onto the Eto.Forms UI thread.</remarks>
public class EtoCredentialProvider(Control? owner = null, bool offerRemember = false) : ICredentialProvider
{
#if NET9_0_OR_GREATER
    private static readonly Lock _lock = new();
#else
    private static readonly object _lock = new();
#endif

    /// <inheritdoc/>
    /// <exception cref="OperationCanceledException">The user canceled the dialog.</exception>
    public NetworkCredential GetCredential(Uri uri, bool previousIncorrect = false)
    {
        #region Sanity checks
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        #endregion

        string target = uri.GetRoot().ToStringRfc();
        Log.Debug($"Prompt for credentials for {target} in GUI");

        if (previousIncorrect)
            Log.Error(string.Format(Resources.InvalidCredentials, target));

        // Do not lock on the UI thread, since a background thread holding the lock may be waiting for the UI thread in Invoke()
        if (Application.Instance.IsUIThread)
            return Show(owner, target, previousIncorrect, offerRemember) ?? throw new OperationCanceledException();

        lock (_lock)
        {
            return Application.Instance.Invoke(() => Show(owner, target, previousIncorrect, offerRemember))
                ?? throw new OperationCanceledException();
        }
    }

    /// <summary>
    /// Displays a dialog asking the user for a user name and password.
    /// </summary>
    /// <returns>The credentials the user entered if they pressed OK; otherwise <c>null</c>.</returns>
    private static NetworkCredential? Show(Control? owner, string target, bool previousIncorrect, bool offerRemember)
    {
        var userNameBox = new TextBox();
        var passwordBox = new PasswordBox();
        var rememberCheckBox = new CheckBox {Text = Resources.RememberCredentials};

        var layout = new StackLayout
        {
            Orientation = Orientation.Vertical,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Spacing = 8,
            Items = {new Label {Text = string.Format(Resources.PleaseEnterCredentials, target), Wrap = WrapMode.Word}}
        };
        if (previousIncorrect)
            layout.Items.Add(new Label {Text = string.Format(Resources.InvalidCredentials, target), TextColor = Colors.Red, Wrap = WrapMode.Word});
        layout.Items.Add(new TableLayout
        {
            Spacing = new(8, 8),
            Rows =
            {
                new TableRow(new Label {Text = Resources.UserName, VerticalAlignment = VerticalAlignment.Center}, new TableCell(userNameBox, scaleWidth: true)),
                new TableRow(new Label {Text = Resources.Password, VerticalAlignment = VerticalAlignment.Center}, new TableCell(passwordBox, scaleWidth: true))
            }
        });
        if (offerRemember)
            layout.Items.Add(rememberCheckBox);

        using var dialog = new Dialog<NetworkCredential?> {Title = target, Padding = 10, Content = layout};

        var okButton = new Button {Text = Resources.OK};
        okButton.Click += delegate
        {
            dialog.Result = offerRemember
                ? new PromptedCredential(userNameBox.Text, passwordBox.Text, remember: rememberCheckBox.Checked == true)
                : new NetworkCredential(userNameBox.Text, passwordBox.Text);
            dialog.Close();
        };
        var cancelButton = new Button {Text = Resources.Cancel};
        cancelButton.Click += delegate
        {
            dialog.Result = null;
            dialog.Close();
        };

        dialog.PositiveButtons.Add(okButton);
        dialog.NegativeButtons.Add(cancelButton);
        dialog.DefaultButton = okButton;
        dialog.AbortButton = cancelButton;
        dialog.Shown += delegate { userNameBox.Focus(); };
        dialog.FitContentHeight(400);
        dialog.CenterOnScreenIfOwnerless();
        dialog.PreventPinning();

        return owner == null ? dialog.ShowModal() : dialog.ShowModal(owner);
    }
}
