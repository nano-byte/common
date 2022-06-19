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
    {
        var process = Start(arguments);
        HandleExitCode(process.StartInfo, process.WaitForExitCode());
    }

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

        string? lastError = null;

        void ReadStderr()
        {
            string? line;
            while ((line = stderr.ReadLine()) != null)
            {
                lastError = line;
                OnStderr(line, stdin);
            }
        }

        onStartup?.Invoke(stdin);
        while (!process.WaitForExit(50)) ReadStderr();
        stdout.WaitForEnd();
        stderr.WaitForEnd();
        ReadStderr();

        HandleExitCode(process.StartInfo, process.WaitForExitCode(), lastError);

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
    /// Hook for handling exit codes.
    /// </summary>
    /// <param name="startInfo">The start info used to launch the process that has now exited.</param>
    /// <param name="exitCode">The <see cref="Process.ExitCode"/>.</param>
    /// <param name="message">An optional error message.</param>
    /// <exception cref="ExitCodeException"><paramref name="exitCode"/> had a non-zero value.</exception>
    protected virtual void HandleExitCode(ProcessStartInfo startInfo, int exitCode, string? message = null)
    {
        if (exitCode == 0) return;

        if (string.IsNullOrEmpty(message))
            throw new ExitCodeException(startInfo, exitCode);
        else
            throw new ExitCodeException(message, exitCode);
    }

    /// <summary>
    /// Hook for handling stderr messages from the process.
    /// </summary>
    /// <param name="line">The line written to stderr.</param>
    /// <param name="stdin">The stream writer providing access to stdin.</param>
    protected virtual void OnStderr(string line, StreamWriter stdin)
        => Log.Warn($"{_fileName}: {line}");
}
