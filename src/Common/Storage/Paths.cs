// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Security;
using NanoByte.Common.Native;

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
            return Path.Combine(paths);
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
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or SecurityException)
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
    /// Indicates whether <paramref name="c"/> cannot be used in file names on at least one common platform (Windows, Linux, macOS).
    /// </summary>
    /// <remarks>Unlike <see cref="Path.GetInvalidFileNameChars"/> this does not depend on the current platform.</remarks>
    public static bool IsNonPortableFileNameChar(char c)
        => c is '/' or '\\' or ':' or '"' or '*' or '?' or '<' or '>' or '|' || char.IsControl(c);

    private static readonly HashSet<string> _windowsDeviceNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL", "CONIN$", "CONOUT$", "CLOCK$",
        "COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM¹", "COM²", "COM³",
        "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9", "LPT¹", "LPT²", "LPT³"
    };

    /// <summary>
    /// Indicates whether <paramref name="fileName"/> refers to a device on Windows (e.g., <c>NUL</c> or <c>COM1</c>).
    /// </summary>
    /// <param name="fileName">A file name without any directory part.</param>
    /// <remarks>Performs the same checks when run on non-Windows platforms.</remarks>
    public static bool IsReservedDeviceName(string? fileName)
        => fileName != null
        && _windowsDeviceNames.Contains(fileName.GetLeftPartAtFirstOccurrence('.').TrimEnd(' '));

    /// <summary>
    /// Indicates whether <paramref name="fileName"/> can be used as a file name on all common platforms (Windows, Linux, macOS).
    /// </summary>
    /// <param name="fileName">A file name without any directory part.</param>
    public static bool IsPortableFileName([NotNullWhen(true)] string? fileName)
        => !string.IsNullOrEmpty(fileName)
        && fileName[^1] is not ('.' or ' ')
        && !fileName.Any(IsNonPortableFileNameChar)
        && !IsReservedDeviceName(fileName);

    /// <summary>
    /// Determines whether two file-system paths point to the same location.
    /// </summary>
    /// <remarks>Applies path normalization. Does not resolve symlinks. Case-insensitive on Windows and macOS.</remarks>
    [Pure]
    public static bool AreEqual(string? path1, string? path2)
    {
        static string? Normalize(string? path)
        {
            try
            {
                return path?.To(Absolute).TrimEnd(Path.DirectorySeparatorChar);
            }
            catch (IOException)
            {
                return path;
            }
        }

        path1 = Normalize(path1);
        path2 = Normalize(path2);

        return WindowsUtils.IsWindows || UnixUtils.IsMacOSX
            ? StringUtils.EqualsIgnoreCase(path1, path2)
            : path1 == path2;
    }

    /// <summary>
    /// Replaces Unix-style directory slashes with <see cref="Path.DirectorySeparatorChar"/>.
    /// </summary>
    [Pure]
    [return: NotNullIfNotNull(nameof(path))]
    public static string? ToNative(string? path)
        => path?.Replace('/', Path.DirectorySeparatorChar);

    /// <summary>
    /// Replaces <see cref="Path.DirectorySeparatorChar"/> with Unix-style directory slashes.
    /// </summary>
    [Pure]
    [return: NotNullIfNotNull(nameof(path))]
    public static string? ToUnix(string? path)
        => path?.Replace(Path.DirectorySeparatorChar, '/');

    /// <summary>
    /// Determines whether a path might escape its parent directory (by being absolute or using ..).
    /// </summary>
    /// <remarks>
    /// Does not depend on the current platform. Both / and \ are treated as directory separators.
    /// This only inspects the path string. It does not detect symlinks.
    /// </remarks>
    [Pure]
    public static bool IsBreakout([Localizable(false)] string? path)
    {
        if (string.IsNullOrEmpty(path)) return false;

        // Null chars may cause the path to be truncated by native APIs
        if (path[0] is '/' or '\\' || path.Contains('\0')) return true;

        static bool HasDriveLetter(string segment)
            => segment.Length >= 2 && segment[1] == ':' && (segment[0] is >= 'a' and <= 'z' or >= 'A' and <= 'Z');

        // Check all segments for drive letters, because they would cause a breakout if the path is combined one segment at a time
        return path.Split('/', '\\').Any(segment => segment == ".." || HasDriveLetter(segment));
    }

    /// <summary>
    /// Returns a relative path pointing to <paramref name="target"/> from <paramref name="baseRef"/>.
    /// </summary>
    [Pure]
    public static string RelativeTo(this FileSystemInfo target, FileSystemInfo baseRef)
    {
        #region Sanity checks
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (baseRef == null) throw new ArgumentNullException(nameof(baseRef));
        #endregion

        string basePath = baseRef.FullName;
        if (baseRef is DirectoryInfo && !basePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            basePath += Path.DirectorySeparatorChar;

        string targetPath = target.FullName;
        if (target is DirectoryInfo && !targetPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            targetPath += Path.DirectorySeparatorChar;

        var relativeUri = new Uri(basePath).MakeRelativeUri(new Uri(targetPath));
        return ToNative(Uri.UnescapeDataString(relativeUri.ToString()).TrimEnd('/'));
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
