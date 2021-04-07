// Copyright Bastian Eicher
// Licensed under the MIT License

using System;

#if NET20
using System.Collections.Generic;
#else
using System.Collections.Concurrent;
#endif

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Transparently caches retrieval requests, passed through to a template method on first request.
    /// </summary>
    /// <remarks>This class is thread-safe.</remarks>
    /// <typeparam name="TKey">The type of keys used to request values.</typeparam>
    /// <typeparam name="TValue">The type of values returned.</typeparam>
    public abstract class TransparentCacheBase<TKey, TValue>
        where TKey : notnull
    {
#if NET20
        private readonly Dictionary<TKey, TValue> _lookup = new();
        private readonly object _lock = new();
#else
        private readonly ConcurrentDictionary<TKey, TValue> _lookup = new();
#endif

        /// <summary>
        /// Retrieves a value from the cache.
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                #region Sanity checks
                if (key == null) throw new ArgumentNullException(nameof(key));
                #endregion

#if NET20
                lock (_lock)
                {
                    if (!_lookup.TryGetValue(key, out var result))
                        _lookup.Add(key, result = Retrieve(key));
                    return result;
                }
#else
                return _lookup.GetOrAdd(key, Retrieve);
#endif
            }
        }

        /// <summary>
        /// Template method used to retrieve values not yet in the cache. Usually only called once per key. May be called multiple times in multi-threaded scenarios.
        /// </summary>
        protected abstract TValue Retrieve(TKey key);

        /// <summary>
        /// Removes the the entry with the specified <paramref name="key"/> from the cache.
        /// </summary>
        /// <returns><c>true</c> if a matching entry was found and removed; <c>false</c> if no matching entry was in the cache.</returns>
        public bool Remove(TKey key)
        {
#if NET20
            lock (_lock)
                return _lookup.Remove(key);
#else
            return _lookup.TryRemove(key, out _);
#endif
        }

        /// <summary>
        /// Removes all entries from the cache.
        /// </summary>
        public void Clear()
        {
#if NET20
            lock (_lock)
#endif
            _lookup.Clear();
        }
    }
}
