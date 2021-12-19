// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls;

/// <summary>
/// How severe/important a message is.
/// </summary>
public enum MsgSeverity
{
    /// <summary>A nice-to-know piece of information.</summary>
    Info,

    /// <summary>A warning that doesn't have to be acted upon immediately.</summary>
    Warn,

    /// <summary>A critical error that should be attended to.</summary>
    Error
}