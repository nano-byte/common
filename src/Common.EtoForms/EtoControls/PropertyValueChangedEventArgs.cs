// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Undo;

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Event data for <see cref="PropertyEditor.PropertyValueChanged"/>.
/// </summary>
/// <param name="property">The property that was changed.</param>
/// <param name="oldValue">The property's value before the change.</param>
/// <param name="newValue">The property's value after the change.</param>
public class PropertyValueChangedEventArgs(PropertyDescriptor property, object? oldValue, object? newValue) : EventArgs
{
    /// <summary>
    /// The property that was changed.
    /// </summary>
    public PropertyDescriptor Property { get; } = property;

    /// <summary>
    /// The property's value before the change.
    /// </summary>
    public object? OldValue { get; } = oldValue;

    /// <summary>
    /// The property's value after the change.
    /// </summary>
    public object? NewValue { get; } = newValue;

    /// <summary>
    /// Creates a <see cref="PropertyChangedCommand"/> that allows the change to be undone and redone.
    /// </summary>
    /// <param name="target">The <see cref="PropertyEditor.SelectedObject"/> the property belongs to.</param>
    public PropertyChangedCommand ToPropertyChangedCommand(object target)
        => new(target, Property, OldValue, NewValue);
}
