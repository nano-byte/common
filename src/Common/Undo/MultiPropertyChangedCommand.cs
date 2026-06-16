// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// An undo command that handles multiple changed properties.
/// </summary>
[RequiresUnreferencedCode("Performs reflection to get and set property values.")]
public class MultiPropertyChangedCommand : PreExecutedCommand
{
    private readonly object[] _targets;
    private readonly PropertyDescriptor _property;
    private readonly object?[] _oldValues;
    private readonly object? _newValue;

    /// <summary>
    /// Initializes the command after the properties were first changed.
    /// </summary>
    /// <param name="targets">The objects the property belongs to.</param>
    /// <param name="property">The property that was changed.</param>
    /// <param name="oldValues">The property's old values.</param>
    /// <param name="newValue">The property's current value.</param>
    public MultiPropertyChangedCommand(object[] targets, PropertyDescriptor property, object?[] oldValues, object? newValue)
    {
        _targets = targets ?? throw new ArgumentNullException(nameof(targets));
        _property = property ?? throw new ArgumentNullException(nameof(property));
        _oldValues = oldValues ?? throw new ArgumentNullException(nameof(oldValues));
        if (targets.Length != oldValues.Length) throw new ArgumentException(Resources.TargetsOldValuesLength, nameof(targets));
        _newValue = newValue;
    }

    /// <summary>
    /// Set the changed property value again.
    /// </summary>
    protected override void OnRedo()
    {
        foreach (var target in _targets)
            // Use reflection to get the specific property for each object and set the new value everywhere
            target.GetType().GetProperty(_property.Name)!.SetValue(target, _newValue, null);
    }

    /// <summary>
    /// Restore the original property values.
    /// </summary>
    protected override void OnUndo()
    {
        for (int i = 0; i < _targets.Length; i++)
            // Use reflection to get the specific property for each object and set the corresponding old value for each
            _targets[i].GetType().GetProperty(_property.Name)!.SetValue(_targets[i], _oldValues[i], null);
    }
}
