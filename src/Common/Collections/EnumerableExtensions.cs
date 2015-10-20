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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using NanoByte.Common.Values;

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Provides extension methods for <see cref="IEnumerable{T}"/>s.
    /// </summary>
    public static class EnumerableExtensions
    {
        #region LINQ
        /// <summary>
        /// Determines whether a sequence of elements contains any of the specified <paramref name="targets"/>.
        /// </summary>
        /// <returns><see langword="true"/> if <paramref name="enumeration"/> contains any element from <paramref name="targets"/>. <see langword="false"/> if <paramref name="enumeration"/> or <paramref name="targets"/> is empty.</returns>
        [Pure]
        public static bool ContainsAny<T>([NotNull, InstantHandle] this IEnumerable<T> enumeration, [NotNull, InstantHandle] IEnumerable<T> targets)
        {
            var set = new HashSet<T>(enumeration);
            return targets.Any(set.Contains);
        }

        /// <summary>
        /// Filters a sequence of elements to remove any that match the <paramref name="predicate"/>.
        /// The opposite of <see cref="Enumerable.Where{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>.
        /// </summary>
        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<T> Except<T>([NotNull] this IEnumerable<T> enumeration, [NotNull] Func<T, bool> predicate)
        {
            return enumeration.Where(x => !predicate(x));
        }

        /// <summary>
        /// Filters a sequence of elements to remove any that are equal to <paramref name="element"/>.
        /// </summary>
        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<T> Except<T>([NotNull] this IEnumerable<T> enumeration, T element)
        {
            return enumeration.Except(new[] {element});
        }

        /// <summary>
        /// Flattens a list of lists.
        /// </summary>
        [NotNull, Pure]
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static IEnumerable<T> Flatten<T>([NotNull] this IEnumerable<IEnumerable<T>> enumeration)
        {
            return enumeration.SelectMany(x => x);
        }

        /// <summary>
        /// Appends an element to a list.
        /// </summary>
        [NotNull, Pure]
        public static IEnumerable<T> Append<T>([NotNull] this IEnumerable<T> enumeration, T element)
        {
            return enumeration.Concat(new[] {element});
        }

        /// <summary>
        /// Prepends an element to a list.
        /// </summary>
        [NotNull, Pure]
        public static IEnumerable<T> Prepend<T>([NotNull] this IEnumerable<T> enumeration, T element)
        {
            return new[] {element}.Concat(enumeration);
        }

        /// <summary>
        /// Filters a sequence of elements to remove any <see langword="null"/> values.
        /// </summary>
        [NotNull, ItemNotNull, Pure, LinqTunnel]
        public static IEnumerable<T> WhereNotNull<T>([NotNull, ItemCanBeNull] this IEnumerable<T> enumeration)
        {
            return enumeration.Where(element => element != null);
        }

        /// <summary>
        /// Determines the element in a list that maximizes a specified expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <typeparam name="TValue">The type of the <paramref name="expression"/>.</typeparam>
        /// <param name="enumeration">The elements to check.</param>
        /// <param name="expression">The expression to maximize.</param>
        /// <returns>The element that maximizes the expression; the default value of <typeparamref name="T"/> if <paramref name="enumeration"/> contains no elements.</returns>
        [CanBeNull, Pure]
        public static T MaxBy<T, TValue>([NotNull, ItemNotNull, InstantHandle] this IEnumerable<T> enumeration, [NotNull, InstantHandle] Func<T, TValue> expression)
        {
            return enumeration.MaxBy(expression, Comparer<TValue>.Default);
        }

        /// <summary>
        /// Determines the element in a list that maximizes a specified expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <typeparam name="TValue">The type of the <paramref name="expression"/>.</typeparam>
        /// <param name="enumeration">The elements to check.</param>
        /// <param name="expression">The expression to maximize.</param>
        /// <param name="comparer">A comprarer used to compare values of <paramref name="expression"/>.</param>
        /// <returns>The element that maximizes the expression; the default value of <typeparamref name="T"/> if <paramref name="enumeration"/> contains no elements.</returns>
        [CanBeNull, Pure]
        public static T MaxBy<T, TValue>([NotNull, ItemNotNull, InstantHandle] this IEnumerable<T> enumeration, [NotNull, InstantHandle] Func<T, TValue> expression, [NotNull] IComparer<TValue> comparer)
        {
            #region Sanity checks
            if (enumeration == null) throw new ArgumentNullException("enumeration");
            if (expression == null) throw new ArgumentNullException("expression");
            if (comparer == null) throw new ArgumentNullException("comparer");
            #endregion

            using (var enumerator = enumeration.GetEnumerator())
            {
                if (!enumerator.MoveNext()) return default(T);
                T maxElement = enumerator.Current;
                TValue maxValue = expression(maxElement);

                while (enumerator.MoveNext())
                {
                    T candidate = enumerator.Current;
                    TValue candidateValue = expression(candidate);
                    if (comparer.Compare(candidateValue, maxValue) > 0)
                    {
                        maxElement = candidate;
                        maxValue = candidateValue;
                    }
                }

                return maxElement;
            }
        }

        /// <summary>
        /// Determines the element in a list that minimizes a specified expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <typeparam name="TValue">The type of the <paramref name="expression"/>.</typeparam>
        /// <param name="enumeration">The elements to check.</param>
        /// <param name="expression">The expression to minimize.</param>
        /// <returns>The element that minimizes the expression; the default value of <typeparamref name="T"/> if <paramref name="enumeration"/> contains no elements.</returns>
        [CanBeNull, Pure]
        public static T MinBy<T, TValue>([NotNull, ItemNotNull, InstantHandle] this IEnumerable<T> enumeration, [NotNull, InstantHandle] Func<T, TValue> expression)
        {
            return enumeration.MinBy(expression, Comparer<TValue>.Default);
        }

        /// <summary>
        /// Determines the element in a list that minimizes a specified expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <typeparam name="TValue">The type of the <paramref name="expression"/>.</typeparam>
        /// <param name="enumeration">The elements to check.</param>
        /// <param name="expression">The expression to minimize.</param>
        /// <param name="comparer">A comprarer used to compare values of <paramref name="expression"/>.</param>
        /// <returns>The element that minimizes the expression; the default value of <typeparamref name="T"/> if <paramref name="enumeration"/> contains no elements.</returns>
        [CanBeNull, Pure,]
        public static T MinBy<T, TValue>([NotNull, ItemNotNull, InstantHandle] this IEnumerable<T> enumeration, [NotNull, InstantHandle] Func<T, TValue> expression, [NotNull] IComparer<TValue> comparer)
        {
            #region Sanity checks
            if (enumeration == null) throw new ArgumentNullException("enumeration");
            if (expression == null) throw new ArgumentNullException("expression");
            if (comparer == null) throw new ArgumentNullException("comparer");
            #endregion

            using (var enumerator = enumeration.GetEnumerator())
            {
                if (!enumerator.MoveNext()) return default(T);
                T minElement = enumerator.Current;
                TValue minValue = expression(minElement);

                while (enumerator.MoveNext())
                {
                    T candidate = enumerator.Current;
                    TValue candidateValue = expression(candidate);
                    if (comparer.Compare(candidateValue, minValue) < 0)
                    {
                        minElement = candidate;
                        minValue = candidateValue;
                    }
                }

                return minElement;
            }
        }

        /// <summary>
        /// Filters a sequence of elements to remove any duplicates based on the equality of a key extracted from the elements.
        /// </summary>
        /// <param name="enumeration">The sequence of elements to filter.</param>
        /// <param name="keySelector">A function mapping elements to their respective equality keys.</param>
        [NotNull, Pure, LinqTunnel]
        public static IEnumerable<T> DistinctBy<T, TKey>([NotNull] this IEnumerable<T> enumeration, [NotNull] Func<T, TKey> keySelector)
        {
            return enumeration.Distinct(new KeyEqualityComparer<T, TKey>(keySelector));
        }

        /// <summary>
        /// Maps elements like <see cref="Enumerable.Select{TSource,TResult}(IEnumerable{TSource},Func{TSource,TResult})"/>, but with exception handling.
        /// </summary>
        /// <typeparam name="TSource">The type of the input elements.</typeparam>
        /// <typeparam name="TResult">The type of the output elements.</typeparam>
        /// <typeparam name="TException">The type of exceptions to ignore. Any other exceptions are passed through.</typeparam>
        /// <param name="source">The elements to map.</param>
        /// <param name="selector">The selector to execute for each <paramref name="source"/> element. When it throws <typeparamref name="TException"/> the element is skipped. Any other exceptions are passed through.</param>
        [NotNull, LinqTunnel]
        public static IEnumerable<TResult> TrySelect<TSource, TResult, TException>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource, TResult> selector)
            where TException : Exception
        {
            #region Sanity checks
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            #endregion

            foreach (var element in source)
            {
                TResult result;
                try
                {
                    result = selector(element);
                }
                catch (TException)
                {
                    continue;
                }
                yield return result;
            }
        }
        #endregion

        #region Equality
        /// <summary>
        /// Determines whether two collections contain the same elements in the same order.
        /// </summary>
        /// <param name="first">The first of the two collections to compare.</param>
        /// <param name="second">The first of the two collections to compare.</param>
        /// <param name="comparer">Controls how to compare elements; leave <see langword="null"/> for default comparer.</param>
        [Pure]
        public static bool SequencedEquals<T>([NotNull, InstantHandle] this ICollection<T> first, [NotNull, InstantHandle] ICollection<T> second, [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            #endregion

            if (first.Count != second.Count) return false;
            if (comparer == null) comparer = EqualityComparer<T>.Default;

            return first.SequenceEqual(second, comparer);
        }

        /// <summary>
        /// Determines whether two collections contain the same elements disregarding the order they are in.
        /// </summary>
        /// <param name="first">The first of the two collections to compare.</param>
        /// <param name="second">The first of the two collections to compare.</param>
        /// <param name="comparer">Controls how to compare elements; leave <see langword="null"/> for default comparer.</param>
        [Pure]
        public static bool UnsequencedEquals<T>([NotNull, InstantHandle] this ICollection<T> first, [NotNull, InstantHandle] ICollection<T> second, [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            #endregion

            if (first.Count != second.Count) return false;
            if (comparer == null) comparer = EqualityComparer<T>.Default;

            if (first.GetUnsequencedHashCode(comparer) != second.GetUnsequencedHashCode(comparer)) return false;
            return first.All(x => second.Contains(x, comparer));
        }

        /// <summary>
        /// Generates a hash code for the contents of a collection. Changing the elements' order will change the hash.
        /// </summary>
        /// <param name="collection">The collection to generate the hash for.</param>
        /// <param name="comparer">Controls how to compare elements; leave <see langword="null"/> for default comparer.</param>
        /// <seealso cref="SequencedEquals{T}(ICollection{T},ICollection{T},IEqualityComparer{T})"/>
        [Pure]
        public static int GetSequencedHashCode<T>([NotNull, InstantHandle] this IEnumerable<T> collection, [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException("collection");
            #endregion

            if (comparer == null) comparer = EqualityComparer<T>.Default;

            unchecked
            {
                int result = 397;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (T unknown in collection.WhereNotNull())
                    result = (result * 397) ^ comparer.GetHashCode(unknown);
                return result;
            }
        }

        /// <summary>
        /// Generates a hash code for the contents of a collection. Changing the elements' order will not change the hash.
        /// </summary>
        /// <param name="collection">The collection to generate the hash for.</param>
        /// <param name="comparer">Controls how to compare elements; leave <see langword="null"/> for default comparer.</param>
        /// <seealso cref="UnsequencedEquals{T}"/>
        [Pure]
        public static int GetUnsequencedHashCode<T>([NotNull, InstantHandle] this IEnumerable<T> collection, [CanBeNull, InstantHandle] IEqualityComparer<T> comparer = null)
        {
            #region Sanity checks
            if (collection == null) throw new ArgumentNullException("collection");
            #endregion

            if (comparer == null) comparer = EqualityComparer<T>.Default;

            unchecked
            {
                int result = 397;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (T unknown in collection.WhereNotNull())
                    result = result ^ comparer.GetHashCode(unknown);
                return result;
            }
        }
        #endregion

        #region Clone
        /// <summary>
        /// Calls <see cref="ICloneable.Clone"/> for every element in a collection and returns the results as a new collection.
        /// </summary>
        [NotNull, Pure]
        public static IEnumerable<T> CloneElements<T>([NotNull, ItemNotNull] this IEnumerable<T> enumerable) where T : ICloneable
        {
            return enumerable.Select(entry => (T)entry.Clone());
        }
        #endregion
    }
}
