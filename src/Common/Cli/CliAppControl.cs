// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics;
using System.IO;
using NanoByte.Common.Properties;
using NanoByte.Common.Streams;

namespace NanoByte.Common.Cli
{
    /// <summary>
    /// Provides an interface to an external command-line application controlled via arguments and stdin and monitored via stdout and stderr.
    /// </summary>
    public abstract class CliAppControl
    {
        /// <summary>
        /// The name of the application's binary (without a file extension).
        /// </summary>
        protected abstract string AppBinary { get; }

        /// <summary>
        /// Runs the external application interactively instead of processing its output.
        /// </summary>
        /// <param name="arguments">Command-line arguments to launch the application with.</param>
        /// <returns>The newly launched process; <c>null</c> if an existing process was reused.</returns>
        /// <exception cref="IOException">The external application could not be launched.</exception>
        public Process StartInteractive(params string[] arguments)
        {
            #region Sanity checks
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            #endregion

            return new ProcessStartInfo
            {
                FileName = AppBinary,
                Arguments = arguments.JoinEscapeArguments()
            }.Start();
        }

        /// <summary>
        /// Runs the external application, processes its output and waits until it has terminated.
        /// </summary>
        /// <param name="arguments">Command-line arguments to launch the application with.</param>
        /// <returns>The application's complete stdout output.</returns>
        /// <exception cref="IOException">The external application could not be launched.</exception>
        public virtual string Execute(params string[] arguments)
        {
            #region Sanity checks
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            #endregion

            Process process;
            try
            {
                process = GetStartInfo(arguments).Start();
            }
            #region Error handling
            catch (IOException ex)
            {
                throw new IOException(string.Format(Resources.UnableToLaunchBundled, AppBinary), ex);
            }
            #endregion

            var stdout = new StreamConsumer(process.StandardOutput);
            var stderr = new StreamConsumer(process.StandardError);
            var stdin = process.StandardInput;

            InitStdin(stdin);
            do
            {
                HandlePending(stderr, stdin);
            } while (!process.WaitForExit(50));
            stdout.WaitForEnd();
            stderr.WaitForEnd();
            HandlePending(stderr, stdin);

            return stdout.ToString();
        }

        /// <summary>
        /// Creates the <see cref="ProcessStartInfo"/> used by <see cref="Execute"/> to launch the external application and redirect its input/output.
        /// </summary>
        /// <param name="arguments">The arguments to pass to the process at startup.</param>
        protected virtual ProcessStartInfo GetStartInfo(params string[] arguments)
        {
            #region Sanity checks
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            #endregion

            ProcessUtils.SanitizeEnvironmentVariables();

            var startInfo = new ProcessStartInfo
            {
                FileName = AppBinary,
                Arguments = arguments.JoinEscapeArguments(),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                ErrorDialog = false
            };

            // Suppress localization to enable programmatic parsing of output
            startInfo.EnvironmentVariables["LANG"] = "C";

            return startInfo;
        }

        /// <summary>
        /// A hook method for writing to the application's stdin right after startup.
        /// </summary>
        /// <param name="writer">The stream writer providing access to stdin.</param>
        protected virtual void InitStdin(StreamWriter writer) {}

        /// <summary>
        /// Reads all currently pending <paramref name="stderr"/> lines and sends responses to <paramref name="stdin"/>.
        /// </summary>
        private void HandlePending(StreamConsumer stderr, StreamWriter stdin)
        {
            string? line;
            while ((line = stderr.ReadLine()) != null)
            {
                string? response = HandleStderr(line);
                if (response != null) stdin.WriteLine(response);
            }
        }

        /// <summary>
        /// A hook method for handling stderr messages from the CLI application.
        /// </summary>
        /// <param name="line">The line written to stderr.</param>
        /// <returns>The response to write to stdin; <c>null</c> for none.</returns>
        protected virtual string? HandleStderr(string line)
        {
            Log.Warn(AppBinary + ": " + line);
            return null;
        }
    }
}
