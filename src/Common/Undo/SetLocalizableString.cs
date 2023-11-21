// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Undo;

/// <summary>
/// An undo command that sets a <see cref="LocalizableString"/> in a <see cref="LocalizableStringCollection"/>.
/// </summary>
/// <param name="collection">The collection to be modified.</param>
/// <param name="element">The entry to be set in the <paramref name="collection"/>.</param>
public sealed class SetLocalizableString(LocalizableStringCollection collection, LocalizableString element) : SimpleCommand
{
    /// <summary>
    /// The collection to be modified.
    /// </summary>
    private readonly LocalizableStringCollection _collection = collection;

    /// <summary>
    /// The element to be added or removed from <see cref="_collection"/>.
    /// </summary>
    private readonly LocalizableString _entry = element;

    private string? _previousValue;

    /// <summary>
    /// Sets the entry in the collection.
    /// </summary>
    protected override void OnExecute()
    {
        try
        {
            _previousValue = _collection.GetExactLanguage(_entry.Language);
        }
        catch (KeyNotFoundException)
        {}
        _collection.Set(_entry.Language, _entry.Value);
    }

    /// <summary>
    /// Restores the original entry in the collection.
    /// </summary>
    protected override void OnUndo() => _collection.Set(_entry.Language, _previousValue);
}
