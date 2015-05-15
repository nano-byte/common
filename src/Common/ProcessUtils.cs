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
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using NanoByte.Common.Native;
using NanoByte.Common.Properties;
using NanoByte.Common.Storage;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides methods for launching child processes.
    /// </summary>
    public static class ProcessUtils
    {
        /// <summary>
        /// Starts a new <see cref="Process"/> and runs it in parallel with this one. Handles and wraps <see cref="Win32Exception"/>s.
        /// </summary>
        /// <returns>The newly launched process; <see langword="null"/> if an existing process was reused.</returns>
        /// <exception cref="OperationCanceledException">The user was asked for intervention by the OS (e.g. a UAC prompt) and the user cancelled.</exception>
        /// <exception cref="FileNotFoundException">The executable file could not be found.</exception>
        /// <exception cref="IOException">There was a problem launching the executable.</exception>
        /// <exception cref="NotAdminException">The target process requires elevation but UAC is not available because <see cref="ProcessStartInfo.UseShellExecute"/> is <see langword="false"/>.</exception>
        [PublicAPI, CanBeNull]
        public static Process Start([NotNull] this ProcessStartInfo startInfo)
        {
            #region Sanity checks
            if (startInfo == null) throw new ArgumentNullException("startInfo");
            #endregion

            Log.Debug("Launching process: " + startInfo.FileName.EscapeArgument() + " " + startInfo.Arguments);
            try
            {
                return Process.Start(startInfo);
            }
            catch (Win32Exception ex)
            {
                switch (ex.NativeErrorCode)
                {
                    case WindowsUtils.Win32Cancelled:
                        throw new OperationCanceledException();

                    case WindowsUtils.Win32RequestedOperationRequiresElevation:
                        throw new NotAdminException(string.Format("'{0}' requires Administrator privileges.", startInfo.FileName));

                    default:
                        throw new IOException(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Starts a new <see cref="Process"/> and runs it in parallel with this one. Handles and wraps <see cref="Win32Exception"/>s.
        /// </summary>
        /// <param name="fileName">The path of the file to open or executable to launch.</param>
        /// <param name="arguments">The command-line arguments to pass to the executable.</param>
        /// <returns>The newly launched process; <see langword="null"/> if an existing process was reused.</returns>
        /// <exception cref="OperationCanceledException">The user was asked for intervention by the OS (e.g. a UAC prompt) and the user cancelled.</exception>
        /// <exception cref="FileNotFoundException">The executable file could not be found.</exception>
        /// <exception cref="IOException">There was a problem launching the executable.</exception>
        /// <exception cref="NotAdminException">The target process requires elevation but UAC is not available because <see cref="ProcessStartInfo.UseShellExecute"/> is <see langword="false"/>.</exception>
        [PublicAPI, CanBeNull]
        public static Process Start([NotNull] string fileName, [NotNull] params string[] arguments)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");
            if (arguments == null) throw new ArgumentNullException("arguments");
            #endregion

            return new ProcessStartInfo(fileName, arguments.JoinEscapeArguments()).Start();
        }

        /// <summary>
        /// Starts a new <see cref="Process"/> and waits for it to complete. Handles and wraps <see cref="Win32Exception"/>s.
        /// </summary>
        /// <returns>The exit code of the child process.</returns>
        /// <exception cref="OperationCanceledException">The user was asked for intervention by the OS (e.g. a UAC prompt) and the user cancelled.</exception>
        /// <exception cref="FileNotFoundException">The executable file could not be found.</exception>
        /// <exception cref="IOException">There was a problem launching the executable.</exception>
        /// <exception cref="NotAdminException">The target process requires elevation but UAC is not available because <see cref="ProcessStartInfo.UseShellExecute"/> is <see langword="false"/>.</exception>
        [PublicAPI]
        public static int Run([NotNull] this ProcessStartInfo startInfo)
        {
            #region Sanity checks
            if (startInfo == null) throw new ArgumentNullException("startInfo");
            #endregion

            var process = Start(startInfo);
            if (process == null) return 0;
            process.WaitForExit();
            return process.ExitCode;
        }


        /// <summary>
        /// Creates a <see cref="ProcessStartInfo"/> for launching an assembly located in <see cref="Locations.InstallBase"/>.
        /// </summary>
        /// <param name="name">The name of the assembly to launch (without the file extension).</param>
        /// <param name="arguments">The command-line arguments to pass to the assembl.</param>
        [PublicAPI, NotNull, Pure]
        public static ProcessStartInfo Assembly([NotNull] string name, [NotNull] params string[] arguments)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (arguments == null) throw new ArgumentNullException("arguments");
            #endregion

            string appPath = Path.Combine(Locations.InstallBase, name + ".exe");
            if (!File.Exists(appPath)) throw new FileNotFoundException(string.Format(Resources.UnableToLocateAssembly, name), appPath);

            // Only Windows can directly launch .NET executables, other platforms must run through Mono
            var startInfo = WindowsUtils.IsWindows
                ? new ProcessStartInfo(appPath, arguments.JoinEscapeArguments())
                : new ProcessStartInfo("mono", appPath.EscapeArgument() + " " + arguments.JoinEscapeArguments());


            return startInfo;
        }

        /// <summary>
        /// Modifies a <see cref="ProcessStartInfo"/> to request elevation to Administrator on Windows.
        /// </summary>
        [PublicAPI, NotNull]
        public static ProcessStartInfo AsAdmin([NotNull] this ProcessStartInfo startInfo)
        {
            #region Sanity checks
            if (startInfo == null) throw new ArgumentNullException("startInfo");
            #endregion

            if (WindowsUtils.IsWindowsNT)
            {
                startInfo.Verb = "runas";
                startInfo.UseShellExecute = true;
            }
            return startInfo;
        }
    }
}
