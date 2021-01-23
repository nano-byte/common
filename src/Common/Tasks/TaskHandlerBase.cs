// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Common base class for <see cref="ITaskHandler"/> implementations.
    /// </summary>
    public abstract class TaskHandlerBase : MarshalNoTimeout, ITaskHandler
    {
        /// <summary>
        /// Starts handling log events.
        /// </summary>
        protected TaskHandlerBase()
        {
            Log.Handler += LogHandler;
        }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            Log.Handler -= LogHandler;
            CancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Reports <see cref="Log"/> messages to the user based on their <see cref="LogSeverity"/> and the current <see cref="Verbosity"/> level.
        /// </summary>
        /// <param name="severity">The type/severity of the entry.</param>
        /// <param name="message">The message text of the entry.</param>
        protected abstract void LogHandler(LogSeverity severity, string message);

        /// <summary>
        /// Used to signal the <see cref="CancellationToken"/>.
        /// </summary>
        protected readonly CancellationTokenSource CancellationTokenSource = new();

        /// <inheritdoc/>
        public CancellationToken CancellationToken => CancellationTokenSource.Token;

        /// <inheritdoc/>
        public abstract ICredentialProvider? CredentialProvider { get; }

        /// <inheritdoc/>
        public Verbosity Verbosity { get; set; }

        /// <inheritdoc/>
        public virtual void RunTask(ITask task)
        {
            #region Sanity checks
            if (task == null) throw new ArgumentNullException(nameof(task));
            #endregion

            task.Run(CancellationToken, CredentialProvider);
        }

        /// <inheritdoc/>
        public bool Ask(string question)
            => Ask(question ?? throw new ArgumentNullException(nameof(question)), MsgSeverity.Warn);

        /// <inheritdoc/>
        public bool Ask(string question, bool defaultAnswer, string? alternateMessage = null)
        {
            #region Sanity checks
            if (question == null) throw new ArgumentNullException(nameof(question));
            #endregion

            if (Verbosity <= Verbosity.Batch)
            {
                if (!string.IsNullOrEmpty(alternateMessage)) Log.Warn(alternateMessage);
                return defaultAnswer;
            }
            return Ask(question,
                // Treat messages that default to "Yes" as less severe than those that default to "No"
                defaultAnswer ? MsgSeverity.Info : MsgSeverity.Warn);
        }

        /// <summary>
        /// Asks the user a Yes/No/Cancel question.
        /// </summary>
        /// <param name="question">The question and comprehensive information to help the user make an informed decision.</param>
        /// <param name="severity">The severity/possible impact of the question.</param>
        /// <returns><c>true</c> if the user answered with 'Yes'; <c>false</c> if the user answered with 'No'.</returns>
        /// <exception cref="OperationCanceledException">The user selected 'Cancel'.</exception>
        protected abstract bool Ask(string question, MsgSeverity severity);

        /// <inheritdoc/>
        public abstract void Output(string title, string message);

        /// <inheritdoc/>
        public virtual void Output<T>(string title, IEnumerable<T> data)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (data == null) throw new ArgumentNullException(nameof(data));
            #endregion

            string message = StringUtils.Join(Environment.NewLine, data.Select(x => x?.ToString() ?? ""));
            Output(title, message);
        }

        /// <inheritdoc/>
        public abstract void Error(Exception exception);
    }
}
