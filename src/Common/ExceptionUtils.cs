// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET20 && !NET40
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
#else
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
#endif

#if NETFRAMEWORK
using System.Diagnostics;
#endif

namespace NanoByte.Common;

/// <summary>
/// Provides helper methods related to <see cref="Exception"/>s.
/// </summary>
public static class ExceptionUtils
{
    /// <summary>
    /// Recursively follows the <see cref="Exception.InnerException"/>s and combines all their <see cref="Exception.Message"/>s, removing duplicates.
    /// </summary>
    [Pure]
    public static string GetMessageWithInner(this Exception exception)
    {
        IEnumerable<string> Messages()
        {
            var ex = exception;
            do
            {
                yield return ex.Message;
                ex = ex.InnerException;
            } while (ex != null);
        }

        return StringUtils.Join(Environment.NewLine, Messages().Distinct());
    }

    /// <summary>
    /// Rethrows an <paramref name="exception"/> while preserving its original stack trace.
    /// </summary>
    /// <returns>This method never returns. You can "throw" the return value to satisfy the compiler's flow analysis if necessary.</returns>
    [DoesNotReturn]
    public static Exception Rethrow(this Exception exception)
    {
        #region Sanity checks
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        #endregion

#if !NET20 && !NET40
        ExceptionDispatchInfo.Capture(exception).Throw();
        return exception;
#else
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
        catch (Exception ex) when (ex is SecurityException or SerializationException or TargetInvocationException)
        {}

        throw exception;
#endif
    }

#if !NET20
    /// <summary>
    /// Rethrows the first of the <see cref="AggregateException.InnerExceptions"/> and logs all others.
    /// </summary>
    /// <returns>This method never returns. You can "throw" the return value to satisfy the compiler's flow analysis if necessary.</returns>
    [DoesNotReturn]
    public static Exception RethrowFirstInner(this AggregateException exception)
    {
        #region Sanity checks
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        #endregion

        var first = exception.InnerExceptions.FirstOrDefault() ?? exception;

        foreach (var other in exception.InnerExceptions.Skip(1))
        {
            if (other is OperationCanceledException) {}
            else if (other.GetType() == first.GetType()) Log.Info("Suppressed additional inner exception of same type", other);
            else Log.Error(other.Message, other);
        }

        throw first.Rethrow();
    }
#endif

    /// <summary>
    /// Applies an operation for all elements of a collection. Automatically applies rollback operations in case of an exception.
    /// </summary>
    /// <typeparam name="T">The type of elements to operate on.</typeparam>
    /// <param name="elements">The elements to apply the action for.</param>
    /// <param name="apply">The action to apply to each element.</param>
    /// <param name="rollback">The action to apply to each element that <paramref name="apply"/> was called on in case of an exception.</param>
    /// <remarks>
    /// <paramref name="rollback"/> is applied to the element that raised an exception in <paramref name="apply"/> and then iterating backwards through all previous elements.
    /// After rollback is complete the exception is passed on.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Suppress exceptions during rollback since they would hide the actual exception that caused the rollback in the first place")]
    public static void ApplyWithRollback<T>([InstantHandle] this IEnumerable<T> elements, [InstantHandle] Action<T> apply, [InstantHandle] Action<T> rollback)
    {
        #region Sanity checks
        if (elements == null) throw new ArgumentNullException(nameof(elements));
        if (apply == null) throw new ArgumentNullException(nameof(apply));
        if (rollback == null) throw new ArgumentNullException(nameof(rollback));
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
                    Log.Error(string.Format(Resources.FailedToRollback, element), ex);
                }
            }

            throw;
        }
    }

    /// <summary>
    /// Applies an operation for the first possible element of a collection.
    /// If the operation succeeds the remaining elements are ignored. If the operation fails it is repeated for the next element.
    /// If the operation fails with <see cref="OperationCanceledException"/>s no further elements are tried.
    /// </summary>
    /// <typeparam name="T">The type of elements to operate on.</typeparam>
    /// <param name="elements">The elements to apply the action for.</param>
    /// <param name="action">The action to apply to an element.</param>
    /// <exception cref="Exception">The exception thrown by <paramref name="action"/> for the last element of <paramref name="elements"/>.</exception>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Last exception is rethrown, other exceptions are logged")]
    public static void TryAny<T>([InstantHandle] this IEnumerable<T> elements, [InstantHandle] Action<T> action)
    {
        #region Sanity checks
        if (elements == null) throw new ArgumentNullException(nameof(elements));
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        using var enumerator = elements.GetEnumerator();
        if (!enumerator.MoveNext()) return;

        while (true)
        {
            try
            {
                action(enumerator.Current);
                return;
            }
            catch (Exception ex) when (ex is not OperationCanceledException && enumerator.MoveNext())
            {
                Log.Warn(ex);
            }
        }
    }

    /// <summary>
    /// Executes a delegate and automatically retries it using exponential back-off if a specific type of exception was raised.
    /// </summary>
    /// <typeparam name="TException">The type of exception to trigger a retry.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="maxRetries">The maximum number of retries to attempt.</param>
    public static void Retry<TException>([InstantHandle] Action action, int maxRetries = 2)
        where TException : Exception
    {
        #region Sanity checks
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        var random = GetRandom();
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                action();
                return;
            }
            catch (TException ex)
            {
                int delay = random.Next(50, 1000 * (1 << (i + 1)));
                Log.Info(string.Format(Resources.RetryDelay, delay), ex);
                Thread.Sleep(delay);
            }
        }

        action();
    }

    /// <summary>
    /// Executes a delegate and automatically retries it using exponential back-off if a specific type of exception was raised.
    /// </summary>
    /// <typeparam name="TException">The type of exception to trigger a retry.</typeparam>
    /// <typeparam name="TResult">The type of result the <paramref name="function"/> produces.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="maxRetries">The maximum number of retries to attempt.</param>
    /// <returns>The result of <paramref name="function"/>.</returns>
    public static TResult Retry<TException, TResult>([InstantHandle] Func<TResult> function, int maxRetries = 2)
        where TException : Exception
    {
        #region Sanity checks
        if (function == null) throw new ArgumentNullException(nameof(function));
        #endregion

        var random = GetRandom();
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                function();
            }
            catch (TException ex)
            {
                int delay = random.Next(50, 1000 * (1 << (i + 1)));
                Log.Info(string.Format(Resources.RetryDelay, delay), ex);
                Thread.Sleep(delay);
            }
        }

        return function();
    }

