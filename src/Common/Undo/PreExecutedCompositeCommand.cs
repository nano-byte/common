// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// Combines multiple already executed <see cref="IUndoCommand"/>s into a single atomic transaction.
/// </summary>
/// <param name="commands">The commands to be contained inside the transaction.</param>
public class PreExecutedCompositeCommand(IEnumerable<IUndoCommand> commands) : PreExecutedCommand
{
    /// <summary>
    /// The commands contained inside the transaction.
    /// </summary>
    public
#if NET20 || NET40
        IList<IUndoCommand>
#else
        IReadOnlyList<IUndoCommand>
#endif
        Commands { get; } = commands.ToList(); // Defensive copy

    /// <summary>
    /// Executes all the contained <see cref="IUndoCommand"/>s in order.
    /// </summary>
    protected override void OnRedo()
    {
        int countExecute = 0;
        try
        { // Try to execute all commands
            for (countExecute = 0; countExecute < Commands.Count; countExecute++)
                Commands[countExecute].Execute();
        }
        catch
        { // Rollback before reporting exception
            for (int countUndo = countExecute - 1; countUndo >= 0; countUndo--)
                Commands[countUndo].Undo();
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
            for (countUndo = Commands.Count - 1; countUndo >= 0; countUndo--)
                Commands[countUndo].Undo();
        }
        catch
        { // Rollback before reporting exception
            for (int countExecute = countUndo + 1; countExecute < Commands.Count; countExecute++)
                Commands[countExecute].Execute();
            throw;
        }
    }
}
