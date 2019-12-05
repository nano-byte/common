// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

#if NETSTANDARD
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
        /// Indicates whether the current process is running in an interactive session (rather than, e.g. as a CI job or a service).
        /// </summary>
        public static bool IsInteractive
            => string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")) && WindowsUtils.IsInteractive;

        /// <summary>
        /// <c>true</c> if the current operating system is 64-bit capable; <c>false</c> otherwise.
        /// </summary>
        public static bool Is64BitOperatingSystem
#if NETSTANDARD
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
#if NETSTANDARD
            => (RuntimeInformation.ProcessArchitecture == Architecture.X64) || (RuntimeInformation.OSArchitecture == Architecture.Arm64);
#elif NET45
            => Environment.Is64BitProcess;
#else
            => IntPtr.Size == 8;
#endif

        private static readonly Regex
            _envVariableLongStyle = new Regex(@"\${([^{}]+)}"),
            _envVariableShortStyle = new Regex(@"\$([^{}\$\s\\/-]+)");

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
                if (x.Index >= 1 && value[x.Index - 1] == '$') // Treat $$ as escaping
                    return "{" + x.Groups[1].Value + "}";

                var parts = x.Groups[1].Value.Split(new[] {":-"}, StringSplitOptions.None);
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
                if (x.Index >= 1 && value[x.Index - 1] == '$') // Treat $$ as escaping
                    return x.Groups[1].Value;

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
