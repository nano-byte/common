// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Threading;

#if NET
using System.Security.Cryptography;
using System.Text;
#endif

namespace NanoByte.Common.Native;

/// <summary>
/// Provides a cross-process object allowing easy detection of application instances (e.g., for use by installers and update tools).
/// </summary>
/// <remarks>
/// <para>Use <see cref="Mutex"/> or <see cref="MutexLock"/> instead for synchronizing access to shared resources.</para>
/// <para>Any local user can block a name by holding its machine-wide mutex or lock, which makes <see cref="Probe"/> return <c>true</c>. Do not rely on this class for security.</para>
/// </remarks>
[MustDisposeResource]
public sealed class AppMutex : IDisposable
{
    private readonly List<IntPtr> _handles = [];

#if NET
    /// <summary>
    /// Keeps instances alive until they are disposed. Otherwise, finalizers would release file locks of instances the caller does not hold on to, unlike native mutex handles which remain open until the process terminates.
    /// </summary>
    private static readonly HashSet<AppMutex> _instances = [];

    private readonly List<FileStream> _lockFiles = [];
#endif

    /// <summary>
    /// Creates or opens a mutex to signal that an application is running.
    /// </summary>
    /// <param name="name">The name to be used as a mutex identifier.</param>
    /// <returns>The handle for the mutex. Can be used to close it again. Will automatically be released once the process terminates.</returns>
    [MustDisposeResource]
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
            TryAdd($@"Global\{name}");
        }
#if NET
        else if (UnixUtils.IsUnix)
        {
            foreach (string path in GetLockFilePaths(name))
            {
                try
                {
                    appMutex._lockFiles.Add(LockFile(path));
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    Log.Debug($"Failed to create {nameof(AppMutex)} {name}", ex);
                }
            }

            lock (_instances) _instances.Add(appMutex);
        }
#endif

        return appMutex;
    }

    /// <summary>
    /// Closes the mutex handle, allowing it to be released if no other instances are running.
    /// </summary>
    public void Dispose()
    {
#if NET
        if (UnixUtils.IsUnix)
        {
            foreach (var lockFile in _lockFiles) lockFile.Dispose();
            _lockFiles.Clear();

            lock (_instances) _instances.Remove(this);
        }
#endif

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

#if NET
        if (UnixUtils.IsUnix)
        {
            return GetLockFilePaths(name).Any(path =>
            {
                try
                {
                    return ProbeLockFile(path);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    Log.Debug($"Failed to probe {nameof(AppMutex)} {name}", ex);
                    return false;
                }
            });
        }
#endif

        if (!WindowsUtils.IsWindowsNT) return false;

        return TryProbe(name)
            || TryProbe($@"Global\{name}");
    }

#if NET
    private const int LockAttempts = 5;

    /// <summary>
    /// Gets the paths of the per-user and the machine-wide lock file for a mutex. These are identical if the user has no private directory.
    /// </summary>
    private static IEnumerable<string> GetLockFilePaths(string name)
    {
        // Hash because names may contain directory separators or exceed the maximum file name length
        string fileName = $".appmutex-{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(name)))}";

        string? runtimeDir = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
        string userDir = string.IsNullOrEmpty(runtimeDir) ? Path.GetTempPath() : runtimeDir;

        return new[] {Path.Combine(userDir, fileName), Path.Combine("/tmp", fileName)}.Distinct();
    }

    /// <summary>
    /// Opens a lock file and holds a shared lock on it, creating the file if necessary.
    /// </summary>
    private static FileStream LockFile(string path)
    {
        for (int attempt = 1;; attempt++)
        {
            try
            {
                EnsureRegularFile(path);
                return new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch (FileNotFoundException) when (attempt < LockAttempts)
            {
                CreateLockFile(path);
            }
            catch (IOException) when (attempt < LockAttempts)
            {
                WaitForProbe();
            }
        }
    }

    /// <summary>
    /// Creates an empty lock file that other users can open. Does nothing if the file already exists.
    /// </summary>
    /// <remarks>
    /// This is separate from opening the file because:
    /// Linux may not allow creating opens on files owned by other users in <c>/tmp</c> (<c>fs.protected_regular</c>).
    /// Creating exclusively avoids following symlinks placed there by other users.
    /// </remarks>
    private static void CreateLockFile(string path)
    {
        try
        {
            using (new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite)) {}
        }
        catch (IOException) when (File.Exists(path))
        {
            return; // Another process was faster
        }

        // Set explicitly because the umask may have removed read access for other users
        if (LibC.chmod(path, 0b110_100_100) != 0) throw LibC.GetLastError(path);
    }

    /// <summary>
    /// Checks whether any process holds a lock on a lock file by briefly trying to get an exclusive lock on it.
    /// </summary>
    private static bool ProbeLockFile(string path)
    {
        for (int attempt = 1;; attempt++)
        {
            try
            {
                EnsureRegularFile(path);
                using (new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
                    return false;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
            catch (IOException ex) when (IsLockConflict(ex) && attempt < LockAttempts)
            {
                WaitForProbe();
            }
            catch (IOException ex) when (IsLockConflict(ex))
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Ensures a lock file is a regular file before opening it.
    /// </summary>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">The path does not point to a regular file.</exception>
    private static void EnsureRegularFile(string path)
    {
        if ((LibC.LStat(path).Mode & LibC.S_IFMT) != LibC.S_IFREG)
            throw new UnauthorizedAccessException($"Not a regular file: {path}");
    }

    /// <summary>
    /// Checks whether an exception was caused by a lock held by someone else rather than by another problem with accessing the file.
    /// </summary>
    private static bool IsLockConflict(IOException ex)
        => ex.HResult == LibC.EWOULDBLOCK;

    /// <summary>
    /// Waits for a concurrent <see cref="Probe"/> to release its short-lived exclusive lock, which can otherwise be mistaken for a lock held by a running instance.
    /// </summary>
    private static void WaitForProbe()
        => Thread.Sleep(RandomShared.Instance.Next(5, 25));
#endif
}
