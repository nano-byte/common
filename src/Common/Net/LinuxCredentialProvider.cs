#if NET
// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Globalization;
using System.Net;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using NanoByte.Common.Native.DBus;
using Tmds.DBus.Protocol;

namespace NanoByte.Common.Net;

/// <summary>
/// Gets <see cref="NetworkCredential"/>s stored in the Linux keyring (e.g., GNOME Keyring or KWallet) via the freedesktop.org Secret Service API.
/// Falls back to another provider if no suitable credentials can be found.
/// </summary>
/// <remarks>
/// Looks for items with the attributes <c>server</c>, <c>protocol</c>, <c>user</c> and optionally <c>port</c>.
/// This matches the items stored by <c>git-credential-libsecret</c> and the <c>org.gnome.keyring.NetworkPassword</c> schema.
/// Credentials can also be stored manually, e.g.: <c>secret-tool store --label="example.com" server example.com protocol https user myname</c>
/// </remarks>
/// <param name="innerProvider">The provider to fall back to if no suitable credentials can be found in the keyring.</param>
/// <param name="interactive">Allow the keyring to ask the user to unlock it if necessary. Ignored if there is no graphical display to show the keyring's prompts on.</param>
[SupportedOSPlatform("linux")]
public class LinuxCredentialProvider(ICredentialProvider? innerProvider = null, bool interactive = false) : KeyringCredentialProvider(innerProvider)
{
    /// <summary>
    /// Allow the keyring to show prompts. Requires a graphical display, since the prompts are shown by the keyring, not by this process.
    /// </summary>
    private readonly bool _interactive = interactive && HasDisplay;

    private static bool HasDisplay
        => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY"))
        || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY"));

    private const string BusName = "org.freedesktop.secrets";
    private static readonly ObjectPath
        _servicePath = "/org/freedesktop/secrets",
        _defaultCollectionPath = "/org/freedesktop/secrets/aliases/default",
        _noPrompt = "/";

    /// <inheritdoc/>
    protected override string KeyringName => "Secret Service";

    /// <inheritdoc/>
    protected override NetworkCredential? Lookup(Uri uri)
        => LookupAsync(uri).GetAwaiter().GetResult();

