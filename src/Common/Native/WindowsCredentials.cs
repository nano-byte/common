// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Security.Credentials;

namespace NanoByte.Common.Native;

/// <summary>
/// Provides an interface to the Windows Credentials API. Supported on Windows XP or newer.
/// </summary>
[SupportedOSPlatform("windows")]
[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "CsWin32 annotates exact Windows builds, versions are checked at runtime with IsWindows*")]
public static class WindowsCredentials
{
    private const int ErrorNoSuchLogonSession = 1312;

    private const int MaxUsernameLength = 513;
    private const int MaxPasswordLength = 257;

    /// <summary>
    /// Determines whether there are any credentials stored for a specific target.
    /// </summary>
    /// <param name="target">A string uniquely identifying the target the credentials are intended for.</param>
    /// <exception cref="PlatformNotSupportedException">The current platform does not support the Credentials API. Needs Windows XP or newer.</exception>
    public static unsafe bool IsCredentialStored(string target)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(target)) throw new ArgumentNullException(nameof(target));
        #endregion

        if (!WindowsUtils.IsWindowsXP) throw new PlatformNotSupportedException();

        if (!PInvoke.CredEnumerate(target, out uint count, out var credentials)) return false;
        PInvoke.CredFree(credentials);

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
    public static unsafe NetworkCredential PromptGui(string target, WindowsCredentialsFlags flags, string? title = null, string? message = null, IntPtr owner = default)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(target)) throw new ArgumentNullException(nameof(target));
        #endregion

        if (!WindowsUtils.IsWindowsXP) throw new PlatformNotSupportedException();

        char* username = stackalloc char[MaxUsernameLength + 1];
        char* password = stackalloc char[MaxPasswordLength + 1];
        try
        {
            fixed (char* targetPtr = target, titlePtr = title, messagePtr = message)
            {
                var info = new CREDUI_INFOW
                {
                    cbSize = (uint)sizeof(CREDUI_INFOW),
                    hwndParent = (HWND)owner,
                    pszCaptionText = titlePtr,
                    pszMessageText = messagePtr
                };
                BOOL persist = true;
                var result = PInvoke.CredUIPromptForCredentials(&info, targetPtr, null, 0, username, MaxUsernameLength + 1, password, MaxPasswordLength + 1, &persist, (CREDUI_FLAGS)flags);
                HandleResult((int)result);
            }

            return new NetworkCredential(new string(username), new string(password));
        }
        finally
        {
            new Span<char>(password, MaxPasswordLength + 1).Clear();
        }
    }

    /// <summary>
    /// Prompts the user for credentials using a command-line interface.
    /// </summary>
    /// <param name="target">A string uniquely identifying the target the credentials are intended for.</param>
    /// <param name="flags">Flags for configuring the prompt.</param>
    /// <exception cref="PlatformNotSupportedException">The current platform does not support the Credentials API. Needs Windows XP or newer.</exception>
    [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "Native API")]
    public static unsafe NetworkCredential PromptCli(string target, WindowsCredentialsFlags flags)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(target)) throw new ArgumentNullException(nameof(target));
        #endregion

        if (!WindowsUtils.IsWindowsXP) throw new PlatformNotSupportedException();

        char* username = stackalloc char[MaxUsernameLength + 1];
        char* password = stackalloc char[MaxPasswordLength + 1];
        try
        {
            fixed (char* targetPtr = target)
            {
                BOOL persist = true;
                uint result = PInvoke.CredUICmdLinePromptForCredentials(targetPtr, null, 0, username, MaxUsernameLength + 1, password, MaxPasswordLength + 1, &persist, (CREDUI_FLAGS)flags);
                HandleResult((int)result);
            }

            return new NetworkCredential(new string(username), new string(password));
        }
        finally
        {
            new Span<char>(password, MaxPasswordLength + 1).Clear();
        }
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
