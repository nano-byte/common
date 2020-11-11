// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using NanoByte.Common.Values;

#if !NET20 && !NET40
using System.Threading;
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
        /// Determines whether the enumeration contains an element or is null.
        /// </summary>
        /// <param name="enumeration">The list to check.</param>
        /// <param name="element">The element to look for.</param>
        /// <remarks>Useful for lists that contain an OR-ed list of restrictions, where an empty list means no restrictions.</remarks>
        public static bool ContainsOrEmpty<T>(this IEnumerable<T> enumeration, T element)
        {
            #region Sanity checks
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));
            if (element == null) throw new ArgumentNullException(nameof(element));
            #endregion

            // ReSharper disable PossibleMultipleEnumeration
            return enumeration.Contains(element) || !enumeration.Any();
            // ReSharper restore PossibleMultipleEnumeration
        }

        /// <summary>
        /// Determines whether one enumeration of elements contains any of the elements in another.
        /// </summary>
        /// <param name="first">The first of the two enumerations to compare.</param>
        /// <param name="second">The first of the two enumerations to compare.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        /// <returns><c>true</c> if <paramref name="first"/> contains any element from <paramref name="second"/>. <c>false</c> if <paramref name="first"/> or <paramref name="second"/> is empty.</returns>
        [Pure]
        public static bool ContainsAny<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T>? comparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            #endregion

            var set =
#if !NET20
                second as ISet<T> ??
