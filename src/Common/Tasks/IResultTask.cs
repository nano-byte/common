// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks;


/// <summary>
/// A task the provides a result.
/// </summary>
public interface IResultTask<out T> : ITask
{
    /// <summary>
    /// The result of the task.
    /// </summary>
    /// <exception cref="InvalidOperationException">The task is not <see cref="TaskState.Complete"/>.</exception>
    T Result { get; }
}
