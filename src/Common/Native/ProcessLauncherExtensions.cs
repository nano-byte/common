// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Native;

/// <summary>
/// Extension methods for <see cref="IProcessLauncher"/>.
/// </summary>
public static class ProcessLauncherExtensions
{
    /// <summary>
    /// Runs the sub process, captures its stdout and stderr output and waits for it to exit.
    /// </summary>
    /// <param name="processLauncher">The sub process.</param>
    /// <param name="arguments">Command-line arguments to launch the process with.</param>
    /// <returns>The process' complete stdout output.</returns>
    /// <exception cref="IOException">The process could not be launched.</exception>
    /// <exception cref="ExitCodeException">The process exited with a non-zero exit code.</exception>
    public static string RunAndCapture(this IProcessLauncher processLauncher, params string[] arguments)
        => processLauncher.RunAndCapture(null, arguments);

    /// <summary>
    /// Runs the sub process, captures its stdout and stderr output and waits for it to exit.
    /// </summary>
    /// <param name="processLauncher">The sub process.</param>
    /// <param name="stdinData">Data to the process' stdin right after startup.</param>
    /// <param name="arguments">Command-line arguments to launch the process with.</param>
    /// <returns>The process' complete stdout output.</returns>
    /// <exception cref="IOException">The process could not be launched.</exception>
    /// <exception cref="ExitCodeException">The process exited with a non-zero exit code.</exception>
    public static string RunAndCapture(this IProcessLauncher processLauncher, ArraySegment<byte> stdinData, params string[] arguments)
        => processLauncher.RunAndCapture(writer =>
        {
            writer.BaseStream.Write(stdinData.Array!, stdinData.Offset, stdinData.Count);
            writer.BaseStream.Flush();
        }, arguments);
}
