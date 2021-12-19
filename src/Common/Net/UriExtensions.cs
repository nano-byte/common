// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Net;

/// <summary>
/// Provides extension methods for <see cref="Uri"/>s.
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// An alternate version of <see cref="Uri.ToString"/> that produces results escaped according to RFC 2396.
    /// </summary>
    [Pure]
    public static string ToStringRfc(this Uri uri)
    {
        #region Sanity checks
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        #endregion

        return (uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString)
              .Replace("%7B", "{").Replace("%7D", "}"); // Leave { and } unescaped for templating support
    }

    /// <summary>
    /// Adds a trailing slash to the URI if it does not already have one.
    /// </summary>
    [Pure]
    public static Uri EnsureTrailingSlash(this Uri uri)
    {
        #region Sanity checks
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        #endregion

        string escapedString = uri.ToStringRfc();
        return escapedString.EndsWith("/")
            ? uri
            : new Uri(escapedString + "/", uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
    }

    /// <summary>
    /// Reparses a URI (generated via conversion) to ensure it is a valid absolute URI.
    /// </summary>
    [Pure]
    public static Uri ReparseAsAbsolute(this Uri uri)
    {
        #region Sanity checks
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        #endregion

        return new Uri(uri.OriginalString, UriKind.Absolute);
    }

    /// <summary>
    /// Extracts the file-name portion of an URI and ensures it is a valid file-name on the local OS.
    /// </summary>
    [Pure]
    public static string GetLocalFileName(this Uri uri)
    {
        #region Sanity checks
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        #endregion

        string fileName = Path.GetFileName(uri.LocalPath).RemoveCharacters(Path.GetInvalidFileNameChars());
        if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(Path.GetDirectoryName(uri.LocalPath)!).RemoveCharacters(Path.GetInvalidFileNameChars());
        if (string.IsNullOrEmpty(fileName)) fileName = "file.ext";

        return fileName;
    }

    /// <summary>
    /// Extracts the base part of an URI, i.e., the part that is used for resolving relative URIs.
    /// </summary>
    [Pure]
    public static Uri GetBaseUri(this Uri uri)
    {
        #region Sanity checks
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        #endregion

        return new Uri(
            new Uri(uri, new Uri("-", UriKind.Relative)).ToStringRfc()[..^1],
            uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
    }
}