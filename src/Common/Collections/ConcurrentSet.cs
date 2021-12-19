#if !NET20

// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections;
using System.Collections.Concurrent;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Represents a thread-safe set of values.
    /// </summary>
    /// <typeparam name="T">The type of the values in the set.</typeparam>
    /// <remarks>This class is thread-safe.</remarks>
    public class ConcurrentSet<T> : ICollection<T>
        where T : notnull
    {
        private readonly ConcurrentDictionary<T, bool> _dictionary;

        /// <summary>
        /// Creates a new concurrent set.
        /// </summary>
        public ConcurrentSet()
        {
            _dictionary = new();
        }

        /// <summary>
        /// Creates a new concurrent set.
        /// </summary>
        /// <param name="concurrencyLevel">The estimated number of threads that will update the set concurrently</param>
        /// <param name="capacity">The initial number of elements that the set can contain.</param>
        public ConcurrentSet(int concurrencyLevel, int capacity)
        {
            _dictionary = new(concurrencyLevel, capacity);
        }

        /// <summary>
        /// Creates a new concurrent set.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements.</param>
        public ConcurrentSet(IEqualityComparer<T> comparer)
        {
            _dictionary = new(comparer);
        }

        /// <summary>
        /// Creates a new concurrent set.
        /// </summary>
        /// <param name="collection">Elements to be copied to the new set.</param>
        public ConcurrentSet(IEnumerable<T> collection)
        {
            _dictionary = new(ToKeyValuePairs(collection));
        }

        /// <summary>
        /// Creates a new concurrent set.
        /// </summary>
        /// <param name="collection">Elements to be copied to the new set.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements.</param>
        public ConcurrentSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            _dictionary = new(ToKeyValuePairs(collection), comparer);
        }

        /// <summary>
        /// Creates a new concurrent set.
        /// </summary>
        /// <param name="concurrencyLevel">The estimated number of threads that will update the set concurrently</param>
        /// <param name="collection">Elements to be copied to the new set.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values.</param>
        public ConcurrentSet(int concurrencyLevel, IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            _dictionary = new(concurrencyLevel, ToKeyValuePairs(collection), comparer);
        }

        private static IEnumerable<KeyValuePair<T, bool>> ToKeyValuePairs(IEnumerable<T> collection)
            => collection.Select(x => new KeyValuePair<T, bool>(x, false));

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _dictionary.Keys.GetEnumerator();

        /// <inheritdoc/>
        public IEnumerator GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();

        /// <inheritdoc/>
        public void Add(T item) => _dictionary.TryAdd(item, false);

        /// <inheritdoc/>
        public void Clear() => _dictionary.Clear();

        /// <inheritdoc/>
        public bool Contains(T item) => _dictionary.ContainsKey(item);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => _dictionary.Keys.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public bool Remove(T item) => _dictionary.TryRemove(item, out _);

        /// <inheritdoc/>
        public int Count => _dictionary.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;
    }
}

#endif
