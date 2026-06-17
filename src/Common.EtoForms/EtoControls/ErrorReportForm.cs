// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Diagnostics;

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Presents the user with a friendly interface in case of an error, offering to report it to the developers.
/// </summary>
public sealed class ErrorReportForm : Dialog
{
    private readonly Exception _exception;
    private readonly Uri _uploadUri;
    private readonly TextArea _commentBox;
    private readonly Button _reportButton;
    private readonly Button _cancelButton;

    /// <summary>
    /// Prepares reporting an error.
    /// </summary>
    /// <param name="exception">The exception object describing the error.</param>
    /// <param name="uploadUri">The URI to upload error reports to.</param>
    private ErrorReportForm(Exception exception, Uri uploadUri)
    {
        _exception = exception ?? throw new ArgumentNullException(nameof(exception));
        _uploadUri = uploadUri ?? throw new ArgumentNullException(nameof(uploadUri));

        Title = Resources.ErrorReport;
        Padding = 10;
        Width = 560;
        Height = 480;
        Resizable = true;

        _commentBox = new TextArea();
        _reportButton = new Button {Text = Resources.ErrorReportSend};
        _cancelButton = new Button {Text = Resources.ErrorReportCancel};

        Content = new StackLayout
        {
            Orientation = Orientation.Vertical,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Spacing = 8,
            Items =
            {
                new Label {Text = Resources.ErrorReportInfo, Wrap = WrapMode.Word},
                new Label {Text = Resources.TechnicalDetails, Font = SystemFonts.Bold()},
                new StackLayoutItem(new TextArea {ReadOnly = true, Wrap = false, Text = exception.ToString()}, expand: true),
                new Label {Text = Resources.Comment, Font = SystemFonts.Bold()},
                new Label {Text = Resources.ErrorReportComment, Wrap = WrapMode.Word},
                new StackLayoutItem(_commentBox, expand: true)
            }
        };

        _reportButton.Click += OnReport;
        _cancelButton.Click += delegate { Close(); };
        PositiveButtons.Add(_reportButton);
        NegativeButtons.Add(_cancelButton);
        DefaultButton = _reportButton;
        AbortButton = _cancelButton;
    }

    private static readonly object _monitoringLock = new();

    /// <summary>
    /// Sets up hooks that catch and report any unhandled exceptions. Calling this more than once has no effect.
    /// </summary>
    /// <param name="uploadUri">The URI to upload error reports to.</param>
    /// <remarks>If an exception is caught any remaining threads will continue to execute until the error has been reported. Then the entire process will be terminated.</remarks>
    [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "If the actual exception is unknown the generic top-level Exception is the most appropriate")]
    [Conditional("ERROR_REPORT")]
    public static void SetupMonitoring(Uri uploadUri)
    {
        #region Sanity checks
        if (uploadUri == null) throw new ArgumentNullException(nameof(uploadUri));
        #endregion

        // Only execute this code once per process
        if (!Monitor.TryEnter(_monitoringLock)) return;

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            var exception = e.ExceptionObject as Exception ?? new Exception("Unknown error");
            if (ErrorReport.ShouldReport(exception)) Report(exception, uploadUri);
            else ErrorBox.Show(null, exception);
            Process.GetCurrentProcess().Kill();
        };
    }

    /// <summary>
    /// Displays the error reporting form.
    /// </summary>
    /// <param name="ex">The exception to report.</param>
    /// <param name="uploadUri">The URI to upload error reports to.</param>
    /// <remarks>Marshals onto the Eto.Forms UI thread. Creates a new <see cref="Application"/> if none is running.</remarks>
    public static void Report(Exception ex, Uri uploadUri)
    {
        if (Application.Instance is {} application)
            application.Invoke(() => new ErrorReportForm(ex, uploadUri).ShowModal());
        else
            new Application().Run(new ErrorReportForm(ex, uploadUri));
    }

    private async void OnReport(object? sender, EventArgs e)
    {
        _commentBox.Enabled = _reportButton.Enabled = _cancelButton.Enabled = false;

        try
        {
            var report = ErrorReport.Generate(_exception, _commentBox.Text);
            string message = await report.SendAsync(_uploadUri);
            Msg.Inform(this, string.IsNullOrWhiteSpace(message) ? Resources.ErrorReportSent : message, MsgSeverity.Info);
            Close();
        }
        #region Error handling
        catch (Exception ex)
        {
            Msg.Inform(this, ex.Message, MsgSeverity.Error);
            _commentBox.Enabled = _reportButton.Enabled = _cancelButton.Enabled = true;
        }
        #endregion
    }
}
