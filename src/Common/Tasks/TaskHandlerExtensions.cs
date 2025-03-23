// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;

namespace NanoByte.Common.Tasks;

/// <summary>
/// Extension methods for <see cref="ITaskHandler"/>
/// </summary>
public static class TaskHandlerExtensions
{
    /// <summary>
    /// Runs an <see cref="IResultTask{T}"/> and returns it's result once it has been completed.
    /// </summary>
    /// <param name="handler">The task handler.</param>
    /// <param name="task">The task to be run. (<see cref="ITask.Run"/> or equivalent is called on it.)</param>
    /// <returns>The <see cref="IResultTask{T}.Result"/>.</returns>
    /// <exception cref="OperationCanceledException">The user canceled the task.</exception>
    /// <exception cref="IOException">The task ended with <see cref="TaskState.IOError"/>.</exception>
    /// <exception cref="WebException">The task ended with <see cref="TaskState.WebError"/>.</exception>
    public static T RunTaskAndReturn<T>(this ITaskHandler handler, ResultTask<T> task)
    {
        handler.RunTask(task);
        return task.Result;
    }

    /// <summary>
    /// Displays multi-line text to the user unless <see cref="Verbosity"/> is <see cref="Tasks.Verbosity.Batch"/>.
    /// </summary>
    /// <param name="handler">The underlying <see cref="ITaskHandler"/>.</param>
    /// <param name="title">A title for the message.</param>
    /// <param name="message">The string to display.</param>
    /// <remarks>Implementations may close the UI as a side effect. Therefore this should be your last call on the handler.</remarks>
    public static void OutputLow(this ITaskHandler handler, [Localizable(true)] string title, [Localizable(true)] string message)
    {
        if (handler.Verbosity > Verbosity.Batch) handler.Output(title, message);
        else Log.Info($"{title}:\n{message}");
    }

    /// <summary>
    /// Displays tabular data to the user unless <see cref="Verbosity"/> is <see cref="Tasks.Verbosity.Batch"/>.
    /// </summary>
    /// <param name="handler">The underlying <see cref="ITaskHandler"/>.</param>
    /// <param name="title">A title for the data.</param>
    /// <param name="data">The data to display.</param>
    /// <remarks>Implementations may close the UI as a side effect. Therefore this should be your last call on the handler.</remarks>
    public static void OutputLow<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(this ITaskHandler handler, [Localizable(true)] string title, IEnumerable<T> data)
    {
        if (handler.Verbosity > Verbosity.Batch) handler.Output(title, data);
        else Log.Info($"{title}:\n{StringUtils.Join(Environment.NewLine, data.Select(x => x?.ToString() ?? ""))}");
    }

    /// <summary>
    /// Displays tree-like data to the user unless <see cref="Verbosity"/> is <see cref="Tasks.Verbosity.Batch"/>.
    /// </summary>
    /// <param name="handler">The underlying <see cref="ITaskHandler"/>.</param>
    /// <param name="title">A title for the data.INamed</param>
    /// <param name="data">The data to display.</param>
    /// <remarks>Implementations may close the UI as a side effect. Therefore this should be your last call on the handler.</remarks>
    public static void OutputLow<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(this ITaskHandler handler, [Localizable(true)] string title, NamedCollection<T> data)
        where T : INamed
    {
        if (handler.Verbosity > Verbosity.Batch) handler.Output(title, data);
        else handler.OutputLow(title, data.AsEnumerable());
    }
}
