// Copyright Bastian Eicher
// Licensed under the MIT License

using System;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Executes tasks silently and suppresses any questions.
    /// </summary>
    public class SilentTaskHandler : TaskHandlerBase
    {
        public SilentTaskHandler()
        {
            Verbosity = Verbosity.Batch;
        }

        /// <summary>
        /// Always returns <paramref name="defaultAnswer"/>.
        /// </summary>
        public override bool Ask(string question, bool? defaultAnswer = null, string? alternateMessage = null)
        {
            Log.Info($"Question: {question}\nAutomatic answer: {defaultAnswer}");
            return defaultAnswer ?? false;
        }

        /// <inheritdoc/>
        public override void Output(string title, string message)
            => Log.Info($"{title}\n{message}");

        /// <inheritdoc/>
        public override void Error(Exception exception)
            => Log.Error(exception);
    }
}
