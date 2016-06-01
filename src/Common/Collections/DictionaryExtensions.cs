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

#if NET45
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
            if (target == null) throw new ArgumentNullException("target");
            if (source == null) throw new ArgumentNullException("source");
            #endregion

            foreach (var pair in source)
                target.Add(pair.Key, pair.Value);
        }

        /// <summary>
        /// Returns the element with the specified <paramref name="key"/> from the <paramref name="dictionary"/>.
        /// Creates, adds and returns a new value using <paramref name="valueFactory"/> if no match was found.
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] TKey key, [NotNull, InstantHandle] Func<TValue> valueFactory)
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            if (key == null) throw new ArgumentNullException("key");
            if (valueFactory == null) throw new ArgumentNullException("valueFactory");
            #endregion

            TValue value;
            if (!dictionary.TryGetValue(key, out value))
                dictionary.Add(key, value = valueFactory());
            return value;
        }

        /// <summary>
        /// Returns the element with the specified <paramref name="key"/> from the <paramref name="dictionary"/>.
        /// Creates, adds and returns a new <typeparamref name="TValue"/> instance if no match was found.
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] TKey key)
            where TValue : new()
        {
            #region Sanity checks
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            if (key == null) throw new ArgumentNullException("key");
            #endregion

            return dictionary.GetOrAdd(key, () => new TValue());
        }

#if NET45
        /// <summary>
        /// Returns the element with the specified <paramref name="key"/> from the <paramref name="dictionary"/>.
        /// Creates, adds and returns a new value using <paramref name="valueFactory"/> if no match was found.
        /// </summary>
        public static async Task<TValue> GetOrAddAsync<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<Task<TValue>> valueFactory)
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
                dictionary.Add(key, value = await valueFactory());
            return value;
        }
#endif
    }
}