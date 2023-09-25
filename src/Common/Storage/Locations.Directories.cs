// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Native;
using static System.IO.Path;

namespace NanoByte.Common.Storage;

partial class Locations
{
    /// <summary>
    /// The home/profile directory of the current user.
    /// </summary>
    public static string HomeDir => WindowsUtils.IsWindows
        ? Environment.GetEnvironmentVariable("USERPROFILE") ?? Combine(@"C:\Users", Environment.UserName)
        : Environment.GetEnvironmentVariable("HOME") ?? Combine("/home", Environment.UserName);

    /// <summary>
    /// The directory to store per-user settings (can roam across different machines).
    /// </summary>
    /// <remarks>On Windows this is <c>%appdata%</c>, on Linux it usually is <c>~/.config</c>.</remarks>
    public static string UserConfigDir => GetEnvironmentVariable("XDG_CONFIG_HOME", WindowsUtils.IsWindows
        ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        : Path.Combine(HomeDir, ".config"));

    /// <summary>
    /// The directory to store per-user data files (should not roam across different machines).
    /// </summary>
    /// <remarks>On Windows this is <c>%localappdata%</c>, on Linux it usually is <c>~/.local/share</c>.</remarks>
    public static string UserDataDir => GetEnvironmentVariable("XDG_DATA_HOME", WindowsUtils.IsWindows
        ? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        : Path.Combine(HomeDir, ".local/share"));

    /// <summary>
    /// The directory to store per-user non-essential data (should not roam across different machines).
    /// </summary>
    /// <remarks>On Windows this is <c>%localappdata%</c>, on Linux it usually is <c>~/.cache</c>.</remarks>
    public static string UserCacheDir => GetEnvironmentVariable("XDG_CACHE_HOME", WindowsUtils.IsWindows
        ? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        : Path.Combine(HomeDir, ".cache"));

    /// <summary>
    /// The directories to store machine-wide settings.
    /// </summary>
    /// <returns>Directories separated by <see cref="Path.PathSeparator"/> sorted by decreasing importance.</returns>
    /// <remarks>On Windows this is <c>CommonApplicationData</c>, on Linux it usually is <c>/etc/xdg</c>.</remarks>
    public static string SystemConfigDirs => GetEnvironmentVariable("XDG_CONFIG_DIRS", WindowsUtils.IsWindows
        ? Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
        : "/etc/xdg");

    /// <summary>
    /// The directories to store machine-wide data files (should not roam across different machines).
    /// </summary>
    /// <returns>Directories separated by <see cref="Path.PathSeparator"/> sorted by decreasing importance.</returns>
    /// <remarks>On Windows this is <c>CommonApplicationData</c>, on Linux it usually is <c>/usr/local/share:/usr/share</c>.</remarks>
    public static string SystemDataDirs => GetEnvironmentVariable("XDG_DATA_DIRS", WindowsUtils.IsWindows
        ? Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
        : "/usr/local/share" + Path.PathSeparator + "/usr/share");

    /// <summary>
    /// The directory to store machine-wide non-essential data.
    /// </summary>
    /// <remarks>On Windows this is <c>CommonApplicationData</c>, on Linux it is <c>/var/cache</c>.</remarks>
    public static string SystemCacheDir => WindowsUtils.IsWindows
        ? Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
        : "/var/cache";

    /// <summary>
    /// Returns a path for a cache directory (should not roam across different machines).
    /// </summary>
    /// <param name="appName">The name of application. Used as part of the path, unless <see cref="IsPortable"/> is <c>true</c>.</param>
    /// <param name="machineWide"><c>true</c> if the directory should be machine-wide.</param>
    /// <param name="resource">The directory name of the resource to be stored.</param>
    /// <returns>A fully qualified directory path. The directory is guaranteed to already exist.</returns>
    /// <exception cref="IOException">A problem occurred while creating a directory.</exception>
    /// <exception cref="UnauthorizedAccessException">Creating a directory is not permitted.</exception>
    public static string GetCacheDirPath([Localizable(false)] string appName, bool machineWide, params string[] resource)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(appName)) throw new ArgumentNullException(nameof(appName));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        #endregion

        string resourceCombined = PathCombine(resource);
        string appPath;
        try
        {
            if (machineWide)
            {
                appPath = Combine(SystemCacheDir, appName);
                CreateSecureMachineWideDir(appPath);
            }
            else
            {
                appPath = IsPortable
                    ? Combine(PortableBase, "cache")
                    : Combine(UserCacheDir, appName);
            }
        }
        #region Error handling
        catch (ArgumentException ex)
        {
            // Wrap exception to add context information
            throw new IOException(string.Format(Resources.InvalidConfigDir, UserCacheDir) + Environment.NewLine + ex.Message, ex);
        }
        #endregion

        string path = Combine(appPath, resourceCombined);

        // Ensure the directory exists
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        return GetFullPath(path);
    }
}
