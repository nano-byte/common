/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Transparently caches retrieval requests, passed through to a template method on first request.
    /// </summary>
    /// <remarks>This class is thread-safe.</remarks>
    /// <typeparam name="TKey">The type of keys used to request values.</typeparam>
    /// <typeparam name="TValue">The type of values returned.</typeparam>
    public abstract class TransparentCacheBase<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _lookup = new Dictionary<TKey, TValue>();
        private readonly object _lock = new object();

        /// <summary>
        /// Retrieves a value from the cache.
        /// </summary>
        public TValue this[[NotNull] TKey key]
        {
            get
            {
                #region Sanity checks
                if (key == null) throw new ArgumentNullException("key");
                #endregion

                lock (_lock)
                {
                    TValue result;
                    if (!_lookup.TryGetValue(key, out result))
                        _lookup.Add(key, result = Retrieve(key));
                    return result;
                }
            }
        }

        /// <summary>
        /// The template method used to retrieve values not yet in the cache. Usually only called once per key. May be called multiple times in multi-threaded scenarios.
        /// </summary>
        protected abstract TValue Retrieve([NotNull] TKey key);

        /// <summary>
        /// Removes the the entry with the specified <paramref name="key"/> from the cache.
        /// </summary>
        /// <returns><c>true</c> if a matching entry was found and removed; <c>false</c> if no matching entry was in the cache.</returns>
        public bool Remove([NotNull] TKey key)
        {
            lock (_lock)
                return _lookup.Remove(key);
        }

        /// <summary>
        /// Removes all entries from the cache.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
                _lookup.Clear();
        }
    }
}