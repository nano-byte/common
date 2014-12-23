/*
 * Copyright 2006-2014 Bastian Eicher
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
using JetBrains.Annotations;

namespace NanoByte.Common
{
    /// <summary>
    /// An immutable class representing command-line arguments passed to an application.
    /// </summary>
    public class Arguments
    {
        #region Properties
        private readonly string _args;

        /// <summary>
        /// Returns the arguments stored in this instance.
        /// </summary>
        public override string ToString()
        {
            return _args;
        }

        private readonly List<string> _files = new List<string>();

        /// <summary>
        /// A list of all file names in the arguments.
        /// </summary>
        public IEnumerable<string> Files { get { return _files; } }

        private readonly Dictionary<string, string> _commands = new Dictionary<string, string>();

        /// <summary>
        /// A list of all commands without leading slash or hyphen in the arguments.
        /// </summary>
        public IEnumerable<string> Commands { get { return _commands.Keys; } }

        /// <summary>
        /// Gets the options for a specific command in the arguments.
        /// </summary>
        /// <param name="command">The command to get the options for.</param>
        /// <returns>The options for <paramref name="command"/> if any; null otherwise.</returns>
        [CanBeNull]
        public string this[[NotNull] string command] { get { return _commands[command]; } }
        #endregion

        #region Constructor

        #region Helper
        /// <returns><see langword="true"/> if <paramref name="value"/> starts with a slash or a hyphen.</returns>
        private static bool IsCommand(string value)
        {
            return value.StartsWith("/", StringComparison.Ordinal) || value.StartsWith("-", StringComparison.Ordinal);
        }
        #endregion

        /// <summary>
        /// Creates a new arguments instance based on the argument array from a Main method.
        /// </summary>
        /// <param name="args">The array of arguments.</param>
        public Arguments([NotNull] string[] args)
        {
            #region Sanity checks
            if (args == null) throw new ArgumentNullException("args");
            #endregion

            _args = string.Concat(args);

            // Temp collections for building the lists
            _files.Clear();

            // Separate the arguments element-wise into categories
            for (int i = 0; i < args.Length; i++)
            {
                if (IsCommand(args[i]))
                {
                    // Is the next element of the argument another command or an option?
                    if (i + 1 < args.Length && !IsCommand(args[i + 1]))
                    { // Command with an option (remove leading slash or hypen)
                        _commands[args[i].Remove(0, 1)] = args[++i];
                    }
                    else
                    { // Command without an option (remove leading slash or hypen)
                        _commands[args[i].Remove(0, 1)] = null;
                    }
                }
                else _files.Add(args[i]);
            }
        }
        #endregion

        /// <summary>
        /// Determines whether a specific command is contained in the arguments.
        /// </summary>
        /// <param name="command">The command to check for.</param>
        /// <returns>True if the command was set; false otherwise.</returns>
        public bool Contains(string command)
        {
            return _commands.ContainsKey(command);
        }
    }
}