#endif
                new HashSet<T>(first, comparer ?? EqualityComparer<T>.Default);
            return second.Any(set.Contains);
        }

        /// <summary>
        /// Determines whether two enumerations contain the same elements in the same order.
        /// </summary>
        /// <param name="first">The first of the two enumerations to compare.</param>
        /// <param name="second">The first of the two enumerations to compare.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        [Pure]
        public static bool SequencedEquals<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T>? comparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            #endregion

            if (ReferenceEquals(first, second)) return true;
            if (first is ICollection<T> a && second is ICollection<T> b && a.Count != b.Count) return false;
            return first.SequenceEqual(second, comparer ?? EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Determines whether two enumerations contain the same elements disregarding the order they are in.
        /// </summary>
        /// <param name="first">The first of the two enumerations to compare.</param>
        /// <param name="second">The first of the two enumerations to compare.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        [Pure]
        public static bool UnsequencedEquals<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T>? comparer = null)
        {
            #region Sanity checks
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            #endregion

            if (ReferenceEquals(first, second)) return true;
            if (first is ICollection<T> a && second is ICollection<T> b && a.Count != b.Count) return false;
            var set = new HashSet<T>(first, comparer ?? EqualityComparer<T>.Default);
            return second.All(set.Contains);
        }

        /// <summary>
        /// Generates a hash code for the contents of the enumeration. Changing the elements' order will change the hash.
        /// </summary>
        /// <param name="enumeration">The enumeration to generate the hash for.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        /// <seealso cref="SequencedEquals{T}"/>
        [Pure]
        public static int GetSequencedHashCode<T>(this IEnumerable<T> enumeration, IEqualityComparer<T>? comparer = null)
        {
            #region Sanity checks
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));
            #endregion

            var hash = new HashCode();
            foreach (T item in enumeration)
                hash.Add(item, comparer);
            return hash.ToHashCode();
        }

        /// <summary>
        /// Generates a hash code for the contents of the enumeration. Changing the elements' order will not change the hash.
        /// </summary>
        /// <param name="enumeration">The enumeration to generate the hash for.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        /// <seealso cref="UnsequencedEquals{T}"/>
        [Pure]
        public static int GetUnsequencedHashCode<T>(this IEnumerable<T> enumeration, IEqualityComparer<T>? comparer = null)
        {
            #region Sanity checks
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));
            #endregion

            comparer ??= EqualityComparer<T>.Default;
            unchecked
            {
                int result = 397;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (T item in enumeration)
                {
                    if (item != null)
                        result ^= comparer.GetHashCode(item);
                }
                return result;
            }
        }

        /// <summary>
        /// Filters a sequence of elements to remove any that match the <paramref name="predicate"/>.
        /// The opposite of <see cref="Enumerable.Where{TSource}(IEnumerable{TSource},Func{TSource,bool})"/>.
        /// </summary>
        [Pure]
        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumeration, Func<T, bool> predicate)
            => enumeration.Where(x => !predicate(x));

        /// <summary>
        /// Filters a sequence of elements to remove any that are equal to <paramref name="element"/>.
        /// </summary>
        [Pure]
        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumeration, T element)
            => enumeration.Except(new[] {element});

        /// <summary>
        /// Flattens a list of lists.
        /// </summary>
        [Pure]
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumeration)
            => enumeration.SelectMany(x => x);

        /// <summary>
        /// Appends an element to a list.
        /// </summary>
        [Pure]
        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumeration, T element)
            => enumeration.Concat(new[] {element});

        /// <summary>
        /// Prepends an element to a list.
        /// </summary>
        [Pure]
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> enumeration, T element)
            => new[] {element}.Concat(enumeration);

        /// <summary>
        /// Filters a sequence of elements to remove any <c>null</c> values.
        /// </summary>
        [Pure]
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumeration)
            where T : class
        {
            foreach (var element in enumeration)
            {
                if (element != null)
                    yield return element;
            }
        }

        /// <summary>
        /// Determines the element in a list that maximizes a specified expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <typeparam name="TValue">The type of the <paramref name="expression"/>.</typeparam>
        /// <param name="enumeration">The elements to check.</param>
        /// <param name="expression">The expression to maximize.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        /// <returns>The element that maximizes the expression.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="enumeration"/> contains no elements.</exception>
        [Pure]
        public static T MaxBy<T, TValue>(this IEnumerable<T> enumeration, Func<T, TValue> expression, IComparer<TValue>? comparer = null)
        {
            #region Sanity checks
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            #endregion

            comparer ??= Comparer<TValue>.Default;

            using var enumerator = enumeration.GetEnumerator();
            if (!enumerator.MoveNext()) throw new InvalidOperationException("Enumeration contains no elements");
            var maxElement = enumerator.Current;
            var maxValue = expression(maxElement);

            while (enumerator.MoveNext())
            {
                var candidate = enumerator.Current;
                var candidateValue = expression(candidate);
                if (comparer.Compare(candidateValue, maxValue) > 0)
                {
                    maxElement = candidate;
                    maxValue = candidateValue;
                }
            }

            return maxElement;
        }

        /// <summary>
        /// Determines the element in a list that minimizes a specified expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <typeparam name="TValue">The type of the <paramref name="expression"/>.</typeparam>
        /// <param name="enumeration">The elements to check.</param>
        /// <param name="expression">The expression to minimize.</param>
        /// <param name="comparer">Controls how to compare elements; leave <c>null</c> for default comparer.</param>
        /// <returns>The element that minimizes the expression.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="enumeration"/> contains no elements.</exception>
        [Pure]
        public static T MinBy<T, TValue>(this IEnumerable<T> enumeration, Func<T, TValue> expression, IComparer<TValue>? comparer = null)
        {
            #region Sanity checks
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            #endregion

            comparer ??= Comparer<TValue>.Default;

            using var enumerator = enumeration.GetEnumerator();
            if (!enumerator.MoveNext()) throw new InvalidOperationException("Enumeration contains no elements");
            var minElement = enumerator.Current;
            var minValue = expression(minElement);

            while (enumerator.MoveNext())
            {
                var candidate = enumerator.Current;
                var candidateValue = expression(candidate);
                if (comparer.Compare(candidateValue, minValue) < 0)
                {
                    minElement = candidate;
                    minValue = candidateValue;
                }
            }

            return minElement;
        }

        /// <summary>
        /// Filters a sequence of elements to remove any duplicates based on the equality of a key extracted from the elements.
        /// </summary>
        /// <param name="enumeration">The sequence of elements to filter.</param>
        /// <param name="keySelector">A function mapping elements to their respective equality keys.</param>
        [Pure]
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> enumeration, Func<T, TKey> keySelector)
            where T : notnull
            where TKey : notnull
            => enumeration.Distinct(new KeyEqualityComparer<T, TKey>(keySelector));

        /// <summary>
        /// Maps elements like <see cref="Enumerable.Select{TSource,TResult}(IEnumerable{TSource},Func{TSource,TResult})"/>, but with exception handling.
        /// </summary>
        /// <typeparam name="TSource">The type of the input elements.</typeparam>
        /// <typeparam name="TResult">The type of the output elements.</typeparam>
        /// <typeparam name="TException">The type of exceptions to ignore. Any other exceptions are passed through.</typeparam>
        /// <param name="source">The elements to map.</param>
        /// <param name="selector">The selector to execute for each <paramref name="source"/> element. When it throws <typeparamref name="TException"/> the element is skipped. Any other exceptions are passed through.</param>
        public static IEnumerable<TResult> TrySelect<TSource, TResult, TException>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
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
        /// Calls <see cref="ICloneable{T}.Clone"/> for every element in a enumeration and returns the results as a new enumeration.
        /// </summary>
        [Pure]
        public static IEnumerable<T> CloneElements<T>(this IEnumerable<T> enumerable) where T : ICloneable<T>
            => (enumerable ?? throw new ArgumentNullException(nameof(enumerable))).Select(x => x.Clone());

        /// <summary>
        /// Performs a topological sort of an object graph.
        /// </summary>
        /// <param name="nodes">The set of nodes to sort.</param>
        /// <param name="getDependencies">A function that retrieves all dependencies of a node.</param>
        /// <exception cref="InvalidDataException">Cyclic dependency found.</exception>
        public static IEnumerable<T> TopologicalSort<T>(this IEnumerable<T> nodes, Func<T, IEnumerable<T>> getDependencies)
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

