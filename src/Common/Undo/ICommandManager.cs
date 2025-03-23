namespace NanoByte.Common.Undo;

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
    T? Target { get; set; }

    /// <summary>
    /// Is raised after <see cref="Target"/> has been updated.
    /// </summary>
    event Action? TargetUpdated;

    /// <summary>
    /// Indicates whether there currently are operations that can be <see cref="Undo"/>ne.
    /// </summary>
    /// <remarks>This can also be used as an indicator for unsaved changes.</remarks>
    bool UndoEnabled { get; }

    /// <summary>
    /// Is raised when the value of <see cref="UndoEnabled"/> has changed.
    /// </summary>
    event Action? UndoEnabledChanged;

    /// <summary>
    /// Indicates whether there currently are operations that can be <see cref="Redo"/>ne.
    /// </summary>
    bool RedoEnabled { get; }

    /// <summary>
    /// Is raised when the value of <see cref="RedoEnabled"/> has changed.
    /// </summary>
    event Action? RedoEnabledChanged;

    /// <summary>
    /// Undoes the last action performed by <see cref="ICommandExecutor.Execute"/>.
    /// </summary>
    void Undo();

    /// <summary>
    /// Redoes the last action undone by <see cref="Undo"/>.
    /// </summary>
    void Redo();

    /// <summary>
    /// Saves the <see cref="Target"/> to an XML file
    /// </summary>
    /// <param name="path">The file to save to.</param>
    /// <exception cref="IOException">A problem occurs while writing the file.</exception>
    /// <exception cref="UnauthorizedAccessException">Write access to the file is not permitted.</exception>
    [RequiresUnreferencedCode("XML serialization reflection to discover properties.")]
    void Save(string path);
}
