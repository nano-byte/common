// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks;

/// <summary>
/// Extension methods for <see cref="ITaskHandler"/>
/// </summary>
public static class TaskHandlerExtensions
{
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
}
