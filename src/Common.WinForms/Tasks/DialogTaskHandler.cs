// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks;

/// <summary>
/// Uses simple WinForms dialog boxes to inform the user about the progress of tasks.
/// </summary>
/// <remarks>This class is thread-safe.</remarks>
public class DialogTaskHandler : GuiTaskHandlerBase
{
    private readonly Control _owner;

    /// <summary>
    /// Creates a new dialog task handler.
    /// Registers a <see cref="Log.Handler"/>.
    /// </summary>
    /// <param name="owner">The parent window for any dialogs created by the handler.</param>
    public DialogTaskHandler(Control owner)
    {
        _owner = owner;
    }

    /// <inheritdoc/>
    public override void RunTask(ITask task)
    {
        #region Sanity checks
        if (task == null) throw new ArgumentNullException(nameof(task));
        #endregion

        Log.Info(task.Name);

        Exception? ex = null;
        _owner.Invoke(() =>
        {
            using var dialog = new TaskRunDialog(task, CredentialProvider, CancellationTokenSource);
            dialog.ShowDialog(_owner);
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
        switch (_owner.Invoke(() => Msg.YesNoCancel(_owner, question, severity)))
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

        _owner.Invoke(() => OutputBox.Show(_owner, title, message.TrimEnd(Environment.NewLine.ToCharArray())));
    }

    /// <inheritdoc/>
    public override void Output<T>(string title, IEnumerable<T> data)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        _owner.Invoke(() => OutputGridBox.Show(_owner, title, data));
    }

    public override void Error(Exception exception)
    {
        #region Sanity checks
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        #endregion

        _owner.Invoke(() => ErrorBox.Show(_owner, exception, LogRtf));
    }
}
