// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// Provides methods for creating <see cref="MultiPropertyChangedCommandFactory"/>s.
/// </summary>
public static class MultiPropertyChangedCommandFactory
{
    /// <summary>
    /// Initializes a <see cref="MultiPropertyChangedCommand"/> after a property was first changed.
    /// </summary>
    /// <param name="gridItem">The grid item representing the property being changed.</param>
    /// <param name="targets">The objects the <see cref="PropertyGrid.SelectedObject"/> is target at.</param>
    /// <param name="oldValues">The property's old values.</param>
    public static MultiPropertyChangedCommand ToMultiPropertyChangedCommand(this GridItem gridItem, object[] targets, object?[] oldValues)
        => new(targets, gridItem.PropertyDescriptor!, oldValues, gridItem.Value);
}
