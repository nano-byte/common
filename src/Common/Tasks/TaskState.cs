// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Tasks;

/// <summary>
/// Represents different states a (usually Web- or IO-related) task can be in.
/// </summary>
/// <seealso cref="TaskSnapshot.State"/>
public enum TaskState
{
    /// <summary>The task is ready to begin.</summary>
    Ready,

    /// <summary>The task has just been started.</summary>
    Started,

    /// <summary>Handling the header.</summary>
    Header,

    /// <summary>Handling the actual data.</summary>
    Data,

    /// <summary>The task has been completed successfully.</summary>
    Complete,

    /// <summary>An error occurred during the task.</summary>
    WebError,

    /// <summary>An error occurred while writing the file.</summary>
    IOError,

    /// <summary>The task was canceled by the user before completion.</summary>
    Canceled
}
