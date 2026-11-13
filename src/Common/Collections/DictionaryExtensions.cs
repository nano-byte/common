// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Threading.Tasks;

namespace NanoByte.Common.Collections;

/// <summary>
/// Provides extension methods for <see cref="Dictionary{TKey,TValue}"/>s.
/// </summary>
public static class DictionaryExtensions
{
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        /// <summary>
        /// Adds multiple pairs to the dictionary in one go.
        /// </summary>
        public void AddRange<TSourceKey, TSourceValue>([InstantHandle] IEnumerable<KeyValuePair<TSourceKey, TSourceValue>> source)
            where TSourceKey : TKey
            where TSourceValue : TValue
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (source == null) throw new ArgumentNullException(nameof(source));
            #endregion

            foreach (var (key, value) in source)
                dictionary.Add(key, value);
        }

        /// <summary>
        /// Returns an existing element with a specific key from a dictionary or creates and adds a new element using a callback if it is missing.
        /// </summary>
        /// <param name="key">The key to look for in the <paramref name="dictionary"/>.</param>
        /// <param name="valueFactory">A callback that provides the value to add to the <paramref name="dictionary"/> if the <paramref name="key"/> is not found.</param>
        /// <returns>The existing element or the newly created element.</returns>
        /// <remarks>No superfluous calls to <paramref name="valueFactory"/> occur. Not thread-safe!</remarks>
        public TValue GetOrAdd([InstantHandle] TKey key, [InstantHandle] Func<TValue> valueFactory)
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));
            #endregion

            if (!dictionary.TryGetValue(key, out var value))
                dictionary.Add(key, value = valueFactory());
            return value;
        }

        /// <summary>
        /// Returns an existing element with a specific key from a dictionary or creates and adds a new element using a callback if it is missing.
        /// </summary>
        /// <param name="key">The key to look for in the <paramref name="dictionary"/>.</param>
        /// <param name="valueFactory">A callback that provides a task that provides the value to add to the <paramref name="dictionary"/> if the <paramref name="key"/> is not found.</param>
        /// <returns>The existing element or the newly created element.</returns>
        /// <remarks>Superfluous calls to <paramref name="valueFactory"/> may occur in case of read races. <see cref="IDisposable.Dispose"/> is called on superfluously created objects if implemented.</remarks>
        public async Task<TValue> GetOrAddAsync([InstantHandle] TKey key, Func<Task<TValue>> valueFactory)
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            #endregion

            if (dictionary.TryGetValue(key, out var existingValue))
                return existingValue;

            var newValue = await valueFactory().ConfigureAwait(false);

            // Detect and handle races
            if (dictionary.TryGetValue(key, out existingValue))
            {
                (newValue as IDisposable)?.Dispose();
                return existingValue;
            }

            dictionary.Add(key, newValue);
            return newValue;
        }

        /// <summary>
        /// Attempts to add the specified key and value to the dictionary.
        /// </summary>
        /// <returns><c>true</c> if the key/value pair was added to the dictionary successfully; <c>false</c> false if the key already existed in the dictionary.</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            #endregion

#if NET
            if (dictionary is Dictionary<TKey, TValue> dict)
                return dict.TryAdd(key, value);
