// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Tasks;

namespace NanoByte.Common.Storage;

/// <summary>
/// Recursively iterates over all elements in a directory.
/// </summary>
public abstract class ReadDirectoryBase : TaskBase
{
    /// <summary>
    /// The directory to read.
    /// </summary>
    protected readonly DirectoryInfo Source;

    /// <summary>
    /// Creates a new directory read task.
    /// </summary>
    /// <param name="path">The path of the directory to read.</param>
    protected ReadDirectoryBase([Localizable(false)] string path)
    {
        Source = new(Path.GetFullPath(path));
    }

    /// <inheritdoc/>
    protected override bool UnitsByte => true;

    /// <summary>
    /// Controls whether to follow symlinks.
    /// </summary>
    public bool FollowSymlinks { get; init; }

    /// <inheritdoc/>
    protected override void Execute()
    {
        var files = new List<FileInfo>();
        var directories = new List<DirectoryInfo>();

        State = TaskState.Header;
        EnumerateElements(Source, files, directories);
        UnitsTotal = files.Sum(file => file.Length);

        State = TaskState.Data;

        foreach (var directory in directories)
        {
            CancellationToken.ThrowIfCancellationRequested();
            HandleDirectory(directory);
        }

        var fileIDs = new Dictionary<long, FileInfo>();
        foreach (var file in files)
        {
            CancellationToken.ThrowIfCancellationRequested();
            HandleFile(file, TryFindHardlink(file, fileIDs));
            UnitsProcessed += file.Length;
        }

        State = TaskState.Complete;
    }

    private static FileInfo? TryFindHardlink(FileInfo file, IDictionary<long, FileInfo> fileIDs)
    {
        long fileID;
        try
        {
            fileID = file.GetFileID();
        }
        catch (IOException)
        {
            return null;
        }

        if (fileIDs.TryGetValue(fileID, out var existingFile))
            return existingFile;
        else
        {
            fileIDs.Add(fileID, file);
            return null;
        }
    }

    private void EnumerateElements(DirectoryInfo directory, List<FileInfo> files, ICollection<DirectoryInfo> directories)
    {
        CancellationToken.ThrowIfCancellationRequested();

        files.AddRange(directory.GetFiles());
        foreach (var subDir in directory.GetDirectories())
        {
            directories.Add(subDir);
            if (FollowSymlinks || !FileUtils.IsSymlink(subDir.FullName))
                EnumerateElements(subDir, files, directories);
        }
    }

    /// <summary>
    /// Called once for every sub-directory below <see cref="Source"/>.
    /// </summary>
    /// <param name="directory">The directory to handle.</param>
    protected abstract void HandleDirectory(DirectoryInfo directory);

    /// <summary>
    /// Called once for every file below <see cref="Source"/>.
    /// </summary>
    /// <param name="file">The file to handle.</param>
    /// <param name="hardlinkTarget">A previously handled file that is hardlinked to <paramref name="file"/>. May be <c>null</c>.</param>
    protected abstract void HandleFile(FileInfo file, FileInfo? hardlinkTarget = null);
}
