// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Diagnostics;

namespace NanoByte.Common.Streams;

/// <summary>
/// Runs a sub process with redirected stdin, stdout and stderr steams.
/// </summary>
public class SubProcess : ISubProcess
{
    private readonly string _fileName;
    private readonly string? _arguments;

    /// <summary>
    /// Prepares a new sub process.
    /// </summary>
    /// <param name="fileName">The file name of the executable to run.</param>
    /// <param name="arguments">The default arguments to always pass to the executable.</param>
    public SubProcess(string fileName, string? arguments = null)
    {
        _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        _arguments = arguments;
    }

    /// <summary>
    /// Prepares a new sub process.
    /// </summary>
    /// <param name="startInfo">The file name of the executable to run and default arguments to always pass. Other information from the <see cref="ProcessStartInfo"/> is ignored!</param>
    public SubProcess(ProcessStartInfo startInfo)
        : this(startInfo.FileName, startInfo.Arguments)
    {}

    /// <inheritdoc/>
    public string Run(Action<StreamWriter>? onStartup, params string[] arguments)
    {
        #region Sanity checks
        if (arguments == null) throw new ArgumentNullException(nameof(arguments));
        #endregion

        var process = GetStartInfo(arguments).Start();
        var stdin = process.StandardInput;
        var stdout = new StreamConsumer(process.StandardOutput);
        var stderr = new StreamConsumer(process.StandardError);

        void ReadStderr()
        {
            string? line;
            while ((line = stderr.ReadLine()) != null)
                OnStderr(line, stdin);
        }

        onStartup?.Invoke(stdin);
        while (!process.WaitForExit(50)) ReadStderr();
        stdout.WaitForEnd();
        stderr.WaitForEnd();
        ReadStderr();

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"{_fileName} exited with exit code {process.ExitCode}.");

        return stdout.ToString();
    }

    /// <summary>
    /// Creates the <see cref="ProcessStartInfo"/> used by <see cref="Run"/> to launch the process and redirect its input/output.
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
            FileName = _fileName,
            Arguments = string.IsNullOrEmpty(_arguments)
                ? arguments.JoinEscapeArguments()
                : arguments + " " + arguments.JoinEscapeArguments(),
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            ErrorDialog = false
        };

        return startInfo;
    }

    /// <summary>
    /// A hook for handling stderr messages from the process.
    /// </summary>
    /// <param name="line">The line written to stderr.</param>
    /// <param name="stdin">The stream writer providing access to stdin.</param>
    protected virtual void OnStderr(string line, StreamWriter stdin)
        => Log.Warn($"{_fileName}: {line}");
}