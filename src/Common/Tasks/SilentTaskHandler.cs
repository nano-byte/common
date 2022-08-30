// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks;

/// <summary>
/// Executes tasks silently and suppresses any questions.
/// </summary>
/// <remarks>This class is thread-safe.</remarks>
public class SilentTaskHandler : ITaskHandler
{
    /// <summary>
    /// Does nothing.
    /// </summary>
    public void Dispose() {}

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    /// <inheritdoc/>
    public CancellationToken CancellationToken => _cancellationTokenSource.Token;

    /// <summary>
    /// Cancels currently running <see cref="ITask"/>s.
    /// </summary>
    public void Cancel() => _cancellationTokenSource.Cancel();

    /// <inheritdoc/>
    public void RunTask(ITask task)
        => task.Run(CancellationToken);

    /// <summary>
    /// Always returns <see cref="NanoByte.Common.Tasks.Verbosity.Batch"/>.
    /// </summary>
    public Verbosity Verbosity
    {
        get => Verbosity.Batch;
        set {}
    }

    /// <summary>
    /// Returns <paramref name="defaultAnswer"/> if specified or <c>false</c> otherwise.
    /// </summary>
    public bool Ask(string question, bool? defaultAnswer = null, string? alternateMessage = null)
    {
        Log.Info($"{alternateMessage ?? question}\nReturning: {defaultAnswer ?? false}");
        return defaultAnswer ?? false;
    }

    /// <inheritdoc/>
    public void Output(string title, string message)
        => Log.Info($"{title}\n{message}");

    /// <inheritdoc/>
    public void Output<T>(string title, IEnumerable<T> data)
        => Output(title, StringUtils.Join(Environment.NewLine, data.Select(x => x?.ToString() ?? "")));

    /// <inheritdoc/>
    public void Output<T>(string title, NamedCollection<T> data) where T : INamed
        => Output(title, data.AsEnumerable());

    /// <inheritdoc/>
    public void Error(Exception exception)
        => Log.Error(exception);
}
