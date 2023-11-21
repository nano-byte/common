// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks;

/// <summary>
/// Reports task progress updates using ANSI console output.
/// </summary>
/// <seealso cref="AnsiCliProgressContext"/>
public class AnsiCliProgress(ProgressTask progressTask) : MarshalByRefObject, IProgress<TaskSnapshot>
{
    /// <inheritdoc/>
    public void Report(TaskSnapshot value)
    {
        switch (value.State)
        {
            case TaskState.Started:
                progressTask.IsIndeterminate = true;
                progressTask.StartTask();
                break;

            case TaskState.Data when value.UnitsTotal > 0:
                progressTask.MaxValue = value.UnitsTotal;
                progressTask.IsIndeterminate = false;
                progressTask.Increment(value.UnitsProcessed - progressTask.Value);
                break;

            case TaskState.Complete:
                progressTask.Increment(progressTask.MaxValue - progressTask.Value);
                progressTask.StopTask();
                break;

            case TaskState.WebError or TaskState.IOError or TaskState.Canceled:
                progressTask.IsIndeterminate = true;
                progressTask.Description = $"[red]{value.State}[/]: {progressTask.Description.TrimOverflow(maxLength: AnsiCli.Error.Profile.Width - 32).EscapeMarkup()}";
                break;
        }
    }
}
