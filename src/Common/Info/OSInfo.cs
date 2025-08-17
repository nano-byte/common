// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Xml.Serialization;

#if !NET20 && !NET40
using System.Runtime.InteropServices;
#endif

namespace NanoByte.Common.Info;

/// <summary>
/// Wraps information about an operating system in a serializer-friendly format.
/// </summary>
[XmlType("os")]
public struct OSInfo
{
    /// <summary>
    /// The operating system platform (e.g., <c>Win32NT</c> or <c>win</c>).
    /// </summary>
    [XmlAttribute("platform")]
    public string Platform;

    /// <summary>
    /// The version of the operating system (e.g., <c>6.0</c> for Vista).
    /// </summary>
    [XmlAttribute("version")]
    public string Version;

    /// <summary>
    /// The version of .NET running the current process.
    /// </summary>
    [XmlAttribute("dotnet-version")]
    public string DotNetVersion;

#if !NET20 && !NET40
    /// <summary>
    /// The processor architecture of the operating system.
    /// </summary>
    [XmlAttribute("os-architecture")]
    public Architecture OSArchitecture { get; set; }

    /// <summary>
    /// The (potentially emulated) processor architecture of the running process.
    /// </summary>
    [XmlAttribute("process-architecture")]
    public Architecture ProcessArchitecture { get; set; }
#endif

    /// <summary>
    /// Information about the current operating system.
    /// </summary>
    public static OSInfo Current { get; } = GetCurrent();

    private static OSInfo GetCurrent() => new()
    {
#if NET
        Platform = RuntimeInformation.RuntimeIdentifier.GetLeftPartAtFirstOccurrence('-'),
#else
        Platform = Environment.OSVersion.Platform.ToString(),
#endif
        Version = Environment.OSVersion.Version.ToString(),
        DotNetVersion = Environment.Version.ToString(),
#if !NET20 && !NET40
        OSArchitecture = RuntimeInformation.OSArchitecture,
        ProcessArchitecture = RuntimeInformation.ProcessArchitecture,
#endif
    };
}
