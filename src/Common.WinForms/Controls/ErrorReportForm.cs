// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Diagnostics;
using NanoByte.Common.Info;
using NanoByte.Common.Native;
using NanoByte.Common.Storage;

#if NET20 || NET40
using System.Net;
#else
using System.Net.Http;
#endif

namespace NanoByte.Common.Controls;

/// <summary>
/// Presents the user with a friendly interface in case of an error, offering to report it to the developers.
/// </summary>
/// <remarks>This class should only be used by <see cref="System.Windows.Forms"/> applications.</remarks>
public sealed partial class ErrorReportForm : Form
{
    #region Variables
    private readonly Exception _exception;
    private readonly Uri _uploadUri;
    #endregion

    #region Constructor
    /// <summary>
    /// Prepares reporting an error.
    /// </summary>
    /// <param name="exception">The exception object describing the error.</param>
    /// <param name="uploadUri">The URI to upload error reports to.</param>
    private ErrorReportForm(Exception exception, Uri uploadUri)
    {
        #region Sanity checks
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        if (uploadUri == null) throw new ArgumentNullException(nameof(uploadUri));
        #endregion

        InitializeComponent();
        Font = DefaultFonts.Modern;
        Text = Resources.ErrorReport;
        infoLabel.Text = Resources.ErrorReportInfo;
        detailsLabel.Text = Resources.TechnicalDetails;
        commentLabel.Text = Resources.Comment;
        commentBox.HintText = Resources.ErrorReportComment;
        buttonReport.Text = Resources.ErrorReportSend;
        buttonCancel.Text = Resources.ErrorReportCancel;

        HandleCreated += delegate { WindowsTaskbar.PreventPinning(Handle); };
        Shown += delegate { this.SetForegroundWindow(); };

        _exception = exception;

        // A missing file as the root is more important than the secondary exceptions it causes
        if (exception.InnerException is FileNotFoundException)
            exception = exception.InnerException;

        // Make the message simpler for missing files
        detailsBox.Text = exception is FileNotFoundException ? exception.Message.Replace("\n", Environment.NewLine) : exception.ToString();

        // Append inner exceptions
        if (exception.InnerException != null)
            detailsBox.Text += Environment.NewLine + Environment.NewLine + exception.InnerException;

        _uploadUri = uploadUri;
    }
    #endregion

    #region Static access
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

        // Disable WinForm's built-in error handling
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException, threadScope: false);

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            Report(e.ExceptionObject as Exception ?? new Exception("Unknown error"), uploadUri);
            Process.GetCurrentProcess().Kill();
        };
    }

    /// <summary>
    /// Displays the error reporting form.
    /// </summary>
    /// <param name="ex">The exception to report.</param>
    /// <param name="uploadUri">The URI to upload error reports to.</param>
    /// <remarks>Modal to all windows on the current thread. Creates a new message loop if none exists.</remarks>
    public static void Report(Exception ex, Uri uploadUri)
    {
        var form = new ErrorReportForm(ex, uploadUri);
        if (Application.MessageLoop) form.ShowDialog();
        else Application.Run(form);
    }
    #endregion

    //--------------------//

    #region Buttons
#if NET20 || NET40
    private void buttonReport_Click(object? sender, EventArgs e)
    {
        Cursor = Cursors.WaitCursor;
        commentBox.Enabled = detailsBox.Enabled = buttonReport.Enabled = buttonCancel.Enabled = false;

        // Create WebClient for upload and register as component for automatic disposal
        var webClient = new WebClient();
        components ??= new Container();
        components.Add(webClient);

        webClient.UploadFileCompleted += (_, uploadEventArgs) =>
        {
            Cursor = Cursors.Default;
            if (uploadEventArgs.Error == null)
            {
                string message = EncodingUtils.Utf8.GetString(uploadEventArgs.Result);
                Msg.Inform(this, string.IsNullOrEmpty(message) ? Resources.ErrorReportSent : message, MsgSeverity.Info);
                Close();
            }
            else
            {
                Msg.Inform(this, uploadEventArgs.Error.Message, MsgSeverity.Error);
                commentBox.Enabled = detailsBox.Enabled = buttonReport.Enabled = buttonCancel.Enabled = true;
            }
        };

        using var tempFile = new TemporaryFile("error-report");
        File.WriteAllText(tempFile, GenerateReport());
        webClient.UploadFileAsync(_uploadUri, tempFile);
    }
#else
    private async void buttonReport_Click(object? sender, EventArgs e)
    {
        Cursor = Cursors.WaitCursor;
        commentBox.Enabled = detailsBox.Enabled = buttonReport.Enabled = buttonCancel.Enabled = false;

        try
        {
            using var httpClient = new HttpClient();
            using var content = new MultipartFormDataContent {{new StringContent(GenerateReport()), "file", "error-report.xml"}};
            using var response = await httpClient.PostAsync(_uploadUri, content);
            response.EnsureSuccessStatusCode();

            string message = await response.Content.ReadAsStringAsync();
            Msg.Inform(this, string.IsNullOrWhiteSpace(message) ? Resources.ErrorReportSent : message, MsgSeverity.Info);
            Close();
        }
        catch (Exception ex)
        {
            Msg.Inform(this, ex.Message, MsgSeverity.Error);
            commentBox.Enabled = detailsBox.Enabled = buttonReport.Enabled = buttonCancel.Enabled = true;
            Cursor = Cursors.Default;
        }
    }
#endif

    private void buttonCancel_Click(object? sender, EventArgs e) => Close();
    #endregion

    #region Generate report
    /// <summary>
    /// Generates a ZIP archive containing crash information.
    /// </summary>
    /// <returns></returns>
    private string GenerateReport() => new ErrorReport
    {
        Application = AppInfo.Current,
        OS = OSInfo.Current,
        Exception = new ExceptionInfo(_exception),
        Log = Log.Content,
        Comments = commentBox.Text
    }.ToXmlString();
    #endregion
}
