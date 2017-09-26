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
using System.Linq;
using JetBrains.Annotations;

#if NET45 || NETSTANDARD2_0
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
        public static void AddRange<TSourceKey, TSourceValue, TTargetKey, TTargetValue>([NotNull] this IDictionary<TTargetKey, TTargetValue> target, [NotNull, InstantHandle] IEnumerable<KeyValuePair<TSourceKey, TSourceValue>> source)
            where TSourceKey : TTargetKey
            where TSourceValue : TTargetValue
        {
            #region Sanity checks
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (source == null) throw new ArgumentNullException(nameof(source));
            #endregion

            foreach (var pair in source)
                target.Add(pair.Key, pair.Value);
        }

        /// <summary>
        /// Returns an existing element with a specific key from a dictionary or the value type's default value if it is missing.
        /// </summary>
        /// <param name="dictionary">The dictionary to get an element from.</param>
        /// <param name="key">The key to look for in the <paramref name="dictionary"/>.</param>
        /// <returns>The existing element or the default value of <typeparamref name="TValue"/>.</returns>
        public static TValue GetOrDefault<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] TKey key)
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
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
        /// <remarks>No superflous calls to <paramref name="valueFactory"/> occur. Not thread-safe!</remarks>
        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] TKey key, [NotNull, InstantHandle] Func<TValue> valueFactory)
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
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
        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] TKey key)
            where TValue : new()
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (key == null) throw new ArgumentNullException(nameof(key));
            #endregion

            return dictionary.GetOrAdd(key, () => new TValue());
        }

#if NET45 || NETSTANDARD2_0
        /// <summary>
        /// Returns an existing element with a specific key from a dictionary or creates and adds a new element using a callback if it is missing.
        /// </summary>
        /// <param name="dictionary">The dictionary to get an element from or to add an element to.</param>
        /// <param name="key">The key to look for in the <paramref name="dictionary"/>.</param>
        /// <param name="valueFactory">A callback that provides a task that provides the value to add to the <paramref name="dictionary"/> if the <paramref name="key"/> is not found.</param>
        /// <returns>The existing element or the newly created element.</returns>
        /// <remarks>Superflous calls to <paramref name="valueFactory"/> may occur in case of read races. <see cref="IDisposable.Dispose"/> is called on superflously created objects if implemented.</remarks>
        public static async Task<TValue> GetOrAddAsync<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, TKey key, [NotNull] Func<Task<TValue>> valueFactory)
        {
            if (dictionary.TryGetValue(key, out var existingValue))
                return existingValue;

            var newValue = await valueFactory();

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
        /// Determines whether two dictionaries contain the same key-value pairs.
        /// </summary>
        /// <param name="first">The first of the two dictionaries to compare.</param>
        /// <param name="second">The first of the two dictionaries to compare.</param>
        /// <param name="valueComparer">Controls how to compare values; leave <c>null</c> for default comparer.</param>
        [Pure]
        public static bool UnsequencedEquals<TKey, TValue>([NotNull, InstantHandle] this IDictionary<TKey, TValue> first, [NotNull, InstantHandle] IDictionary<TKey, TValue> second, [CanBeNull] IEqualityComparer<TValue> valueComparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            #endregion

            if (first == second) return true;
            if (first.Count != second.Count) return false;

            var keySet = new HashSet<TKey>(first.Keys);
            if (!second.Keys.All(keySet.Contains)) return false;

            if (valueComparer == null) valueComparer = EqualityComparer<TValue>.Default;
            foreach (var pair in first)
            {
                if (!valueComparer.Equals(pair.Value, second[pair.Key]))
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
        public static int GetUnsequencedHashCode<TKey, TValue>([NotNull, InstantHandle] this IDictionary<TKey, TValue> dictionary, [CanBeNull] IEqualityComparer<TValue> valueComparer = null)
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            #endregion

            if (valueComparer == null) valueComparer = EqualityComparer<TValue>.Default;
            unchecked
            {
                int result = 397;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var pair in dictionary)
                    result = result ^ (pair.Key.GetHashCode() + valueComparer.GetHashCode(pair.Value));
                return result;
            }
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