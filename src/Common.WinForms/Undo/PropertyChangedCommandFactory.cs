// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// Provides methods for creating <see cref="PropertyChangedCommand"/>s.
/// </summary>
public static class PropertyChangedCommandFactory
{
    /// <summary>
    /// Initializes a <see cref="PropertyChangedCommand"/> after a property was first changed.
    /// </summary>
    /// <param name="e">The event data from the <see cref="PropertyGrid.PropertyValueChanged"/>.</param>
    /// <param name="target">The object the <see cref="PropertyGrid.SelectedObject"/> is target at.</param>
    public static PropertyChangedCommand ToPropertyChangedCommand(this PropertyValueChangedEventArgs e, object target)
        => new(target, e.ChangedItem!.PropertyDescriptor!, e.OldValue, e.ChangedItem.Value);
}
