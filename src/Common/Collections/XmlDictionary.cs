// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// A string dictionary that supports data-binding and can be XML serialized.
/// </summary>
[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This class behaves like a dictionary but doesn't implement the corresponding interfaces because that would prevent XML serialization")]
[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This class behaves like a dictionary but doesn't implement the corresponding interfaces because that would prevent XML serialization")]
[Serializable]
public class XmlDictionary : BindingList<XmlDictionaryEntry>, ICloneable<XmlDictionary>
{
    /// <summary>
    /// Adds a new value and links it to a key
    /// </summary>
    /// <param name="key">The key object</param>
    /// <param name="value">The value</param>
    /// <exception cref="ArgumentException">The <paramref name="key"/> already exists in the dictionary.</exception>
    public void Add(string key, string value)
    {
        if (!string.IsNullOrEmpty(key) && ContainsKey(key))
            throw new ArgumentException(Resources.KeyAlreadyPresent, nameof(key));
        Add(new(key, value));
    }

    /// <inheritdoc/>
    protected override void InsertItem(int index, XmlDictionaryEntry item)
    {
        if (!string.IsNullOrEmpty(item.Key) && ContainsKey(item.Key))
            throw new ArgumentException(Resources.KeyAlreadyPresent, nameof(item));
        item.Parent = this;

        base.InsertItem(index, item);
    }

    /// <summary>
    /// Removes all values assigned to this key.
    /// </summary>
    /// <param name="key">The key to look for.</param>
    /// <returns><c>true</c> if one or more elements were removed; otherwise, <c>false</c>.</returns>
    public bool Remove(string key)
    {
        // Build a list of elements to remove
        var pendingRemove = new LinkedList<XmlDictionaryEntry>();
        foreach (var pair in this.Where(pair => pair.Key.Equals(key)))
            pendingRemove.AddLast(pair);

        // Remove the elements one-by-one
        foreach (var pair in pendingRemove) Remove(pair);

        // Were any elements removed?
        return pendingRemove.Count > 0;
    }

    /// <summary>
    /// Checks whether this collection contains a certain key.
    /// </summary>
    /// <param name="key">The key to look for.</param>
    /// <returns><c>true</c> if the key was found.</returns>
    public bool ContainsKey(string key) => this.Any(entry => entry.Key == key);

    /// <summary>
    /// Checks whether this collection contains a certain value.
    /// </summary>
    /// <param name="value">The value to look for.</param>
    /// <returns><c>true</c> if the value was found.</returns>
    public bool ContainsValue(string value) => this.Any(entry => entry.Value == value);

    /// <summary>
    /// Sorts all entries alphabetically by their key.
    /// </summary>
    public void Sort()
    {
        // Get list to sort
        var items = Items as List<XmlDictionaryEntry>;

        // Apply the sorting algorithms, if items are sortable
        items?.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.OrdinalIgnoreCase));

        // Let bound controls know they should refresh their views
        OnListChanged(new(ListChangedType.Reset, -1));
    }

    /// <summary>
    /// Returns the value associated to a specific key.
    /// </summary>
    /// <param name="key">The key to look for.</param>
    /// <returns>The value associated to <paramref name="key"/>.</returns>
    /// <exception cref="KeyNotFoundException"><paramref name="key"/> was not found in the collection.</exception>
    public string GetValue(string key)
    {
        foreach (var pair in this.Where(pair => pair.Key.Equals(key)))
            return pair.Value;
        throw new KeyNotFoundException();
    }

    #region Conversion
    /// <summary>
    /// Convert this <see cref="XmlDictionary"/> to a <see cref="Dictionary{TKey,TValue}"/> for better lookup-performance.
    /// </summary>
    /// <returns>A dictionary containing the same data as this collection.</returns>
    public IDictionary<string, string> ToDictionary() => this.ToDictionary(entry => entry.Key, entry => entry.Value);
    #endregion

    #region Clone
    /// <summary>
    /// Creates a deep copy of this <see cref="XmlDictionary"/> (elements are cloned).
    /// </summary>
    /// <returns>The cloned <see cref="XmlDictionary"/>.</returns>
    public virtual XmlDictionary Clone()
    {
        var newDict = new XmlDictionary();
        foreach (var entry in this)
            newDict.Add(entry.Clone());

        return newDict;
    }
    #endregion
}