#if !NET20 && !NET40
    /// <summary>
    /// Applies an operation for all elements of a collection. Automatically applies rollback operations in case of an exception.
    /// </summary>
    /// <typeparam name="T">The type of elements to operate on.</typeparam>
    /// <param name="elements">The elements to apply the action for.</param>
    /// <param name="apply">The action to apply to each element.</param>
    /// <param name="rollback">The action to apply to each element that <paramref name="apply"/> was called on in case of an exception.</param>
    /// <remarks>
    /// <paramref name="rollback"/> is applied to the element that raised an exception in <paramref name="apply"/> and then iterating backwards through all previous elements.
    /// After rollback is complete the exception is passed on.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Suppress exceptions during rollback since they would hide the actual exception that caused the rollback in the first place")]
    public static async Task ApplyWithRollbackAsync<T>(this IEnumerable<T> elements, Func<T, Task> apply, Func<T, Task> rollback)
    {
        #region Sanity checks
        if (elements == null) throw new ArgumentNullException(nameof(elements));
        if (apply == null) throw new ArgumentNullException(nameof(apply));
        if (rollback == null) throw new ArgumentNullException(nameof(rollback));
        #endregion

        var rollbackJournal = new LinkedList<T>();
        try
        {
            foreach (var element in elements)
            {
                // Remember the element for potential rollback
                rollbackJournal.AddFirst(element);

                await apply(element).ConfigureAwait(true);
            }
        }
        catch
        {
            foreach (var element in rollbackJournal)
            {
                try
                {
                    await rollback(element).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // Suppress exceptions during rollback since they would hide the actual exception that caused the rollback in the first place
                    Log.Error(string.Format(Resources.FailedToRollback, element), ex);
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
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Last exception is rethrown, other exceptions are logged")]
    public static async Task TryAnyAsync<T>(this IEnumerable<T> elements, Func<T, Task> action)
    {
        #region Sanity checks
        if (elements == null) throw new ArgumentNullException(nameof(elements));
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        using var enumerator = elements.GetEnumerator();
        if (!enumerator.MoveNext()) return;

        while (true)
        {
            try
            {
                await action(enumerator.Current).ConfigureAwait(false);
                return;
            }
            catch (Exception ex) when (enumerator.MoveNext())
            {
                Log.Error("Caught error and trying next", ex);
            }
        }
    }

    /// <summary>
    /// Executes an asynchronous delegate and automatically retries it using exponential back-off if a specific type of exception was raised.
    /// </summary>
    /// <typeparam name="TException">The type of exception to trigger a retry.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="maxRetries">The maximum number of retries to attempt.</param>
    public static async Task RetryAsync<TException>(Func<Task> action, int maxRetries = 2)
        where TException : Exception
    {
        #region Sanity checks
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        var random = GetRandom();
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await action();
                return;
            }
            catch (TException ex)
            {
                int delay = random.Next(50, 1000 * (1 << (i + 1)));
                Log.Info(string.Format(Resources.RetryDelay, delay), ex);
                await Task.Delay(delay);
            }
        }

        await action();
    }

    /// <summary>
    /// Executes an asynchronous and automatically retries it using exponential back-off if a specific type of exception was raised.
    /// </summary>
    /// <typeparam name="TException">The type of exception to trigger a retry.</typeparam>
    /// <typeparam name="TResult">The type of result the <paramref name="function"/> produces.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <param name="maxRetries">The maximum number of retries to attempt.</param>
    /// <returns>The result of <paramref name="function"/>.</returns>
    public static async Task<TResult> Retry<TException, TResult>([InstantHandle] Func<Task<TResult>> function, int maxRetries = 2)
        where TException : Exception
    {
        #region Sanity checks
        if (function == null) throw new ArgumentNullException(nameof(function));
        #endregion

        var random = GetRandom();
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                return await function();
            }
            catch (TException ex)
            {
                int delay = random.Next(50, 1000 * (1 << (i + 1)));
                Log.Info(string.Format(Resources.RetryDelay, delay), ex);
                await Task.Delay(delay);
            }
        }

        return await function();
    }
#endif

    /// <summary>
    /// Uses process ID as a random seed to ensure we get different values than other competing processes on the same machine.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Generic exception catch only used to ensure safe random seeding.")]
    private static Random GetRandom()
    {
        try
        {
#if NETFRAMEWORK
            return new Random(Process.GetCurrentProcess().Id);
#else
            return new Random(Environment.ProcessId);
#endif
        }
        catch (Exception)
        {
            return new Random();
        }
    }
}
