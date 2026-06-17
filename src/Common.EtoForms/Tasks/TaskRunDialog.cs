// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// A dialog with a progress bar that runs and tracks the progress of an <see cref="ITask"/>.
/// </summary>
internal sealed class TaskRunDialog : Dialog
{
    private readonly ITask _task;
    private readonly ICredentialProvider? _credentialProvider;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Progress<TaskSnapshot> _progress = new();
    private readonly Button _cancelButton;
    private bool _allowClose;

    /// <summary>
    /// An exception thrown by <see cref="ITask.Run"/>, if any.
    /// </summary>
    public Exception? Exception { get; private set; }

    /// <summary>
    /// Creates a new task tracking dialog.
    /// </summary>
    /// <param name="task">The trackable task to execute and display.</param>
    /// <param name="credentialProvider">Object used to retrieve credentials for specific <see cref="Uri"/>s on demand; can be <c>null</c>.</param>
    /// <param name="cancellationTokenSource">Used to signal if the user pressed the Cancel button.</param>
    public TaskRunDialog(ITask task, ICredentialProvider? credentialProvider, CancellationTokenSource cancellationTokenSource)
    {
        _task = task ?? throw new ArgumentNullException(nameof(task));
        _credentialProvider = credentialProvider;
        _cancellationTokenSource = cancellationTokenSource;
        var taskThread = new Thread(RunTask);

        Title = task.Name;
        Padding = 10;
        Width = 400;

        var progressBar = new TaskProgressBar();
        var progressLabel = new TaskLabel();
        Content = new StackLayout
        {
            Orientation = Orientation.Vertical,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Spacing = 8,
            Items = {progressLabel, progressBar}
        };

        _cancelButton = new Button {Text = Resources.Cancel, Enabled = task.CanCancel};
        _cancelButton.Click += delegate { Close(); };
        NegativeButtons.Add(_cancelButton);
        AbortButton = _cancelButton;

        _progress.ProgressChanged += (_, snapshot) =>
        {
            progressBar.Report(snapshot);
            progressLabel.Report(snapshot);

            if (snapshot.State == TaskState.Complete)
                Application.Instance.AsyncInvoke(CloseInternal);
        };

        Shown += delegate { taskThread.Start(); };
        Closing += OnClosing;
    }

    private void CloseInternal()
    {
        _allowClose = true;
        Close();
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions are rethrown on the UI thread.")]
    private void RunTask()
    {
        try
        {
            _task.Run(_cancellationTokenSource.Token, _credentialProvider, _progress);
        }
        catch (Exception ex)
        {
            Exception = ex;
            Application.Instance.AsyncInvoke(CloseInternal);
        }
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        if (_allowClose) return;

        if (_task.CanCancel)
        {
            _cancelButton.Enabled = false; // Don't tempt user to press Cancel more than once
            _cancellationTokenSource.Cancel();
        }
        e.Cancel = true;
    }
}
