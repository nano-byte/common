// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Threading;

namespace NanoByte.Common.Native;

/// <summary>
/// Provides a cross-process object allowing easy detection of application instances (e.g., for use by installers and update tools).
/// No-op on non-Windows platforms.
/// </summary>
/// <remarks>Use <see cref="Mutex"/> or <see cref="MutexLock"/> instead for synchronizing access to shared resources.</remarks>
public sealed class AppMutex : IDisposable
{
    private readonly List<IntPtr> _handles = new();

    /// <summary>
    /// Creates or opens a mutex to signal that an application is running.
    /// </summary>
    /// <param name="name">The name to be used as a mutex identifier.</param>
    /// <returns>The handle for the mutex. Can be used to close it again. Will automatically be released once the process terminates.</returns>
    public static AppMutex Create([Localizable(false)] string name)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        #endregion

        var appMutex = new AppMutex();

        void TryAdd(string mutexName)
        {
            try
            {
                appMutex._handles.Add(WindowsMutex.Create(mutexName, out _));
            }
            catch (Win32Exception ex)
            {
                Log.Debug($"Failed to create {nameof(AppMutex)} {name}", ex);
            }
        }

        if (WindowsUtils.IsWindowsNT)
        {
            TryAdd(name);
            TryAdd("Global\\" + name);
        }

        return appMutex;
    }

    /// <summary>
    /// Closes the mutex handle, allowing it to be released if no other instances are running.
    /// </summary>
    public void Dispose()
    {
        if (!WindowsUtils.IsWindowsNT) return;

        foreach (var handle in _handles.Where(handle => handle != IntPtr.Zero))
            WindowsMutex.Close(handle);
        _handles.Clear();
    }

    /// <summary>
    /// Checks whether a specific mutex exists (local or global) without opening a lasting handle.
    /// </summary>
    /// <param name="name">The name to be used as a mutex identifier.</param>
    /// <returns><c>true</c> if an existing mutex was found; <c>false</c> if none existed.</returns>
    public static bool Probe([Localizable(false)] string name)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        #endregion

        bool TryProbe(string mutexName)
        {
            try
            {
                return WindowsMutex.Probe(mutexName);
            }
            catch (Win32Exception ex)
            {
                Log.Debug($"Failed to probe {nameof(AppMutex)} {name}", ex);
                return false;
            }
        }

        if (!WindowsUtils.IsWindowsNT) return false;

        return TryProbe(name)
            || TryProbe("Global\\" + name);
    }
}
