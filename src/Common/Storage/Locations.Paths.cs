// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Reflection;

namespace NanoByte.Common.Storage;

partial class Locations
{
    /// <summary>
    /// Returns a path for storing a configuration resource (can roam across different machines).
    /// </summary>
    /// <param name="appName">The name of application. Used as part of the path, unless <see cref="IsPortable"/> is <c>true</c>.</param>
    /// <param name="isFile"><c>true</c> if the last part of <paramref name="resource"/> refers to a file instead of a directory.</param>
    /// <param name="resource">The path elements (directory and/or file names) of the resource to be stored.</param>
    /// <returns>A fully qualified path to use to store the resource. Directories are guaranteed to already exist; files are not.</returns>
    /// <exception cref="IOException">A problem occurred while creating a directory.</exception>
    /// <exception cref="UnauthorizedAccessException">Creating a directory is not permitted.</exception>
    public static string GetSaveConfigPath([Localizable(false)] string appName, bool isFile, params string[] resource)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(appName)) throw new ArgumentNullException(nameof(appName));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        #endregion

        string resourceCombined = PathCombine(resource);
        string path;
        try
        {
            path = IsPortable
                ? PathCombine(PortableBase, "config", resourceCombined)
                : PathCombine(UserConfigDir, appName, resourceCombined);
        }
        #region Error handling
        catch (ArgumentException ex)
        {
            // Wrap exception to add context information
            throw new IOException(string.Format(Resources.InvalidConfigDir, UserConfigDir) + Environment.NewLine + ex.Message, ex);
        }
        #endregion

        // Ensure the directory part of the path exists
        string dirPath = isFile ? (Path.GetDirectoryName(path) ?? path) : path;
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

        return Path.GetFullPath(path);
    }

    /// <summary>
    /// Returns a path for storing a system-wide configuration resource.
    /// </summary>
    /// <param name="appName">The name of application. Used as part of the path, unless <see cref="IsPortable"/> is <c>true</c>.</param>
    /// <param name="isFile"><c>true</c> if the last part of <paramref name="resource"/> refers to a file instead of a directory.</param>
    /// <param name="resource">The path elements (directory and/or file names) of the resource to be stored.</param>
    /// <returns>A fully qualified path to use to store the resource. Directories are guaranteed to already exist; files are not.</returns>
    /// <exception cref="IOException">A problem occurred while creating a directory.</exception>
    /// <exception cref="UnauthorizedAccessException">Creating a directory is not permitted.</exception>
    public static string GetSaveSystemConfigPath([Localizable(false)] string appName, bool isFile, params string[] resource)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(appName)) throw new ArgumentNullException(nameof(appName));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        #endregion

        if (IsPortable) throw new IOException(Resources.NoSystemConfigInPortableMode);

        string systemConfigDir = SystemConfigDirs.Split(Path.PathSeparator).Last();
        string resourceCombined = PathCombine(resource);
        string path;
        try
        {
            path = PathCombine(systemConfigDir, appName, resourceCombined);
        }
        #region Error handling
        catch (ArgumentException ex)
        {
            // Wrap exception to add context information
            throw new IOException(string.Format(Resources.InvalidConfigDir, systemConfigDir) + Environment.NewLine + ex.Message, ex);
        }
        #endregion

        // Ensure the directory part of the path exists
        string dirPath = isFile ? (Path.GetDirectoryName(path) ?? path) : path;
        CreateSecureMachineWideDir(dirPath);

        return Path.GetFullPath(path);
    }

    /// <summary>
    /// Returns a list of paths for loading a configuration resource.
    /// </summary>
    /// <param name="appName">The name of application. Used as part of the path, unless <see cref="IsPortable"/> is <c>true</c>.</param>
    /// <param name="isFile"><c>true</c> if the last part of <paramref name="resource"/> refers to a file instead of a directory.</param>
    /// <param name="resource">The path elements (directory and/or file names) of the resource to be loaded.</param>
    /// <returns>
    /// A list of fully qualified paths to use to load the resource sorted by decreasing importance.
    /// This list will always reflect the current state in the filesystem and can not be modified! It may be empty.
    /// </returns>
    public static IEnumerable<string> GetLoadConfigPaths([Localizable(false)] string appName, bool isFile, params string[] resource)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(appName)) throw new ArgumentNullException(nameof(appName));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        #endregion

        string resourceCombined = PathCombine(resource);
        string path;
        if (IsPortable)
        {
            // Check in portable base directory
            path = PathCombine(PortableBase, "config", resourceCombined);
            if ((isFile && File.Exists(path)) || (!isFile && Directory.Exists(path)))
                yield return Path.GetFullPath(path);
        }
        else
        {
            // Check in user profile and system directories
            foreach (string dirPath in (UserConfigDir + Path.PathSeparator + SystemConfigDirs).Split(Path.PathSeparator))
            {
                try
                {
                    path = PathCombine(dirPath, appName, resourceCombined);
                }
                #region Error handling
                catch (ArgumentException ex)
                {
                    // Wrap exception to add context information
                    throw new IOException(string.Format(Resources.InvalidConfigDir, dirPath) + Environment.NewLine + ex.Message, ex);
                }
                #endregion

                if ((isFile && File.Exists(path)) || (!isFile && Directory.Exists(path)))
                    yield return Path.GetFullPath(path);
            }
        }
    }

    /// <summary>
    /// Returns a path for storing a data resource (should not roam across different machines).
    /// </summary>
    /// <param name="appName">The name of application. Used as part of the path, unless <see cref="IsPortable"/> is <c>true</c>.</param>
    /// <param name="isFile"><c>true</c> if the last part of <paramref name="resource"/> refers to a file instead of a directory.</param>
    /// <param name="resource">The path elements (directory and/or file names) of the resource to be stored.</param>
    /// <returns>A fully qualified path to use to store the resource. Directories are guaranteed to already exist; files are not.</returns>
    /// <exception cref="IOException">A problem occurred while creating a directory.</exception>
    /// <exception cref="UnauthorizedAccessException">Creating a directory is not permitted.</exception>
    public static string GetSaveDataPath([Localizable(false)] string appName, bool isFile, params string[] resource)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(appName)) throw new ArgumentNullException(nameof(appName));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        #endregion

        string resourceCombined = PathCombine(resource);
        string path;
        try
        {
            path = IsPortable
                ? PathCombine(PortableBase, "data", resourceCombined)
                : PathCombine(UserDataDir, appName, resourceCombined);
        }
        #region Error handling
        catch (ArgumentException ex)
        {
            // Wrap exception to add context information
            throw new IOException(string.Format(Resources.InvalidConfigDir, UserDataDir) + Environment.NewLine + ex.Message, ex);
        }
        #endregion

        // Ensure the directory part of the path exists
        string dirPath = isFile ? (Path.GetDirectoryName(path) ?? path) : path;
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

        return Path.GetFullPath(path);
    }

    /// <summary>
    /// Returns a list of paths for loading a data resource (should not roam across different machines).
    /// </summary>
    /// <param name="appName">The name of application. Used as part of the path, unless <see cref="IsPortable"/> is <c>true</c>.</param>
    /// <param name="isFile"><c>true</c> if the last part of <paramref name="resource"/> refers to a file instead of a directory.</param>
    /// <param name="resource">The path elements (directory and/or file names) of the resource to be loaded.</param>
    /// <returns>
    /// A list of fully qualified paths to use to load the resource sorted by decreasing importance.
    /// This list will always reflect the current state in the filesystem and can not be modified! It may be empty.
    /// </returns>
    public static IEnumerable<string> GetLoadDataPaths([Localizable(false)] string appName, bool isFile, params string[] resource)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(appName)) throw new ArgumentNullException(nameof(appName));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        #endregion

        string resourceCombined = PathCombine(resource);
        string path;
        if (IsPortable)
        {
            // Check in portable base directory
            path = PathCombine(PortableBase, "data", resourceCombined);
            if ((isFile && File.Exists(path)) || (!isFile && Directory.Exists(path)))
                yield return Path.GetFullPath(path);
        }
        else
        {
            // Check in user profile and system directories
            foreach (string dirPath in (UserDataDir + Path.PathSeparator + SystemDataDirs).Split(Path.PathSeparator))
            {
                try
                {
                    path = PathCombine(dirPath, appName, resourceCombined);
                }
                #region Error handling
                catch (ArgumentException ex)
                {
                    // Wrap exception to add context information
                    throw new IOException(string.Format(Resources.InvalidConfigDir, dirPath) + Environment.NewLine + ex.Message, ex);
                }
                #endregion

                if ((isFile && File.Exists(path)) || (!isFile && Directory.Exists(path)))
                    yield return Path.GetFullPath(path);
            }
        }
    }

    /// <summary>
    /// Tries to locate a file either in <see cref="InstallBase"/>, the location of the NanoByte.Common.dll or in the PATH.
    /// </summary>
    /// <param name="fileName">The file name of the file to search for.</param>
    /// <returns>The fully qualified path of the first located instance of the file.</returns>
    /// <exception cref="IOException">The file could not be found.</exception>
    public static string GetInstalledFilePath([Localizable(false)] string fileName)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));
        #endregion

        try
        {
            return new[] {InstallBase, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!}
                  .Concat((Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
                  .Select(x => Path.Combine(x, fileName))
                  .First(File.Exists);
        }
        catch (InvalidOperationException)
        {
            throw new IOException(string.Format(Resources.FileNotFound, fileName));
        }
    }

    /// <summary>
    /// Combines an array of strings into a path.
    /// </summary>
    private static string PathCombine(params string[] paths)
#if NET20
        => (paths.Length == 0) ? "" : paths.Aggregate(Path.Combine);
#else
        => Path.Combine(paths);
#endif
}
