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
using System.Xml.Serialization;
using NanoByte.Common.Native;

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

        private static OSInfo Load()
        {
            return new OSInfo
            {
                Platform = Environment.OSVersion.Platform.ToString(),
                Is64Bit = WindowsUtils.Is64BitOperatingSystem,
                Version = Environment.OSVersion.Version.ToString(),
                ServicePack = Environment.OSVersion.ServicePack,
                FrameworkVersion = Environment.Version.ToString()
            };
        }
        #endregion
    }
}
