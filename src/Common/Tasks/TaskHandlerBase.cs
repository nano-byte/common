using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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

        /// <summary>
        /// Reports <see cref="Log"/> messages to the user based on their <see cref="LogSeverity"/> and the current <see cref="Verbosity"/> level.
        /// </summary>
        /// <param name="severity">The type/severity of the entry.</param>
        /// <param name="message">The message text of the entry.</param>
        protected abstract void LogHandler(LogSeverity severity, string message);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Stops handling log events.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            Log.Handler -= LogHandler;
            if (disposing) CancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Used to signal the <see cref="CancellationToken"/>.
        /// </summary>
        [NotNull]
        protected readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        /// <inheritdoc/>
        public CancellationToken CancellationToken => CancellationTokenSource.Token;

        private ICredentialProvider _credentialProvider;

        private readonly object _credentialProviderLock = new object();

        /// <inheritdoc/>
        public ICredentialProvider CredentialProvider
        {
            get
            {
                // Double-checked locking
                if (_credentialProvider == null)
                {
                    lock (_credentialProviderLock)
                    {
                        if (_credentialProvider == null)
                        {
                            var provider = BuildCrendentialProvider();
                            if (provider != null) _credentialProvider = new CachedCredentialProvider(provider);
                        }
                    }
                }
                return _credentialProvider;
            }
        }

        /// <summary>
        /// Template method for building an <see cref="ICredentialProvider"/>. Called on first use of <see cref="CredentialProvider"/>.
        /// </summary>
        [CanBeNull]
        protected abstract ICredentialProvider BuildCrendentialProvider();

        /// <inheritdoc/>
        public virtual Verbosity Verbosity { get; set; }

        /// <inheritdoc/>
        public abstract void RunTask(ITask task);

        /// <inheritdoc/>
        public bool Ask(string question)
        {
            #region Sanity checks
            if (question == null) throw new ArgumentNullException(nameof(question));
            #endregion

            return Ask(question, MsgSeverity.Warn);
        }

        /// <inheritdoc/>
        public bool Ask(string question, bool defaultAnswer, string alternateMessage = null)
        {
            #region Sanity checks
            if (question == null) throw new ArgumentNullException(nameof(question));
            #endregion

            if (Verbosity <= Verbosity.Batch)
            {
                if (!string.IsNullOrEmpty(alternateMessage)) Log.Warn(alternateMessage);
                return defaultAnswer;
            }
            else
            {
                return Ask(question,
                    // Treat messages that default to "Yes" as less severe than those that default to "No"
                    defaultAnswer ? MsgSeverity.Info : MsgSeverity.Warn);
            }
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
        public virtual void OutputLow(string title, string message)
        {
            if (Verbosity > Verbosity.Batch) Output(title, message);
        }

        /// <inheritdoc/>
        public virtual void Output<T>(string title, IEnumerable<T> data)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (data == null) throw new ArgumentNullException(nameof(data));
            #endregion

            string message = StringUtils.Join(Environment.NewLine, data.Select(x => x.ToString()));
            Output(title, message);
        }

        /// <inheritdoc/>
        public virtual void OutputLow<T>(string title, IEnumerable<T> data)
        {
            if (Verbosity > Verbosity.Batch) Output(title, data);
        }

        /// <inheritdoc/>
        public abstract void Error(Exception exception);
    }
}
