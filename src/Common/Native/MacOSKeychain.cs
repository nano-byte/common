#if NET
// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace NanoByte.Common.Native;

/// <summary>
/// P/Invoke declarations and helpers for the MacOS X Keychain (Security framework).
/// </summary>
[SupportedOSPlatform("macos")]
internal static unsafe partial class MacOSKeychain
{
    private const string
        CoreFoundationLib = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation",
        SecurityLib = "/System/Library/Frameworks/Security.framework/Security";

    private const int errSecSuccess = 0, errSecItemNotFound = -25300, errSecInteractionNotAllowed = -25308, errSecUserCanceled = -128, errSecAuthFailed = -25293;
    private const int kCFNumberSInt32Type = 3;

    /// <summary>
    /// Finds an internet password stored in the Keychain.
    /// </summary>
    /// <param name="server">The host name of the server.</param>
    /// <param name="scheme">The URI scheme used to access the server. Only <c>http</c> and <c>https</c> are used to narrow the search.</param>
    /// <param name="port">The port used to access the server; <c>null</c> for any port.</param>
    /// <param name="allowUserInteraction">Allow the Keychain to ask the user for permission or to unlock it.</param>
    /// <returns>The account name and password; <c>null</c> if no matching item was found or the user denied access.</returns>
    /// <exception cref="IOException">The Keychain reported an unexpected error.</exception>
    public static (string Account, string Password)? FindInternetPassword(string server, string scheme, int? port, bool allowUserInteraction)
    {
        var cf = NativeLibrary.Load(CoreFoundationLib);
        var sec = NativeLibrary.Load(SecurityLib);
        nint Symbol(nint lib, string name) => *(nint*)NativeLibrary.GetExport(lib, name);

        var query = CFDictionaryCreateMutable(0, 0, NativeLibrary.GetExport(cf, "kCFTypeDictionaryKeyCallBacks"), NativeLibrary.GetExport(cf, "kCFTypeDictionaryValueCallBacks"));
        var toRelease = new List<nint> {query};
        try
        {
            nint Owned(nint value)
            {
                toRelease.Add(value);
                return value;
            }

            nint kCFBooleanTrue = Symbol(cf, "kCFBooleanTrue");
            CFDictionarySetValue(query, Symbol(sec, "kSecClass"), Symbol(sec, "kSecClassInternetPassword"));
            CFDictionarySetValue(query, Symbol(sec, "kSecAttrServer"), Owned(CreateString(server)));
            switch (scheme)
            {
                case "https":
                    CFDictionarySetValue(query, Symbol(sec, "kSecAttrProtocol"), Symbol(sec, "kSecAttrProtocolHTTPS"));
                    break;
                case "http":
                    CFDictionarySetValue(query, Symbol(sec, "kSecAttrProtocol"), Symbol(sec, "kSecAttrProtocolHTTP"));
                    break;
            }
            if (port is {} portValue)
            {
                int value = portValue;
                CFDictionarySetValue(query, Symbol(sec, "kSecAttrPort"), Owned(CFNumberCreate(0, kCFNumberSInt32Type, &value)));
            }
            CFDictionarySetValue(query, Symbol(sec, "kSecMatchLimit"), Symbol(sec, "kSecMatchLimitOne"));
            CFDictionarySetValue(query, Symbol(sec, "kSecReturnAttributes"), kCFBooleanTrue);
            CFDictionarySetValue(query, Symbol(sec, "kSecReturnData"), kCFBooleanTrue);
            if (!allowUserInteraction)
                CFDictionarySetValue(query, Symbol(sec, "kSecUseAuthenticationUI"), Symbol(sec, "kSecUseAuthenticationUIFail"));

            int status = CopyMatching(query, allowUserInteraction, out nint result);
            switch (status)
            {
                case errSecSuccess:
                    toRelease.Add(result);
                    break;
                case errSecItemNotFound:
                    return null;
                case errSecInteractionNotAllowed or errSecUserCanceled or errSecAuthFailed:
                    Log.Debug($"Access to Keychain item for {server} denied (OSStatus {status})");
                    return null;
                default:
                    throw new IOException($"Keychain error (OSStatus {status})");
            }

            nint account = CFDictionaryGetValue(result, Symbol(sec, "kSecAttrAccount"));
            nint data = CFDictionaryGetValue(result, Symbol(sec, "kSecValueData"));
            if (account == 0 || data == 0) return null;

            return (ReadString(account), System.Text.Encoding.UTF8.GetString(new ReadOnlySpan<byte>(CFDataGetBytePtr(data), (int)CFDataGetLength(data))));
        }
        finally
        {
            foreach (var handle in toRelease)
                CFRelease(handle);
        }
    }

