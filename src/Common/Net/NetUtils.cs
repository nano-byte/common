// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security;
using NanoByte.Common.Native;

#if NET
using System.Net.Http;
using System.Threading.Tasks;
using Tmds.DBus;
#endif

namespace NanoByte.Common.Net;

/// <summary>
/// Provides helper methods for the <see cref="System.Net"/> subsystem.
/// </summary>
public static class NetUtils
{
    /// <summary>
    /// Applies environment variable HTTP proxy server configuration if present.
    /// </summary>
    /// <remarks>Uses classic Linux environment variables: http_proxy, http_proxy_user, http_proxy_pass</remarks>
    public static void ApplyProxy()
    {
#if NETFRAMEWORK
        try
        {
#endif
            if (GetProxyAddress() is {} address)
            {
                var proxy = new WebProxy(address);
                if (GetProxyCredentials() is {} credentials)
                    proxy.Credentials = credentials;

                WebRequest.DefaultWebProxy = proxy;
#if NET
                HttpClient.DefaultProxy = proxy;
#endif
            }
            else
            {
                if (WebRequest.DefaultWebProxy is {} defaultWebProxy)
                    defaultWebProxy.Credentials ??= CredentialCache.DefaultCredentials;
#if NET
                HttpClient.DefaultProxy.Credentials ??= CredentialCache.DefaultCredentials;
#endif
            }
#if NETFRAMEWORK
        }
        #region Error handling
        catch (System.Configuration.ConfigurationErrorsException ex)
        {
            Log.Warn("Failed to apply proxy configuration due problems with .NET Framework machine configuration", ex);
        }
        #endregion
#endif
    }

    private static Uri? GetProxyAddress()
    {
        if (GetEnvVar("http_proxy") is not {} value) return null;
        if (Uri.TryCreate(value, UriKind.Absolute, out var address))
        {
            Log.Warn("Unable to parse http_proxy value as URI: " + value);
            return null;
        }
        return address;
    }

    private static NetworkCredential? GetProxyCredentials()
        => GetEnvVar("http_proxy_user") is {} username
        && GetEnvVar("http_proxy_pass") is {} password
            ? new(username, password)
            : null;

    private static string? GetEnvVar(string name)
        => (Environment.GetEnvironmentVariable(name)
         ?? Environment.GetEnvironmentVariable(name.ToUpperInvariant())).EmptyAsNull();

    /// <summary>
    /// Enables TLS 1.2 and TLS 1.3 support if available.
    /// </summary>
    public static void ConfigureTls()
    {
        try
        {
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072; // TLS 1.2
        }
        catch (NotSupportedException)
        {
            Log.Warn(Resources.Tls12SupportMissing);
        }

        try
        {
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)12288; // TLS 1.3
        }
        catch (NotSupportedException)
        {}
    }

    /// <summary>
    /// Makes the SSL validation subsystem trust a set of certificates, even if their certificate chain is not trusted.
    /// </summary>
    /// <param name="publicKeys">The public keys of the certificates to trust.</param>
    /// <remarks>This method affects the global state of the <see cref="AppDomain"/>. Calling it more than once is not cumulative and will overwrite previous certificates. You should call this method exactly once near the beginning of your application.</remarks>
    public static void TrustCertificates(params string[] publicKeys)
    {
        #region Sanity checks
        if (publicKeys == null) throw new ArgumentNullException(nameof(publicKeys));
        #endregion

        ServicePointManager.ServerCertificateValidationCallback = (_, certificate, _, sslPolicyErrors)
            => sslPolicyErrors == SslPolicyErrors.None
            || sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors && certificate != null && publicKeys.Contains(certificate.GetPublicKeyString());
    }

    /// <summary>
    /// Returns the current state of the internet connection.
    /// When unsure assumes a connection is available.
    /// </summary>
    public static Connectivity GetInternetConnectivity()
    {
        try
        {
            if (WindowsUtils.IsWindows102004 && SafeNativeMethods.GetNetworkConnectivityHint(out var connectivity) == 0)
            {
                if (connectivity.ConnectivityLevel is SafeNativeMethods.NetworkConnectivityLevel.None or SafeNativeMethods.NetworkConnectivityLevel.ConstrainedInternetAccess)
                    return Connectivity.None;

                if (connectivity.ConnectivityCost is SafeNativeMethods.NetworkConnectivityCost.Fixed or SafeNativeMethods.NetworkConnectivityCost.Variable
                 || connectivity.Roaming
                 || connectivity.OverDataLimit)
                    return Connectivity.Metered;

                return Connectivity.Normal;
            }

            if (WindowsUtils.IsWindowsNT)
                return SafeNativeMethods.InternetGetConnectedState(out _, 0) ? Connectivity.Normal : Connectivity.None;

#if NET
            if (UnixUtils.IsLinux && Guards.NotTrimmed)
            {
                var networkManager = Connection.System.CreateProxy<INetworkManager>("org.freedesktop.NetworkManager", "/org/freedesktop/NetworkManager");
                if (networkManager.GetAsync<uint>("Connectivity").Result is INetworkManager.ConnectivityNone or INetworkManager.ConnectivityPortal)
                    return Connectivity.None;
                if (networkManager.GetAsync<uint>("Metered").Result is INetworkManager.MeteredYes or INetworkManager.MeteredGuessYes)
                    return Connectivity.Metered;
            }
#endif

            return NetworkInterface.GetIsNetworkAvailable() ? Connectivity.Normal : Connectivity.None;
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Debug("Problem detecting internet connectivity state", ex);
            return Connectivity.Normal;
        }
        #endregion
    }

    [SuppressUnmanagedCodeSecurity]
    private static class SafeNativeMethods
    {
        [DllImport("wininet", SetLastError = true)]
        public static extern bool InternetGetConnectedState(out int lpdwFlags, int dwReserved);

        [DllImport("iphlpapi", SetLastError = false)]
        public static extern uint GetNetworkConnectivityHint(out NetworkConnectivityHint connectivityHint);

        [StructLayout(LayoutKind.Sequential)]
        public struct NetworkConnectivityHint
        {
            public NetworkConnectivityLevel ConnectivityLevel;
            public NetworkConnectivityCost ConnectivityCost;
            [MarshalAs(UnmanagedType.U1)] public bool ApproachingDataLimit;
            [MarshalAs(UnmanagedType.U1)] public bool OverDataLimit;
            [MarshalAs(UnmanagedType.U1)] public bool Roaming;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
		public enum NetworkConnectivityCost
		{
            Unknown,
			Unrestricted,
			Fixed,
			Variable
		}

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
		public enum NetworkConnectivityLevel
		{
			Unknown,
			None,
			LocalAccess,
			InternetAccess,
			ConstrainedInternetAccess,
			Hidden
		}
    }

#if NET
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("ReSharper", "UnusedMember.Local")]
    [DBusInterface("org.freedesktop.NetworkManager")]
    internal interface INetworkManager : IDBusObject
    {
        const uint ConnectivityUnknown = 0, ConnectivityNone = 1, ConnectivityPortal = 2, ConnectivityLimited = 3, ConnectivityFull = 4;
        const uint MeteredUnknown = 0, MeteredYes = 1, MeteredNo = 2, MeteredGuessYes = 3, MeteredGuessNo = 4;

        Task<T> GetAsync<T>(string prop);
    }
#endif
}
