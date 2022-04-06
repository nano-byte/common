// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Diagnostics;
using NanoByte.Common.Streams;

namespace NanoByte.Common.Native;

/// <summary>
/// Runs a child process.
/// </summary>
public class ProcessLauncher : IProcessLauncher
{
    private readonly string _fileName;
    private readonly string? _arguments;

    static ProcessLauncher()
    {
        ProcessUtils.SanitizeEnvironmentVariables();
    }

    /// <summary>
    /// Prepares a new sub process.
    /// </summary>
    /// <param name="fileName">The file name of the executable to run.</param>
    /// <param name="arguments">The default arguments to always pass to the executable.</param>
    public ProcessLauncher(string fileName, string? arguments = null)
    {
        _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        _arguments = arguments;
    }

    /// <summary>
    /// Prepares a new sub process.
    /// </summary>
    /// <param name="startInfo">The file name of the executable to run and default arguments to always pass. Other information from the <see cref="ProcessStartInfo"/> is ignored!</param>
    public ProcessLauncher(ProcessStartInfo startInfo)
        : this(startInfo.FileName, startInfo.Arguments)
    {}

    /// <inheritdoc/>
    public Process Start(params string[] arguments)
        => GetStartInfo(arguments).Start();

    /// <inheritdoc/>
    public void Run(params string[] arguments)
        => WaitForExit(Start(arguments));

    /// <inheritdoc/>
    public string RunAndCapture(Action<StreamWriter>? onStartup, params string[] arguments)
    {
        var startInfo = GetStartInfo(arguments);
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardInput = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        var process = startInfo.Start();
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

        WaitForExit(process);

        return stdout.ToString();
    }

    /// <inheritdoc/>
    public virtual ProcessStartInfo GetStartInfo(params string[] arguments)
    {
        #region Sanity checks
        if (arguments == null) throw new ArgumentNullException(nameof(arguments));
        #endregion

        return new()
        {
            FileName = _fileName,
            Arguments = string.IsNullOrEmpty(_arguments)
                ? arguments.JoinEscapeArguments()
                : arguments + " " + arguments.JoinEscapeArguments(),
            UseShellExecute = false,
            ErrorDialog = false
        };
    }

    /// <summary>
    /// Hook for waiting for a process to exit and handle its exit code.
    /// </summary>
    protected virtual void WaitForExit(Process process)
        => process.WaitForSuccess();

    /// <summary>
    /// Hook for handling stderr messages from the process.
    /// </summary>
    /// <param name="line">The line written to stderr.</param>
    /// <param name="stdin">The stream writer providing access to stdin.</param>
    protected virtual void OnStderr(string line, StreamWriter stdin)
        => Log.Warn($"{_fileName}: {line}");
}
