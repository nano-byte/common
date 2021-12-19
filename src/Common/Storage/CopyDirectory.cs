// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

/// <summary>
/// Copies the content of a directory to a new location preserving file timestamps, symlinks and hard links.
/// </summary>
public class CopyDirectory : ReadDirectoryBase
{
    private readonly DirectoryInfo _destination;

    /// <summary>
    /// Creates a new directory copy task.
    /// </summary>
    /// <param name="sourcePath">The path of source directory. Must exist!</param>
    /// <param name="destinationPath">The path of the target directory.</param>
    public CopyDirectory([Localizable(false)] string sourcePath, [Localizable(false)] string destinationPath)
        : base(sourcePath)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(sourcePath)) throw new ArgumentNullException(nameof(sourcePath));
        if (string.IsNullOrEmpty(destinationPath)) throw new ArgumentNullException(nameof(destinationPath));
        if (sourcePath == destinationPath) throw new ArgumentException(Resources.SourceDestinationEqual);
        #endregion

        _destination = new(destinationPath);
    }

    /// <inheritdoc/>
    public override string Name => Resources.CopyFiles;

    /// <inheritdoc/>
    protected override bool UnitsByte => true;

    /// <summary>
    /// Overwrite existing files and directories at the destination path. This will even replace read-only files!
    /// </summary>
    public bool Overwrite { get; init; }

    /// <inheritdoc />
    protected override void Execute()
    {
        if (_destination.Exists)
        { // Fail if overwrite is off but the target directory already exists and contains elements
            if (!Overwrite && _destination.GetFileSystemInfos().Length > 0)
                throw new IOException(Resources.DestinationDirExist);
        }
        else _destination.Create();

        base.Execute();
    }

    /// <inheritdoc/>
    protected override void HandleDirectory(DirectoryInfo directory)
    {
        if (!FollowSymlinks && directory.IsSymlink(out string? symlinkTarget))
            CreateSymlink(PathInDestination(directory), symlinkTarget);
        else
            Directory.CreateDirectory(PathInDestination(directory));
    }

    /// <inheritdoc/>
    protected override void HandleFile(FileInfo file, FileInfo? hardlinkTarget = null)
    {
        var destinationFile = new FileInfo(PathInDestination(file));
        if (destinationFile.Exists)
        {
            if (!Overwrite) return;
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

        if (hardlinkTarget != null)
            FileUtils.CreateHardlink(destinationFile.FullName, PathInDestination(hardlinkTarget));
        else
        {
            if (!FollowSymlinks && file.IsSymlink(out string? symlinkTarget))
                CreateSymlink(destinationFile.FullName, symlinkTarget);
            else
                CopyFile(file, destinationFile);
        }
    }

    /// <summary>
    /// Copies a single file from one location to another. Can be overridden to modify the copying behavior.
    /// </summary>
    /// <exception cref="IOException">A problem occurred while copying the file.</exception>
    /// <exception cref="UnauthorizedAccessException">Read access to the <paramref name="sourceFile"/> or write access to the <paramref name="destinationFile"/> is not permitted.</exception>
    protected virtual void CopyFile(FileInfo sourceFile, FileInfo destinationFile)
    {
        sourceFile.CopyTo(destinationFile.FullName, Overwrite);

        destinationFile.Refresh();
        destinationFile.Attributes &= ~(FileAttributes.ReadOnly | FileAttributes.Hidden);
        destinationFile.LastWriteTimeUtc = destinationFile.LastWriteTimeUtc;
    }

    /// <summary>
    /// Creates a symbolic link.
    /// </summary>
    /// <param name="linkPath">The path of the link to create.</param>
    /// <param name="linkTarget">The path of the existing file or directory to point to (relative to <paramref name="linkPath"/>).</param>
    /// <exception cref="IOException">A problem occurred while creating the symlink.</exception>
    /// <exception cref="UnauthorizedAccessException">Write access to the <paramref name="linkPath"/> is not permitted.</exception>
    private void CreateSymlink([Localizable(false)] string linkPath, [Localizable(false)] string linkTarget)
    {
        if (File.Exists(linkPath) && Overwrite) File.Delete(linkPath);
        FileUtils.CreateSymlink(linkPath, linkTarget);
    }

    private string PathInDestination(FileSystemInfo element)
        => Path.Combine(_destination.FullName, element.RelativeTo(Source));
}