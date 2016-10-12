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
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using NanoByte.Common.Values;

#if NET45
using System.Threading.Tasks;
#endif

namespace NanoByte.Common.Collections
{
    /// <summary>
    /// Provides extension methods for <see cref="IEnumerable{T}"/>s.
    /// </summary>
    public static class EnumerableExtensions
    {
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
        /// Filters a sequence of elements to remove any <c>null</c> values.
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
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        /// <returns>The element that maximizes the expression; the default value of <typeparamref name="T"/> if <paramref name="enumeration"/> contains no elements.</returns>
        [CanBeNull, Pure]
        public static T MaxBy<T, TValue>([NotNull, ItemNotNull, InstantHandle] this IEnumerable<T> enumeration, [NotNull, InstantHandle] Func<T, TValue> expression, [CanBeNull] IComparer<TValue> comparer = null)
        {
            #region Sanity checks
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            #endregion

            if (comparer == null) comparer = Comparer<TValue>.Default;

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
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        /// <returns>The element that minimizes the expression; the default value of <typeparamref name="T"/> if <paramref name="enumeration"/> contains no elements.</returns>
        [CanBeNull, Pure]
        public static T MinBy<T, TValue>([NotNull, ItemNotNull, InstantHandle] this IEnumerable<T> enumeration, [NotNull, InstantHandle] Func<T, TValue> expression, [CanBeNull] IComparer<TValue> comparer = null)
        {
            #region Sanity checks
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            #endregion

            if (comparer == null) comparer = Comparer<TValue>.Default;

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
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
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

        /// <summary>
        /// Calls <see cref="ICloneable.Clone"/> for every element in a collection and returns the results as a new collection.
        /// </summary>
        [NotNull, Pure]
        public static IEnumerable<T> CloneElements<T>([NotNull, ItemNotNull] this IEnumerable<T> enumerable) where T : ICloneable
        {
            #region Sanity checks
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            #endregion

            return enumerable.Select(entry => (T)entry.Clone());
        }

        /// <summary>
        /// Performs a topological sort of an object graph.
        /// </summary>
        /// <param name="nodes">The set of nodes to sort.</param>
        /// <param name="getDependencies">A function that retrieves all dependencies of a node.</param>
        /// <exception cref="InvalidDataException">Cyclic dependency found.</exception>
        public static IEnumerable<T> TopologicalSort<T>([NotNull] this IEnumerable<T> nodes, [NotNull, InstantHandle] Func<T, IEnumerable<T>> getDependencies)
        {
            #region Sanity checks
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));
            if (getDependencies == null) throw new ArgumentNullException(nameof(getDependencies));
            #endregion

            var sorted = new List<T>();
            var visited = new HashSet<T>();

            foreach (var item in nodes)
                TopologicalSortVisit(item, visited, sorted, getDependencies);

            return sorted;
        }

        private static void TopologicalSortVisit<T>(T node, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> getDependencies)
        {
            if (visited.Contains(node))
            {
                if (!sorted.Contains(node))
                    throw new InvalidDataException($"Cyclic dependency found at: {node}");
            }
            else
            {
                visited.Add(node);

                foreach (var dep in getDependencies(node))
                    TopologicalSortVisit(dep, visited, sorted, getDependencies);

                sorted.Add(node);
            }
        }

#if NET45
        /// <summary>
        /// Runs asynchronous operations for each element in an enumeration. Runs multiple tasks using cooperative multitasking.
        /// </summary>
        /// <param name="enumerable">The input elements to enumerate over.</param>
        /// <param name="taskFactory">Creates a <see cref="Task"/> for each input element.</param>
        /// <param name="maxParallel">The maximum number of <see cref="Task"/>s to run in parallel. Use 0 or lower for unbounded.</param>
        /// <exception cref="InvalidOperationException"><see cref="SynchronizationContext.Current"/> is <c>null</c>.</exception>
        /// <remarks>
        /// <see cref="SynchronizationContext.Current"/> must not be null.
        /// The synchronization context is required to ensure that task continuations are scheduled sequentially and do not run in parallel.
        /// </remarks>
        [NotNull]
        public static async Task ForEachAsync<T>([NotNull] this IEnumerable<T> enumerable, [NotNull] Func<T, Task> taskFactory, int maxParallel = 0)
        {
            if (SynchronizationContext.Current == null)
                throw new InvalidOperationException("SynchronizationContext.Current must not be null when using ForEachAsync().");

            var tasks = new List<Task>(maxParallel);

            foreach (var task in enumerable.Select(taskFactory))
            {
                tasks.Add(task);

                if (tasks.Count == maxParallel)
                {
                    var completedTask = await Task.WhenAny(tasks.ToArray());
                    await completedTask; // observe exceptions
                    tasks.Remove(completedTask);
                }
            }

            await Task.WhenAll(tasks.ToArray());
        }
#endif
    }
}
