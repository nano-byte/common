// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// Executes <see cref="IUndoCommand"/>s and collects them into a <see cref="CompositeCommand"/> allowing a combined undo later on.
/// </summary>
public class CommandCollector : ICommandExecutor
{
    /// <inheritdoc/>
    public string? Path { get; set; }

    private readonly List<IUndoCommand> _commands = [];

    /// <summary>
    /// Store an <see cref="IUndoCommand"/> for later execution.
    /// </summary>
    /// <param name="command">The command to be stored.</param>
    public void Execute(IUndoCommand command)
    {
        #region Sanity checks
        if (command == null) throw new ArgumentNullException(nameof(command));
        #endregion

        command.Execute();
        _commands.Add(command);
    }

    /// <summary>
    /// Creates a new <see cref="CompositeCommand"/> containing all <see cref="IUndoCommand"/>s collected so far.
    /// </summary>
    public IUndoCommand BuildComposite() => new PreExecutedCompositeCommand(_commands);
}
