/*
 * Copyright 2006-2017 Bastian Eicher
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

#if NETSTANDARD2_0
using System.Runtime.InteropServices;
#endif

namespace NanoByte.Common.Native
{
    /// <summary>
    /// Provides helper methods related to operating system functionality across multiple platforms.
    /// </summary>
    public static class OSUtils
    {
        /// <summary>
        /// <c>true</c> if the current operating system is 64-bit capable; <c>false</c> otherwise.
        /// </summary>
        public static bool Is64BitOperatingSystem
#if NETSTANDARD2_0
            => (RuntimeInformation.OSArchitecture == Architecture.X64) || (RuntimeInformation.OSArchitecture == Architecture.Arm64);
#elif NET45
            => Environment.Is64BitOperatingSystem;
#else
            => Is64BitProcess;
#endif

        /// <summary>
        /// <c>true</c> if the current process is 64-bit; <c>false</c> otherwise.
        /// </summary>
        public static bool Is64BitProcess
#if NETSTANDARD2_0
            => (RuntimeInformation.ProcessArchitecture == Architecture.X64) || (RuntimeInformation.OSArchitecture == Architecture.Arm64);
#elif NET45
            => Environment.Is64BitProcess;
#else
            => IntPtr.Size == 8;
#endif

        private static readonly Regex
            _envVariableLongStyle = new Regex(@"\${([^}]+)}"),
            _envVariableShortStyle = new Regex(@"\$([^\$\s\\/-]+)");

        /// <summary>
        /// Expands/substitutes any Unix-style environment variables in the string.
        /// </summary>
        /// <param name="value">The string containing variables to be expanded.</param>
        /// <param name="variables">The list of variables available for expansion.</param>
        /// <remarks>Supports default values for unset variables (<c>${VAR-default}</c>) and for unset or empty variables (<c>${VAR:-default}</c>).</remarks>
        [NotNull]
        public static string ExpandVariables([NotNull] string value, [NotNull] IDictionary<string, string> variables)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (variables == null) throw new ArgumentNullException(nameof(variables));

            string intermediate = _envVariableLongStyle.Replace(value, x =>
            {
                var parts = x.Groups[1].Value.Split(new[] { ":-" }, StringSplitOptions.None);
                if (variables.TryGetValue(parts[0], out string ret) && !string.IsNullOrEmpty(ret))
                    return ret;
                else if (parts.Length > 1)
                    return StringUtils.Join(":-", parts.Skip(1));
                else
                {
                    parts = x.Groups[1].Value.Split('-');
                    if (variables.TryGetValue(parts[0], out ret))
                        return ret;
                    else if (parts.Length > 1)
                        return StringUtils.Join("-", parts.Skip(1));
                    else
                    {
                        Log.Warn($"Variable '{parts[0]}' not set. Defaulting to empty string.");
                        return "";
                    }
                }
            });

            return _envVariableShortStyle.Replace(intermediate, x =>
            {
                string key = x.Groups[1].Value;
                if (variables.TryGetValue(key, out string ret))
                    return ret;
                else
                {
                    Log.Warn($"Variable '{key}' not set. Defaulting to empty string.");
                    return "";
                }
            });
        }

        /// <summary>
        /// Expands/substitutes any Unix-style environment variables in the string.
        /// </summary>
        /// <param name="value">The string containing variables to be expanded.</param>
        /// <param name="variables">The list of variables available for expansion.</param>
        /// <remarks>Supports default values for unset variables (<c>${VAR-default}</c>) and for unset or empty variables (<c>${VAR:-default}</c>).</remarks>
        [NotNull]
        public static string ExpandVariables([NotNull] string value, [NotNull] StringDictionary variables)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (variables == null) throw new ArgumentNullException(nameof(variables));

            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string key in variables.Keys)
                dictionary[key] = variables[key];

            return ExpandVariables(value, dictionary);
        }
    }
}
