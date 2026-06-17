// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Native;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// Common base class for Eto.Forms <see cref="ITaskHandler"/> implementations.
/// </summary>
/// <remarks>This class is thread-safe.</remarks>
[MustDisposeResource]
public abstract class EtoTaskHandlerBase : TaskHandlerBase
{
    /// <summary>
    /// Aggregated <see cref="Log"/> entries for later display.
    /// </summary>
    protected readonly FormattedTextBuilder LogRichText = new();

    /// <summary>
    /// Aggregates <see cref="Log"/> entries in <see cref="LogRichText"/> for later display.
    /// </summary>
    protected override void DisplayLogEntry(LogSeverity severity, string message)
        => LogRichText.AppendLine(severity, message);

    /// <inheritdoc/>
    protected override ICredentialProvider CredentialProvider
        => new NetrcCredentialProvider(
            WindowsUtils.IsWindowsNT
                ? IsInteractive ? new WindowsGuiCredentialProvider() : new WindowsNonInteractiveCredentialProvider()
                : null);

    /// <inheritdoc/>
    protected override bool IsInteractive
        => base.IsInteractive && (!WindowsUtils.IsWindows || WindowsUtils.IsGuiSession);

    /// <inheritdoc/>
    protected override bool AskInteractive(string question, bool defaultAnswer)
    {
        Log.Debug($"Question: {question}");

        // Treat questions that default to "Yes" as less severe than those that default to "No"
        var severity = defaultAnswer ? MsgSeverity.Info : MsgSeverity.Warn;

        switch (Application.Instance.Invoke(() => Msg.YesNoCancel(null, question, severity)))
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

        Application.Instance.Invoke(() => OutputBox.Show(null, title, message.TrimEnd(Environment.NewLine.ToCharArray())));
    }

    /// <inheritdoc/>
    public override void Output<T>(string title, IEnumerable<T> data)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        Application.Instance.Invoke(() => OutputGridBox.Show(null, title, data));
    }

    /// <inheritdoc/>
    public override void Output<T>(string title, NamedCollection<T> data)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        Application.Instance.Invoke(() => OutputGridBox.Show(null, title, data));
    }

    /// <inheritdoc/>
    public override void Error(Exception exception)
    {
        #region Sanity checks
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        #endregion

        if (Verbosity == Verbosity.Batch)
            Log.Error(exception);
        else
            Application.Instance.Invoke(() => ErrorBox.Show(null, exception, LogRichText));
    }
}
