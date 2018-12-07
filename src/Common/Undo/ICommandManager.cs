using System;
using JetBrains.Annotations;

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// Executes <see cref="IUndoCommand"/>s for editing a specific object and allows undo/redo operations.
    /// </summary>
    /// <typeparam name="T">The type of the object being edited.</typeparam>
    public interface ICommandManager<T> : ICommandExecutor
        where T : class
    {
        /// <summary>
        /// The object being edited.
        /// </summary>
        [NotNull]
        T Target { get; set; }

        /// <summary>
        /// Is raised after <see cref="Target"/> has been updated.
        /// </summary>
        event Action TargetUpdated;

        /// <summary>
        /// Indicates whether <see cref="Undo"/> can presently be called.
        /// </summary>
        bool UndoEnabled { get; }

        /// <summary>
        /// Is raised when the value of <see cref="UndoEnabled"/> has changed.
        /// </summary>
        event Action UndoEnabledChanged;

        /// <summary>
        /// Indicates whether <see cref="Redo"/> can presently be called.
        /// </summary>
        bool RedoEnabled { get; }

        /// <summary>
        /// Is raised when the value of <see cref="RedoEnabled"/> has changed.
        /// </summary>
        event Action RedoEnabledChanged;

        /// <summary>
        /// Undoes the last action performed by <see cref="ICommandExecutor.Execute"/>.
        /// </summary>
        void Undo();

        /// <summary>
        /// Redoes the last action undone by <see cref="Undo"/>.
        /// </summary>
        void Redo();
    }
}