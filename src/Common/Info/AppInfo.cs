/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using NanoByte.Common.Values;

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
        public string Name { get; set; }

        /// <summary>
        /// The name of the product the application is a part of.
        /// </summary>
        [XmlAttribute("product-name")]
        public string ProductName { get; set; }

        /// <summary>
        /// The version number of the application.
        /// </summary>
        [XmlIgnore]
        public Version Version;

        /// <summary>Used for XML serialization.</summary>
        /// <seealso cref="Version"/>
        [XmlAttribute("version"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string VersionString { get => Version?.ToString(); set => Version = string.IsNullOrEmpty(value) ? null : new Version(value); }

        /// <summary>
        /// The <see cref="Name"/> and <see cref="Version"/> combined.
        /// </summary>
        [XmlIgnore]
        public string NameVersion => Name + " " + Version;

        /// <summary>
        /// The copyright information for the application.
        /// </summary>
        [XmlIgnore]
        public string Copyright { get; set; }

        /// <summary>
        /// A description of the application.
        /// </summary>
        [XmlIgnore]
        public string Description { get; set; }

        /// <summary>
        /// The command-line arguments the application was started with.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Used for XML serialization.")]
        [XmlElement("arg")]
        public string[] Arguments { get; set; }

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
        public static AppInfo Load(Assembly assembly)
        {
            if (assembly == null) return new AppInfo();

            var assemblyInfo = assembly.GetName();
            return new AppInfo
            {
                Name = assembly.GetAttributeValue((AssemblyTitleAttribute x) => x.Title) ?? assemblyInfo.Name,
                ProductName = assembly.GetAttributeValue((AssemblyProductAttribute x) => x.Product) ?? assemblyInfo.Name,
                Version = new Version(assemblyInfo.Version.Major, assemblyInfo.Version.Minor, assemblyInfo.Version.Build),
                Description = assembly.GetAttributeValue((AssemblyDescriptionAttribute x) => x.Description),
                Copyright = assembly.GetAttributeValue((AssemblyCopyrightAttribute x) => x.Copyright)
            };
        }
    }
}
