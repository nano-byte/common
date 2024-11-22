// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET
using System.Runtime.Serialization;
#endif

namespace NanoByte.Common;

/// <summary>
/// Like a <see cref="UnauthorizedAccessException"/> but with the additional hint that retrying the operation as an administrator would fix the problem.
/// </summary>
#if !NET
[Serializable]
#endif
public class NotAdminException : UnauthorizedAccessException
{
    /// <inheritdoc/>
    public NotAdminException() {}

    /// <inheritdoc/>
    public NotAdminException(string message, Exception inner)
        : base(message, inner)
    {}

    /// <inheritdoc/>
    public NotAdminException(string message)
        : base(message)
    {}

#if !NET
    /// <inheritdoc/>
    protected NotAdminException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {}
#endif
}
