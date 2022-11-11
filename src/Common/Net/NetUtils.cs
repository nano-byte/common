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
        string? GetEnvVar(string name)
            => Environment.GetEnvironmentVariable(name)
            ?? Environment.GetEnvironmentVariable(name.ToUpperInvariant());

        if (GetEnvVar("http_proxy") is {Length: > 0} address)
        {
            var proxy = new WebProxy(address);

            if (GetEnvVar("http_proxy_user") is {Length: > 0} username
             && GetEnvVar("http_proxy_pass") is {Length: > 0} password)
                proxy.Credentials = new NetworkCredential(username, password);

            WebRequest.DefaultWebProxy = proxy;
#if NET
            HttpClient.DefaultProxy = proxy;
#endif
        }
    }

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
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Debug("Problem detecting internet connectivity state", ex);
        }
        #endregion

        return NetworkInterface.GetIsNetworkAvailable() ? Connectivity.Normal : Connectivity.None;
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
}
