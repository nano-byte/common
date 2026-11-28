#if NET
// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

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

    private const int errSecSuccess = 0, errSecItemNotFound = -25300, errSecDuplicateItem = -25299, errSecInteractionNotAllowed = -25308, errSecUserCanceled = -128, errSecAuthFailed = -25293;
    private const int kCFNumberSInt32Type = 3;

    /// <summary>
    /// Finds an internet password stored in the Keychain.
    /// </summary>
    /// <param name="server">The host name of the server.</param>
    /// <param name="scheme">The URI scheme used to access the server. Only <c>http</c> and <c>https</c> are used to narrow the search.</param>
    /// <param name="port">The port used to access the server; <c>0</c> to match only items stored without a port.</param>
    /// <param name="allowUserInteraction">Allow the Keychain to ask the user for permission or to unlock it.</param>
    /// <returns>The account name and password; <c>null</c> if no matching item was found or the user denied access.</returns>
    /// <exception cref="IOException">The Keychain reported an unexpected error.</exception>
    public static (string Account, string Password)? FindInternetPassword(string server, string scheme, int port, bool allowUserInteraction)
    {
        using var scope = new CFScope();
        var query = scope.CreateQuery(server, scheme, port);
        CFDictionarySetValue(query, scope.Sec("kSecMatchLimit"), scope.Sec("kSecMatchLimitOne"));
        CFDictionarySetValue(query, scope.Sec("kSecReturnAttributes"), scope.CF("kCFBooleanTrue"));
        CFDictionarySetValue(query, scope.Sec("kSecReturnData"), scope.CF("kCFBooleanTrue"));
        if (!allowUserInteraction)
            CFDictionarySetValue(query, scope.Sec("kSecUseAuthenticationUI"), scope.Sec("kSecUseAuthenticationUIFail"));

        nint result = 0;
        int status;
        using (SuppressUserInteraction(allowUserInteraction))
            status = SecItemCopyMatching(query, &result);
        switch (status)
        {
            case errSecSuccess:
                scope.Own(result);
                break;
            case errSecItemNotFound:
                return null;
            case errSecInteractionNotAllowed or errSecUserCanceled or errSecAuthFailed:
                Log.Debug($"Access to Keychain item for {server} denied (OSStatus {status})");
                return null;
            default:
                throw new IOException($"Keychain error (OSStatus {status})");
        }

        nint account = CFDictionaryGetValue(result, scope.Sec("kSecAttrAccount"));
        nint data = CFDictionaryGetValue(result, scope.Sec("kSecValueData"));
        if (account == 0 || data == 0) return null;

        return (ReadString(account), Encoding.UTF8.GetString(new ReadOnlySpan<byte>(CFDataGetBytePtr(data), (int)CFDataGetLength(data))));
    }

    /// <summary>
    /// Stores an internet password in the Keychain, replacing the password of an existing item for the same account.
    /// </summary>
    /// <param name="server">The host name of the server.</param>
    /// <param name="scheme">The URI scheme used to access the server.</param>
    /// <param name="port">The port used to access the server; <c>0</c> for items stored without a port.</param>
    /// <param name="account">The user name.</param>
    /// <param name="password">The password.</param>
    /// <param name="label">A human-readable label for the item.</param>
    /// <param name="allowUserInteraction">Allow the Keychain to ask the user for permission or to unlock it.</param>
    /// <exception cref="IOException">The Keychain reported an error.</exception>
    public static void AddInternetPassword(string server, string scheme, int port, string account, string password, string label, bool allowUserInteraction)
    {
        using var scope = new CFScope();
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        nint passwordData;
        fixed (byte* ptr = passwordBytes)
            passwordData = scope.Own(CFDataCreate(0, ptr, passwordBytes.Length));

        var item = scope.CreateQuery(server, scheme, port);
        CFDictionarySetValue(item, scope.Sec("kSecAttrAccount"), scope.Own(CreateString(account)));
        CFDictionarySetValue(item, scope.Sec("kSecAttrLabel"), scope.Own(CreateString(label)));
        CFDictionarySetValue(item, scope.Sec("kSecValueData"), passwordData);

        int status;
        using (SuppressUserInteraction(allowUserInteraction))
            status = SecItemAdd(item, null);
        if (status == errSecDuplicateItem)
        {
            var query = scope.CreateQuery(server, scheme, port);
            CFDictionarySetValue(query, scope.Sec("kSecAttrAccount"), scope.Own(CreateString(account)));
            var update = scope.CreateDictionary();
            CFDictionarySetValue(update, scope.Sec("kSecValueData"), passwordData);
            using (SuppressUserInteraction(allowUserInteraction))
                status = SecItemUpdate(query, update);
        }
        if (status != errSecSuccess) throw new IOException($"Keychain error (OSStatus {status})");
    }

    /// <summary>
    /// Removes internet passwords for a specific account from the Keychain.
    /// </summary>
    /// <param name="server">The host name of the server.</param>
    /// <param name="scheme">The URI scheme used to access the server.</param>
    /// <param name="port">The port used to access the server; <c>0</c> for items stored without a port.</param>
    /// <param name="account">The user name.</param>
    /// <param name="allowUserInteraction">Allow the Keychain to ask the user for permission or to unlock it.</param>
    /// <exception cref="IOException">The Keychain reported an error.</exception>
    public static void DeleteInternetPassword(string server, string scheme, int port, string account, bool allowUserInteraction)
    {
        using var scope = new CFScope();
        var query = scope.CreateQuery(server, scheme, port);
        CFDictionarySetValue(query, scope.Sec("kSecAttrAccount"), scope.Own(CreateString(account)));

        int status;
        using (SuppressUserInteraction(allowUserInteraction))
            status = SecItemDelete(query);
        if (status is not (errSecSuccess or errSecItemNotFound)) throw new IOException($"Keychain error (OSStatus {status})");
    }

    /// <summary>
    /// Temporarily disables user interaction for legacy file-based keychains, unless <paramref name="allowUserInteraction"/> is set.
    /// </summary>
    /// <returns>Call <see cref="IDisposable.Dispose"/> to restore the original state.</returns>
    /// <remarks>
    /// <c>kSecUseAuthenticationUI</c> does not cover access control prompts of legacy keychains.
    /// The setting is process-wide, so other Keychain accesses running concurrently on other threads are also unable to prompt while it is in effect.
    /// Keep the scope as short as possible, i.e., only around a single Security framework call.
    /// </remarks>
    [MustDisposeResource]
    private static IDisposable? SuppressUserInteraction(bool allowUserInteraction)
    {
        if (allowUserInteraction) return null;

        byte previous;
        if (SecKeychainGetUserInteractionAllowed(&previous) != errSecSuccess || previous == 0) return null;

        SecKeychainSetUserInteractionAllowed(0);
        byte restore = previous; // Cannot capture variable whose address was taken
        return new Disposable(() => SecKeychainSetUserInteractionAllowed(restore));
    }

    /// <summary>
    /// Resolves CoreFoundation and Security framework constants and releases owned CoreFoundation objects when disposed.
    /// </summary>
    private sealed class CFScope : IDisposable
    {
        private static readonly nint _cf = NativeLibrary.Load(CoreFoundationLib), _sec = NativeLibrary.Load(SecurityLib);
        private readonly List<nint> _owned = [];

        /// <summary>Gets the value of a CoreFoundation constant.</summary>
        public nint CF(string name) => *(nint*)NativeLibrary.GetExport(_cf, name);

        /// <summary>Gets the value of a Security framework constant.</summary>
        public nint Sec(string name) => *(nint*)NativeLibrary.GetExport(_sec, name);

        /// <summary>Releases the object when this scope is disposed.</summary>
        public nint Own(nint value)
        {
            if (value != 0) _owned.Add(value);
            return value;
        }

        /// <summary>Creates an empty mutable dictionary that retains its keys and values.</summary>
        public nint CreateDictionary()
            => Own(CFDictionaryCreateMutable(0, 0, NativeLibrary.GetExport(_cf, "kCFTypeDictionaryKeyCallBacks"), NativeLibrary.GetExport(_cf, "kCFTypeDictionaryValueCallBacks")));

        /// <summary>Creates a dictionary identifying internet passwords for a specific server.</summary>
        public nint CreateQuery(string server, string scheme, int port)
        {
            var query = CreateDictionary();
            CFDictionarySetValue(query, Sec("kSecClass"), Sec("kSecClassInternetPassword"));
            CFDictionarySetValue(query, Sec("kSecAttrServer"), Own(CreateString(server)));
            switch (scheme)
            {
                case "https":
                    CFDictionarySetValue(query, Sec("kSecAttrProtocol"), Sec("kSecAttrProtocolHTTPS"));
                    break;
                case "http":
                    CFDictionarySetValue(query, Sec("kSecAttrProtocol"), Sec("kSecAttrProtocolHTTP"));
                    break;
            }
            // Always set the port, since leaving it out would match items for any port
            CFDictionarySetValue(query, Sec("kSecAttrPort"), Own(CFNumberCreate(0, kCFNumberSInt32Type, &port)));
            return query;
        }

        public void Dispose()
        {
            foreach (var handle in _owned)
                CFRelease(handle);
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
    private static partial nint CFDataCreate(nint allocator, byte* bytes, nint length);

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
    private static partial int SecItemAdd(nint attributes, nint* result);

    [LibraryImport(SecurityLib)]
    private static partial int SecItemUpdate(nint query, nint attributesToUpdate);

    [LibraryImport(SecurityLib)]
    private static partial int SecItemDelete(nint query);

    [LibraryImport(SecurityLib)]
    private static partial int SecKeychainGetUserInteractionAllowed(byte* state);

    [LibraryImport(SecurityLib)]
    private static partial int SecKeychainSetUserInteractionAllowed(byte state);
    #endregion
}
#endif
