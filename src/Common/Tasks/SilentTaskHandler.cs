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
    /// Executes tasks silently and suppresses any questions.
    /// </summary>
    public class SilentTaskHandler : ITaskHandler
    {
        /// <inheritdoc/>
        public virtual void Dispose() {}

        /// <inheritdoc/>
        public virtual CancellationToken CancellationToken => default;

        /// <inheritdoc/>
        public virtual ICredentialProvider? CredentialProvider => null;

        /// <inheritdoc/>
        public void RunTask(ITask task)
        {
            #region Sanity checks
            if (task == null) throw new ArgumentNullException(nameof(task));
            #endregion

            Log.Debug("Task: " + task.Name);
            task.Run(CancellationToken, CredentialProvider);
        }

        /// <summary>
        /// Always returns <see cref="Tasks.Verbosity.Batch"/>.
        /// </summary>
        public Verbosity Verbosity { get => Verbosity.Batch; set {} }

        /// <summary>
        /// Always returns <c>false</c>.
        /// </summary>
        public bool Ask(string question) => Ask(question, defaultAnswer: false);

        /// <summary>
        /// Always returns <paramref name="defaultAnswer"/>.
        /// </summary>
        public bool Ask(string question, bool defaultAnswer, string? alternateMessage = null)
        {
            Log.Info($"Question: {question}\nAutomatic answer: {defaultAnswer}");
            return defaultAnswer;
        }

        /// <inheritdoc/>
        public void Output(string title, string message) => Log.Info($"{title}\n{message}");

        /// <inheritdoc/>
        public void Output<T>(string title, IEnumerable<T> data)
        {
            string message = StringUtils.Join(Environment.NewLine, (data ?? throw new ArgumentNullException(nameof(data))).Select(x => x?.ToString() ?? ""));
            Output(title ?? throw new ArgumentNullException(nameof(title)), message);
        }

        /// <inheritdoc />
        public void Output<T>(string title, NamedCollection<T> data)
            where T : INamed
            => Output(title, data.AsEnumerable());

        /// <inheritdoc/>
        public void Error(Exception exception) => Log.Error(exception);
    }
}
