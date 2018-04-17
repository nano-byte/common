// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// Controls editing a target using <see cref="IUndoCommand"/>s.
    /// </summary>
    /// <typeparam name="T">The type of the root object being edited.</typeparam>
    public abstract class CommandManager<T> : ICommandExecutor
    {
        #region Variables
        /// <summary>Entries used by the undo-system to undo changes</summary>
        protected readonly Stack<IUndoCommand> UndoStack = new Stack<IUndoCommand>();

        /// <summary>Entries used by the undo-system to redo changes previously undone</summary>
        protected readonly Stack<IUndoCommand> RedoStack = new Stack<IUndoCommand>();
        #endregion

        #region Properties
        /// <summary>
        /// The root object being edited.
        /// </summary>
        public abstract T Target { get; set; }

        /// <summary>
        /// The path of the file the <see cref="Target"/> was loaded from. <c>null</c> if none.
        /// </summary>
        public string Path { get; protected set; }

        /// <summary>
        /// Indicates whether the <see cref="Target"/> has unsaved changes.
        /// </summary>
        public bool Changed { get; protected set; }

        private bool _undoEnabled;

        /// <summary>
        /// Indicates whether <see cref="Undo"/> can presently be called.
        /// </summary>
        public bool UndoEnabled
        {
            get => _undoEnabled;
            private set
            {
                _undoEnabled = value;
                UndoEnabledChanged?.Invoke();
            }
        }

        private bool _redoEnabled;

        /// <summary>
        /// Indicates whether <see cref="Redo"/> can presently be called.
        /// </summary>
        public bool RedoEnabled
        {
            get => _redoEnabled;
            private set
            {
                _redoEnabled = value;
                RedoEnabledChanged?.Invoke();
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Is raised after an <see cref="IUndoCommand"/> has been executed.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
        public event Action Updated;

        /// <summary>
        /// Is raised when the value of <see cref="UndoEnabled"/> has changed.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Cannot rename System.Action<T>.")]
        public event Action UndoEnabledChanged;

        /// <summary>
        /// Is raised when the value of <see cref="RedoEnabled"/> has changed.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Cannot rename System.Action<T>.")]
        public event Action RedoEnabledChanged;

        protected void OnUpdated() => Updated?.Invoke();
        #endregion

        #region Commands
        /// <inheritdoc/>
        public void Execute(IUndoCommand command)
        {
            #region Sanity checks
            if (command == null) throw new ArgumentNullException(nameof(command));
            #endregion

            // Execute the command and store it for later undo
            command.Execute();

            // Store the command and invalidate previous redo history
            UndoStack.Push(command);
            RedoStack.Clear();

            // Only enable the buttons that still have a use
            UndoEnabled = true;
            RedoEnabled = false;
            OnUpdated();

            Changed = true;
        }

        /// <summary>
        /// Undoes the last action performed by <see cref="Execute"/>.
        /// </summary>
        public void Undo()
        {
            if (UndoStack.Count == 0) return;

            // Remove last command from the undo list, execute it and add it to the redo list
            var lastCommand = UndoStack.Pop();
            lastCommand.Undo();
            RedoStack.Push(lastCommand);

            // Only enable the buttons that still have a use
            if (UndoStack.Count == 0)
            {
                UndoEnabled = false;
                Changed = false;
            }
            RedoEnabled = true;

            OnUpdated();
        }

        /// <summary>
        /// Redoes the last action undone by <see cref="Undo"/>.
        /// </summary>
        public void Redo()
        {
            if (RedoStack.Count == 0) return;

            // Remove last command from the redo list, execute it and add it to the undo list
            var lastCommand = RedoStack.Pop();
            lastCommand.Execute();
            UndoStack.Push(lastCommand);

            // Mark as "to be saved" again
            Changed = true;

            // Only enable the buttons that still have a use
            RedoEnabled = RedoStack.Count > 0;
            UndoEnabled = true;

            OnUpdated();
        }

        /// <summary>
        /// Resets the entire undo system, clearing all stacks.
        /// </summary>
        public void Reset()
        {
            Changed = false;

            UndoStack.Clear();
            RedoStack.Clear();

            UndoEnabled = false;
            RedoEnabled = false;
        }
        #endregion
    }
}
