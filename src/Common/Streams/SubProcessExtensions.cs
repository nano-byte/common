// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Streams;

/// <summary>
/// Extension methods for <see cref="ISubProcess"/>.
/// </summary>
public static class SubProcessExtensions
{
    /// <summary>
    /// Runs the external process, captures its output and waits until it has terminated.
    /// </summary>
    /// <param name="subProcess">The sub process.</param>
    /// <param name="arguments">Command-line arguments to launch the process with.</param>
    /// <returns>The process' complete stdout output.</returns>
    /// <exception cref="IOException">The external process could not be launched.</exception>
    /// <exception cref="InvalidOperationException">The external process returned a non-zero exit code.</exception>
    public static string Run(this ISubProcess subProcess, params string[] arguments)
        => subProcess.Run(null, arguments);

    /// <summary>
    /// Runs the external process, captures its output and waits until it has terminated.
    /// </summary>
    /// <param name="subProcess">The sub process.</param>
    /// <param name="stdinData">Data to the process' stdin right after startup.</param>
    /// <param name="arguments">Command-line arguments to launch the process with.</param>
    /// <returns>The process' complete stdout output.</returns>
    /// <exception cref="IOException">The external process could not be launched.</exception>
    /// <exception cref="InvalidOperationException">The external process returned a non-zero exit code.</exception>
    public static string Run(this ISubProcess subProcess, ArraySegment<byte> stdinData, params string[] arguments)
        => subProcess.Run(writer =>
        {
            writer.BaseStream.Write(stdinData.Array!, stdinData.Offset, stdinData.Count);
            writer.BaseStream.Flush();
        }, arguments);
}
