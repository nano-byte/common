// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

/// <summary>
/// Changes the current working working directory until the object is disposed.
/// </summary>
public sealed class WorkingDirectory : IDisposable
{
    private readonly string _previousWorkingDirectory;

    /// <summary>
    /// Changes the current working directory to <paramref name="path"/>.
    /// </summary>
    public WorkingDirectory(string path)
    {
        _previousWorkingDirectory = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(path);
    }

    /// <summary>
    /// Restores the previous working directory.
    /// </summary>
    public void Dispose()
    {
        Directory.SetCurrentDirectory(_previousWorkingDirectory);
    }
}