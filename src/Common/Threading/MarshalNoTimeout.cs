// Copyright Bastian Eicher
// Licensed under the MIT License

#if NETFRAMEWORK
using System.Security.Permissions;
#endif

namespace NanoByte.Common.Threading;

/// <summary>
/// Derive from this class to enable remoting without timeouts. Keeps remoting object alive as long as process is running.
/// </summary>
public abstract class MarshalNoTimeout : MarshalByRefObject
{
#if NETFRAMEWORK
    /// <inheritdoc/>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
    public override object? InitializeLifetimeService() => null;
#endif
}
