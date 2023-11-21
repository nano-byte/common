// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Storage;

namespace NanoByte.Common.Undo;

/// <summary>
/// Executes <see cref="IUndoCommand"/>s for editing a specific object and allows undo/redo operations.
/// </summary>
/// <param name="target">The object being edited.</param>
/// <param name="path">The path of the file the <paramref name="target"/> was loaded from. <c>null</c> if none.</param>
/// <typeparam name="T">The type of the object being edited.</typeparam>
public class CommandManager<T>(T target, string? path = null) : ICommandManager<T>
    where T : class
{
    private readonly Stack<IUndoCommand>
        _undoStack = new(),
        _redoStack = new();

    /// <inheritdoc/>
    public T? Target { get; set; } = target ?? throw new ArgumentNullException(nameof(target));

    /// <inheritdoc/>
    public event Action? TargetUpdated;

    /// <summary>
    /// The path of the file the <see cref="Target"/> was loaded from. <c>null</c> if none.
    /// </summary>
    public string? Path { get; set; } = path;

    /// <inheritdoc/>
    public bool UndoEnabled { get; private set; }

    /// <inheritdoc/>
    public event Action? UndoEnabledChanged;

    private void SetUndoEnabled(bool value)
    {
        UndoEnabled = value;
        UndoEnabledChanged?.Invoke();
    }

    /// <inheritdoc/>
    public bool RedoEnabled { get; private set; }

    /// <inheritdoc/>
    public event Action? RedoEnabledChanged;

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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual void Save(string path)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        if (Target == null) return;

        Target.SaveXml(path);
        Path = path;
        ClearUndo();
    }

    /// <summary>
    /// Loads an object from an XML file.
    /// </summary>
    /// <param name="path">The file to load from.</param>
    /// <returns>An <see cref="ICommandManager{T}"/> containing the loaded object.</returns>
    /// <exception cref="IOException">A problem occurs while reading the file.</exception>
    /// <exception cref="UnauthorizedAccessException">Read access to the file is not permitted.</exception>
    /// <exception cref="InvalidDataException">A problem occurs while deserializing the XML data.</exception>
    public static CommandManager<T> Load(string path)
        => new(XmlStorage.LoadXml<T>(path), path);
}

/// <summary>
/// Factory methods for <see cref="ICommandManager{T}"/>.
/// </summary>
public static class CommandManager
{
    /// <summary>
    /// Creates a new command manager.
    /// </summary>
    /// <param name="target">The object being edited.</param>
    /// <param name="path">The path of the file the <paramref name="target"/> was loaded from. <c>null</c> if none.</param>
    public static ICommandManager<T> For<T>(T target, string? path = null)
        where T : class
        => new CommandManager<T>(target, path);
}
