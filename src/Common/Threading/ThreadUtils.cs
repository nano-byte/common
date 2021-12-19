// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.Versioning;
using NanoByte.Common.Native;

namespace NanoByte.Common.Threading;

/// <summary>
/// Provides helper methods for launching <see cref="Thread"/>s.
/// </summary>
public static class ThreadUtils
{
    /// <summary>
    /// Starts executing a delegate in a new thread suitable for WinForms.
    /// </summary>
    /// <param name="execute">The delegate to execute.</param>
    /// <param name="name">A short name for the new thread; can be <c>null</c>.</param>
    /// <returns>The newly launched thread.</returns>
    public static Thread StartAsync(ThreadStart execute, [Localizable(false)] string? name = null)
    {
        #region Sanity checks
        if (execute == null) throw new ArgumentNullException(nameof(execute));
        #endregion

        Log.Debug("Starting async thread: " + name);

        var thread = new Thread(execute) {Name = name};
        if (WindowsUtils.IsWindows)
            thread.SetApartmentState(ApartmentState.STA); // Make COM work
        thread.Start();
        return thread;
    }

    /// <summary>
    /// Starts executing a delegate in a new background thread (automatically terminated when application exits).
    /// </summary>
    /// <param name="execute">The delegate to execute.</param>
    /// <param name="name">A short name for the new thread; can be <c>null</c>.</param>
    /// <returns>The newly launched thread.</returns>
    public static Thread StartBackground(ThreadStart execute, [Localizable(false)] string? name = null)
    {
        #region Sanity checks
        if (execute == null) throw new ArgumentNullException(nameof(execute));
        #endregion

        Log.Debug("Starting background thread: " + name);

        var thread = new Thread(execute) {Name = name, IsBackground = true};
        thread.Start();
        return thread;
    }

    /// <summary>
    /// Executes a delegate in a new <see cref="ApartmentState.STA"/> thread. Blocks the caller until the execution completes.
    /// </summary>
    /// <param name="execute">The delegate to execute.</param>
    /// <remarks>This is useful for code that needs to be executed in a Single-Threaded Apartment (e.g. WinForms code) when the calling thread is not set up to handle COM.</remarks>
    [SupportedOSPlatform("windows")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions are rethrown on calling thread.")]
    public static void RunSta(Action execute)
    {
        #region Sanity checks
        if (execute == null) throw new ArgumentNullException(nameof(execute));
        #endregion

        Log.Debug("Running STA thread");

        Exception? error = null;
        var thread = new Thread(new ThreadStart(delegate
        {
            try
            {
                execute();
            }
            catch (Exception ex)
            {
                error = ex;
            }
        }));
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        error?.Rethrow();
    }

    /// <summary>
    /// Executes a delegate in a new <see cref="ApartmentState.STA"/> thread. Blocks the caller until the execution completes.
    /// </summary>
    /// <typeparam name="T">The type of the return value of <paramref name="execute"/>.</typeparam>
    /// <param name="execute">The delegate to execute.</param>
    /// <returns>The return value of <paramref name="execute"/></returns>
    /// <remarks>This is useful for code that needs to be executed in a Single-Threaded Apartment (e.g. WinForms code) when the calling thread is not set up to handle COM.</remarks>
    [SupportedOSPlatform("windows")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions are rethrown on calling thread.")]
    public static T RunSta<T>(Func<T> execute)
    {
        #region Sanity checks
        if (execute == null) throw new ArgumentNullException(nameof(execute));
        #endregion

        Log.Debug("Running STA thread");

        T result = default!;
        Exception? error = null;
        var thread = new Thread(new ThreadStart(delegate
        {
            try
            {
                result = execute();
            }
            catch (Exception ex)
            {
                error = ex;
            }
        }));
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        error?.Rethrow();
        return result;
    }
}