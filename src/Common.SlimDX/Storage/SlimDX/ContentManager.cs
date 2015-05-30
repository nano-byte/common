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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using JetBrains.Annotations;
using NanoByte.Common.Collections;
using NanoByte.Common.Properties;
using NanoByte.Common.Streams;

namespace NanoByte.Common.Storage.SlimDX
{
    /// <summary>
    /// Provides a virtual file system for combining data from multiple directories and archives (useful for modding).
    /// </summary>
    public static class ContentManager
    {
        #region Constants
        /// <summary>
        /// The file extensions of content archives.
        /// </summary>
        [PublicAPI]
        public const string ArchiveFileExt = ".pk5";

        /// <summary>
        /// The name of an environment variable that can be used to configure the content manager externally.
        /// </summary>
        [PublicAPI]
        public const string
            EnvVarNameBaseDir = "CONTENTMANAGER_BASE_DIR",
            EnvVarNameBaseArchives = "CONTENTMANAGER_BASE_ARCHIVES",
            EnvVarNameModDir = "CONTENTMANAGER_MOD_DIR",
            EnvVarNamerModArchives = "CONTENTMANAGER_MOD_ARCHIVES";
        #endregion

        #region Variables
        private static readonly string
            _envVarBaseDir = Environment.GetEnvironmentVariable(EnvVarNameBaseDir),
            _envVarBaseArchives = Environment.GetEnvironmentVariable(EnvVarNameBaseArchives),
            _envVarModDir = Environment.GetEnvironmentVariable(EnvVarNameModDir),
            _envVarModArchives = Environment.GetEnvironmentVariable(EnvVarNamerModArchives);

        private static DirectoryInfo
            _baseDir = new DirectoryInfo(_envVarBaseDir ?? Path.Combine(Locations.InstallBase, "content")),
            _modDir = (_envVarModDir == null) ? null : new DirectoryInfo(_envVarModDir);

        private static readonly List<ZipFile> _loadedArchives = new List<ZipFile>();

        private static readonly Dictionary<string, ContentArchiveEntry>
            _baseArchiveEntries = new Dictionary<string, ContentArchiveEntry>(StringComparer.OrdinalIgnoreCase),
            _modArchiveEntries = new Dictionary<string, ContentArchiveEntry>(StringComparer.OrdinalIgnoreCase);
        #endregion

        #region Properties
        /// <summary>
        /// The base directory where all the content files are stored; should not be <see langword="null"/>.
        /// </summary>
        /// <remarks>Can be set externally with <see cref="EnvVarNameBaseDir"/>.</remarks>
        /// <exception cref="DirectoryNotFoundException">The specified directory could not be found.</exception>
        public static DirectoryInfo BaseDir
        {
            get { return _baseDir; }
            set
            {
                if (value != null && !value.Exists)
                    throw new DirectoryNotFoundException(Resources.NotFoundGameContentDir + "\n" + value.FullName);
                _baseDir = value;
            }
        }

        /// <summary>
        /// A directory overriding the base directory for creating mods; can be <see langword="null"/>.
        /// </summary>
        /// <remarks>Can be set externally with <see cref="EnvVarNameModDir"/>.</remarks>
        /// <exception cref="DirectoryNotFoundException">The specified directory could not be found.</exception>
        public static DirectoryInfo ModDir
        {
            get { return _modDir; }
            set
            {
                if (value != null && !value.Exists)
                    throw new DirectoryNotFoundException(Resources.NotFoundModContentDir + "\n" + value.FullName);
                _modDir = value;
            }
        }
        #endregion

        //--------------------//

        #region Load archives
        /// <summary>
        /// Loads any <see cref="ArchiveFileExt"/> archives in <see cref="BaseDir"/> and <see cref="ModDir"/> or specified by <see cref="EnvVarNameBaseArchives"/> or <see cref="EnvVarNamerModArchives"/>.
        /// </summary>
        public static void LoadArchives()
        {
            if (_loadedArchives.Count != 0) throw new InvalidOperationException(Resources.ContentArchivesAlreadyLoaded);

            if (_envVarBaseArchives != null)
            {
                foreach (string path in _envVarBaseArchives.Split(Path.PathSeparator))
                    LoadArchive(path, _baseArchiveEntries);
            }
            foreach (string path in BaseDir.GetFiles("*" + ArchiveFileExt).Select(x => x.FullName))
                LoadArchive(path, _baseArchiveEntries);

            if (_envVarModArchives != null)
            {
                foreach (string path in _envVarModArchives.Split(Path.PathSeparator))
                    LoadArchive(path, _modArchiveEntries);
            }
            if (ModDir != null)
            {
                foreach (string path in ModDir.GetFiles("*" + ArchiveFileExt).Select(x => x.FullName))
                    LoadArchive(path, _modArchiveEntries);
            }
        }

