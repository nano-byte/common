// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

/// <summary>
/// Helper methods for resolving file system paths.
/// </summary>
public static class Paths
{
    /// <summary>
    /// Combines an array of strings into a path.
    /// </summary>
    /// <exception cref="IOException">One of the strings in the array contains invalid characters.</exception>
    public static string Combine(params string[] paths)
    {
        try
        {
#if NET20
            return (paths.Length == 0) ? "" : paths.Aggregate(Path.Combine);
#else
            return Path.Combine(paths);
#endif
        }
        #region Error handling
        catch (ArgumentException ex)
        {
            // Wrap exception since only certain exception types are allowed
            throw new IOException(ex.Message, ex);
        }
        #endregion
    }

    /// <summary>
    /// Returns the absolute path for the specified path string.
    /// </summary>
    /// <exception cref="IOException">The <paramref name="path"/> contains invalid characters.</exception>
    public static string Absolute(string path)
    {
        try
        {
            return Path.GetFullPath(path);
        }
        #region Error handling
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
        {
            // Wrap exception since only certain exception types are allowed
            throw new IOException(ex.Message, ex);
        }
        #endregion
    }

    /// <summary>
    /// Indicates whether the specified path string is an absolute path.
    /// </summary>
    /// <exception cref="IOException">The <paramref name="path"/> contains invalid characters.</exception>
    public static bool IsAbsolute(string path)
    {
        try
        {
            return Path.IsPathRooted(path);
        }
        #region Error handling
        catch (ArgumentException ex)
        {
            // Wrap exception since only certain exception types are allowed
            throw new IOException(ex.Message, ex);
        }
        #endregion
    }

    /// <summary>
    /// Returns the absolute path of the parent directory of the specified path string.
    /// </summary>
    /// <exception cref="IOException">The <paramref name="path"/> contains invalid characters.</exception>
    public static string Parent(string path)
    {
        try
        {
            string absolute = Absolute(path);
            return Path.GetDirectoryName(absolute) ?? absolute;
        }
        #region Error handling
        catch (ArgumentException ex)
        {
            // Wrap exception since only certain exception types are allowed
            throw new IOException(ex.Message, ex);
        }
        #endregion
    }

    /// <summary>
    /// Returns the file name portion of the specified path string.
    /// </summary>
    /// <exception cref="IOException">The <paramref name="path"/> contains invalid characters.</exception>
    public static string FileName(string path)
    {
        try
        {
            return Path.GetFileName(path);
        }
        #region Error handling
        catch (ArgumentException ex)
        {
            // Wrap exception since only certain exception types are allowed
            throw new IOException(ex.Message, ex);
        }
        #endregion
    }

    /// <summary>
    /// Resolves paths to absolute file paths with wildcard support.
    /// </summary>
    /// <param name="paths">The paths to resolve.</param>
    /// <param name="defaultPattern">The default pattern to use for finding files when a directory is specified.</param>
    /// <returns>Handles to all matching files that were found</returns>
    /// <exception cref="FileNotFoundException">A file that was explicitly specified in <paramref name="paths"/> (no wildcards) could not be found.</exception>
    /// <remarks><paramref name="paths"/> are first interpreted as files, then as directories. Directories are searched using the <paramref name="defaultPattern"/>. * and ? characters are considered as wildcards.</remarks>
    public static IList<FileInfo> ResolveFiles([InstantHandle] IEnumerable<string> paths, [Localizable(false)] string defaultPattern = "*")
    {
        #region Sanity checks
        if (paths == null) throw new ArgumentNullException(nameof(paths));
        if (string.IsNullOrEmpty(defaultPattern)) throw new ArgumentNullException(nameof(defaultPattern));
        #endregion

        var result = new List<FileInfo>();

        foreach (string entry in paths)
        {
            if (entry.Contains('*') || entry.Contains('?'))
            {
                string dewildcardedPath = entry.Replace("*", "x").Replace("?", "x");
                string directory = Path.GetDirectoryName(Absolute(dewildcardedPath)) ?? Directory.GetCurrentDirectory();
                string filePattern = FileName(entry).EmptyAsNull() ?? defaultPattern;
                result.AddRange(Directory.GetFiles(directory, filePattern).Select(file => new FileInfo(Absolute(file))));
            }
            else if (File.Exists(entry)) result.Add(new(Absolute(entry)));
            else if (Directory.Exists(entry))
                result.AddRange(Directory.GetFiles(entry, defaultPattern).Select(file => new FileInfo(file)));
            else throw new FileNotFoundException(string.Format(Resources.FileNotFound, entry), entry);
        }

        return result;
    }
}
