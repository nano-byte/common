// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Linq;

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// Combines multiple <see cref="IUndoCommand"/>s into a single atomic transaction.
    /// </summary>
    public class CompositeCommand : SimpleCommand
    {
        private readonly IUndoCommand[] _commands;

        /// <summary>
        /// Creates a new composite command.
        /// </summary>
        /// <param name="commands">The commands to be contained inside the transaction.</param>
        public CompositeCommand(params IUndoCommand[] commands)
        {
            // Defensive copy
            _commands = (commands ?? throw new ArgumentNullException(nameof(commands))).ToArray();
        }

        /// <summary>
        /// Executes all the contained <see cref="IUndoCommand"/>s in order.
        /// </summary>
        protected override void OnExecute()
        {
            int countExecute = 0;
            try
            { // Try to execute all commands
                for (countExecute = 0; countExecute < _commands.Length; countExecute++)
                    _commands[countExecute].Execute();
            }
            catch
            { // Rollback before reporting exception
                for (int countUndo = countExecute - 1; countUndo >= 0; countUndo--)
                    _commands[countUndo].Undo();
                throw;
            }
        }

        /// <summary>
        /// Undoes all the contained <see cref="IUndoCommand"/>s in reverse order.
        /// </summary>
        protected override void OnUndo()
        {
            int countUndo = 0;
            try
            { // Try to undo all commands
                for (countUndo = _commands.Length - 1; countUndo >= 0; countUndo--)
                    _commands[countUndo].Undo();
            }
            catch
            { // Rollback before reporting exception
                for (int countExecute = countUndo + 1; countExecute < _commands.Length; countExecute++)
                    _commands[countExecute].Execute();
                throw;
            }
        }
    }
}
