// Copyright Bastian Eicher
// Licensed under the MIT License

using System;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Transparently caches retrieval requests, passed through to a callback on first request.
    /// </summary>
    /// <remarks>This class is thread-safe.</remarks>
    /// <typeparam name="TKey">The type of keys used to request values.</typeparam>
    /// <typeparam name="TValue">The type of values returned.</typeparam>
    public sealed class TransparentCache<TKey, TValue> : TransparentCacheBase<TKey, TValue>
    {
        private readonly Func<TKey, TValue> _retriever;

        /// <summary>
        /// Creates a new transparent cache.
        /// </summary>
        /// <param name="retriever">The callback used to retrieve values not yet in the cache. Usually only called once per key. May be called multiple times in multi-threaded scenarios.</param>
        public TransparentCache(Func<TKey, TValue> retriever)
        {
            _retriever = retriever;
        }

        /// <inheritdoc/>
        protected override TValue Retrieve(TKey key) => _retriever(key);
    }
}
