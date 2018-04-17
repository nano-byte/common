// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;

namespace NanoByte.Common.Storage
{
    /// <summary>
    /// Moves the content of a directory to a new location preserving the original file modification times.
    /// </summary>
    public class MoveDirectory : CopyDirectory
    {
        /// <summary>
        /// Creates a new directory move task.
        /// </summary>
        /// <param name="sourcePath">The path of source directory. Must exist!</param>
        /// <param name="destinationPath">The path of the target directory. May exist. Must be empty if <paramref name="overwrite"/> is <c>false</c>.</param>
        /// <param name="preserveDirectoryTimestamps"><c>true</c> to preserve the modification times for directories as well; <c>false</c> to preserve only the file modification times.</param>
        /// <param name="overwrite">Overwrite exisiting files and directories at the <paramref name="destinationPath"/>. This will even replace read-only files!</param>
        public MoveDirectory(string sourcePath, string destinationPath, bool preserveDirectoryTimestamps = true, bool overwrite = false)
            : base(sourcePath, destinationPath, preserveDirectoryTimestamps, overwrite)
        {}

        protected override void Execute()
        {
            base.Execute();

            Directory.Delete(SourcePath, recursive: true);
        }

        /// <inheritdoc/>
        protected override void CopyFile(FileInfo sourceFile, FileInfo destinationFile)
        {
            #region Sanity checks
            if (sourceFile == null) throw new ArgumentNullException(nameof(sourceFile));
            if (destinationFile == null) throw new ArgumentNullException(nameof(destinationFile));
            #endregion

            if (Overwrite && destinationFile.Exists) destinationFile.Delete();
            sourceFile.MoveTo(destinationFile.FullName);
        }
    }
}
