// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Security.Permissions;

namespace NanoByte.Common
{
    /// <summary>
    /// Derive from this class to enable remoting without timeouts. Keeps remoting object alive as long as process is running.
    /// </summary>
    public abstract class MarshalNoTimeout : MarshalByRefObject
    {
        /// <inheritdoc/>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object? InitializeLifetimeService() => null;
    }
}
