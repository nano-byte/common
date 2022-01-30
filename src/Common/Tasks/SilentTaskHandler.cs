// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks;

/// <summary>
/// Executes tasks silently and suppresses any questions.
/// </summary>
/// <remarks>This class is thread-safe.</remarks>
public class SilentTaskHandler : TaskHandlerBase
{
    public SilentTaskHandler()
    {
        Verbosity = Verbosity.Batch;
    }

    /// <summary>
    /// Always returns <paramref name="defaultAnswer"/>.
    /// </summary>
    protected override bool AskInteractive(string question, bool defaultAnswer)
    {
        Log.Info($"Question: {question}\nAutomatic/silent answer: {defaultAnswer}");
        return defaultAnswer;
    }

    /// <inheritdoc/>
    public override void Output(string title, string message)
        => Log.Info($"{title}\n{message}");

    /// <inheritdoc/>
    public override void Error(Exception exception)
        => Log.Error(exception);

    /// <summary>
    /// Cancels currently running <see cref="ITask"/>s.
    /// </summary>
    public void Cancel()
        => CancellationTokenSource.Cancel();
}
