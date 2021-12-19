// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// Combines multiple already executed <see cref="IUndoCommand"/>s into a single atomic transaction.
/// </summary>
public class PreExecutedCompositeCommand : PreExecutedCommand
{
    private readonly List<IUndoCommand> _commands;

    /// <summary>
    /// Creates a new composite command.
    /// </summary>
    /// <param name="commands">The commands to be contained inside the transaction.</param>
    public PreExecutedCompositeCommand(IEnumerable<IUndoCommand> commands)
    {
        // Defensive copy
        _commands = (commands ?? throw new ArgumentNullException(nameof(commands))).ToList();
    }

    /// <summary>
    /// Executes all the contained <see cref="IUndoCommand"/>s in order.
    /// </summary>
    protected override void OnRedo()
    {
        int countExecute = 0;
        try
        { // Try to execute all commands
            for (countExecute = 0; countExecute < _commands.Count; countExecute++)
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
            for (countUndo = _commands.Count - 1; countUndo >= 0; countUndo--)
                _commands[countUndo].Undo();
        }
        catch
        { // Rollback before reporting exception
            for (int countExecute = countUndo + 1; countExecute < _commands.Count; countExecute++)
                _commands[countExecute].Execute();
            throw;
        }
    }
}