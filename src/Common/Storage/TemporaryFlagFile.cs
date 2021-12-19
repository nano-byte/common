// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Storage;

/// <summary>
/// A temporary directory with a file that may or may not exist to indicate whether a certain condition is true or false.
/// </summary>
[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
public class TemporaryFlagFile : TemporaryDirectory
{
    /// <inheritdoc/>
    public TemporaryFlagFile(string prefix)
        : base(prefix)
    {}

    /// <summary>
    /// The fully qualified path of the flag file.
    /// </summary>
    public new string Path => System.IO.Path.Combine(base.Path, "flag");

    public static implicit operator string(TemporaryFlagFile dir) => dir.Path;

    /// <summary>
    /// Indicates or controls whether the file exists.
    /// </summary>
    public bool Set
    {
        get => File.Exists(Path);
        set
        {
            if (value) File.WriteAllText(Path, "");
            else File.Delete(Path);
        }
    }
}