#endif

            try
            {
                dictionary.Add(key, value);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether two dictionaries contain the same key-value pairs.
        /// </summary>
        /// <param name="other">The other dictionary to compare with.</param>
        /// <param name="valueComparer">Controls how to compare values; leave <c>null</c> for default comparer.</param>
        [Pure]
        public bool UnsequencedEquals(IDictionary<TKey, TValue> other, IEqualityComparer<TValue>? valueComparer = null)
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (other == null) throw new ArgumentNullException(nameof(other));
            #endregion

            if (dictionary == other) return true;
            if (dictionary.Count != other.Count) return false;

            var keySet = new HashSet<TKey>(dictionary.Keys);
            if (!other.Keys.All(keySet.Contains)) return false;

            valueComparer ??= EqualityComparer<TValue>.Default;
            foreach (var (key, value) in dictionary)
            {
                if (!valueComparer.Equals(value, other[key]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Generates a hash code for the contents of the dictionary.
        /// </summary>
        /// <param name="valueComparer">Controls how to compare values; leave <c>null</c> for default comparer.</param>
        /// <seealso cref="UnsequencedEquals{TKey,TValue}"/>
        [Pure]
        public int GetUnsequencedHashCode([InstantHandle] IEqualityComparer<TValue>? valueComparer = null)
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            #endregion

            valueComparer ??= EqualityComparer<TValue>.Default;
            int result = 397;
            foreach (var (key, value) in dictionary)
            {
                var hash = new HashCode();
                hash.Add(key);
                hash.Add(value, valueComparer);
                result ^= hash.ToHashCode();
            }
            return result;
        }
    }

    /// <summary>
    /// Builds a <see cref="MultiDictionary{TKey,TValue}"/> from an enumerable.
    /// </summary>
    /// <param name="elements">The elements to build the dictionary from.</param>
    /// <param name="keySelector">Selects the dictionary key from an input element.</param>
    /// <param name="valueSelector">Selects a dictionary value from an input element.</param>
    [Pure]
    public static MultiDictionary<TKey, TValue> ToMultiDictionary<TSource, TKey, TValue>([InstantHandle] this IEnumerable<TSource> elements, [InstantHandle] Func<TSource, TKey> keySelector, [InstantHandle] Func<TSource, TValue> valueSelector)
        where TKey : notnull
    {
        #region Sanity checks
        if (elements == null) throw new ArgumentNullException(nameof(elements));
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
        if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));
        #endregion

        var result = new MultiDictionary<TKey, TValue>();
        foreach (TSource item in elements)
            result.Add(keySelector(item), valueSelector(item));
        return result;
    }

    /// <summary>
    /// Returns an existing element with a specific key from a dictionary or creates and adds a new element using the default constructor if it is missing.
    /// </summary>
    /// <param name="dictionary">The dictionary to get an element from or to add an element to.</param>
    /// <param name="key">The key to look for in the <paramref name="dictionary"/>.</param>
    /// <returns>The existing element or the newly created element.</returns>
    public static TValue GetOrAdd<TKey, TValue>([InstantHandle] this IDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
        where TValue : new()
    {
        #region Sanity checks
        if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
        #endregion

        return dictionary.GetOrAdd(key, () => new TValue());
    }

    /// <summary>
    /// Deconstructs a <see cref="KeyValuePair{TKey,TValue}"/> like a tuple.
    /// </summary>
    /// <example>
    /// foreach (var (key, value) in dictionary)
    /// {/*...*/}
    /// </example>
    [Pure]
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
    {
        key = pair.Key;
        value = pair.Value;
    }

#if !NET
    /// <param name="dictionary">The dictionary to get an element from.</param>
    extension<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        /// <summary>
        /// Returns an existing element with a specific key from a dictionary or the value type's default value if it is missing.
        /// </summary>
        /// <param name="key">The key to look for in the <paramref name="dictionary"/>.</param>
        /// <returns>The existing element or the default value of <typeparamref name="TValue"/>.</returns>
        [Pure]
        public TValue? GetValueOrDefault(TKey key)
            => dictionary.GetValueOrDefault(key, default!);

        /// <summary>
        /// Returns an existing element with a specific key from a dictionary or a default value if it is missing.
        /// </summary>
        /// <param name="key">The key to look for in the <paramref name="dictionary"/>.</param>
        /// <param name="defaultValue">The default value to return if the element is missing.</param>
        /// <returns>The existing element or <paramref name="defaultValue"/>.</returns>
        [Pure]
        public TValue GetValueOrDefault(TKey key, TValue defaultValue)
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            #endregion

            return dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;
        }
    }
#endif
}
