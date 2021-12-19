// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

/// <summary>
/// Like <see cref="TemporaryDirectory"/> but also sets the current working directory to <see cref="TemporaryDirectory.Path"/>.
/// </summary>
public sealed class TemporaryWorkingDirectory : TemporaryDirectory
{
    #region Variables
    private readonly string _oldWorkingDir;
    #endregion

    #region Constructor
    /// <inheritdoc/>
    public TemporaryWorkingDirectory(string prefix)
        : base(prefix)
    {
        // Remember the current working directory for later restoration and then change it
        _oldWorkingDir = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(Path);
    }
    #endregion

    #region Dispose
    public override void Dispose()
    {
        try
        {
            // Restore the original working directory
            Directory.SetCurrentDirectory(_oldWorkingDir);
        }
        finally
        {
            base.Dispose();
        }
    }
    #endregion
}