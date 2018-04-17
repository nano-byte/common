// Copyright Bastian Eicher
// Licensed under the MIT License

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
        public override string ToString() => _args;

        private readonly List<string> _files = new List<string>();

        /// <summary>
        /// A list of all file names in the arguments.
        /// </summary>
        public IEnumerable<string> Files => _files;

        private readonly Dictionary<string, string> _commands = new Dictionary<string, string>();

        /// <summary>
        /// A list of all commands without leading slash or hyphen in the arguments.
        /// </summary>
        public IEnumerable<string> Commands => _commands.Keys;

        /// <summary>
        /// Gets the options for a specific command in the arguments.
        /// </summary>
        /// <param name="command">The command to get the options for.</param>
        /// <returns>The options for <paramref name="command"/> if any; null otherwise.</returns>
        [CanBeNull]
        public string this[[NotNull] string command] => _commands[command];
        #endregion

        #region Constructor
        #region Helper
        /// <returns><c>true</c> if <paramref name="value"/> starts with a slash or a hyphen.</returns>
        private static bool IsCommand(string value) => value.StartsWith("/", StringComparison.Ordinal) || value.StartsWith("-", StringComparison.Ordinal);
        #endregion

        /// <summary>
        /// Creates a new arguments instance based on the argument array from a Main method.
        /// </summary>
        /// <param name="args">The array of arguments.</param>
        public Arguments([NotNull] string[] args)
        {
            #region Sanity checks
            if (args == null) throw new ArgumentNullException(nameof(args));
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
        public bool Contains(string command) => _commands.ContainsKey(command);
    }
}
