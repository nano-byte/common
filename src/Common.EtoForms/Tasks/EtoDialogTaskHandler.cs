// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks;

/// <summary>
/// Uses simple Eto.Forms dialog boxes to inform the user about the progress of tasks.
/// </summary>
/// <param name="owner">The parent control for any dialogs created by the handler; <c>null</c> to show them parentless (e.g. for an initially headless tool).</param>
/// <remarks>This class is thread-safe.</remarks>
[MustDisposeResource]
public class EtoDialogTaskHandler(Control? owner = null) : EtoTaskHandlerBase
{
    /// <inheritdoc/>
    public override void RunTask(ITask task)
    {
        #region Sanity checks
        if (task == null) throw new ArgumentNullException(nameof(task));
        #endregion

        Log.Info(task.Name);

        Exception? ex = null;
        Application.Instance.Invoke(() =>
        {
            using var dialog = new TaskRunDialog(task, CredentialProvider, CancellationTokenSource);
            if (owner == null) dialog.ShowModal();
            else dialog.ShowModal(owner);
            ex = dialog.Exception;
        });
        ex?.Rethrow();
    }

    /// <inheritdoc/>
    protected override bool AskInteractive(string question, bool defaultAnswer)
    {
        #region Sanity checks
        if (question == null) throw new ArgumentNullException(nameof(question));
        #endregion

        // Treat questions that default to "Yes" as less severe than those that default to "No"
        var severity = defaultAnswer ? MsgSeverity.Info : MsgSeverity.Warn;

        Log.Debug($"Question: {question}");
        switch (Application.Instance.Invoke(() => Msg.YesNoCancel(owner, question, severity)))
        {
            case DialogResult.Yes:
                Log.Debug("Answer: Yes");
                return true;
            case DialogResult.No:
                Log.Debug("Answer: No");
                return false;
            default:
                Log.Debug("Answer: Cancel");
                throw new OperationCanceledException();
        }
    }

    /// <inheritdoc/>
    public override void Output(string title, string message)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (message == null) throw new ArgumentNullException(nameof(message));
        #endregion

        Application.Instance.Invoke(() => OutputBox.Show(owner, title, message.TrimEnd(Environment.NewLine.ToCharArray())));
    }

    /// <inheritdoc/>
    public override void Output<T>(string title, IEnumerable<T> data)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        Application.Instance.Invoke(() => OutputGridBox.Show(owner, title, data));
    }

    /// <inheritdoc/>
    public override void Output<T>(string title, NamedCollection<T> data)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        Application.Instance.Invoke(() => OutputGridBox.Show(owner, title, data));
    }

    /// <inheritdoc/>
    public override void Error(Exception exception)
    {
        #region Sanity checks
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        #endregion

        Application.Instance.Invoke(() => ErrorBox.Show(owner, exception, LogRichText));
    }
}
