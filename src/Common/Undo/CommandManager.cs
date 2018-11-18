// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// Executes <see cref="IUndoCommand"/>s for editing a specific object and allows undo/redo operations.
    /// </summary>
    /// <typeparam name="T">The type of the object being edited.</typeparam>
    [PublicAPI]
    public class CommandManager<T> : ICommandManager<T>
        where T : class
    {
        private readonly Stack<IUndoCommand>
            _undoStack = new Stack<IUndoCommand>(),
            _redoStack = new Stack<IUndoCommand>();

        /// <inheritdoc/>
        public T Target { get; set; }

        /// <inheritdoc/>
        public event Action TargetUpdated;

        /// <summary>
        /// The path of the file the <see cref="Target"/> was loaded from. <c>null</c> if none.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Creates a new command manager.
        /// </summary>
        /// <param name="target">The root object being edited.</param>
        /// <param name="path">The path of the file the <see cref="Target"/> was loaded from. <c>null</c> if none.</param>
        public CommandManager([NotNull] T target, [CanBeNull] string path = null)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Path = path;
        }

        /// <inheritdoc/>
        public bool UndoEnabled { get; private set; }

        /// <inheritdoc/>
        public event Action UndoEnabledChanged;

        private void SetUndoEnabled(bool value)
        {
            UndoEnabled = value;
            UndoEnabledChanged?.Invoke();
        }

        /// <inheritdoc/>
        public bool RedoEnabled { get; private set; }

        /// <inheritdoc/>
        public event Action RedoEnabledChanged;

        private void SetRedoEnabled(bool value)
        {
            RedoEnabled = value;
            RedoEnabledChanged?.Invoke();
        }

        /// <inheritdoc/>
        public void Execute(IUndoCommand command)
        {
            #region Sanity checks
            if (command == null) throw new ArgumentNullException(nameof(command));
            #endregion

            // Execute the command and store it for later undo
            command.Execute();

            // Store the command and invalidate previous redo history
            _undoStack.Push(command);
            _redoStack.Clear();

            // Only enable the buttons that still have a use
            SetUndoEnabled(true);
            SetRedoEnabled(false);
            TargetUpdated?.Invoke();
        }

        /// <summary>
        /// Undoes the last action performed by <see cref="Execute"/>.
        /// </summary>
        public void Undo()
        {
            if (_undoStack.Count == 0) return;

            // Remove last command from the undo list, execute it and add it to the redo list
            var lastCommand = _undoStack.Pop();
            lastCommand.Undo();
            _redoStack.Push(lastCommand);

            // Only enable the buttons that still have a use
            if (_undoStack.Count == 0) SetUndoEnabled(false);
            SetRedoEnabled(true);

            TargetUpdated?.Invoke();
        }

        /// <summary>
        /// Redoes the last action undone by <see cref="Undo"/>.
        /// </summary>
        public void Redo()
        {
            if (_redoStack.Count == 0) return;

            // Remove last command from the redo list, execute it and add it to the undo list
            var lastCommand = _redoStack.Pop();
            lastCommand.Execute();
            _undoStack.Push(lastCommand);

            // Only enable the buttons that still have a use
            SetUndoEnabled(true);
            SetRedoEnabled(_redoStack.Count > 0);

            TargetUpdated?.Invoke();
        }

        /// <summary>
        /// Clears the undo/redo stacks.
        /// </summary>
        protected void ClearUndo()
        {
            _undoStack.Clear();
            _redoStack.Clear();

            SetUndoEnabled(false);
            SetRedoEnabled(false);
        }
    }
}
