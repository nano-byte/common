// Copyright Bastian Eicher
// Licensed under the MIT License

using System;

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// Executes <see cref="IUndoCommand"/>s without any additional handling.
    /// </summary>
    public class SimpleCommandExecutor : ICommandExecutor
    {
        /// <inheritdoc/>
        public string? Path { get; set; }

        /// <inheritdoc/>
        public void Execute(IUndoCommand command)
        {
            #region Sanity checks
            if (command == null) throw new ArgumentNullException(nameof(command));
            #endregion

            command.Execute();
        }
    }
}
