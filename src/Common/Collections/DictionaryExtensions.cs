// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System.Threading.Tasks;
#endif

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Provides extension methods for <see cref="Dictionary{TKey,TValue}"/>s.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Adds multiple pairs to the dictionary in one go.
        /// </summary>
        public static void AddRange<TSourceKey, TSourceValue, TTargetKey, TTargetValue>(this IDictionary<TTargetKey, TTargetValue> target, [InstantHandle] IEnumerable<KeyValuePair<TSourceKey, TSourceValue>> source)
            where TSourceKey : TTargetKey
            where TSourceValue : TTargetValue
            where TTargetKey : notnull
        {
            #region Sanity checks
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (source == null) throw new ArgumentNullException(nameof(source));
            #endregion

            foreach (var (key, value) in source)
                target.Add(key, value);
        }

        /// <summary>
        /// Returns an existing element with a specific key from a dictionary or the value type's default value if it is missing.
        /// </summary>
        /// <param name="dictionary">The dictionary to get an element from.</param>
        /// <param name="key">The key to look for in the <paramref name="dictionary"/>.</param>
        /// <returns>The existing element or the default value of <typeparamref name="TValue"/>.</returns>
        [Pure]
        [return: MaybeNull]
        public static TValue GetOrDefault<TKey, TValue>([InstantHandle] this IDictionary<TKey, TValue> dictionary, TKey key)
            where TKey : notnull
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            #endregion

            dictionary.TryGetValue(key, out var value);
            return value;
        }

        /// <summary>
        /// Returns an existing element with a specific key from a dictionary or creates and adds a new element using a callback if it is missing.
        /// </summary>
        /// <param name="dictionary">The dictionary to get an element from or to add an element to.</param>
        /// <param name="key">The key to look for in the <paramref name="dictionary"/>.</param>
        /// <param name="valueFactory">A callback that provides the value to add to the <paramref name="dictionary"/> if the <paramref name="key"/> is not found.</param>
        /// <returns>The existing element or the newly created element.</returns>
        /// <remarks>No superfluous calls to <paramref name="valueFactory"/> occur. Not thread-safe!</remarks>
        public static TValue GetOrAdd<TKey, TValue>([InstantHandle] this IDictionary<TKey, TValue> dictionary, TKey key, [InstantHandle] Func<TValue> valueFactory)
            where TKey : notnull
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

#if !NET20 && !NET40
        /// <summary>
        /// Returns an existing element with a specific key from a dictionary or creates and adds a new element using a callback if it is missing.
        /// </summary>
        /// <param name="dictionary">The dictionary to get an element from or to add an element to.</param>
        /// <param name="key">The key to look for in the <paramref name="dictionary"/>.</param>
        /// <param name="valueFactory">A callback that provides a task that provides the value to add to the <paramref name="dictionary"/> if the <paramref name="key"/> is not found.</param>
        /// <returns>The existing element or the newly created element.</returns>
        /// <remarks>Superfluous calls to <paramref name="valueFactory"/> may occur in case of read races. <see cref="IDisposable.Dispose"/> is called on superfluously created objects if implemented.</remarks>
        public static async Task<TValue> GetOrAddAsync<TKey, TValue>([InstantHandle] this IDictionary<TKey, TValue> dictionary, TKey key, Func<Task<TValue>> valueFactory)
            where TKey : notnull
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
#endif

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
        /// Determines whether two dictionaries contain the same key-value pairs.
        /// </summary>
        /// <param name="first">The first of the two dictionaries to compare.</param>
        /// <param name="second">The first of the two dictionaries to compare.</param>
        /// <param name="valueComparer">Controls how to compare values; leave <c>null</c> for default comparer.</param>
        [Pure]
        public static bool UnsequencedEquals<TKey, TValue>([InstantHandle] this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second, IEqualityComparer<TValue>? valueComparer = null)
            where TKey : notnull
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            #endregion

            if (first == second) return true;
            if (first.Count != second.Count) return false;

            var keySet = new HashSet<TKey>(first.Keys);
            if (!second.Keys.All(keySet.Contains)) return false;

            valueComparer ??= EqualityComparer<TValue>.Default;
            foreach (var (key, value) in first)
            {
                if (!valueComparer.Equals(value, second[key]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Generates a hash code for the contents of the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to generate the hash for.</param>
        /// <param name="valueComparer">Controls how to compare values; leave <c>null</c> for default comparer.</param>
        /// <seealso cref="UnsequencedEquals{TKey,TValue}"/>
        [Pure]
        public static int GetUnsequencedHashCode<TKey, TValue>([InstantHandle] this IDictionary<TKey, TValue> dictionary, IEqualityComparer<TValue>? valueComparer = null)
            where TKey : notnull
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            #endregion

            var hash = new HashCode();
            foreach (var (key, value) in dictionary)
            {
                hash.Add(key);
                hash.Add(value, valueComparer);
            }
            return hash.ToHashCode();
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
    }
}
