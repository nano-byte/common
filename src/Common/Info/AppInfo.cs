// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace NanoByte.Common.Info
{
    /// <summary>
    /// Wraps information about an application in a serializer-friendly format.
    /// </summary>
    [XmlType("application")]
    public struct AppInfo
    {
        /// <summary>
        /// The name of the application.
        /// </summary>
        [XmlAttribute("name")]
        public string? Name { get; set; }

        /// <summary>
        /// The name of the product the application is a part of.
        /// </summary>
        [XmlAttribute("product-name")]
        public string? ProductName { get; set; }

        /// <summary>
        /// The version number of the application.
        /// </summary>
        [XmlAttribute("version")]
        public string? Version { get; set; }

        /// <summary>
        /// The <see cref="Name"/> and <see cref="Version"/> combined.
        /// </summary>
        [XmlIgnore]
        public string NameVersion => Name + " " + Version;

        /// <summary>
        /// The copyright information for the application.
        /// </summary>
        [XmlIgnore]
        public string? Copyright { get; set; }

        /// <summary>
        /// A description of the application.
        /// </summary>
        [XmlIgnore]
        public string? Description { get; set; }

        /// <summary>
        /// The command-line arguments the application was started with.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Used for XML serialization.")]
        [XmlElement("arg")]
        public string[]? Arguments { get; set; }

        /// <summary>
        /// Information about the currently running application (looks at the entry assembly).
        /// </summary>
        public static AppInfo Current
        {
            get
            {
                var appInfo = Load(Assembly.GetEntryAssembly());
                if (appInfo.Name == null || appInfo.Name.Length < 2) appInfo.Name = "Hosted";
                appInfo.Arguments = Environment.GetCommandLineArgs();
                return appInfo;
            }
        }

        /// <summary>
        /// Information about the currently running library (looks at the calling assembly).
        /// </summary>
        public static AppInfo CurrentLibrary
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get => Load(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Loads application information for a specific <see cref="Assembly"/>.
        /// </summary>
        public static AppInfo Load(Assembly? assembly)
        {
            if (assembly == null) return new AppInfo();

            var assemblyInfo = assembly.GetName();
            return new AppInfo
            {
                Name = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? assemblyInfo.Name,
                ProductName = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? assemblyInfo.Name,
                Version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion?.GetLeftPartAtFirstOccurrence('+'), // trim Git commit hash
                Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description,
                Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright
            };
        }
    }
}
