// Copyright Bastian Eicher
// Licensed under the MIT License

using Eto;

namespace NanoByte.Common.Threading;

/// <summary>
/// Runs an Eto.Forms <see cref="Application"/> message loop on the calling (main) thread while executing program workload on a background thread.
/// The workload can show GUI (e.g., via <see cref="Tasks.EtoDialogTaskHandler"/>), which is marshaled back onto the main thread.
/// </summary>
/// <remarks>
/// Eto.Forms requires its <see cref="Application"/> to own the main thread (mandatory on macOS), and <see cref="Application.Run()"/> blocks.
/// This helper inverts the usual "workload on main thread" model: the message loop owns the main thread and the workload runs on a worker thread.
/// </remarks>
public static class EtoApp
{
    /// <summary>
    /// Runs <paramref name="workload"/> on a worker thread while an Eto.Forms message loop runs on the calling (main) thread, then terminates the process with the workload's exit code. Does not return.
    /// </summary>
    /// <param name="platform">The Eto.Forms platform to run.</param>
    /// <param name="workload">The program workload. Runs on a background thread; may show GUI which is marshaled to the main thread. Returns the process exit code.</param>
    /// <remarks>
    /// When the workload completes, the process is terminated via <see cref="Environment.Exit"/> with the workload's return value as the exit code.
    /// This is necessary because <see cref="Application.Run()"/> does not return after a quit on some platforms (e.g. macOS, where the native application loop exits the process directly), so the exit code cannot be propagated by returning from <see cref="Application.Run()"/>.
    /// </remarks>
    [DoesNotReturn]
    public static void Run(Platform platform, Func<int> workload)
    {
        #region Sanity checks
        if (platform == null) throw new ArgumentNullException(nameof(platform));
        if (workload == null) throw new ArgumentNullException(nameof(workload));
        #endregion

        var application = new Application(platform);
        application.Initialized += delegate
        {
            ThreadUtils.StartBackground(() =>
                {
                    try
                    {
                        Environment.Exit(workload());
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        Environment.Exit(1);
                    }
                }, name: $"${nameof(EtoApp)} workload");
        };
        application.Run(); // Blocks the calling (main) thread; the process exits via the worker thread above

        Environment.Exit(0); // unreachable
    }

    /// <summary>
    /// Runs <paramref name="workload"/> on a worker thread while an Eto.Forms message loop runs on the calling (main) thread, then terminates the process with the workload's exit code. Does not return.
    /// </summary>
    /// <param name="platform">The Eto.Forms platform to run.</param>
    /// <param name="workload">The program workload. Runs on a background thread; may show GUI which is marshaled to the main thread.</param>
    /// <remarks>
    /// When the workload completes, the process is terminated via <see cref="Environment.Exit"/> with <c>0</c> on success, <c>1</c> if <paramref name="workload"/> throws.
    /// This is necessary because <see cref="Application.Run()"/> does not return after a quit on some platforms (e.g. macOS, where the native application loop exits the process directly), so the exit code cannot be propagated by returning from <see cref="Application.Run()"/>.
    /// </remarks>
    [DoesNotReturn]
    public static void Run(Platform platform, Action workload)
        => Run(platform, () =>
        {
            (workload ?? throw new ArgumentNullException(nameof(workload)))();
            return 0;
        });
}