        private static void LoadArchive(string path, Dictionary<string, ContentArchiveEntry> archiveEntries)
        {
            Log.Info("Load data archive: " + path);
            var zipFile = new ZipFile(path);
            foreach (ZipEntry zipEntry in zipFile)
                archiveEntries.AddEntry(zipEntry, zipFile);
            _loadedArchives.Add(zipFile);
        }

        private static void AddEntry(this Dictionary<string, ContentArchiveEntry> dictionary, ZipEntry zipEntry, ZipFile zipFile)
        {
            if (!zipEntry.IsFile) return;
            Debug.Assert(zipEntry.Name != null);
            string filename = FileUtils.UnifySlashes(zipEntry.Name);

            // Overwrite existing entries
            if (dictionary.ContainsKey(filename)) _baseArchiveEntries.Remove(filename);
            dictionary.Add(filename, new ContentArchiveEntry(zipFile, zipEntry));
        }
        #endregion

        #region Close archives
        /// <summary>
        /// Closes the content archives loaded by <see cref="LoadArchives"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Errors on shutdown because of an inconsistent state are useless and annoying")]
        public static void CloseArchives()
        {
            _baseArchiveEntries.Clear();
            _modArchiveEntries.Clear();

            foreach (var archive in _loadedArchives)
            {
                Log.Info("Close archive: " + archive.Name);
                try
                {
                    archive.Close();
                }
                    // TODO: Catch only specifc exception types
                catch
                {}
            }
            _loadedArchives.Clear();
        }
        #endregion

        //--------------------//

        #region Create directory path
        /// <summary>
        /// Creates a path for a content directory (using the <see cref="ModDir"/> if available).
        /// </summary>
        /// <param name="type">The type of file (e.g. Textures, Sounds, ...).</param>
        /// <returns>The absolute path to the requested directory.</returns>
        /// <exception cref="DirectoryNotFoundException">The specified directory could not be found.</exception>
        [NotNull]
        public static string CreateDirPath([NotNull, Localizable(false)] string type)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException("type");
            #endregion

            type = FileUtils.UnifySlashes(type);

            // Use mod directory if available
            string pathBase;
            if (ModDir != null) pathBase = ModDir.FullName;
            else if (BaseDir != null) pathBase = _baseDir.FullName;
            else throw new DirectoryNotFoundException(Resources.NotFoundGameContentDir + "\n-");

            // Check the path before returning it
            var directory = new DirectoryInfo(Path.Combine(pathBase, type));
            if (!directory.Exists) directory.Create();
            return directory.FullName;
        }
        #endregion

        #region Create file path
        /// <summary>
        /// Creates a path for a content file (using <see cref="ModDir"/> if available).
        /// </summary>
        /// <param name="type">The type of file (e.g. Textures, Sounds, ...).</param>
        /// <param name="id">The file name of the content.</param>
        /// <returns>The absolute path to the requested content file.</returns>
        [NotNull]
        public static string CreateFilePath([NotNull, Localizable(false)] string type, [NotNull, Localizable(false)] string id)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            #endregion

