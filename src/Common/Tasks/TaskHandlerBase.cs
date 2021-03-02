// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using NanoByte.Common.Collections;
using NanoByte.Common.Native;
using NanoByte.Common.Net;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Common base class for <see cref="ITaskHandler"/> implementations.
    /// </summary>
    public abstract class TaskHandlerBase : MarshalNoTimeout, ITaskHandler
    {
        /// <inheritdoc/>
        public virtual void Dispose()
        {
            CancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Used to signal the <see cref="CancellationToken"/>.
        /// </summary>
        protected CancellationTokenSource CancellationTokenSource { get; init; } = new();

        /// <inheritdoc/>
        public CancellationToken CancellationToken => CancellationTokenSource.Token;

        /// <inheritdoc/>
        public virtual ICredentialProvider? CredentialProvider => null;

        /// <inheritdoc/>
        public Verbosity Verbosity { get; set; }

        /// <summary>
        /// Indicates whether the user can provide input.
        /// </summary>
        protected bool IsInteractive
            => OSUtils.IsInteractive && Verbosity != Verbosity.Batch;

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
}
