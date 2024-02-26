// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// A dictionary that allows a key to reference multiple values.
/// </summary>
/// <typeparam name="TKey">The type to use as a key to identify entries in the dictionary.</typeparam>
/// <typeparam name="TValue">The type to use as elements to store in the dictionary.</typeparam>
/// <remarks>This structure internally uses hash maps, so most operations run in O(1).</remarks>
[Serializable]
public class MultiDictionary<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>
    where TKey : notnull
{
    /// <summary>
    /// A collection containing the values in the dictionary.
    /// </summary>
    [CollectionAccess(CollectionAccessType.Read)]
    public new IEnumerable<TValue> Values => base.Values.SelectMany(x => x);

    /// <summary>
    /// Adds an element with the provided key and value to the dictionary.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    [CollectionAccess(CollectionAccessType.UpdatedContent)]
    public void Add(TKey key, TValue value)
    {
        #region Sanity checks
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (value == null) throw new ArgumentNullException(nameof(value));
        #endregion

        this
           .GetOrAdd(key, () => new HashSet<TValue>())
           .Add(value);
    }

    /// <summary>
    /// Removes an element with the provided key and value from the dictionary.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <param name="value">The value of the element to remove.</param>
    /// <returns><c>true</c> if any elements were successfully removed; otherwise, <c>false</c>.</returns>
    [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
    public bool Remove(TKey key, TValue value)
    {
        #region Sanity checks
        if (value == null) throw new ArgumentNullException(nameof(value));
        #endregion

        if (TryGetValue(key, out var values))
        {
            if (values.Remove(value))
            {
                if (values.Count == 0) Remove(key);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets a collection containing the values with the specified key.
    /// </summary>
    /// <param name="key">The key of the element to get.</param>
    /// <returns>A list of elements with the specified key. Empty list if the key was not found.</returns>
    [CollectionAccess(CollectionAccessType.Read)]
    public new IEnumerable<TValue> this[TKey key]
        => TryGetValue(key, out var result) ? result : [];
}
