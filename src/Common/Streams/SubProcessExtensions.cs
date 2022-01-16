// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Streams;

/// <summary>
/// Extension methods for <see cref="ISubProcess"/>.
/// </summary>
public static class SubProcessExtensions
{
    /// <summary>
    /// Runs the sub process, captures its stdout and stderr output and waits for it to exit.
    /// </summary>
    /// <param name="subProcess">The sub process.</param>
    /// <param name="arguments">Command-line arguments to launch the process with.</param>
    /// <returns>The process' complete stdout output.</returns>
    /// <exception cref="IOException">The process could not be launched.</exception>
    /// <exception cref="ExitCodeException">The process exited with a non-zero exit code.</exception>
    public static string RunAndCapture(this ISubProcess subProcess, params string[] arguments)
        => subProcess.RunAndCapture(null, arguments);

    /// <summary>
    /// Runs the sub process, captures its stdout and stderr output and waits for it to exit.
    /// </summary>
    /// <param name="subProcess">The sub process.</param>
    /// <param name="stdinData">Data to the process' stdin right after startup.</param>
    /// <param name="arguments">Command-line arguments to launch the process with.</param>
    /// <returns>The process' complete stdout output.</returns>
    /// <exception cref="IOException">The process could not be launched.</exception>
    /// <exception cref="ExitCodeException">The process exited with a non-zero exit code.</exception>
    public static string RunAndCapture(this ISubProcess subProcess, ArraySegment<byte> stdinData, params string[] arguments)
        => subProcess.RunAndCapture(writer =>
        {
            writer.BaseStream.Write(stdinData.Array!, stdinData.Offset, stdinData.Count);
            writer.BaseStream.Flush();
        }, arguments);
}
