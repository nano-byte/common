#if NET
// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Native.DBus;

// Extends the proxy generated from org.freedesktop.NetworkManager.xml
partial class NetworkManager
{
    /// <summary>Values of the <c>Connectivity</c> property.</summary>
    public const uint ConnectivityUnknown = 0, ConnectivityNone = 1, ConnectivityPortal = 2, ConnectivityLimited = 3, ConnectivityFull = 4;

    /// <summary>Values of the <c>Metered</c> property.</summary>
    public const uint MeteredUnknown = 0, MeteredYes = 1, MeteredNo = 2, MeteredGuessYes = 3, MeteredGuessNo = 4;
}
#endif
