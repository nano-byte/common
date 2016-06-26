/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.IO;
using JetBrains.Annotations;
using NanoByte.Common.Native;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Storage
{
    partial class Locations
    {
        /// <summary>
        /// The home/profile directory of the current user.
        /// </summary>
        [PublicAPI, NotNull]
        public static string HomeDir { get { return Environment.GetEnvironmentVariable(WindowsUtils.IsWindows ? "userprofile" : "HOME") ?? ""; } }

        /// <summary>
        /// The directory to store per-user settings (can roam across different machines).
        /// </summary>
        /// <remarks>On Windows this is <c>%appdata%</c>, on Linux it usually is <c>~/.config</c>.</remarks>
        [PublicAPI, NotNull]
        public static string UserConfigDir
        {
            get
            {
                return GetEnvironmentVariable("XDG_CONFIG_HOME", WindowsUtils.IsWindows
                    ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    : Path.Combine(HomeDir, ".config"));
            }
        }

        /// <summary>
        /// The directory to store per-user data files (should not roam across different machines).
        /// </summary>
        /// <remarks>On Windows this is <c>%localappdata%</c>, on Linux it usually is <c>~/.local/share</c>.</remarks>
        [PublicAPI, NotNull]
        public static string UserDataDir
        {
            get
            {
                return GetEnvironmentVariable("XDG_DATA_HOME", WindowsUtils.IsWindows
                    ? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                    : Path.Combine(HomeDir, ".local/share"));
            }
        }

        /// <summary>
        /// The directory to store per-user non-essential data (should not roam across different machines).
        /// </summary>
        /// <remarks>On Windows this is <c>%localappdata%</c>, on Linux it usually is <c>~/.cache</c>.</remarks>
        [PublicAPI, NotNull]
        public static string UserCacheDir
        {
            get
            {
                return GetEnvironmentVariable("XDG_CACHE_HOME", WindowsUtils.IsWindows
                    ? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                    : Path.Combine(HomeDir, ".cache"));
            }
        }

        /// <summary>
        /// The directories to store machine-wide settings.
        /// </summary>
        /// <returns>Directories separated by <see cref="Path.PathSeparator"/> sorted by decreasing importance.</returns>
        /// <remarks>On Windows this is <c>CommonApplicationData</c>, on Linux it usually is <c>/etc/xdg</c>.</remarks>
        [PublicAPI, NotNull]
        public static string SystemConfigDirs
        {
            get
            {
                return GetEnvironmentVariable("XDG_CONFIG_DIRS", WindowsUtils.IsWindows
                    ? Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                    : "/etc/xdg");
            }
        }

        /// <summary>
        /// The directories to store machine-wide data files (should not roam across different machines).
        /// </summary>
        /// <returns>Directories separated by <see cref="Path.PathSeparator"/> sorted by decreasing importance.</returns>
        /// <remarks>On Windows this is <c>CommonApplicationData</c>, on Linux it usually is <c>/usr/local/share:/usr/share</c>.</remarks>
        [PublicAPI, NotNull]
        public static string SystemDataDirs
        {
            get
            {
                return GetEnvironmentVariable("XDG_DATA_DIRS", WindowsUtils.IsWindows
                    ? Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                    : "/usr/local/share" + Path.PathSeparator + "/usr/share");
            }
        }

        /// <summary>
        /// The directory to store machine-wide non-essential data.
        /// </summary>
        /// <remarks>On Windows this is <c>CommonApplicationData</c>, on Linux it is <c>/var/cache</c>.</remarks>
        [PublicAPI, NotNull]
        public static string SystemCacheDir
        {
            get
            {
                return WindowsUtils.IsWindows
                    ? Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                    : "/var/cache";
            }
        }

        /// <summary>
        /// Returns a path for a cache directory (should not roam across different machines).
        /// </summary>
        /// <param name="appName">The name of application. Used as part of the path, unless <see cref="IsPortable"/> is <c>true</c>.</param>
        /// <param name="machineWide"><c>true</c> if the directory should be machine-wide.</param>
        /// <param name="resource">The directory name of the resource to be stored.</param>
        /// <returns>A fully qualified directory path. The directory is guaranteed to already exist.</returns>
        /// <exception cref="IOException">A problem occurred while creating a directory.</exception>
        /// <exception cref="UnauthorizedAccessException">Creating a directory is not permitted.</exception>
        [PublicAPI, NotNull]
        public static string GetCacheDirPath([NotNull, Localizable(false)] string appName, bool machineWide, [NotNull, ItemNotNull] params string[] resource)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(appName)) throw new ArgumentNullException("appName");
            if (resource == null) throw new ArgumentNullException("resource");
            #endregion

            string resourceCombined = FileUtils.PathCombine(resource);
            string appPath;
            try
            {
                if (machineWide)
                {
                    appPath = Path.Combine(SystemCacheDir, appName);
                    CreateSecureMachineWideDir(appPath);
                }
                else
                {
                    appPath = _isPortable
                        ? Path.Combine(_portableBase, "cache")
                        : Path.Combine(UserCacheDir, appName);
                }
            }
                #region Error handling
            catch (ArgumentException ex)
            {
                // Wrap exception to add context information
                throw new IOException(string.Format(Resources.InvalidConfigDir, UserCacheDir) + Environment.NewLine + ex.Message, ex);
            }
            #endregion

            string path = Path.Combine(appPath, resourceCombined);

            // Ensure the directory exists
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Returns a path for a directory that can safley be used for desktop integration. It ignores <see cref="IsPortable"/>.
        /// </summary>
        /// <param name="appName">The name of application. Used as part of the path.</param>
        /// <param name="machineWide"><c>true</c> if the directory should be machine-wide and machine-specific instead of roaming with the user profile.</param>
        /// <param name="resource">The directory name of the resource to be stored.</param>
        /// <returns>A fully qualified directory path. The directory is guaranteed to already exist.</returns>
        /// <exception cref="IOException">A problem occurred while creating a directory.</exception>
        /// <exception cref="UnauthorizedAccessException">Creating a directory is not permitted.</exception>
        /// <remarks>If a new directory is created with <paramref name="machineWide"/> set to <c>true</c> on Windows, ACLs are set to deny write access for non-Administrator users.</remarks>
        [PublicAPI, NotNull]
        public static string GetIntegrationDirPath([NotNull, Localizable(false)] string appName, bool machineWide, [NotNull, ItemNotNull] params string[] resource)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(appName)) throw new ArgumentNullException("appName");
            if (resource == null) throw new ArgumentNullException("resource");
            #endregion

            string resourceCombined = FileUtils.PathCombine(resource);
            string appPath = Path.Combine(
                Environment.GetFolderPath(machineWide ? Environment.SpecialFolder.CommonApplicationData : Environment.SpecialFolder.ApplicationData),
                appName);
            if (machineWide) CreateSecureMachineWideDir(appPath);
            string path = Path.Combine(appPath, resourceCombined);

            // Ensure the directory exists
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            return Path.GetFullPath(path);
        }
    }
}
