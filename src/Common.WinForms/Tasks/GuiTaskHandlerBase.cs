using NanoByte.Common.Native;
using NanoByte.Common.Net;
using NanoByte.Common.Threading;

namespace NanoByte.Common.Tasks;

/// <summary>
/// Common base class for WinForms <see cref="ITaskHandler"/> implementations.
/// </summary>
/// <remarks>This class is thread-safe.</remarks>
public abstract class GuiTaskHandlerBase : TaskHandlerBase
{
    /// <summary>
    /// Aggregated <see cref="Log"/> entries in rich-text form.
    /// </summary>
    protected readonly RtfBuilder LogRtf = new();

    /// <summary>
    /// Aggregates <see cref="Log"/> entries in <see cref="LogRtf"/> for later display.
    /// </summary>
    protected override void DisplayLogEntry(LogSeverity severity, string message)
        => LogRtf.AppendPar(message, GetLogColor(severity));

    /// <summary>
    /// Determines the color to use for a log entry based on the <see cref="LogSeverity"/>.
    /// </summary>
    protected static RtfColor GetLogColor(LogSeverity severity)
        => severity switch
        {
            LogSeverity.Debug => RtfColor.Blue,
            LogSeverity.Info => RtfColor.Green,
            LogSeverity.Warn => RtfColor.Orange,
            LogSeverity.Error => RtfColor.Red,
            _ => RtfColor.Black
        };

    /// <inheritdoc/>
    protected override ICredentialProvider CredentialProvider
        => new NetrcCredentialProvider(
            WindowsUtils.IsWindowsNT
                ? IsInteractive ? new WindowsGuiCredentialProvider() : new WindowsNonInteractiveCredentialProvider()
                : null);

    /// <inheritdoc/>
    protected override bool IsInteractive
        => base.IsInteractive && (WindowsUtils.IsGuiSession || !WindowsUtils.IsWindows);

    /// <inheritdoc/>
    protected override bool AskInteractive(string question, bool defaultAnswer)
    {
        Log.Debug($"Question: {question}");

        // Treat questions that default to "Yes" as less severe than those that default to "No"
        var severity = defaultAnswer ? MsgSeverity.Info : MsgSeverity.Warn;

        switch (ThreadUtils.RunSta(() => Msg.YesNoCancel(null, question, severity)))
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

        ThreadUtils.RunSta(() => OutputBox.Show(null, title, message.TrimEnd(Environment.NewLine.ToCharArray())));
    }

    /// <inheritdoc/>
    public override void Output<T>(string title, IEnumerable<T> data)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        ThreadUtils.RunSta(() => OutputGridBox.Show(null, title, data));
    }

    /// <inheritdoc/>
    public override void Output<T>(string title, NamedCollection<T> data)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        ThreadUtils.RunSta(() => OutputTreeBox.Show(null, title, data));
    }

    public override void Error(Exception exception)
    {
        #region Sanity checks
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        #endregion

        if (Verbosity == Verbosity.Batch)
            Log.Error(exception);
        else
            ThreadUtils.RunSta(() => ErrorBox.Show(null, exception, LogRtf));
    }
}
