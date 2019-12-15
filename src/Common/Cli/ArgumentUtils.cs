// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Cli
{
    /// <summary>
    /// Provides helper methods for for parsing command-line arguments.
    /// </summary>
    public static class ArgumentUtils
    {
        /// <summary>
        /// Parses command-line arguments as file paths including wildcard support.
        /// </summary>
        /// <param name="args">The arguments to parse.</param>
        /// <param name="defaultPattern">The default pattern to use for finding files when a directory is specified.</param>
        /// <returns>Handles to all matching files that were found</returns>
        /// <exception cref="FileNotFoundException">A file that was explicitly specified in <paramref name="args"/> (no wildcards) could not be found.</exception>
        /// <remarks><paramref name="args"/> are first interpreted as files, then as directories. Directories are searched using the <paramref name="defaultPattern"/>. * and ? characters are considered as wildcards.</remarks>
        public static IList<FileInfo> GetFiles(IEnumerable<string> args, [Localizable(false)] string defaultPattern = "*")
        {
            #region Sanity checks
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (string.IsNullOrEmpty(defaultPattern)) throw new ArgumentNullException(nameof(defaultPattern));
            #endregion

            var result = new List<FileInfo>();

            foreach (string entry in args)
            {
                if (entry.Contains("*") || entry.Contains("?"))
                {
                    string dewildcardedPath = entry.Replace("*", "x").Replace("?", "x");
                    string directory = Path.GetDirectoryName(Path.GetFullPath(dewildcardedPath)) ?? Directory.GetCurrentDirectory();
                    string filePattern = Path.GetFileName(entry);
                    if (string.IsNullOrEmpty(filePattern)) filePattern = defaultPattern;
                    result.AddRange(Directory.GetFiles(directory, filePattern).Select(file => new FileInfo(Path.GetFullPath(file))));
                }
                else if (File.Exists(entry)) result.Add(new FileInfo(Path.GetFullPath(entry)));
                else if (Directory.Exists(entry))
                    result.AddRange(Directory.GetFiles(entry, defaultPattern).Select(file => new FileInfo(file)));
                else throw new FileNotFoundException(string.Format(Resources.FileNotFound, entry), entry);
            }

            return result;
        }
    }
}
