// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// An undo command that handles a changed property.
/// </summary>
/// <param name="target">The object the property belongs to.</param>
/// <param name="property">The property that was changed.</param>
/// <param name="oldValue">The property's old value.</param>
/// <param name="newValue">The property's current value.</param>
public class PropertyChangedCommand(object target, PropertyDescriptor property, object? oldValue, object? newValue) : PreExecutedCommand
{
    /// <summary>
    /// The object the property belongs to.
    /// </summary>
    public object Target { get; } = target;

    /// <summary>
    /// The property that was changed.
    /// </summary>
    public PropertyDescriptor Property { get; } = property;

    /// <summary>
    /// Set the changed property value again.
    /// </summary>
    protected override void OnRedo() => Property.SetValue(Target, newValue);

    /// <summary>
    /// Restore the original property value.
    /// </summary>
    protected override void OnUndo() => Property.SetValue(Target, oldValue);
}
