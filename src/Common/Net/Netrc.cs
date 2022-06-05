// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Net;
using NanoByte.Common.Storage;

namespace NanoByte.Common.Net;

/// <summary>
/// Represents data loaded from a .netrc file as a map from host names to credentials.
/// </summary>
public class Netrc : Dictionary<string, NetworkCredential>
{
    /// <summary>
    /// The default path for the <c>.netrc</c> file. Usually in the home directory. Can be overriden via <c>NETRC</c> environment variable.
    /// </summary>
    public static string DefaultPath { get; } = Environment.GetEnvironmentVariable("NETRC") ?? Path.Combine(Locations.HomeDir, ".netrc");

    /// <summary>
    /// Loads credentials from a .netrc file.
    /// </summary>
    /// <param name="path">The path of the file to load.</param>
    /// <exception cref="IOException">A problem occurred while loading the file.</exception>
    /// <exception cref="UnauthorizedAccessException">Read access to the file was denied.</exception>
    public static Netrc Load(string path)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        #endregion

        var result = new Netrc();

        string? previousToken = null, machine = null, login = null, password = null;

        void AddCredential()
        {
            if (machine != null && login != null && password != null)
                result.Add(machine, new NetworkCredential(login, password));
        }

        foreach (string token in File.ReadAllText(path)
                                     .Split(new[] {' ', '\n', '\r', '\t'}, StringSplitOptions.RemoveEmptyEntries))
        {
            switch (previousToken)
            {
                case "machine":
                    AddCredential();
                    machine = token;
                    login = null;
                    password = null;
                    break;
                case "login":
                    login = token;
                    break;
                case "password":
                    password = token;
                    break;
            }

            previousToken = previousToken == null ? token : null;
        }

        AddCredential();

        return result;
    }

    /// <summary>
    /// Loads credentials from <see cref="DefaultPath"/>. Catches any exceptions and returns empty <see cref="Netrc"/> instead.
    /// </summary>
    public static Netrc LoadSafe()
    {
        try
        {
            return Load(DefaultPath);
        }
        catch (FileNotFoundException)
        {
            return new();
        }
        catch (Exception ex)
        {
            Log.Warn($"Failed to load .netrc file from {DefaultPath}", ex);
            return new();
        }
    }
}
