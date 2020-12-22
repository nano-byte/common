// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using NanoByte.Common.Properties;
using NanoByte.Common.Tasks;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Copies the content of a directory to a new location preserving the original file modification times and relative Unix symlinks.
    /// </summary>
    public class CopyDirectory : TaskBase
    {
        /// <inheritdoc/>
        public override string Name => Resources.CopyFiles;

        /// <inheritdoc/>
        protected override bool UnitsByte => true;

        /// <summary>
        /// The path of source directory. Must exist!
        /// </summary>
        public string SourcePath { get; }

        /// <summary>
        /// The path of the target directory. May exist. Must be empty if <see cref="Overwrite"/> is <c>false</c>.
        /// </summary>
        public string DestinationPath { get; }

        /// <summary>
        /// <c>true</c> to preserve the modification times for directories as well; <c>false</c> to preserve only the file modification times.
        /// </summary>
        public bool PreserveDirectoryTimestamps { get; }

        /// <summary>
        /// Overwrite existing files and directories at the <see cref="DestinationPath"/>. This will even replace read-only files!
        /// </summary>
        public bool Overwrite { get; }

        /// <summary>
        /// Creates a new directory copy task.
        /// </summary>
        /// <param name="sourcePath">The path of source directory. Must exist!</param>
        /// <param name="destinationPath">The path of the target directory. May exist. Must be empty if <paramref name="overwrite"/> is <c>false</c>.</param>
        /// <param name="preserveDirectoryTimestamps"><c>true</c> to preserve the modification times for directories as well; <c>false</c> to preserve only the file modification times.</param>
        /// <param name="overwrite">Overwrite existing files and directories at the <paramref name="destinationPath"/>. This will even replace read-only files!</param>
        public CopyDirectory([Localizable(false)] string sourcePath, [Localizable(false)] string destinationPath, bool preserveDirectoryTimestamps = true, bool overwrite = false)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentNullException(nameof(sourcePath));
            if (string.IsNullOrEmpty(destinationPath)) throw new ArgumentNullException(nameof(destinationPath));
            if (sourcePath == destinationPath) throw new ArgumentException(Resources.SourceDestinationEqual);
            #endregion

            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            PreserveDirectoryTimestamps = preserveDirectoryTimestamps;
            Overwrite = overwrite;
        }

        private DirectoryInfo? _source, _destination;

        /// <inheritdoc/>
        protected override void Execute()
        {
            _source = new(SourcePath);
            _destination = new(DestinationPath);
            if (!_source.Exists) throw new DirectoryNotFoundException(Resources.SourceDirNotExist);
            if (_destination.Exists)
            { // Fail if overwrite is off but the target directory already exists and contains elements
                if (!Overwrite && _destination.GetFileSystemInfos().Length > 0)
                    throw new IOException(Resources.DestinationDirExist);
            }
            else _destination.Create();

            State = TaskState.Header;
            var sourceDirectories = new List<DirectoryInfo>();
            var sourceFiles = new List<FileInfo>();
            _source.Walk(sourceDirectories.Add, sourceFiles.Add);
            UnitsTotal = sourceFiles.Sum(file => file.Length);

            State = TaskState.Data;
            CopyDirectories(sourceDirectories);
            CopyFiles(sourceFiles);
            if (PreserveDirectoryTimestamps)
                CopyDirectoryTimestamps(sourceDirectories);
        }

        private void CopyDirectories(IEnumerable<DirectoryInfo> sourceDirectories)
        {
            foreach (var sourceDirectory in sourceDirectories)
            {
                CancellationToken.ThrowIfCancellationRequested();

                if (sourceDirectory.IsSymlink(out string? symlinkTarget))
                    CreateSymlink(PathInDestination(sourceDirectory), symlinkTarget);
                else
                    Directory.CreateDirectory(PathInDestination(sourceDirectory));
            }
        }

        private void CopyFiles(IEnumerable<FileInfo> sourceFiles)
        {
            foreach (var sourceFile in sourceFiles)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var destinationFile = new FileInfo(PathInDestination(sourceFile));
                if (destinationFile.Exists)
                {
                    if (!Overwrite)
                    {
                        UnitsProcessed += sourceFile.Length;
                        continue;
                    }
                    try
                    {
                        destinationFile.Attributes &= ~(FileAttributes.ReadOnly | FileAttributes.Hidden);
                    }
                    #region Error handling
                    catch (ArgumentException ex)
                    {
                        // The .NET BCL implementation of FileSystemInfo.Attributes_set raises ArgumentException instead of UnauthorizedAccessException for ERROR_ACCESS_DENIED
                        throw new UnauthorizedAccessException(ex.Message, ex);
                    }
                    #endregion
                }

                if (sourceFile.IsSymlink(out string? symlinkTarget))
                    CreateSymlink(destinationFile.FullName, symlinkTarget);
                else
                    CopyFile(sourceFile, destinationFile);

                UnitsProcessed += sourceFile.Length;
            }
        }

        /// <summary>
        /// Copies a single file from one location to another. Can be overridden to modify the copying behavior.
        /// </summary>
        /// <exception cref="IOException">A problem occurred while copying the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Read access to the <paramref name="sourceFile"/> or write access to the <paramref name="destinationFile"/> is not permitted.</exception>
        protected virtual void CopyFile(FileInfo sourceFile, FileInfo destinationFile)
        {
            #region Sanity checks
            if (sourceFile == null) throw new ArgumentNullException(nameof(sourceFile));
            if (destinationFile == null) throw new ArgumentNullException(nameof(destinationFile));
            #endregion

            sourceFile.CopyTo(destinationFile.FullName, Overwrite);

            destinationFile.Refresh();
            destinationFile.Attributes &= ~(FileAttributes.ReadOnly | FileAttributes.Hidden);
            destinationFile.LastWriteTimeUtc = sourceFile.LastWriteTimeUtc;
        }

        /// <summary>
        /// Creates a Unix symbolic link. Can be overridden to modify the symlinking behavior.
        /// </summary>
        /// <param name="linkPath">The path of the link to create.</param>
        /// <param name="linkTarget">The path of the existing file or directory to point to (relative to <paramref name="linkPath"/>).</param>
        /// <exception cref="InvalidOperationException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        /// <exception cref="IOException">The underlying Unix subsystem failed to process the request (e.g. because of insufficient rights).</exception>
        protected virtual void CreateSymlink([Localizable(false)] string linkPath, [Localizable(false)] string linkTarget)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(linkPath)) throw new ArgumentNullException(nameof(linkPath));
            if (string.IsNullOrEmpty(linkTarget)) throw new ArgumentNullException(nameof(linkTarget));
            #endregion

            if (File.Exists(linkPath) && Overwrite) File.Delete(linkPath);
            FileUtils.CreateSymlink(linkPath, linkTarget);
        }

        private void CopyDirectoryTimestamps(IEnumerable<DirectoryInfo> sourceDirectories)
        {
            foreach (var sourceDirectory in sourceDirectories)
            {
                CancellationToken.ThrowIfCancellationRequested();

                Directory.SetLastWriteTimeUtc(PathInDestination(sourceDirectory), sourceDirectory.LastWriteTimeUtc);
            }
        }

        private string PathInDestination(FileSystemInfo element) => Path.Combine(DestinationPath, element.RelativeTo(_source!));
    }
}
