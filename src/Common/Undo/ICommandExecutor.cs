// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo
{
    /// <summary>
    /// Executes <see cref="IUndoCommand"/>s.
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// The path of the file the data structure being modified was loaded from. <c>null</c> if none.
        /// </summary>
        [Localizable(false)]
        string? Path { get; }

        /// <summary>
        /// Executes an <see cref="IUndoCommand"/> and stores it for later undo-operations.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        void Execute(IUndoCommand command);
    }
}
