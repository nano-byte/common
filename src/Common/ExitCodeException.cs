// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Diagnostics;
using System.Runtime.Serialization;

#if NETFRAMEWORK
using System.Security.Permissions;
#endif

namespace NanoByte.Common;

/// <summary>
/// Indicates that a <see cref="Process"/> exited with an unexpected <see cref="Process.ExitCode"/>.
/// </summary>
[Serializable]
public sealed class ExitCodeException : IOException
{
    /// <summary>
    /// The <see cref="Process.ExitCode"/>.
    /// </summary>
    public int ExitCode { get; }

    /// <summary>
    /// Creates a new exit code exception.
    /// </summary>
    /// <param name="startInfo">The start info used to launch the process that has now exited.</param>
    /// <param name="exitCode">The <see cref="Process.ExitCode"/>.</param>
    public ExitCodeException(ProcessStartInfo startInfo, int exitCode)
        : this(string.Format(Resources.ProcessExitCode, startInfo.ToCommandLine(), exitCode), exitCode)
    {}

    /// <summary>
    /// Creates a new exit code exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="exitCode">The <see cref="Process.ExitCode"/>.</param>
    public ExitCodeException(string message, int exitCode)
        : base(message)
    {
        ExitCode = exitCode;
    }

    /// <inheritdoc/>
    public ExitCodeException()
        : base(Resources.ProcessExitCodeUnexpected)
    {}

    /// <inheritdoc/>
    public ExitCodeException(string message)
        : base(message)
    {}

    /// <inheritdoc/>
    public ExitCodeException(string message, Exception innerException)
        : base(message, innerException)
    {}

    #region Serialization
    /// <summary>
    /// Deserializes an exception.
    /// </summary>
    private ExitCodeException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        #region Sanity checks
        if (info == null) throw new ArgumentNullException(nameof(info));
        #endregion

        ExitCode = info.GetInt32(nameof(ExitCode));
    }

    /// <inheritdoc/>
#if NETFRAMEWORK
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        #region Sanity checks
        if (info == null) throw new ArgumentNullException(nameof(info));
        #endregion

        info.AddValue(nameof(ExitCode), ExitCode);

        base.GetObjectData(info, context);
    }
    #endregion
}
