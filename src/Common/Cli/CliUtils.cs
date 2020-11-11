// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.IO;
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
        public static string ReadString([Localizable(true)] string prompt)
        {
            Console.Error.Write(prompt + " ");
            string? line = Console.ReadLine();
            if (line == null) throw new IOException("input stream closed, unable to get user input");
            return line;
        }

        /// <summary>
        /// Asks the user to input a password without echoing it.
        /// </summary>
        /// <param name="prompt">The prompt to display to the user on <see cref="Console.Error"/>.</param>
        /// <returns>The password the user entered; <see cref="string.Empty"/> if none.</returns>
        public static string ReadPassword([Localizable(true)] string prompt)
        {
            Console.Error.Write(prompt + " ");

            string password = "";

            var key = Console.ReadKey(intercept: true);
            while (key.Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
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
