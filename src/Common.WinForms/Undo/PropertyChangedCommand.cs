// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// An undo command that handles a changed property - usually used with a <see cref="PropertyGrid"/>.
/// </summary>
/// <param name="target">The object the property belongs to.</param>
/// <param name="property">The property that was changed.</param>
/// <param name="oldValue">The property's old value.</param>
/// <param name="newValue">The property's current value.</param>
public class PropertyChangedCommand(object target, PropertyDescriptor property, object? oldValue, object? newValue) : PreExecutedCommand
{
    /// <summary>
    /// Initializes the command after the property was first changed.
    /// </summary>
    /// <param name="target">The object the <see cref="PropertyGrid.SelectedObject"/> is target at.</param>
    /// <param name="e">The event data from the <see cref="PropertyGrid.PropertyValueChanged"/>.</param>
    [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "The arguments are passed on to a different overload of the constructor")]
    [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "This is simply a comfort wrapper for extracting values from the event arguments")]
    public PropertyChangedCommand(object target, PropertyValueChangedEventArgs e)
        : this(target, e.ChangedItem!.PropertyDescriptor!, e.OldValue, e.ChangedItem.Value)
    {}

    /// <summary>
    /// Set the changed property value again.
    /// </summary>
    protected override void OnRedo() => property.SetValue(target, newValue);

    /// <summary>
    /// Restore the original property value.
    /// </summary>
    protected override void OnUndo() => property.SetValue(target, oldValue);
}
