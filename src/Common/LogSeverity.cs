// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

/// <summary>
/// Describes how severe/important a <see cref="Log"/> entry is.
/// </summary>
/// <seealso cref="LogEntryEventHandler"/>
public enum LogSeverity
{
    /// <summary>Information to help developers diagnose problems.</summary>
    Debug,

    /// <summary>A nice-to-know piece of information.</summary>
    Info,

    /// <summary>A warning that doesn't have to be acted upon immediately.</summary>
    Warn,

    /// <summary>A critical error that should be attended to.</summary>
    Error
}
