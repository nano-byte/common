// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// A dialog with a progress bar that runs and tracks the progress of an <see cref="ITask"/>.
/// </summary>
internal sealed partial class TaskRunDialog : Form
{
    private readonly ITask _task;
    private readonly Thread _taskThread;
    private readonly ICredentialProvider? _credentialProvider;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Progress<TaskSnapshot> _progress = new();

    /// <summary>
    /// Creates a new task tracking dialog.
    /// </summary>
    /// <param name="task">The trackable task to execute and display.</param>
    /// <param name="credentialProvider">Object used to retrieve credentials for specific <see cref="Uri"/>s on demand; can be <c>null</c>.</param>
    /// <param name="cancellationTokenSource">Used to signal if the user pressed the Cancel button.</param>
    public TaskRunDialog(ITask task, ICredentialProvider? credentialProvider, CancellationTokenSource cancellationTokenSource)
    {
        #region Sanity checks
        if (task == null) throw new ArgumentNullException(nameof(task));
        #endregion

        InitializeComponent();

        buttonCancel.Text = Resources.Cancel;
        buttonCancel.Enabled = task.CanCancel;
        Text = task.Name;

        _task = task;
        _taskThread = new Thread(RunTask);
        _cancellationTokenSource = cancellationTokenSource;
        _credentialProvider = credentialProvider;
    }

    private void TaskRunDialog_Shown(object? sender, EventArgs e)
    {
        _progress.ProgressChanged += OnProgressChanged;
        _taskThread.Start();
    }

    private void OnProgressChanged(object? sender, TaskSnapshot snapshot)
    {
        progressBarTask.Report(snapshot);
        labelTask.Report(snapshot);

        if (snapshot.State == TaskState.Complete)
        {
            _allowClose = true;
            Close();
        }
    }

    /// <summary>
    /// An exception thrown by <see cref="ITask.Run"/>, if any.
    /// </summary>
    public Exception? Exception { get; private set; }

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
            _allowClose = true;
            this.BeginInvoke(() =>
            {
                _allowClose = true;
                Close();
            });
        }
    }

    private bool _allowClose;

    private void TaskRunDialog_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!_allowClose)
        {
            if (_task.CanCancel)
            {
                buttonCancel.Enabled = false; // Don't tempt user to press Cancel more than once
                _cancellationTokenSource.Cancel();
            }
            e.Cancel = true;
        }
    }
}