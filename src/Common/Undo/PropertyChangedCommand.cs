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
    /// Set the changed property value again.
    /// </summary>
    protected override void OnRedo() => property.SetValue(target, newValue);

    /// <summary>
    /// Restore the original property value.
    /// </summary>
    protected override void OnUndo() => property.SetValue(target, oldValue);
}
