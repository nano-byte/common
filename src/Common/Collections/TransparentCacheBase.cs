// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Concurrent;

namespace NanoByte.Common.Collections;

/// <summary>
/// Transparently caches retrieval requests, passed through to a template method on first request.
/// </summary>
/// <remarks>This class is thread-safe.</remarks>
/// <typeparam name="TKey">The type of keys used to request values.</typeparam>
/// <typeparam name="TValue">The type of values returned.</typeparam>
public abstract class TransparentCacheBase<TKey, TValue>
    where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TValue> _lookup = [];

    /// <summary>
    /// Retrieves a value from the cache.
    /// </summary>
    [CollectionAccess(CollectionAccessType.Read)]
    public TValue this[TKey key]
    {
        get
        {
            #region Sanity checks
            if (key == null) throw new ArgumentNullException(nameof(key));
            #endregion

            return _lookup.GetOrAdd(key, Retrieve);
        }
    }

    /// <summary>
    /// Template method used to retrieve values not yet in the cache. Usually only called once per key. May be called multiple times in multi-threaded scenarios.
    /// </summary>
    protected abstract TValue Retrieve(TKey key);

    /// <summary>
    /// Removes the entry with the specified <paramref name="key"/> from the cache.
    /// </summary>
    /// <returns><c>true</c> if a matching entry was found and removed; <c>false</c> if no matching entry was in the cache.</returns>
    [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
    public bool Remove(TKey key)
        => _lookup.TryRemove(key, out _);

    /// <summary>
    /// Removes all entries from the cache.
    /// </summary>
    [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
    public void Clear()
        => _lookup.Clear();
}