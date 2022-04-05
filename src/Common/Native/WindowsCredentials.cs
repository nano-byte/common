// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using System.Runtime.Versioning;
using System.Text;

namespace NanoByte.Common.Native;

/// <summary>
/// Provides an interface to the Windows Credentials API. Supported on Windows XP or newer.
/// </summary>
[SupportedOSPlatform("windows")]
public static partial class WindowsCredentials
{
    /// <summary>
    /// Determines whether there are any credentials stored for a specific target.
    /// </summary>
    /// <param name="target">A string uniquely identifying the target the credentials are intended for.</param>
    /// <exception cref="PlatformNotSupportedException">The current platform does not support the Credentials API. Needs Windows XP or newer.</exception>
    public static bool IsCredentialStored(string target)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(target)) throw new ArgumentNullException(nameof(target));
        #endregion

        if (!WindowsUtils.IsWindowsXP) throw new PlatformNotSupportedException();

        NativeMethods.CredEnumerate(target, 0, out int count, out _);

        return (count > 0);
    }

    /// <summary>
    /// Prompts the user for credentials using a GUI dialog.
    /// </summary>
    /// <param name="target">A string uniquely identifying the target the credentials are intended for.</param>
    /// <param name="flags">Flags for configuring the prompt.</param>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="message">The message to display in the dialog.</param>
    /// <param name="owner">The parent window for the dialog; can be <c>null</c>.</param>
    /// <exception cref="PlatformNotSupportedException">The current platform does not support the Credentials API. Needs Windows XP or newer.</exception>
    [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "Native API")]
    public static NetworkCredential PromptGui(string target, WindowsCredentialsFlags flags, string? title = null, string? message = null, IntPtr owner = default)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(target)) throw new ArgumentNullException(nameof(target));
        #endregion

        if (!WindowsUtils.IsWindowsXP) throw new PlatformNotSupportedException();

        var usernameBuffer = new StringBuilder(1000);
        var passwordBuffer = new StringBuilder(1000);

        var credUI = CreateCredUIInfo(owner, title, message);
        bool persist = true;
        int result = NativeMethods.CredUIPromptForCredentials(ref credUI, target, IntPtr.Zero, 0, usernameBuffer, MaxUsernameLength, passwordBuffer, MaxPasswordLength, ref persist, flags);
        HandleResult(result);

        return new NetworkCredential(usernameBuffer.ToString(), passwordBuffer.ToString());
    }

    /// <summary>
    /// Prompts the user for credentials using a command-line interface.
    /// </summary>
    /// <param name="target">A string uniquely identifying the target the credentials are intended for.</param>
    /// <param name="flags">Flags for configuring the prompt.</param>
    /// <exception cref="PlatformNotSupportedException">The current platform does not support the Credentials API. Needs Windows XP or newer.</exception>
    [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "Native API")]
    public static NetworkCredential PromptCli(string target, WindowsCredentialsFlags flags)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(target)) throw new ArgumentNullException(nameof(target));
        #endregion

        if (!WindowsUtils.IsWindowsXP) throw new PlatformNotSupportedException();

        var usernameBuffer = new StringBuilder(1000);
        var passwordBuffer = new StringBuilder(1000);

        bool persist = true;
        int result = NativeMethods.CredUICmdLinePromptForCredentials(target, IntPtr.Zero, 0, usernameBuffer, MaxUsernameLength, passwordBuffer, MaxPasswordLength, ref persist, flags);
        HandleResult(result);

        return new NetworkCredential(usernameBuffer.ToString(), passwordBuffer.ToString());
    }

    private static void HandleResult(int result)
    {
        switch (result)
        {
            case 0:
                break;
            case WindowsUtils.Win32ErrorCancelled:
                throw new OperationCanceledException();
            case ErrorNoSuchLogonSession:
                throw new NotSupportedException();
            default:
                throw new IOException();
        }
    }
}
