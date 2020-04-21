// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using NanoByte.Common.Native;
using NanoByte.Common.Properties;

#if NET20 || NET35
using NanoByte.Common.Values;
#endif

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Provides filesystem-related helper methods.
    /// </summary>
    public static class FileUtils
    {
        #region Paths
        /// <summary>
        /// Combines an array of strings into a path.
        /// </summary>
        /// <remarks>Backport of the n-parameter version of <see cref="Path.Combine(string,string)"/> introduced in .NET 4.0.</remarks>
        public static string PathCombine(params string[] paths)
        {
            #region Sanity checks
            if (paths == null) throw new ArgumentNullException(nameof(paths));
            #endregion

#if NET20 || NET35
            return (paths.Length == 0) ? "" : paths.Aggregate(Path.Combine);
#else
            return Path.Combine(paths);
#endif
        }

        /// <summary>
        /// Replaces Unix-style directory slashes with <see cref="Path.DirectorySeparatorChar"/>.
        /// </summary>
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
#if NETSTANDARD2_1
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("value")]
#endif
        public static string? UnifySlashes(string? value) => value?.Replace('/', Path.DirectorySeparatorChar);

        /// <summary>
        /// Determines whether a path might escape its parent directory (by being absolute or using ..).
        /// </summary>
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
        public static bool IsBreakoutPath([Localizable(false)] string path)
        {
            path = UnifySlashes(path ?? throw new ArgumentNullException(nameof(path)));
            return Path.IsPathRooted(path) || path.Split(Path.DirectorySeparatorChar).Contains("..");
        }

        /// <summary>
        /// Returns a relative path pointing to <paramref name="target"/> from <paramref name="baseRef"/> using Unix/Uri-style directory separators.
        /// </summary>
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
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
            return Uri.UnescapeDataString(relativeUri.ToString()).TrimEnd('/');
        }
        #endregion

        #region Exists
        /// <summary>
        /// Like <see cref="File.Exists"/> but case-sensitive, even on Windows.
        /// </summary>
        public static bool ExistsCaseSensitive([Localizable(false)] string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            return File.Exists(path)
                   // Make sure the file found is a string-exact match
                && Directory.GetFiles(Path.GetDirectoryName(path) ?? Directory.GetCurrentDirectory(), Path.GetFileName(path)).Contains(path);
        }
        #endregion

        #region Touch
        /// <summary>
        /// Sets the "last modified" timestamp for a file to now. Creates a new empty file if it does not exist yet.
        /// </summary>
        /// <exception cref="IOException">Creating the file or updating its timestamp failed.</exception>
        /// <exception cref="UnauthorizedAccessException">You have insufficient rights to create the file or update its timestamp.</exception>
        public static void Touch([Localizable(false)] string path)
        {
            var fileInfo = new FileInfo(path ?? throw new ArgumentNullException(nameof(path)));
            if (fileInfo.Exists)
                fileInfo.LastWriteTimeUtc = DateTime.UtcNow;
            else
                File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete).Dispose();
        }
        #endregion

        #region Time
        /// <summary>
        /// Converts a <see cref="DateTime"/> into the number of seconds since the Unix epoch (1970-1-1).
        /// </summary>
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
        public static long ToUnixTime(this DateTime time)
        {
            var timespan = (time.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return (long)timespan.TotalSeconds;
        }

        /// <summary>
        /// Converts a number of seconds since the Unix epoch (1970-1-1) into a <see cref="DateTime"/>.
        /// </summary>
#if NETSTANDARD
        [System.Diagnostics.Contracts.Pure]
#endif
        public static DateTime FromUnixTime(long time)
        {
            var timespan = TimeSpan.FromSeconds(time);
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + timespan;
        }

        /// <summary>
        /// Determines the accuracy with which the filesystem underlying a specific directory can store file-changed times.
        /// </summary>
        /// <param name="path">The path of the directory to check.</param>
        /// <returns>The accuracy in number of seconds. (i.e. 0 = perfect, 1 = may be off by up to one second)</returns>
        /// <exception cref="DirectoryNotFoundException">The specified directory doesn't exist.</exception>
        /// <exception cref="IOException">Writing to the directory fails.</exception>
        /// <exception cref="UnauthorizedAccessException">You have insufficient rights to write to the directory.</exception>
        public static int DetermineTimeAccuracy([Localizable(false)] string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            if (!Directory.Exists(path)) throw new DirectoryNotFoundException(string.Format(Resources.FileNotFound, path));

            // Prepare a file name and fake change time
            var referenceTime = new DateTime(2000, 1, 1, 0, 0, 1); // 1 second past mid-night on 1st of January 2000
            string tempFile = Path.Combine(path, Path.GetRandomFileName());

            File.WriteAllText(tempFile, @"a");
            File.SetLastWriteTimeUtc(tempFile, referenceTime);
            var resultTime = File.GetLastWriteTimeUtc(tempFile);
            File.Delete(tempFile);

            return Math.Abs((resultTime - referenceTime).Seconds);
        }
        #endregion

        #region Temp
        /// <summary>
        /// Creates a uniquely named, empty temporary file on disk and returns the full path of that file.
        /// </summary>
        /// <param name="prefix">A short string the filename should start with.</param>
        /// <returns>The full path of the newly created temporary file.</returns>
        /// <exception cref="IOException">A problem occurred while creating a file in <see cref="Path.GetTempPath"/>.</exception>
        /// <exception cref="UnauthorizedAccessException">Creating a file in <see cref="Path.GetTempPath"/> is not permitted.</exception>
        /// <remarks>Use this method, because <see cref="Path.GetTempFileName"/> exhibits buggy behaviour in some Mono versions.</remarks>
        public static string GetTempFile([Localizable(false)] string prefix)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentNullException(nameof(prefix));
            #endregion

            // Make sure there are no name collisions
            string path;
            do
            {
                path = Path.Combine(Path.GetTempPath(), prefix + '-' + Path.GetRandomFileName());
            } while (File.Exists(path));

            // Create the file to ensure nobody else uses the name
            File.WriteAllBytes(path, new byte[0]);

            return path;
        }

        /// <summary>
        /// Creates a uniquely named, empty temporary directory on disk and returns the full path of that directory.
        /// </summary>
        /// <param name="prefix">A short string the filename should start with.</param>
        /// <returns>The full path of the newly created temporary directory.</returns>
        /// <exception cref="IOException">A problem occurred while creating a directory in <see cref="Path.GetTempPath"/>.</exception>
        /// <exception cref="UnauthorizedAccessException">Creating a directory in <see cref="Path.GetTempPath"/> is not permitted.</exception>
        /// <remarks>Use this method, because <see cref="Path.GetTempFileName"/> exhibits buggy behaviour in some Mono versions.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Delivers a new value on each call")]
        public static string GetTempDirectory([Localizable(false)] string prefix)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentNullException(nameof(prefix));
            #endregion

            string tempDir = GetTempFile(prefix);
            File.Delete(tempDir);
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }
        #endregion

        #region Replace
        /// <summary>
        /// Replaces one file with another. Rolls back in case of problems. If the destination file does not exist yet, this acts like a simple rename.
        /// </summary>
        /// <param name="sourcePath">The path of source directory.</param>
        /// <param name="destinationPath">The path of the target directory. Must reside on the same filesystem as <paramref name="sourcePath"/>.</param>
        /// <exception cref="ArgumentException"><paramref name="sourcePath"/> and <paramref name="destinationPath"/> are equal.</exception>
        /// <exception cref="IOException">The file could not be replaced.</exception>
        /// <exception cref="UnauthorizedAccessException">The read or write access to one of the files was denied.</exception>
        public static void Replace([Localizable(false)] string sourcePath, [Localizable(false)] string destinationPath)
        {
            if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentNullException(nameof(sourcePath));
            if (string.IsNullOrEmpty(destinationPath)) throw new ArgumentNullException(nameof(destinationPath));
            if (sourcePath == destinationPath) throw new ArgumentException(Resources.SourceDestinationEqual);

            if (UnixUtils.IsUnix) UnixUtils.Rename(sourcePath, destinationPath);
            else if (WindowsUtils.IsWindowsXP) ReplaceNT();
            else ReplaceSimple();

            void ReplaceSimple()
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                string backupPath = Path.Combine(
                    Path.GetDirectoryName(Path.GetFullPath(destinationPath)),
                    "backup." + Path.GetRandomFileName() + "." + Path.GetFileName(destinationPath));

                if (File.Exists(destinationPath)) File.Move(destinationPath, backupPath);
                try
                {
                    File.Move(sourcePath, destinationPath);
                }
                catch
                {
                    if (File.Exists(backupPath)) File.Move(backupPath, destinationPath);
                    throw;
                }
                finally
                {
                    if (File.Exists(backupPath)) File.Delete(backupPath);
                }
            }

            void ReplaceNT()
            {
                if (!File.Exists(destinationPath))
                {
                    // File.Replace() fails if destinationPath does not exist yet
                    File.Move(sourcePath, destinationPath);
                    return;
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                string backupPath = Path.Combine(
                    Path.GetDirectoryName(Path.GetFullPath(destinationPath)),
                    "backup." + Path.GetRandomFileName() + "." + Path.GetFileName(destinationPath));

                try
                {
                    File.Replace(sourcePath, destinationPath, backupPath, ignoreMetadataErrors: true);
                }
                catch (IOException ex)
                {
                    Log.Debug("File.Replace() failed. Falling back to simple 'delete and move' in case we are running on a Windows version that does not support File.Replace() (e.g. Windows Azure).");
                    Log.Debug(ex);
                    ReplaceSimple();
                }
                finally
                {
                    if (File.Exists(backupPath)) File.Delete(backupPath);
                }
            }
        }
        #endregion

        #region Read
        /// <summary>
        /// Reads the first line of text from a file.
        /// </summary>
        /// <param name="file">The file to read from.</param>
        /// <param name="encoding">The text encoding to use for reading.</param>
        /// <returns>The first line of text in the file; <c>null</c> if decoding does not work on the contents.</returns>
        /// <exception cref="IOException">A problem occurred while reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Read access to the file is not permitted.</exception>
        public static string? ReadFirstLine(this FileInfo file, Encoding encoding)
        {
            #region Sanity checks
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            #endregion

            using var stream = file.OpenRead();
            return new StreamReader(stream, encoding).ReadLine();
        }
        #endregion

        #region Directories
        /// <summary>
        /// Walks a directory structure recursively and performs an action for every directory and file encountered.
        /// </summary>
        /// <param name="directory">The directory to walk.</param>
        /// <param name="dirAction">The action to perform for every found directory (including the starting <paramref name="directory"/>); can be <c>null</c>.</param>
        /// <param name="fileAction">The action to perform for every found file; can be <c>null</c>.</param>
        /// <param name="followDirSymlinks">If <c>true</c> recurse into directory symlinks; if <c>false</c> only execute <paramref name="dirAction"/> for directory symlinks but do not recurse.</param>
        public static void Walk(this DirectoryInfo directory, Action<DirectoryInfo>? dirAction = null, Action<FileInfo>? fileAction = null, bool followDirSymlinks = false)
        {
            #region Sanity checks
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (!directory.Exists) throw new DirectoryNotFoundException(Resources.SourceDirNotExist);
            #endregion

            dirAction?.Invoke(directory);

            if (fileAction != null)
            {
                foreach (var file in directory.GetFiles())
                    fileAction(file);
            }

            foreach (var subDir in directory.GetDirectories())
            {
                if (followDirSymlinks || !IsSymlink(subDir.FullName))
                    Walk(subDir, dirAction, fileAction);
                else
                    dirAction?.Invoke(subDir);
            }
        }

        /// <summary>
        /// Skips through any directories that only contain a single subdirectory and no files.
        /// </summary>
        /// <remarks>Ignores files that start with a dot.</remarks>
        public static DirectoryInfo WalkThroughPrefix(this DirectoryInfo directory)
        {
            #region Sanity checks
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            #endregion

            var subdirectories = directory.GetDirectories();
            var files = directory.GetFiles().Where(x => !x.Name.StartsWith("."));

            if (subdirectories.Length == 1 && !files.Any()) return WalkThroughPrefix(subdirectories[0]);
            else return directory;
        }

        /// <summary>
        /// Returns the full paths of all files in a directory and its subdirectories.
        /// </summary>
        /// <param name="path">The path of the directory to search for files.</param>
        public static string[] GetFilesRecursive(string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            var paths = new List<string>();
            new DirectoryInfo(path).Walk(fileAction: file => paths.Add(file.FullName));
            return paths.ToArray();
        }
        #endregion

        #region ACLs
        /// <summary>
        /// Removes any custom ACLs a user may have set, restores ACL inheritance and sets the Administrators group as the owner.
        /// </summary>
        public static void ResetAcl(this DirectoryInfo directoryInfo)
        {
            try
            {
                directoryInfo.Walk(
                    dir => ResetAcl(dir.GetAccessControl, dir.SetAccessControl!),
                    file => ResetAcl(file.GetAccessControl, file.SetAccessControl!));
            }
            #region Error handling
            catch (ArgumentException ex)
            {
                Log.Error(ex);
            }
            catch (IdentityNotMappedException ex)
            {
                Log.Error(ex);
            }
            #endregion
        }

        /// <summary>
        /// Helper method for <see cref="ResetAcl(DirectoryInfo)"/>.
        /// </summary>
        private static void ResetAcl<T>(Func<T> getAcl, Action<T> setAcl) where T : FileSystemSecurity
        {
            // Give ownership to administrators
            var acl = getAcl();
            acl.SetOwner(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null));
            setAcl(acl);

            // Inherit rules from container and remove any custom rules
            acl = getAcl();
            acl.CanonicalizeAcl();
            acl.SetAccessRuleProtection(isProtected: false, preserveInheritance: true);
            foreach (FileSystemAccessRule rule in acl.GetAccessRules(true, false, typeof(NTAccount)))
                acl.RemoveAccessRule(rule);
            setAcl(acl);
        }

        /// <summary>
        /// Fixes ACLs that are not canonical (not ordered correctly).
        /// </summary>
        public static void CanonicalizeAcl(this ObjectSecurity objectSecurity)
        {
            #region Sanity checks
            if (objectSecurity == null) throw new ArgumentNullException(nameof(objectSecurity));
            #endregion

            if (objectSecurity.AreAccessRulesCanonical) return;

            var securityDescriptor = new RawSecurityDescriptor(objectSecurity.GetSecurityDescriptorSddlForm(AccessControlSections.Access));
            var denied = new List<CommonAce>();
            var deniedObject = new List<CommonAce>();
            var allowed = new List<CommonAce>();
            var allowedObject = new List<CommonAce>();
            var inherited = new List<CommonAce>();
            foreach (var ace in securityDescriptor.DiscretionaryAcl.Cast<CommonAce>())
            {
                if (ace.AceFlags.HasFlag(AceFlags.Inherited)) inherited.Add(ace);
                else
                {
                    switch (ace.AceType)
                    {
                        case AceType.AccessDenied:
                            denied.Add(ace);
                            break;
                        case AceType.AccessDeniedObject:
                            deniedObject.Add(ace);
                            break;
                        case AceType.AccessAllowed:
                            allowed.Add(ace);
                            break;
                        case AceType.AccessAllowedObject:
                            allowedObject.Add(ace);
                            break;
                    }
                }
            }

            int aceIndex = 0;
            var newDacl = new RawAcl(securityDescriptor.DiscretionaryAcl.Revision, securityDescriptor.DiscretionaryAcl.Count);
            denied.ForEach(ace => newDacl.InsertAce(aceIndex++, ace));
            deniedObject.ForEach(ace => newDacl.InsertAce(aceIndex++, ace));
            allowed.ForEach(ace => newDacl.InsertAce(aceIndex++, ace));
            allowedObject.ForEach(ace => newDacl.InsertAce(aceIndex++, ace));
            inherited.ForEach(ace => newDacl.InsertAce(aceIndex++, ace));

            if (aceIndex != securityDescriptor.DiscretionaryAcl.Count) throw new InvalidOperationException(Resources.CannotCanonicalizeDacl);
            securityDescriptor.DiscretionaryAcl = newDacl;
            objectSecurity.SetSecurityDescriptorSddlForm(securityDescriptor.GetSddlForm(AccessControlSections.Access), AccessControlSections.Access);
        }
        #endregion

        #region Write protection
        /// <summary>
        /// Uses the best means the current platform provides to prevent further write access to a directory (read-only attribute, ACLs, Unix octals, etc.).
        /// </summary>
        /// <remarks>May do nothing if the platform doesn't provide any known protection mechanisms.</remarks>
        /// <param name="path">The directory to protect.</param>
        /// <exception cref="IOException">There was a problem applying the write protection.</exception>
        /// <exception cref="UnauthorizedAccessException">You have insufficient rights to apply the write protection.</exception>
        public static void EnableWriteProtection([Localizable(false)] string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!Directory.Exists(path)) throw new DirectoryNotFoundException(Resources.SourceDirNotExist);
            #endregion

            var directory = new DirectoryInfo(path);

            if (UnixUtils.IsUnix) directory.ToggleWriteProtectionUnix(true);
            else if (WindowsUtils.IsWindowsNT) directory.ToggleWriteProtectionWinNT(true);
        }

        /// <summary>
        /// Removes whatever means the current platform provides to prevent write access to a directory (read-only attribute, ACLs, Unix octals, etc.).
        /// </summary>
        /// <remarks>May do nothing if the platform doesn't provide any known protection mechanisms.</remarks>
        /// <param name="path">The directory to unprotect.</param>
        /// <exception cref="IOException">There was a problem removing the write protection.</exception>
        /// <exception cref="UnauthorizedAccessException">You have insufficient rights to remove the write protection.</exception>
        public static void DisableWriteProtection([Localizable(false)] string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!Directory.Exists(path)) throw new DirectoryNotFoundException(Resources.SourceDirNotExist);
            #endregion

            var directory = new DirectoryInfo(path);

            if (UnixUtils.IsUnix) directory.ToggleWriteProtectionUnix(false);
            else if (WindowsUtils.IsWindows)
            {
                if (WindowsUtils.IsWindowsNT) directory.ToggleWriteProtectionWinNT(false);

                // Remove classic read-only attributes
                try
                {
                    Walk(directory,
                        dir => dir.Attributes = FileAttributes.Normal,
                        file => file.IsReadOnly = false);
                }
                catch (ArgumentException)
                {}
            }
        }

        #region Helpers
        private static void ToggleWriteProtectionUnix(this DirectoryInfo directory, bool enable)
        {
            try
            {
                Walk(directory,
                    dir =>
                    {
                        if (enable) UnixUtils.MakeReadOnly(dir.FullName);
                        else UnixUtils.MakeWritable(dir.FullName);
                    },
                    file =>
                    {
                        if (UnixUtils.IsSymlink(file.FullName)) return; // Cannot set permissions on a POSIX symlink
                        if (enable) UnixUtils.MakeReadOnly(file.FullName);
                        else UnixUtils.MakeWritable(file.FullName);
                    });
            }
            #region Error handling
            catch (InvalidOperationException ex)
            {
                throw new IOException(Resources.UnixSubsystemFail, ex);
            }
            catch (IOException ex)
            {
                throw new IOException(Resources.UnixSubsystemFail, ex);
            }
            #endregion
        }

        private static readonly FileSystemAccessRule _denyEveryoneWrite = new FileSystemAccessRule(new SecurityIdentifier("S-1-1-0" /*Everyone*/), FileSystemRights.Write | FileSystemRights.Delete | FileSystemRights.DeleteSubdirectoriesAndFiles, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Deny);

        private static void ToggleWriteProtectionWinNT(this DirectoryInfo directory, bool enable)
        {
            try
            {
                var acl = directory.GetAccessControl();
                acl.CanonicalizeAcl();
                if (enable) acl.AddAccessRule(_denyEveryoneWrite);
                else acl.RemoveAccessRule(_denyEveryoneWrite);
                directory.SetAccessControl(acl);
            }
            #region Error handling
            catch (ArgumentException ex)
            {
                Log.Error(ex);
            }
            catch (IdentityNotMappedException ex)
            {
                Log.Error(ex);
            }
            #endregion
        }
        #endregion
        #endregion

        #region Links
        /// <summary>
        /// Creates a new symbolic link to a file or directory.
        /// </summary>
        /// <param name="sourcePath">The path of the link to create.</param>
        /// <param name="targetPath">The path of the existing file or directory to point to (relative to <paramref name="sourcePath"/>).</param>
        /// <exception cref="IOException">Creating the symbolic link failed.</exception>
        /// <exception cref="UnauthorizedAccessException">You have insufficient rights to create the symbolic link.</exception>
        /// <exception cref="PlatformNotSupportedException">This method is called on a system with no symbolic link support.</exception>
        public static void CreateSymlink([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentNullException(nameof(sourcePath));
            if (string.IsNullOrEmpty(targetPath)) throw new ArgumentNullException(nameof(targetPath));
            #endregion

            if (UnixUtils.IsUnix)
            {
                try
                {
                    UnixUtils.CreateSymlink(sourcePath, targetPath);
                }
                #region Error handling
                catch (InvalidOperationException ex)
                {
                    throw new IOException(Resources.UnixSubsystemFail, ex);
                }
                catch (IOException ex)
                {
                    throw new IOException(Resources.UnixSubsystemFail, ex);
                }
                #endregion
            }
            else if (WindowsUtils.IsWindowsVista)
            {
                try
                {
                    WindowsUtils.CreateSymlink(sourcePath, targetPath);
                }
                #region Error handling
                catch (Win32Exception ex)
                {
                    // Wrap exception since only certain exception types are allowed
                    throw new IOException(ex.Message, ex);
                }
                #endregion
            }
            else throw new PlatformNotSupportedException();
        }

        /// <summary>
        /// Creates a new hard link between two files.
        /// </summary>
        /// <param name="sourcePath">The path of the link to create.</param>
        /// <param name="targetPath">The absolute path of the existing file to point to.</param>
        /// <exception cref="IOException">Creating the hard link failed.</exception>
        /// <exception cref="UnauthorizedAccessException">You have insufficient rights to create the hard link.</exception>
        /// <exception cref="PlatformNotSupportedException">This method is called on a system with no hard link support.</exception>
        public static void CreateHardlink([Localizable(false)] string sourcePath, [Localizable(false)] string targetPath)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentNullException(nameof(sourcePath));
            if (string.IsNullOrEmpty(targetPath)) throw new ArgumentNullException(nameof(targetPath));
            #endregion

            if (UnixUtils.IsUnix)
            {
                try
                {
                    UnixUtils.CreateHardlink(sourcePath, targetPath);
                }
                #region Error handling
                catch (InvalidOperationException ex)
                {
                    throw new IOException(Resources.UnixSubsystemFail, ex);
                }
                catch (IOException ex)
                {
                    throw new IOException(Resources.UnixSubsystemFail, ex);
                }
                #endregion
            }
            else if (WindowsUtils.IsWindowsNT)
            {
                try
                {
                    WindowsUtils.CreateHardlink(sourcePath, targetPath);
                }
                #region Error handling
                catch (Win32Exception ex)
                {
                    // Wrap exception since only certain exception types are allowed
                    throw new IOException(ex.Message, ex);
                }
                #endregion
            }
            else throw new PlatformNotSupportedException();
        }

        /// <summary>
        /// Determines whether two files are hardlinked.
        /// </summary>
        /// <param name="path1">The path of the first file.</param>
        /// <param name="path2">The path of the second file.</param>
        /// <exception cref="IOException">Creating the files failed.</exception>
        /// <exception cref="UnauthorizedAccessException">You have insufficient rights to check the files.</exception>
        public static bool AreHardlinked([Localizable(false)] string path1, [Localizable(false)] string path2)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path1)) throw new ArgumentNullException(nameof(path1));
            if (string.IsNullOrEmpty(path2)) throw new ArgumentNullException(nameof(path2));
            #endregion

            if (UnixUtils.IsUnix)
            {
                try
                {
                    return UnixUtils.AreHardlinked(path1, path2);
                }
                #region Error handling
                catch (InvalidOperationException ex)
                {
                    throw new IOException(Resources.UnixSubsystemFail, ex);
                }
                catch (IOException ex)
                {
                    throw new IOException(Resources.UnixSubsystemFail, ex);
                }
                #endregion
            }
            else if (WindowsUtils.IsWindowsNT)
            {
                try
                {
                    return WindowsUtils.AreHardlinked(path1, path2);
                }
                #region Error handling
                catch (Win32Exception ex)
                {
                    // Wrap exception since only certain exception types are allowed
                    throw new IOException(ex.Message, ex);
                }
                #endregion
            }
            else return false;
        }
        #endregion

        #region Unix
        /// <summary>
        /// Checks whether a file is a regular file (i.e. not a device file, symbolic link, etc.).
        /// </summary>
        /// <returns><c>true</c> if <paramref name="path"/> points to a regular file; <c>false</c> otherwise.</returns>
        /// <remarks>Will return <c>false</c> for non-existing files.</remarks>
        /// <exception cref="UnauthorizedAccessException">You have insufficient rights to query the file's properties.</exception>
        public static bool IsRegularFile([Localizable(false)] string path)
        {
            if (!File.Exists(path)) return false;

            if (UnixUtils.IsUnix)
            {
                try
                {
                    return UnixUtils.IsRegularFile(path);
                }
                #region Error handling
                catch (InvalidOperationException ex)
                {
                    throw new IOException(Resources.UnixSubsystemFail, ex);
                }
                catch (IOException ex)
                {
                    throw new IOException(Resources.UnixSubsystemFail, ex);
                }
                #endregion
            }
            else if (WindowsUtils.IsWindowsVista)
            {
                try
                {
                    return !WindowsUtils.IsSymlink(path);
                }
                #region Error handling
                catch (Win32Exception ex)
                {
                    // Wrap exception since only certain exception types are allowed
                    throw new IOException(ex.Message, ex);
                }
                #endregion
            }
            else return true;
        }

        /// <summary>
        /// Checks whether a file is a symbolic link.
        /// </summary>
        /// <param name="path">The path of the file to check.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
        /// <exception cref="IOException">There was an IO problem reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Read access to the file was denied.</exception>
        public static bool IsSymlink([Localizable(false)] string path)
        {
            if (UnixUtils.IsUnix)
            {
                try
                {
                    return UnixUtils.IsSymlink(path);
                }
                #region Error handling
                catch (InvalidOperationException ex)
                {
                    throw new IOException(Resources.UnixSubsystemFail, ex);
                }
                catch (IOException ex)
                {
                    throw new IOException(Resources.UnixSubsystemFail, ex);
                }
                #endregion
            }
            else if (WindowsUtils.IsWindowsVista)
            {
                try
                {
                    return WindowsUtils.IsSymlink(path);
                }
                #region Error handling
                catch (Win32Exception ex)
                {
                    // Wrap exception since only certain exception types are allowed
                    throw new IOException(ex.Message, ex);
                }
                #endregion
            }
            else return false;
        }

        /// <summary>
        /// Checks whether a file is a symbolic link.
        /// </summary>
        /// <param name="path">The path of the file to check.</param>
        /// <param name="target">Returns the target the symbolic link points to if it exists.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to a symbolic link; <c>false</c> otherwise.</returns>
        /// <exception cref="IOException">There was an IO problem reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Read access to the file was denied.</exception>
        public static bool IsSymlink(
            [Localizable(false)] string path,
#if NETSTANDARD2_1
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            out string? target)
        {
            if (UnixUtils.IsUnix)
            {
                try
                {
                    return UnixUtils.IsSymlink(path, out target);
                }
                #region Error handling
                catch (InvalidOperationException ex)
                {
                    throw new IOException(Resources.UnixSubsystemFail, ex);
                }
                catch (IOException ex)
                {
                    throw new IOException(Resources.UnixSubsystemFail, ex);
                }
                #endregion
            }
            else if (WindowsUtils.IsWindowsVista)
            {
                try
                {
                    return WindowsUtils.IsSymlink(path, out target);
                }
                #region Error handling
                catch (Win32Exception ex)
                {
                    // Wrap exception since only certain exception types are allowed
                    throw new IOException(ex.Message, ex);
                }
                #endregion
            }
            else
            {
                target = null;
                return false;
            }
        }

        /// <summary>
        /// Checks whether a file is a Unix symbolic link.
        /// </summary>
        /// <param name="item">The file to check.</param>
        /// <param name="target">Returns the target the symbolic link points to if it exists.</param>
        /// <returns><c>true</c> if <paramref name="item"/> points to a symbolic link; <c>false</c> otherwise.</returns>
        /// <exception cref="IOException">There was an IO problem reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Read access to the file was denied.</exception>
        public static bool IsSymlink(
            this FileSystemInfo item,
#if NETSTANDARD2_1
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            out string? target)
        {
            #region Sanity checks
            if (item == null) throw new ArgumentNullException(nameof(item));
            #endregion

            return IsSymlink(item.FullName, out target);
        }

        /// <summary>
        /// Checks whether a file is marked as Unix-executable.
        /// </summary>
        /// <returns><c>true</c> if <paramref name="path"/> points to an executable; <c>false</c> otherwise.</returns>
        /// <remarks>Will return <c>false</c> for non-existing files. Will always return <c>false</c> on non-Unixoid systems.</remarks>
        /// <exception cref="UnauthorizedAccessException">You have insufficient rights to query the file's properties.</exception>
        public static bool IsExecutable([Localizable(false)] string path)
        {
            if (!File.Exists(path) || !UnixUtils.IsUnix) return false;

            try
            {
                return UnixUtils.IsExecutable(path);
            }
            #region Error handling
            catch (InvalidOperationException ex)
            {
                throw new IOException(Resources.UnixSubsystemFail, ex);
            }
            catch (IOException ex)
            {
                throw new IOException(Resources.UnixSubsystemFail, ex);
            }
            #endregion
        }

        /// <summary>
        /// Marks a file as Unix-executable or not Unix-executable. Only works on Unixoid systems!
        /// </summary>
        /// <param name="path">The file to mark as executable or not executable.</param>
        /// <param name="executable"><c>true</c> to mark the file as executable, <c>true</c> to mark it as not executable.</param>
        /// <exception cref="FileNotFoundException"><paramref name="path"/> points to a file that does not exist or cannot be accessed.</exception>
        /// <exception cref="UnauthorizedAccessException">You have insufficient rights to change the file's properties.</exception>
        /// <exception cref="PlatformNotSupportedException">This method is called on a non-Unixoid system.</exception>
        public static void SetExecutable([Localizable(false)] string path, bool executable)
        {
            #region Sanity checks
            if (!File.Exists(path)) throw new FileNotFoundException("", path);
            if (!UnixUtils.IsUnix) throw new PlatformNotSupportedException();
            #endregion

            try
            {
                UnixUtils.SetExecutable(path, executable);
            }
            #region Error handling
            catch (InvalidOperationException ex)
            {
                throw new IOException(Resources.UnixSubsystemFail, ex);
            }
            catch (IOException ex)
            {
                throw new IOException(Resources.UnixSubsystemFail, ex);
            }
            #endregion
        }

        /// <summary>
        /// Checks whether a directory is located on a filesystem with support for Unixoid features such as executable bits.
        /// </summary>
        /// <returns><c>true</c> if <paramref name="path"/> points to directory on a Unixoid filesystem; <c>false</c> otherwise.</returns>
        /// <remarks>
        /// Will always return <c>false</c> on non-Unixoid systems.
        /// Only requires read access on Linux to determine file system.
        /// Requires write access on other Unixes (e.g. MacOS X).
        /// </remarks>
        /// <exception cref="DirectoryNotFoundException">The specified directory doesn't exist.</exception>
        /// <exception cref="IOException">Checking the directory failed.</exception>
        /// <exception cref="UnauthorizedAccessException">You have insufficient right to stat to the directory.</exception>
        public static bool IsUnixFS([Localizable(false)] string path)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            #endregion

            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture))) path += Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);
            if (!Directory.Exists(path)) throw new DirectoryNotFoundException(string.Format(Resources.FileNotFound, path));
            if (!UnixUtils.IsUnix) return false;

            try
            {
                return UnixUtils.GetFileSystem(path) switch
                {
                    // FAT
                    "msdos" => false,
                    "vfat" => false,
                    // HPFS
                    "hpfs" => false,
                    // NTFS
                    "ntfs" => false,
                    "ntfs-3g" => false,
                    // Windows Network Share
                    "smbfs" => false,
                    "cifs" => false,
                    // Other
                    _ => true
                };
            }
            catch (IOException)
            {
                return IsUnixFSFallback(path);
            }
        }

        /// <summary>
        /// Checks whether a directory is located on a filesystem with support for executable bits by setting and reading them back.
        /// </summary>
        private static bool IsUnixFSFallback([Localizable(false)] string path)
        {
            string probeFile = Path.Combine(path, ".unixfs_probe_" + Path.GetRandomFileName());
            Touch(probeFile);

            try
            {
                UnixUtils.SetExecutable(probeFile, false);
                if (UnixUtils.IsExecutable(probeFile)) return false;

                UnixUtils.SetExecutable(probeFile, true);
                if (!UnixUtils.IsExecutable(probeFile)) return false;

                return true;
            }
            #region Error handling
            catch (InvalidOperationException ex)
            {
                throw new IOException(Resources.UnixSubsystemFail, ex);
            }
            catch (IOException ex)
            {
                throw new IOException(Resources.UnixSubsystemFail, ex);
            }
            #endregion

            finally
            {
                File.Delete(probeFile);
            }
        }
        #endregion

        #region Extended metadata
        /// <summary>
        /// Reads metadata from an NTFS Alternate Data Stream (Windows) or extended file attribute (Unixoid).
        /// </summary>
        /// <param name="path">The path of the file the Alternate Data Stream is associated with.</param>
        /// <param name="name">The name of the metadata stream.</param>
        /// <returns>The contents of the metadata stream; <c>null</c> if the file exists but the stream specified by <paramref name="name"/> does not.</returns>
        /// <exception cref="FileNotFoundException">The file specified by <paramref name="path"/> does not exist.</exception>
        /// <exception cref="IOException">There was a problem reading the metadata stream.</exception>
        /// <exception cref="PlatformNotSupportedException">The current operating system provides no method for storing extended metadata.</exception>
        public static byte[]? ReadExtendedMetadata([Localizable(false)] string path, [Localizable(false)] string name)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            #endregion

            if (!File.Exists(path)) throw new FileNotFoundException(string.Format(Resources.FileNotFound, path), path);

            if (WindowsUtils.IsWindowsNT) return WindowsUtils.ReadAllBytes(path + ":" + name);
            else if (UnixUtils.IsUnix) return UnixUtils.GetXattr(path, "user." + name);
            else throw new PlatformNotSupportedException();
        }

        /// <summary>
        /// Writes metadata to an NTFS Alternate Data Stream (Windows) or extended file attribute (Unixoid).
        /// </summary>
        /// <param name="path">The path of the file to associate the metadata with.</param>
        /// <param name="name">The name of the metadata stream.</param>
        /// <param name="data">The data to write to the metadata stream.</param>
        /// <exception cref="FileNotFoundException">The file specified by <paramref name="path"/> does not exist.</exception>
        /// <exception cref="IOException">There was a problem writing the metadata stream.</exception>
        /// <exception cref="UnauthorizedAccessException">You have insufficient rights to write the metadata.</exception>
        /// <exception cref="PlatformNotSupportedException">The current operating system provides no method for storing extended metadata.</exception>
        public static void WriteExtendedMetadata([Localizable(false)] string path, [Localizable(false)] string name, byte[] data)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (data == null) throw new ArgumentNullException(nameof(data));
            #endregion

            if (!File.Exists(path)) throw new FileNotFoundException(string.Format(Resources.FileNotFound, path), path);

            if (WindowsUtils.IsWindowsNT)
            {
                try
                {
                    WindowsUtils.WriteAllBytes(path + ":" + name, data);
                }
                #region Error handling
                catch (Win32Exception ex)
                {
                    // Wrap exception since only certain exception types are allowed
                    throw new IOException(ex.Message, ex);
                }
                #endregion
            }
            else if (UnixUtils.IsUnix) UnixUtils.SetXattr(path, "user." + name, data);
            else throw new PlatformNotSupportedException();
        }
        #endregion
    }
}
