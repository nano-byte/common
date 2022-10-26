// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Net;

/// <summary>
/// State of a network connection.
/// </summary>
public enum Connectivity
{
    /// <summary>
    /// Normal connection available.
    /// </summary>
    Normal,

    /// <summary>
    /// Metered (potentially expensive) connection available.
    /// </summary>
    Metered,

    /// <summary>
    /// No connection available.
    /// </summary>
    None
}
