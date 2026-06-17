// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks;

/// <summary>
/// Uses simple WinForms dialog boxes to inform the user about the progress of tasks.
/// </summary>
/// <param name="owner">The parent control for any dialogs created by the handler; <c>null</c> to show them parentless (e.g. for an initially headless tool, in which case it must be used from an STA thread).</param>
/// <remarks>This class is thread-safe.</remarks>
[MustDisposeResource]
public class DialogTaskHandler(Control? owner = null) : GuiTaskHandlerBase
{
    /// <inheritdoc/>
    public override void RunTask(ITask task)
    {
        #region Sanity checks
        if (task == null) throw new ArgumentNullException(nameof(task));
        #endregion

        Log.Info(task.Name);

        Exception? ex = null;
        Invoke(() =>
        {
            using var dialog = new TaskRunDialog(task, CredentialProvider, CancellationTokenSource);
            dialog.ShowDialog(owner);
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
        switch (Invoke(() => Msg.YesNoCancel(owner, question, severity)))
        {
            case DialogResult.Yes:
                Log.Debug("Answer: Yes");
                return true;
            case DialogResult.No:
                Log.Debug("Answer: No");
                return false;
            case DialogResult.Cancel:
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

        Invoke(() => OutputBox.Show(owner, title, message.TrimEnd(Environment.NewLine.ToCharArray())));
    }

    /// <inheritdoc/>
    public override void Output<T>(string title, IEnumerable<T> data)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        Invoke(() => OutputGridBox.Show(owner, title, data));
    }

    public override void Error(Exception exception)
    {
        #region Sanity checks
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        #endregion

        Invoke(() => ErrorBox.Show(owner, exception, LogRtf));
    }

    private void Invoke(Action action)
    {
        if (owner == null) action();
        else owner.Invoke(action);
    }

    private TResult Invoke<TResult>(Func<TResult> action)
        => owner == null
            ? action()
            :
#if NETFRAMEWORK
                (TResult)
#endif
                owner.Invoke(action);
}
