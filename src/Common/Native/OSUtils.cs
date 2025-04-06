// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

#if NET
using System.Threading.Tasks;
using NanoByte.Common.Info;
using Tmds.DBus;
#endif

namespace NanoByte.Common.Native;

/// <summary>
/// Provides helper methods related to operating system functionality across multiple platforms.
/// </summary>
public static class OSUtils
{
    private static readonly Regex
        _envVariableLongStyle = new(@"\${([^{}]+)}"),
        _envVariableShortStyle = new(@"\$([^{}\$\s\\/-]+)");

    /// <summary>
    /// Expands/substitutes any Unix-style environment variables in the string.
    /// </summary>
    /// <param name="value">The string containing variables to be expanded.</param>
    /// <param name="variables">The list of variables available for expansion.</param>
    /// <remarks>Supports default values for unset variables (<c>${VAR-default}</c>) and for unset or empty variables (<c>${VAR:-default}</c>).</remarks>
    public static string ExpandVariables(string value, IDictionary<string, string?> variables)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (variables == null) throw new ArgumentNullException(nameof(variables));

        string intermediate = _envVariableLongStyle.Replace(value, x =>
        {
            if (x.Index >= 1 && value[x.Index - 1] == '$') // Treat $$ as escaping
                return "{" + x.Groups[1].Value + "}";

            var parts = x.Groups[1].Value.Split([":-"], StringSplitOptions.None);
            if (variables.TryGetValue(parts[0], out string? ret) && !string.IsNullOrEmpty(ret))
                return ret;
            else if (parts.Length > 1)
                return StringUtils.Join(":-", parts.Skip(1));
            else
            {
                parts = x.Groups[1].Value.Split('-');
                if (variables.TryGetValue(parts[0], out ret) && ret != null)
                    return ret;
                else if (parts.Length > 1)
                    return StringUtils.Join("-", parts.Skip(1));
                else
                {
                    Log.Info($"Variable '{parts[0]}' not set. Defaulting to empty string.");
                    return "";
                }
            }
        });

        return _envVariableShortStyle.Replace(intermediate, x =>
        {
            if (x.Index >= 1 && value[x.Index - 1] == '$') // Treat $$ as escaping
                return x.Groups[1].Value;

            string key = x.Groups[1].Value;
            if (variables.TryGetValue(key, out string? ret) && ret != null)
                return ret;
            else
            {
                Log.Info($"Variable '{key}' not set. Defaulting to empty string.");
                return "";
            }
        });
    }

    /// <summary>
    /// Expands/substitutes any Unix-style environment variables in the string.
    /// </summary>
    /// <param name="value">The string containing variables to be expanded.</param>
    /// <param name="variables">The list of variables available for expansion.</param>
    /// <remarks>Supports default values for unset variables (<c>${VAR-default}</c>) and for unset or empty variables (<c>${VAR:-default}</c>).</remarks>
    public static string ExpandVariables(string value, StringDictionary variables)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (variables == null) throw new ArgumentNullException(nameof(variables));

        var dictionary = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (string key in variables.Keys)
            dictionary[key] = variables[key]!;

        return ExpandVariables(value, dictionary);
    }

    /// <summary>
    /// Asks the operating system not to enter idle mode.
    /// Useful to avoid standby or hibernation during a long-running task.
    /// </summary>
    /// <param name="reason">Why the system should not enter idle mode.</param>
    /// <returns>Call <see cref="IDisposable.Dispose"/> to restore the original state.</returns>
    [MustDisposeResource]
    public static IDisposable? PreventIdle(string reason)
    {
        try
        {
            if (WindowsUtils.IsWindowsNT) return KeepAwakeWindows(NativeMethods.ExecutionState.SystemRequired);

#if NET
            if (UnixUtils.IsLinux && Guards.NotTrimmed)
            {
                var login = Connection.System.CreateProxy<IManager>("org.freedesktop.login1", "/org/freedesktop/login1");
                return login.InhibitAsync("idle", AppInfo.Current.Name ?? "App", reason, "block").Result;
            }
#endif
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Debug("Problem keeping system awake", ex);
        }
        #endregion

        return null;
    }

    /// <summary>
    /// Asks the operating system not to turn off the display.
    /// </summary>
    /// <param name="reason">Why the display should not be turned off.</param>
    /// <returns>Call <see cref="IDisposable.Dispose"/> to restore the original state.</returns>
    [MustDisposeResource]
    public static IDisposable? PreventDisplayOff(string reason)
    {
        try
        {
            if (WindowsUtils.IsWindowsNT) return KeepAwakeWindows(NativeMethods.ExecutionState.DisplayRequired);

#if NET
            if (UnixUtils.IsLinux && Guards.NotTrimmed)
            {
                var screenSaver = Connection.Session.CreateProxy<IScreenSaver>("org.freedesktop.ScreenSaver", "/org/freedesktop/ScreenSaver");
                uint cookie = screenSaver.InhibitAsync(AppInfo.Current.Name ?? "App", reason).Result;
                return new Disposable(() => screenSaver.UnInhibitAsync(cookie));
            }
#endif
        }
        #region Error handling
        catch (Exception ex)
        {
            Log.Debug("Problem keeping display awake", ex);
        }
        #endregion

        return null;
    }

    [SupportedOSPlatform("windows")]
    [MustDisposeResource]
    private static IDisposable? KeepAwakeWindows(NativeMethods.ExecutionState state)
    {
        var previous = NativeMethods.SetThreadExecutionState(NativeMethods.ExecutionState.Continuous | state);
        return Marshal.GetLastWin32Error() == 0 ? new Disposable(() => NativeMethods.SetThreadExecutionState(previous)) : null;
    }

    private static class NativeMethods
    {
        [Flags]
        public enum ExecutionState : uint
        {
            Continuous = 0x80000000,
            SystemRequired = 0x00000001,
            DisplayRequired = 0x00000002
        }

        [DllImport("kernel32", SetLastError = true)]
        public static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);
    }

#if NET
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("ReSharper", "InconsistentNaming")]
    [DBusInterface("org.freedesktop.login1.Manager")]
    internal interface IManager : IDBusObject
    {
        Task<CloseSafeHandle> InhibitAsync(string What, string Who, string Why, string Mode);
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("ReSharper", "InconsistentNaming")]
    [DBusInterface("org.freedesktop.ScreenSaver")]
    internal interface IScreenSaver : IDBusObject
    {
        Task<uint> InhibitAsync(string ApplicationName, string ReasonForInhibit);
        Task UnInhibitAsync(uint Cookie);
    }
#endif
}
