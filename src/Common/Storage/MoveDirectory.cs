// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

/// <summary>
/// Moves the content of a directory to a new location preserving file timestamps, symlinks and hard links.
/// </summary>
public class MoveDirectory : CopyDirectory
{
    /// <summary>
    /// Creates a new directory move task.
    /// </summary>
    /// <param name="sourcePath">The path of source directory. Must exist!</param>
    /// <param name="destinationPath">The path of the target directory. May exist.</param>>
    public MoveDirectory([Localizable(false)] string sourcePath, [Localizable(false)] string destinationPath)
        : base(sourcePath, destinationPath)
    {}

    protected override void Execute()
    {
        base.Execute();

        Source.Delete(recursive: true);
    }

    /// <inheritdoc/>
    protected override void CopyFile(FileInfo sourceFile, FileInfo destinationFile)
    {
        if (Overwrite && destinationFile.Exists) destinationFile.Delete();
        sourceFile.MoveTo(destinationFile.FullName);
    }
}