    private async Task<NetworkCredential?> LookupAsync(Uri uri)
    {
        var connection = DBusConnection.Session;
        var service = new Service(connection, BusName, _servicePath);

        var (unlocked, locked) = await service.SearchItemsAsync(GetSearchAttributes(uri)).ConfigureAwait(false);
        var (itemPath, user) = await FindMatchAsync(connection, unlocked, uri).ConfigureAwait(false);
        if (itemPath == null && _interactive)
        {
            (itemPath, user) = await FindMatchAsync(connection, locked, uri).ConfigureAwait(false);
            if (itemPath is {} lockedPath && !await UnlockAsync(connection, service, lockedPath).ConfigureAwait(false))
                return null;
        }
        if (itemPath is not {} path || user == null) return null;

        var (_, sessionPath) = await service.OpenSessionAsync("plain", VariantValue.String("")).ConfigureAwait(false);
        try
        {
            var secrets = await service.GetSecretsAsync([path], sessionPath).ConfigureAwait(false);
            return secrets.TryGetValue(path, out var secret)
                ? new NetworkCredential(user, Encoding.UTF8.GetString(secret.Item3))
                : null;
        }
        finally
        {
            await new Session(connection, BusName, sessionPath).CloseAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Finds the first item that has a user name and a port compatible with the <paramref name="uri"/>.
    /// </summary>
    private static async Task<(ObjectPath? Path, string? User)> FindMatchAsync(DBusConnection connection, IEnumerable<ObjectPath> items, Uri uri)
    {
        foreach (var path in items)
        {
            var attributes = await new Item(connection, BusName, path).GetAttributesAsync().ConfigureAwait(false);
            if (!attributes.TryGetValue("user", out string? user) || string.IsNullOrEmpty(user)) continue;
            if (PortMatches(attributes, uri)) return (path, user);
        }
        return (null, null);
    }

    private static Dictionary<string, string> GetSearchAttributes(Uri uri)
        => new() {["server"] = uri.Host, ["protocol"] = uri.Scheme};

    private static bool PortMatches(Dictionary<string, string> attributes, Uri uri)
        => attributes.TryGetValue("port", out string? port)
            ? port == uri.Port.ToString(CultureInfo.InvariantCulture)
            : uri.IsDefaultPort;

    /// <inheritdoc/>
    protected override void Store(Uri uri, NetworkCredential credential)
        => StoreAsync(uri, credential).GetAwaiter().GetResult();

    private async Task StoreAsync(Uri uri, NetworkCredential credential)
    {
        var connection = DBusConnection.Session;
        var service = new Service(connection, BusName, _servicePath);
        var collection = new Collection(connection, BusName, _defaultCollectionPath);

        if (_interactive && !await UnlockAsync(connection, service, _defaultCollectionPath).ConfigureAwait(false))
            throw new OperationCanceledException("User refused to unlock the keyring");

        var attributes = new Dict<string, string>
        {
            ["xdg:schema"] = "org.gnome.keyring.NetworkPassword",
            ["server"] = uri.Host,
            ["protocol"] = uri.Scheme,
            ["user"] = credential.UserName
        };
        if (!uri.IsDefaultPort) attributes["port"] = uri.Port.ToString(CultureInfo.InvariantCulture);
        var properties = new Dictionary<string, VariantValue>
        {
            ["org.freedesktop.Secret.Item.Label"] = VariantValue.String(uri.GetRoot().ToStringRfc()),
            ["org.freedesktop.Secret.Item.Attributes"] = attributes
        };

        var (_, sessionPath) = await service.OpenSessionAsync("plain", VariantValue.String("")).ConfigureAwait(false);
        try
        {
            var secret = (sessionPath, Array.Empty<byte>(), Encoding.UTF8.GetBytes(credential.Password), "text/plain; charset=utf8");
            var (itemPath, promptPath) = await collection.CreateItemAsync(properties, secret, replace: true).ConfigureAwait(false);
            if (itemPath == _noPrompt)
            {
                if (promptPath == _noPrompt || !_interactive) throw new InvalidOperationException("The keyring is locked");
                if (!await RunPromptAsync(connection, promptPath).ConfigureAwait(false))
                    throw new OperationCanceledException("User refused to save to the keyring");
            }
        }
        finally
        {
            await new Session(connection, BusName, sessionPath).CloseAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    protected override void Delete(Uri uri, string userName)
        => DeleteAsync(uri, userName).GetAwaiter().GetResult();

    private async Task DeleteAsync(Uri uri, string userName)
    {
        var connection = DBusConnection.Session;
        var service = new Service(connection, BusName, _servicePath);

        var attributes = GetSearchAttributes(uri);
        attributes["user"] = userName;
        var (unlocked, locked) = await service.SearchItemsAsync(attributes).ConfigureAwait(false);
        foreach (var path in unlocked.Concat(locked))
        {
            var item = new Item(connection, BusName, path);
            if (!PortMatches(await item.GetAttributesAsync().ConfigureAwait(false), uri)) continue;

            // Locked items cannot be deleted, see org.freedesktop.Secret.Error.IsLocked
            if (locked.Contains(path))
            {
                if (!_interactive) throw new InvalidOperationException("The keyring is locked");
                if (!await UnlockAsync(connection, service, path).ConfigureAwait(false))
                    throw new OperationCanceledException("User refused to unlock the keyring");
            }

            var promptPath = await item.DeleteAsync().ConfigureAwait(false);
            if (promptPath != _noPrompt && (!_interactive || !await RunPromptAsync(connection, promptPath).ConfigureAwait(false)))
                throw new OperationCanceledException("User refused to remove the item from the keyring");
        }
    }

    /// <summary>
    /// Unlocks an item or collection, asking the user via a prompt provided by the keyring if necessary.
    /// </summary>
    /// <returns><c>true</c> if the object was unlocked; <c>false</c> if the user dismissed the prompt.</returns>
    private static async Task<bool> UnlockAsync(DBusConnection connection, Service service, ObjectPath obj)
    {
        var (unlocked, promptPath) = await service.UnlockAsync([obj]).ConfigureAwait(false);

        // Some implementations return the canonical path for aliases (e.g., /org/freedesktop/secrets/aliases/default), so do not compare paths
        if (unlocked.Any()) return true;
        if (promptPath == _noPrompt) return false;

        Log.Debug($"Asking user to unlock {obj} in Secret Service");
        return await RunPromptAsync(connection, promptPath).ConfigureAwait(false);
    }

    /// <summary>
    /// How long to wait for the user to complete a prompt provided by the keyring.
    /// </summary>
    private static readonly TimeSpan _promptTimeout = TimeSpan.FromMinutes(2);

    /// <summary>
    /// Shows a prompt provided by the keyring and waits for the user to complete it.
    /// </summary>
    /// <returns><c>true</c> if the user completed the prompt; <c>false</c> if the user dismissed it or did not respond in time.</returns>
    private static async Task<bool> RunPromptAsync(DBusConnection connection, ObjectPath promptPath)
    {
        var completed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var prompt = new Prompt(connection, BusName, promptPath);
        using (await prompt.WatchCompletedAsync(x => completed.TrySetResult(!x.Dismissed), emitOnCapturedContext: false).ConfigureAwait(false))
        {
            await prompt.PromptAsync("").ConfigureAwait(false);
            Log.Info("Waiting for you to respond to the keyring prompt");
            try
            {
                return await completed.Task.WaitAsync(_promptTimeout).ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                Log.Warn($"Secret Service prompt was not completed within {_promptTimeout.TotalMinutes} minutes");
                try
                {
                    await prompt.DismissAsync().ConfigureAwait(false);
                }
                catch (DBusExceptionBase ex)
                {
                    Log.Debug("Unable to dismiss Secret Service prompt", ex);
                }
                return false;
            }
        }
    }

    /// <summary>
    /// Indicates whether a Secret Service is running or can be activated on the session bus.
    /// </summary>
    internal static bool IsAvailable => _isAvailable.Value;

    private static readonly Lazy<bool> _isAvailable = new(() =>
    {
        try
        {
            return IsAvailableAsync().GetAwaiter().GetResult();
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Debug("Secret Service is not available", ex);
            return false;
        }
        #endregion
    });

    private static async Task<bool> IsAvailableAsync()
    {
        var connection = DBusConnection.Session;
        return (await connection.ListServicesAsync().ConfigureAwait(false)).Contains(BusName)
            || (await connection.ListActivatableServicesAsync().ConfigureAwait(false)).Contains(BusName);
    }
}
#endif
