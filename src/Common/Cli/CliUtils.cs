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
using System.IO;
using JetBrains.Annotations;
using NanoByte.Common.Native;

namespace NanoByte.Common.Cli
{
    /// <summary>
    /// Provides helper methods for communication with the user via the <see cref="Console"/>.
    /// </summary>
    public static partial class CliUtils
    {
        /// <summary>
        /// Asks the user to input a string.
        /// </summary>
        /// <param name="prompt">The prompt to display to the user on <see cref="Console.Error"/>.</param>
        /// <returns>The string the user entered.</returns>
        /// <exception cref="IOException">The <see cref="Console.In"/> stream has been closed.</exception>
        [NotNull]
        public static string ReadString([NotNull, Localizable(true)] string prompt)
        {
            Console.Error.Write(prompt + " ");
            string line = Console.ReadLine();
            if (line == null) throw new IOException("input stream closed, unable to get user input");
            return line;
        }

        /// <summary>
        /// Asks the user to input a password without echoing it.
        /// </summary>
        /// <param name="prompt">The prompt to display to the user on <see cref="Console.Error"/>.</param>
        /// <returns>The password the user entered; <see cref="string.Empty"/> if none.</returns>
        [NotNull]
        public static string ReadPassword([NotNull, Localizable(true)] string prompt)
        {
            Console.Error.Write(prompt + " ");

            string password = "";

            var key = Console.ReadKey(intercept: true);
            while (key.Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                        password = password.StripFromEnd(count: 1);
                }
                else password += key.KeyChar;

                key = Console.ReadKey(true);
            }
            Console.Error.WriteLine();

            return password;
        }

        /// <summary>
        /// Indicates whether the stdout stream has been redirected.
        /// </summary>
        /// <remarks>This only works on Windows systems. On other operating systems it always returns <c>false</c>.</remarks>
        public static bool StandardInputRedirected
        {
            get
            {
                if (!WindowsUtils.IsWindows) return false;
                return SafeNativeMethods.FileType.Char != SafeNativeMethods.GetFileType(SafeNativeMethods.GetStdHandle(SafeNativeMethods.StdHandle.Stdin));
            }
        }

        /// <summary>
        /// Indicates whether the stdout stream has been redirected.
        /// </summary>
        /// <remarks>This only works on Windows systems. On other operating systems it always returns <c>false</c>.</remarks>
        public static bool StandardOutputRedirected
        {
            get
            {
                if (!WindowsUtils.IsWindows) return false;
                return SafeNativeMethods.FileType.Char != SafeNativeMethods.GetFileType(SafeNativeMethods.GetStdHandle(SafeNativeMethods.StdHandle.Stdout));
            }
        }

        /// <summary>
        /// Indicates whether the stdout stream has been redirected.
        /// </summary>
        /// <remarks>This only works on Windows systems. On other operating systems it always returns <c>false</c>.</remarks>
        public static bool StandardErrorRedirected
        {
            get
            {
                if (!WindowsUtils.IsWindows) return false;
                return SafeNativeMethods.FileType.Char != SafeNativeMethods.GetFileType(SafeNativeMethods.GetStdHandle(SafeNativeMethods.StdHandle.Stderr));
            }
        }
    }
}
