/*
 * Copyright 2006-2014 Bastian Eicher, Simon E. Silva Lauinger
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
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using NanoByte.Common.Native;
using NanoByte.Common.Properties;
using NanoByte.Common.Storage;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides methods for launching child processes in OS-specific ways.
    /// </summary>
    public static class ProcessUtils
    {
        /// <summary>
        /// Launches a .NET assembly located in the application's base directory.
        /// </summary>
        /// <param name="assembly">The name of the assembly to launch (without the file extension).</param>
        /// <param name="arguments">The command-line arguments to pass to the assembly; can be <see langword="null"/>.</param>
        /// <returns>The newly created process.</returns>
        /// <exception cref="FileNotFoundException">The assembly could not be located.</exception>
        /// <exception cref="Win32Exception">There was a problem launching the assembly.</exception>
        [PublicAPI]
        public static Process LaunchAssembly([NotNull, Localizable(false)] string assembly, [CanBeNull, Localizable(false)] string arguments = null)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(assembly)) throw new ArgumentNullException("assembly");
            #endregion

            Log.Debug("Launching assembly: " + assembly.EscapeArgument() + " " + arguments);

            return Process.Start(CreateAssemblyStartInfo(assembly, arguments));
        }

        /// <summary>
        /// Launches a .NET assembly located in the application's base directory and waits for it to exit.
        /// </summary>
        /// <param name="assembly">The name of the assembly to launch (without the file extension).</param>
        /// <param name="arguments">The command-line arguments to pass to the assembly; can be <see langword="null"/>.</param>
        /// <returns>The exit code of the target process.</returns>
        /// <exception cref="FileNotFoundException">The assembly could not be located.</exception>
        [PublicAPI]
        public static int RunAssembly([NotNull, Localizable(false)] string assembly, [CanBeNull, Localizable(false)] string arguments = null)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(assembly)) throw new ArgumentNullException("assembly");
            #endregion

            Log.Debug("Running assembly: " + assembly + " " + arguments);

            try
            {
                var process = Process.Start(CreateAssemblyStartInfo(assembly, arguments));
                Debug.Assert(process != null);
                process.WaitForExit();
                return process.ExitCode;
            }
            catch (Win32Exception)
            {
                return 1;
            }
        }

        /// <summary>
        /// Launches a .NET assembly located in the application's base directory as an administrator (using UAC).
        /// </summary>
        /// <param name="assembly">The name of the assembly to launch (without the file extension).</param>
        /// <param name="arguments">The command-line arguments to pass to the assembly; can be <see langword="null"/>.</param>
        /// <returns>The newly created process.</returns>
        /// <exception cref="FileNotFoundException">The assembly could not be located.</exception>
        /// <exception cref="Win32Exception">There was a problem launching the assembly.</exception>
        [PublicAPI]
        public static Process LaunchAssemblyAsAdmin([NotNull, Localizable(false)] string assembly, [CanBeNull, Localizable(false)] string arguments = null)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(assembly)) throw new ArgumentNullException("assembly");
            #endregion

            Log.Debug("Launching assembly as admin: " + assembly + " " + arguments);

            return Process.Start(CreateAssemblyStartInfo(assembly, arguments, admin: true));
        }

        /// <summary>
        /// Launches a .NET assembly located in the application's base directory as an administrator (using UAC) and waits for it to exit.
        /// </summary>
        /// <param name="assembly">The name of the assembly to launch (without the file extension).</param>
        /// <param name="arguments">The command-line arguments to pass to the assembly; can be <see langword="null"/>.</param>
        /// <returns>The exit code of the target process.</returns>
        /// <exception cref="FileNotFoundException">The assembly could not be located.</exception>
        [PublicAPI]
        public static int RunAssemblyAsAdmin([NotNull, Localizable(false)] string assembly, [CanBeNull, Localizable(false)] string arguments = null)
        {
            Log.Debug("Running assembly as admin: " + assembly + " " + arguments);

            try
            {
                var process = Process.Start(CreateAssemblyStartInfo(assembly, arguments, admin: true));
                Debug.Assert(process != null);
                process.WaitForExit();
                return process.ExitCode;
            }
            catch (Win32Exception)
            {
                return 1;
            }
        }

        private static ProcessStartInfo CreateAssemblyStartInfo(string assembly, string arguments, bool admin = false)
        {
            string appPath = Path.Combine(Locations.InstallBase, assembly + ".exe");
            if (!File.Exists(appPath)) throw new FileNotFoundException(string.Format(Resources.UnableToLocateAssembly, assembly), appPath);

            // Only Windows can directly launch .NET executables, other platforms must run through Mono
            var startInfo = WindowsUtils.IsWindows
                ? new ProcessStartInfo(appPath, arguments)
                : new ProcessStartInfo("mono", appPath.EscapeArgument() + " " + arguments);

            if (admin && WindowsUtils.IsWindowsNT) startInfo.Verb = "runas";
            return startInfo;
        }
    }
}
