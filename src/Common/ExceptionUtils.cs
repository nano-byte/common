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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;
using JetBrains.Annotations;
using NanoByte.Common.Properties;

namespace NanoByte.Common
{
    /// <summary>
    /// Delegate used by <seealso cref="ExceptionUtils.Retry{TException}"/>.
    /// </summary>
    /// <param name="lastAttempt">Indicates whether this retry run is the last attempt before giving up and passing the exception through.</param>
    /// <seealso cref="ExceptionUtils.Retry{TException}"/>
    public delegate void RetryAction(bool lastAttempt);

    /// <summary>
    /// Provides helper methods related to <see cref="Exception"/>s.
    /// </summary>
    public static class ExceptionUtils
    {
        /// <summary>
        /// Throws a previously thrown exception again, preserving the existing stack trace if possible.
        /// </summary>
        public static void Rethrow([NotNull] this Exception exception)
        {
            #region Sanity checks
            if (exception == null) throw new ArgumentNullException("exception");
            #endregion

            var serializationInfo = new SerializationInfo(exception.GetType(), new FormatterConverter());
            var streamingContext = new StreamingContext(StreamingContextStates.CrossAppDomain);
            exception.GetObjectData(serializationInfo, streamingContext);

            try
            {
                var objectManager = new ObjectManager(null, streamingContext);
                objectManager.RegisterObject(exception, 1, serializationInfo);
                objectManager.DoFixups();
            }
                // Ignore if preserving stack trace is not possible
            catch (SecurityException)
            {}
            catch (SerializationException)
            {}

            throw exception;
        }

        /// <summary>
        /// Applies an operation for all elements of a collection. Automatically applies rollback operations in case of an exception.
        /// </summary>
        /// <typeparam name="T">The type of elements to operate on.</typeparam>
        /// <param name="elements">The elements to apply the action for.</param>
        /// <param name="apply">The action to apply to each element.</param>
        /// <param name="rollback">The action to apply to each element that <paramref name="apply"/> was called on in case of an exception.</param>
        /// <remarks>
        /// <paramref name="rollback"/> is applied to the element that raised an exception in <paramref name="apply"/> and then interating backwards through all previous elements.
        /// After rollback is complete the exception is passed on.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Suppress exceptions during rollback since they would hide the actual exception that caused the rollback in the first place")]
        public static void ApplyWithRollback<T>([NotNull, InstantHandle] this IEnumerable<T> elements, [NotNull, InstantHandle] Action<T> apply, [NotNull, InstantHandle] Action<T> rollback)
        {
            #region Sanity checks
            if (elements == null) throw new ArgumentNullException("elements");
            if (apply == null) throw new ArgumentNullException("apply");
            if (rollback == null) throw new ArgumentNullException("rollback");
            #endregion

            var rollbackJournal = new LinkedList<T>();
            try
            {
                foreach (var element in elements)
                {
                    // Remember the element for potential rollback
                    rollbackJournal.AddFirst(element);

                    apply(element);
                }
            }
            catch
            {
                foreach (var element in rollbackJournal)
                {
                    try
                    {
                        rollback(element);
                    }
                    catch (Exception ex)
                    {
                        // Suppress exceptions during rollback since they would hide the actual exception that caused the rollback in the first place
                        Log.Error(string.Format(Resources.FailedToRollback, element));
                        Log.Error(ex);
                    }
                }

                throw;
            }
        }

        /// <summary>
        /// Applies an operation for the first possible element of a collection.
        /// If the operation succeeds the remaining elements are ignored. If the operation fails it is repeated for the next element.
        /// </summary>
        /// <typeparam name="T">The type of elements to operate on.</typeparam>
        /// <param name="elements">The elements to apply the action for.</param>
        /// <param name="action">The action to apply to an element.</param>
        /// <exception cref="Exception">The exception thrown by <paramref name="action"/> for the last element of <paramref name="elements"/>.</exception>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Last excption is rethrown, other exceptions are logged")]
        public static void TryAny<T>([NotNull, InstantHandle] this IEnumerable<T> elements, [NotNull, InstantHandle] Action<T> action)
        {
            #region Sanity checks
            if (elements == null) throw new ArgumentNullException("elements");
            if (action == null) throw new ArgumentNullException("action");
            #endregion

            var enumerator = elements.GetEnumerator();
            if (!enumerator.MoveNext()) return;

            while (true)
            {
                try
                {
                    action(enumerator.Current);
                    return;
                }
                catch (Exception ex)
                {
                    if (enumerator.MoveNext()) Log.Error(ex); // Log exception and try next element
                    else throw; // Rethrow exception if there are no more elements
                }
            }
        }

        /// <summary>
        /// Executes a delegate and automatically retries it using exponential backoff if a specifc type of exception was raised.
        /// </summary>
        /// <typeparam name="TException">The type of exception to triger a retry.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <param name="maxRetries">The maximum number of retries to attempt.</param>
        public static void Retry<TException>([NotNull, InstantHandle] RetryAction action, int maxRetries = 2)
            where TException : Exception
        {
            #region Sanity checks
            if (action == null) throw new ArgumentNullException("action");
            #endregion

            int retryCounter = 0;
            Retry:
            if (retryCounter >= maxRetries)
                action(lastAttempt: true);
            else
            {
                try
                {
                    action(lastAttempt: false);
                }
                catch (TException ex)
                {
                    Log.Info(ex);

                    Random random;
                    try
                    {
                        // Use process ID as a seed to ensure we get different values than other competing processes on the same machine
                        random = new Random(Process.GetCurrentProcess().Id);
                    }
                    catch (Exception)
                    {
                        random = new Random();
                    }

                    int delay = random.Next(50, 1000 * (1 << retryCounter));
                    Log.Info("Retrying in " + delay + " milliseconds");
                    Thread.Sleep(delay);

                    retryCounter++;
                    goto Retry;
                }
            }
        }
    }
}