    /// <summary>
    /// Calls <c>SecItemCopyMatching</c>, temporarily disabling user interaction for legacy file-based keychains if requested.
    /// </summary>
    private static int CopyMatching(nint query, bool allowUserInteraction, out nint result)
    {
        nint value = 0;
        if (allowUserInteraction)
        {
            int status = SecItemCopyMatching(query, &value);
            result = value;
            return status;
        }

        // kSecUseAuthenticationUI does not cover access control prompts of legacy keychains
        byte previous;
        bool restore = SecKeychainGetUserInteractionAllowed(&previous) == errSecSuccess && previous != 0;
        if (restore) SecKeychainSetUserInteractionAllowed(0);
        try
        {
            int status = SecItemCopyMatching(query, &value);
            result = value;
            return status;
        }
        finally
        {
            if (restore) SecKeychainSetUserInteractionAllowed(1);
        }
    }

    private static nint CreateString(string value)
    {
        fixed (char* chars = value)
            return CFStringCreateWithCharacters(0, chars, value.Length);
    }

    private static string ReadString(nint cfString)
    {
        nint length = CFStringGetLength(cfString);
        var buffer = new char[length];
        fixed (char* ptr = buffer)
            CFStringGetCharacters(cfString, new CFRange(0, length), ptr);
        return new(buffer);
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct CFRange(nint location, nint length)
    {
        public readonly nint Location = location, Length = length;
    }

    #region CoreFoundation
    [LibraryImport(CoreFoundationLib)]
    private static partial nint CFDictionaryCreateMutable(nint allocator, nint capacity, nint keyCallBacks, nint valueCallBacks);

    [LibraryImport(CoreFoundationLib)]
    private static partial void CFDictionarySetValue(nint dictionary, nint key, nint value);

    [LibraryImport(CoreFoundationLib)]
    private static partial nint CFDictionaryGetValue(nint dictionary, nint key);

    [LibraryImport(CoreFoundationLib)]
    private static partial nint CFStringCreateWithCharacters(nint allocator, char* chars, nint numChars);

    [LibraryImport(CoreFoundationLib)]
    private static partial nint CFStringGetLength(nint str);

    [LibraryImport(CoreFoundationLib)]
    private static partial void CFStringGetCharacters(nint str, CFRange range, char* buffer);

    [LibraryImport(CoreFoundationLib)]
    private static partial nint CFNumberCreate(nint allocator, int type, void* valuePtr);

    [LibraryImport(CoreFoundationLib)]
    private static partial nint CFDataGetLength(nint data);

    [LibraryImport(CoreFoundationLib)]
    private static partial byte* CFDataGetBytePtr(nint data);

    [LibraryImport(CoreFoundationLib)]
    private static partial void CFRelease(nint cf);
    #endregion

    #region Security
    [LibraryImport(SecurityLib)]
    private static partial int SecItemCopyMatching(nint query, nint* result);

    [LibraryImport(SecurityLib)]
    private static partial int SecKeychainGetUserInteractionAllowed(byte* state);

    [LibraryImport(SecurityLib)]
    private static partial int SecKeychainSetUserInteractionAllowed(byte state);
    #endregion
}
#endif
