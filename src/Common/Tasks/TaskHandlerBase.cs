// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using NanoByte.Common.Collections;
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
        public abstract bool Ask(string question, bool? defaultAnswer = null, string? alternateMessage = null);

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

        /// <inheritdoc />
        public virtual void Output<T>(string title, NamedCollection<T> data)
            where T : INamed
            => Output(title, data.AsEnumerable());

        /// <inheritdoc/>
        public abstract void Error(Exception exception);
    }
}