#if !NET20 && !NET40
        /// <summary>
        /// Runs asynchronous operations for each element in an enumeration. Runs multiple tasks using cooperative multitasking.
        /// </summary>
        /// <param name="enumerable">The input elements to enumerate over.</param>
        /// <param name="taskFactory">Creates a <see cref="Task"/> for each input element.</param>
        /// <param name="maxParallel">The maximum number of <see cref="Task"/>s to run in parallel. Use 0 or lower for unbounded.</param>
        /// <exception cref="InvalidOperationException"><see cref="TaskScheduler.Current"/> is equal to <see cref="TaskScheduler.Default"/>.</exception>
        /// <remarks>
        /// <see cref="SynchronizationContext.Current"/> must not be null.
        /// The synchronization context is required to ensure that task continuations are scheduled sequentially and do not run in parallel.
        /// </remarks>
        public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> taskFactory, int maxParallel = 0)
        {
            if (TaskScheduler.Current == TaskScheduler.Default)
                throw new InvalidOperationException("TaskScheduler.Current must not be equal to TaskScheduler.Default value when using ForEachAsync().");

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

        /// <summary>
        /// Generates all possible permutations of a set of <paramref name="elements"/>.
        /// </summary>
        [Pure]
        public static IEnumerable<T[]> Permutate<T>(this IEnumerable<T> elements)
        {
            #region Sanity checks
            if (elements == null) throw new ArgumentNullException(nameof(elements));
            #endregion

            IEnumerable<T[]> Helper(T[] array, int index)
            {
                if (index >= array.Length - 1)
                    yield return array;
                else
                {
                    for (int i = index; i < array.Length; i++)
                    {
                        var subArray = array.ToArray();
                        var t1 = subArray[index];
                        subArray[index] = subArray[i];
                        subArray[i] = t1;

                        foreach (var element in Helper(subArray, index + 1))
                            yield return element;
                    }
                }
            }

            return Helper(elements.ToArray(), index: 0);
        }
    }
}
