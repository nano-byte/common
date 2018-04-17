// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Xml.Serialization;
using NanoByte.Common.Native;

#if NETSTANDARD2_0
using System.Runtime.InteropServices;
#endif

namespace NanoByte.Common.Info
{
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

        /// <summary>
        /// True if the operating system is a 64-bit version of Windows.
        /// </summary>
        [XmlAttribute("is64bit")]
        public bool Is64Bit;

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

        public override string ToString() => $"{Platform}{(Is64Bit ? " 64-bit" : "")} {Version} {ServicePack}";

        #region Static
        /// <summary>
        /// Information about the current operating system.
        /// </summary>
        public static OSInfo Current { get; } = Load();

        private static OSInfo Load() => new OSInfo
        {
#if NETSTANDARD2_0
            FrameworkVersion = RuntimeInformation.FrameworkDescription,
            Platform = RuntimeInformation.OSDescription,
#else
            FrameworkVersion = Environment.Version.ToString(),
            Platform = Environment.OSVersion.Platform.ToString(),
#endif
            Is64Bit = OSUtils.Is64BitOperatingSystem,
            Version = Environment.OSVersion.Version.ToString(),
            ServicePack = Environment.OSVersion.ServicePack
        };
        #endregion
    }
}
