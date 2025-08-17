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
    /// The operating system platform (e.g. Windows NT).
    /// </summary>
    [XmlAttribute("platform")]
    public string Platform;

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
    /// The version of the operating system (e.g. 6.0 for Vista).
    /// </summary>
    [XmlAttribute("version")]
    public string Version;

    /// <summary>
    /// The service pack level (e.g. "Service Pack 1").
    /// </summary>
    [XmlAttribute("service-pack"), DefaultValue("")]
    public string ServicePack;

    /// <summary>
    /// The version of the operating system (e.g. 6.0 for Vista).
    /// </summary>
    [XmlAttribute("framework-version")]
    public string FrameworkVersion;

    /// <summary>
    /// Information about the current operating system.
    /// </summary>
    public static OSInfo Current { get; } = GetCurrent();

    private static OSInfo GetCurrent() => new()
    {
#if NET20 || NET40
            FrameworkVersion = Environment.Version.ToString(),
            Platform = Environment.OSVersion.Platform.ToString(),
#else
        FrameworkVersion = RuntimeInformation.FrameworkDescription,
        Platform = RuntimeInformation.OSDescription,
        OSArchitecture = RuntimeInformation.OSArchitecture,
        ProcessArchitecture = RuntimeInformation.ProcessArchitecture,
#endif
        Version = Environment.OSVersion.Version.ToString(),
        ServicePack = Environment.OSVersion.ServicePack
    };
}
