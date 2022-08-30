// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// Common base class for <see cref="ITaskHandler"/> implementations.
/// </summary>
/// <remarks>This class is thread-safe.</remarks>
public abstract class TaskHandlerBase : ITaskHandler
{
    /// <summary>
    /// Registers a <see cref="Log.Handler"/>.
    /// </summary>
    protected TaskHandlerBase()
    {
        Log.Handler += LogHandler;
    }

    /// <summary>
    /// Unregisters the <see cref="Log.Handler"/>.
    /// </summary>
    public virtual void Dispose()
    {
        Log.Handler -= LogHandler;
        CancellationTokenSource.Dispose();
    }

    /// <summary>
    /// Called for each <see cref="Log"/> entry. Handles exception messages.
    /// </summary>
    /// <param name="severity">The type/severity of the entry.</param>
    /// <param name="message">The message of the entry.</param>
    /// <param name="exception">An optional exception associated with the entry.</param>
    private void LogHandler(LogSeverity severity, string? message, Exception? exception)
    {
        if ((severity == LogSeverity.Debug && Verbosity < Verbosity.Debug)
         || (severity == LogSeverity.Info && Verbosity < Verbosity.Verbose)) return;

        if (exception == null) message ??= "";
        else if (message == null) message = exception.GetMessageWithInner();
        else message += Environment.NewLine + exception;
        DisplayLogEntry(severity, message);

        if (exception != null && Verbosity == Verbosity.Debug)
            DisplayLogEntry(LogSeverity.Debug, exception.ToString());
    }

    /// <summary>
    /// Hook called when a <see cref="Log"/> entry should be shown to the user.
    /// </summary>
    /// <param name="severity">The type/severity of the entry.</param>
    /// <param name="message">The message of the entry including.</param>
    protected abstract void DisplayLogEntry(LogSeverity severity, string message);

    /// <summary>
    /// Used to signal the <see cref="CancellationToken"/>.
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; init; } = new();

    /// <inheritdoc/>
    public CancellationToken CancellationToken => CancellationTokenSource.Token;

    /// <summary>
    /// Used to ask the user or a keyring for <see cref="NetworkCredential"/>s for specific <see cref="Uri"/>s; can be <c>null</c>.
    /// </summary>
    protected virtual ICredentialProvider? CredentialProvider => null;

    /// <inheritdoc/>
    public Verbosity Verbosity { get; set; }

    /// <summary>
    /// Indicates whether the user can provide input.
    /// </summary>
    protected virtual bool IsInteractive
        => Verbosity != Verbosity.Batch;

    /// <inheritdoc/>
    public virtual void RunTask(ITask task)
    {
        #region Sanity checks
        if (task == null) throw new ArgumentNullException(nameof(task));
        #endregion

        task.Run(CancellationToken, CredentialProvider);
    }

    private readonly object _askInteractiveLock = new();

    /// <inheritdoc/>
    public bool Ask(string question, bool? defaultAnswer = null, string? alternateMessage = null)
    {
        #region Sanity checks
        if (question == null) throw new ArgumentNullException(nameof(question));
        #endregion

        if (IsInteractive || !defaultAnswer.HasValue)
        {
            lock (_askInteractiveLock)
            {
                Log.Debug("Question: " + question);
                try
                {
                    bool answer = AskInteractive(question, defaultAnswer ?? false);
                    Log.Debug(answer ? "Answer: Yes" : "Answer: No");
                    return answer;
                }
                catch (OperationCanceledException)
                {
                    Log.Debug("Answer: Cancel");
                    throw;
                }
            }
        }
        else
        {
            if (string.IsNullOrEmpty(alternateMessage))
                Log.Info("Using default answer " + (defaultAnswer.Value ? "Yes" : "No") + " for question: " + question);
            else
                Log.Warn(alternateMessage);
            return defaultAnswer.Value;
        }
    }

    /// <summary>
    /// Asks the user a Yes/No/Cancel question.
    /// </summary>
    /// <param name="question">The question and comprehensive information to help the user make an informed decision.</param>
    /// <param name="defaultAnswer">The default answer to preselect.</param>
    /// <returns><c>true</c> if the user answered with 'Yes'; <c>false</c> if the user answered with 'No'.</returns>
    /// <exception cref="OperationCanceledException">Throw if the user answered with 'Cancel'.</exception>
    protected abstract bool AskInteractive(string question, bool defaultAnswer);

    /// <inheritdoc/>
    public abstract void Output(string title, string message);

    /// <inheritdoc/>
    public virtual void Output<T>(string title, IEnumerable<T> data)
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        Output(title, StringUtils.Join(Environment.NewLine, data.Select(x => x?.ToString() ?? "")));
    }

    /// <inheritdoc />
    public virtual void Output<T>(string title, NamedCollection<T> data)
        where T : INamed
    {
        #region Sanity checks
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (data == null) throw new ArgumentNullException(nameof(data));
        #endregion

        Output(title, data.AsEnumerable());
    }

    /// <inheritdoc/>
    public abstract void Error(Exception exception);
}
