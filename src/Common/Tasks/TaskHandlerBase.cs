using System;
using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;

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
        public CancellationToken CancellationToken { get { return CancellationTokenSource.Token; } }

        /// <inheritdoc/>
        public virtual Verbosity Verbosity { get; set; }

        /// <inheritdoc/>
        public abstract void RunTask(ITask task);

        /// <inheritdoc/>
        public abstract bool Ask(string question);

        /// <inheritdoc/>
        public abstract void Output(string title, string message);

        /// <inheritdoc/>
        public abstract void Output<T>(string title, IEnumerable<T> data);

        /// <inheritdoc/>
        public abstract ICredentialProvider BuildCredentialProvider();
    }
}