            type = FileUtils.UnifySlashes(type);
            id = FileUtils.UnifySlashes(id);
            return Path.Combine(CreateDirPath(type), id);
        }
        #endregion

        #region File exists
        /// <summary>
        /// Checks whether a certain content file exists.
        /// </summary>
        /// <param name="type">The type of file (e.g. Textures, Sounds, ...).</param>
        /// <param name="id">The file name of the content.</param>
        /// <param name="searchArchives">Whether to search for the file in archives as well.</param>
        /// <returns><see langword="true"/> if the requested content file exists.</returns>
        public static bool FileExists([NotNull, Localizable(false)] string type, [NotNull, Localizable(false)] string id, bool searchArchives = true)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            #endregion

            type = FileUtils.UnifySlashes(type);
            id = FileUtils.UnifySlashes(id);
            string fullID = Path.Combine(type, id);

            if (ModDir != null && File.Exists(Path.Combine(ModDir.FullName, fullID)))
                return true;
            if (BaseDir != null && File.Exists(Path.Combine(BaseDir.FullName, fullID)))
                return true;
            return searchArchives && _baseArchiveEntries.ContainsKey(fullID);
        }
        #endregion

        #region Get file list

        #region Helpers
        /// <summary>
        /// Adds a specific file to the <paramref name="files"/> list.
        /// </summary>
        /// <param name="files">The collection to add the file to.</param>
        /// <param name="type">The type-subdirectory the file belongs to.</param>
        /// <param name="name">The file name to be added to the list.</param>
        /// <param name="flagAsMod">Set to <see langword="true"/> when handling mod files to detect added and changed files.</param>
        private static void AddFileToList(NamedCollection<FileEntry> files, string type, string name, bool flagAsMod)
        {
            if (flagAsMod)
            {
                // Detect whether this is a new file or a replacement for an existing one
                if (files.Contains(name))
                {
                    var previousEntry = files[name];

                    // Only mark as modified if the pre-existing file isn't already a mod file itself
                    if (previousEntry.EntryType == FileEntryType.Normal)
                    {
                        files.Remove(previousEntry);
                        files.Add(new FileEntry(type, name, FileEntryType.Modified));
                    }
                }
                else files.Add(new FileEntry(type, name, FileEntryType.Added));
            }
            else
            {
                // Prevent duplicate entries
                if (!files.Contains(name)) files.Add(new FileEntry(type, name));
            }
        }

        /// <summary>
        /// Recursively finds all files in <paramref name="directory"/> ending with <paramref name="extension"/> and adds them to the <paramref name="files"/> list.
        /// </summary>
        /// <param name="files">The collection to add the found files to.</param>
        /// <param name="type">The type-subdirectory the files belong to.</param>
        /// <param name="extension">The file extension to look for.</param>
        /// <param name="directory">The directory to look in.</param>
        /// <param name="prefix">A prefix to add before the file name in the list (used to indicate current sub-directory).</param>
        /// <param name="flagAsMod">Set to <see langword="true"/> when handling mod files to detect added and changed files.</param>
        private static void AddDirectoryToList(NamedCollection<FileEntry> files, string type, string extension, DirectoryInfo directory, string prefix, bool flagAsMod)
        {
            // Add the files in this directory to the list
            foreach (FileInfo file in directory.GetFiles("*" + extension))
                AddFileToList(files, type, prefix + file.Name, flagAsMod);

            // Recursively call this method for all sub-directories
            foreach (DirectoryInfo subDir in directory.GetDirectories()
                // Don't add dot directories (e.g. .svn)
                .Where(subDir => !subDir.Name.StartsWith(".")))
                AddDirectoryToList(files, type, extension, subDir, prefix + subDir.Name + Path.DirectorySeparatorChar, flagAsMod);
        }

        /// <summary>
        /// Finds all files in <paramref name="archiveData"/> ending with <paramref name="extension"/> and adds them to the <paramref name="files"/> collection
        /// </summary>
        /// <param name="files">The collection to add the found files to.</param>
        /// <param name="extension">The file extension to look for.</param>
        /// <param name="type">The type-subdirectory to look in.</param>
        /// <param name="archiveData">The archive data list to look in.</param>
        /// <param name="flagAsMod">Set to <see langword="true"/> when handling mod files to detect added and changed files.</param>
        private static void AddArchivesToList(NamedCollection<FileEntry> files, string type, string extension, IEnumerable<KeyValuePair<string, ContentArchiveEntry>> archiveData, bool flagAsMod)
        {
            foreach (var pair in archiveData
                .Where(pair => pair.Key.StartsWith(type, StringComparison.OrdinalIgnoreCase) &&
                               pair.Key.EndsWith(extension, StringComparison.OrdinalIgnoreCase)))
            {
                // Cut away the type part of the path
                AddFileToList(files, type, pair.Key.Substring(type.Length + 1), flagAsMod);
            }
        }
        #endregion

        /// <summary>
        /// Gets a list of all files of a certain type
        /// </summary>
        /// <param name="type">The type of files you want (e.g. Textures, Sounds, ...)</param>
        /// <param name="extension">The file extension to so search for</param>
        /// <returns>An collection of strings with file IDs</returns>
        public static NamedCollection<FileEntry> GetFileList([NotNull, Localizable(false)] string type, [NotNull, Localizable(false)] string extension)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(extension)) throw new ArgumentNullException("extension");
            #endregion

            type = FileUtils.UnifySlashes(type);

            // Create an alphabetical list of files without duplicates
            var files = new NamedCollection<FileEntry>();

            #region Find all base files
            // Find real files
            if (Directory.Exists(Path.Combine(BaseDir.FullName, type)))
            {
                AddDirectoryToList(files, type, extension,
                    new DirectoryInfo(Path.Combine(BaseDir.FullName, type)), "", false);
            }

            // Find files in archives
            AddArchivesToList(files, type, extension, _baseArchiveEntries, false);
            #endregion

            if (ModDir != null)
            {
                #region Find all mod files
                // Find real files
                if (Directory.Exists(Path.Combine(ModDir.FullName, type)))
                {
                    AddDirectoryToList(files, type, extension,
                        new DirectoryInfo(Path.Combine(ModDir.FullName, type)), "", true);
                }

                // Find files in archives
                AddArchivesToList(files, type, extension, _modArchiveEntries, true);
                #endregion
            }

            return files;
        }
        #endregion

        #region Get file path
        /// <summary>
        /// Gets the file path for a content file (does not search in archives)
        /// </summary>
        /// <param name="type">The type of file (e.g. Textures, Sounds, ...).</param>
        /// <param name="id">The file name of the content.</param>
        /// <returns>The absolute path to the requested content file</returns>
        /// <exception cref="FileNotFoundException">The specified file could not be found.</exception>
        [NotNull]
        public static string GetFilePath([NotNull, Localizable(false)] string type, [NotNull, Localizable(false)] string id)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            #endregion

            type = FileUtils.UnifySlashes(type);
            id = FileUtils.UnifySlashes(id);

            string path;

            if (ModDir != null)
            {
                path = Path.Combine(ModDir.FullName, Path.Combine(type, id));
                if (File.Exists(path)) return path;
            }

            if (BaseDir != null)
            {
                path = Path.Combine(BaseDir.FullName, Path.Combine(type, id));
                if (File.Exists(path)) return path;
            }

            throw new FileNotFoundException(Resources.NotFoundGameContentFile + "\n" + Path.Combine(type, id), Path.Combine(type, id));
        }
        #endregion

        #region Get file stream
        /// <summary>
        /// Gets a reading stream for a content file (searches in archives)
        /// </summary>
        /// <param name="type">The type of file (e.g. Textures, Sounds, ...).</param>
        /// <param name="id">The file name of the content.</param>
        /// <returns>The absolute path to the requested content file</returns>
        /// <exception cref="FileNotFoundException">The specified file could not be found.</exception>
        /// <exception cref="IOException">There was an error reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Read access to the file is not permitted.</exception>
        public static Stream GetFileStream([NotNull, Localizable(false)] string type, [NotNull, Localizable(false)] string id)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            #endregion

            type = FileUtils.UnifySlashes(type);
            id = FileUtils.UnifySlashes(id);

            // First try to load a real file
            if (FileExists(type, id, searchArchives: false))
                return File.OpenRead(GetFilePath(type, id));

            // Then look in the archives
            string fullID = Path.Combine(type, id);

            #region Mod
            if (ModDir != null)
            {
                // Real file
                string path = Path.Combine(ModDir.FullName, fullID);
                if (File.Exists(path)) return File.OpenRead(path);

                // Archive entry
                if (_modArchiveEntries.ContainsKey(fullID))
                {
                    // Copy from ZIP file to MemoryStream to provide seeking capability
                    Stream memoryStream = new MemoryStream();
                    try
                    {
                        using (var inputStream = _modArchiveEntries[fullID].ZipFile.GetInputStream(_modArchiveEntries[fullID].ZipEntry))
                            inputStream.CopyTo(memoryStream);
                    }
                        #region Error handling
                    catch (ZipException ex)
                    {
                        throw new IOException(ex.Message, ex);
                    }
                    #endregion

                    return memoryStream;
                }
            }
            #endregion

            #region Base
            if (BaseDir != null)
            {
                // Real file
                string path = Path.Combine(BaseDir.FullName, fullID);
                if (File.Exists(path)) return File.OpenRead(path);

                // Archive entry
                if (_baseArchiveEntries.ContainsKey(fullID))
                {
                    // Copy from ZIP file to MemoryStream to provide seeking capability
                    Stream memoryStream = new MemoryStream();
                    using (var inputStream = _baseArchiveEntries[fullID].ZipFile.GetInputStream(_baseArchiveEntries[fullID].ZipEntry))
                        inputStream.CopyTo(memoryStream);
                    return memoryStream;
                }
            }
            #endregion

            throw new FileNotFoundException(Resources.NotFoundGameContentFile + "\n" + Path.Combine(type, id), Path.Combine(type, id));
        }
        #endregion

        #region Delete mod file
        /// <summary>
        /// Deletes a file in <see cref="ModDir"/>. Will not touch files in archives or in <see cref="BaseDir"/>.
        /// </summary>
        /// <param name="type">The type of file (e.g. Textures, Sounds, ...).</param>
        /// <param name="id">The file name of the content.</param>
        /// <exception cref="InvalidOperationException"><see cref="ModDir"/> is not set.</exception>
        /// <exception cref="FileNotFoundException">The specified file could not be found.</exception>
        /// <exception cref="IOException">The specified file could not be deleted.</exception>
        /// <exception cref="UnauthorizedAccessException">You have insufficient rights to delete the file.</exception>
        public static void DeleteModFile([NotNull, Localizable(false)] string type, [NotNull, Localizable(false)] string id)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            #endregion

            // Ensure there is an active mod
            if (ModDir == null) throw new InvalidOperationException(Resources.NoModActive);

            // Try to delete a file in that mod
            File.Delete(Path.Combine(ModDir.FullName, Path.Combine(type, id)));
        }
        #endregion
    }
}
