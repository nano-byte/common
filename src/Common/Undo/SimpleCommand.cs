// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// An undo command that automatically tracks when <see cref="Execute"/> and <see cref="Undo"/> can be called.
/// </summary>
public abstract class SimpleCommand : IUndoCommand
{
    private bool _undoAvailable;

    /// <summary>
    /// Performs the desired action.
    /// </summary>
    public void Execute()
    {
        // We cannot perform the action repeatedly in a row
        if (_undoAvailable) throw new InvalidOperationException(Resources.RedoNotAvailable);

        OnExecute();

        // Ready for undo, don't redo
        _undoAvailable = true;
    }

    /// <summary>
    /// Template method to perform the desired action.
    /// </summary>
    protected abstract void OnExecute();

    /// <summary>
    /// Undoes the changes made by <see cref="Execute"/>.
    /// </summary>
    public virtual void Undo()
    {
        // If the action has not been performed yet, we cannot undo it
        if (!_undoAvailable) throw new InvalidOperationException(Resources.UndoNotAvailable);

        OnUndo();

        // As if the action had never happened
        _undoAvailable = false;
    }

    /// <summary>
    /// Template method to undo the changes made by <see cref="OnExecute"/>.
    /// </summary>
    protected abstract void OnUndo();
}
