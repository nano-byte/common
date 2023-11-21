// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Collections;

/// <summary>
/// Transparently caches retrieval requests, passed through to a callback on first request.
/// </summary>
/// <param name="retriever">The callback used to retrieve values not yet in the cache. Usually only called once per key. May be called multiple times in multi-threaded scenarios.</param>
/// <typeparam name="TKey">The type of keys used to request values.</typeparam>
/// <typeparam name="TValue">The type of values returned.</typeparam>
/// <remarks>This class is thread-safe.</remarks>
public sealed class TransparentCache<TKey, TValue>(Func<TKey, TValue> retriever) : TransparentCacheBase<TKey, TValue>
    where TKey : notnull
{
    /// <inheritdoc/>
    protected override TValue Retrieve(TKey key) => retriever(key);
}
