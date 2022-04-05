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

#if !NET
using System.Diagnostics;
#endif

namespace NanoByte.Common;

/// <summary>
/// Delegate used by <see cref="ExceptionUtils.Retry{TException}"/>.
/// </summary>
/// <param name="lastAttempt">Indicates whether this retry run is the last attempt before giving up and passing the exception through.</param>
/// <seealso cref="ExceptionUtils.Retry{TException}"/>
public delegate void RetryAction(bool lastAttempt);

#if !NET20 && !NET40
/// <summary>
/// Delegate used by <see cref="ExceptionUtils.RetryAsync{TException}"/>.
/// </summary>
/// <param name="lastAttempt">Indicates whether this retry run is the last attempt before giving up and passing the exception through.</param>
/// <seealso cref="ExceptionUtils.Retry{TException}"/>
public delegate Task RetryAsyncAction(bool lastAttempt);
#endif

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
        catch (SecurityException)
        {}
        catch (SerializationException)
        {}
        catch (TargetInvocationException)
        {}

        throw exception;
#endif
    }

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
            catch (Exception ex)
            {
                if (enumerator.MoveNext()) Log.Error(ex); // Log exception and try next element
                else throw; // Rethrow exception if there are no more elements
            }
        }
    }

    /// <summary>
    /// Executes a delegate and automatically retries it using exponential back-off if a specific type of exception was raised.
    /// </summary>
    /// <typeparam name="TException">The type of exception to trigger a retry.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="maxRetries">The maximum number of retries to attempt.</param>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Generic exception catch only used to ensure safe random seeding.")]
    public static void Retry<TException>([InstantHandle] RetryAction action, int maxRetries = 2)
        where TException : Exception
    {
        #region Sanity checks
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        var random = GetRandom();
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

                int delay = random.Next(50, 1000 * (1 << retryCounter));
                Log.Info("Retrying in " + delay + " milliseconds");
                Thread.Sleep(delay);

                retryCounter++;
                goto Retry;
            }
        }
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
                    await rollback(element).ConfigureAwait(true);
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
                await action(enumerator.Current).ConfigureAwait(true);
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
    /// Executes an asynchronous delegate and automatically retries it using exponential back-off if a specific type of exception was raised.
    /// </summary>
    /// <typeparam name="TException">The type of exception to trigger a retry.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="maxRetries">The maximum number of retries to attempt.</param>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Generic exception catch only used to ensure safe random seeding.")]
    public static async Task RetryAsync<TException>(RetryAsyncAction action, int maxRetries = 2)
        where TException : Exception
    {
        #region Sanity checks
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        var random = GetRandom();
        int retryCounter = 0;
        Retry:
        if (retryCounter >= maxRetries)
            await action(lastAttempt: true).ConfigureAwait(false);
        else
        {
            try
            {
                await action(lastAttempt: false).ConfigureAwait(true);
            }
            catch (TException ex)
            {
                Log.Info(ex);

                int delay = random.Next(50, 1000 * (1 << retryCounter));
                Log.Info("Retrying in " + delay + " milliseconds");
                await Task.Delay(delay).ConfigureAwait(true);

                retryCounter++;
                goto Retry;
            }
        }
    }
#endif

    /// <summary>
    /// Uses process ID as a random seed to ensure we get different values than other competing processes on the same machine.
    /// </summary>
    private static Random GetRandom()
    {
        try
        {
#if NET
            return new Random(Environment.ProcessId);
#else
            return new Random(Process.GetCurrentProcess().Id);
#endif
        }
        catch (Exception)
        {
            return new Random();
        }
    }
}
