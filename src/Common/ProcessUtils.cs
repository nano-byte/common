// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using NanoByte.Common.Native;
using NanoByte.Common.Storage;

namespace NanoByte.Common;

/// <summary>
/// Provides methods for launching child processes.
/// </summary>
public static class ProcessUtils
{
    /// <summary>
    /// Starts a new <see cref="Process"/> and runs it in parallel with this one. Handles and wraps <see cref="Win32Exception"/>s.
    /// </summary>
    /// <returns>The newly launched process.</returns>
    /// <exception cref="IOException">There was a problem launching the executable.</exception>
    /// <exception cref="FileNotFoundException">The executable file could not be found.</exception>
    /// <exception cref="NotAdminException">The target process requires elevation but the UAC prompt could not be displayed because <see cref="ProcessStartInfo.UseShellExecute"/> is <c>false</c>.</exception>
    /// <exception cref="OperationCanceledException">The user was asked for intervention by the OS (e.g. a UAC prompt) and the user cancelled.</exception>
    public static Process Start(this ProcessStartInfo startInfo)
    {
        #region Sanity checks
        if (startInfo == null) throw new ArgumentNullException(nameof(startInfo));
        #endregion

        Log.Debug($"Launching process: {startInfo.ToCommandLine()}");
        try
        {
            return Process.Start(startInfo) ?? throw new IOException(string.Format(Resources.FailedToStart, startInfo.FileName));
        }
        catch (Win32Exception ex)
        {
            throw ex.NativeErrorCode switch
            {
                WindowsUtils.Win32ErrorCancelled => new OperationCanceledException(),
                WindowsUtils.Win32ErrorRequestedOperationRequiresElevation => new NotAdminException(string.Format(Resources.LaunchNeedsAdmin, startInfo.FileName)),
                _ => new IOException(ex.Message, ex)
            };
        }
    }

    /// <summary>
    /// Starts a new <see cref="Process"/> and runs it in parallel with this one. Handles and wraps <see cref="Win32Exception"/>s.
    /// </summary>
    /// <param name="fileName">The path of the file to open or executable to launch.</param>
    /// <param name="arguments">The command-line arguments to pass to the executable.</param>
    /// <returns>The newly launched process; <c>null</c> if an existing process was reused.</returns>
    /// <exception cref="IOException">There was a problem launching the executable.</exception>
    /// <exception cref="FileNotFoundException">The executable file could not be found.</exception>
    /// <exception cref="NotAdminException">The target process requires elevation but the UAC prompt could not be displayed because <see cref="ProcessStartInfo.UseShellExecute"/> is <c>false</c>.</exception>
    /// <exception cref="OperationCanceledException">The user was asked for intervention by the OS (e.g. a UAC prompt) and the user cancelled.</exception>
    public static Process Start(string fileName, params string[] arguments)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));
        if (arguments == null) throw new ArgumentNullException(nameof(arguments));
        #endregion

        return new ProcessStartInfo(fileName, arguments.JoinEscapeArguments()).Start();
    }

    /// <summary>
    /// Starts a new <see cref="Process"/> and waits for it to complete. Handles and wraps <see cref="Win32Exception"/>s.
    /// </summary>
    /// <returns>The exit code of the process.</returns>
    /// <exception cref="IOException">There was a problem launching the executable.</exception>
    /// <exception cref="FileNotFoundException">The executable file could not be found.</exception>
    /// <exception cref="NotAdminException">The target process requires elevation but the UAC prompt could not be displayed because <see cref="ProcessStartInfo.UseShellExecute"/> is <c>false</c>.</exception>
    /// <exception cref="OperationCanceledException">The user was asked for intervention by the OS (e.g. a UAC prompt) and the user cancelled.</exception>
    public static int Run(this ProcessStartInfo startInfo)
        => Start(startInfo ?? throw new ArgumentNullException(nameof(startInfo))).WaitForExitCode();

    /// <summary>
    /// Waits for a running <see cref="Process"/> to complete.
    /// </summary>
    /// <returns>The exit code of the process.</returns>
    public static int WaitForExitCode(this Process process)
    {
        #region Sanity checks
        if (process == null) throw new ArgumentNullException(nameof(process));
        #endregion

        process.WaitForExit();
        Log.Debug($"Process finished with exit code {process.ExitCode}: {process.StartInfo.ToCommandLine()}");
        return process.ExitCode;
    }

    /// <summary>
    /// Waits for a running <see cref="Process"/> to complete with an exit code of zero.
    /// </summary>
    /// <exception cref="ExitCodeException">The process exited with a non-zero <see cref="Process.ExitCode"/>.</exception>
    public static void WaitForSuccess(this Process process)
    {
        int exitCode = process.WaitForExitCode();
        if (exitCode != 0)
            throw new ExitCodeException(process.StartInfo.ToCommandLine(), exitCode);
    }

    /// <summary>
    /// Creates a <see cref="ProcessStartInfo"/> for launching an assembly located in <see cref="Locations.InstallBase"/>.
    /// </summary>
    /// <param name="name">The name of the assembly to launch (without the file extension).</param>
    /// <param name="arguments">The command-line arguments to pass to the assembly.</param>
    [Pure]
    public static ProcessStartInfo Assembly(string name, params string[] arguments)
    {
        #region Sanity checks
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        if (arguments == null) throw new ArgumentNullException(nameof(arguments));
        #endregion

        string executablePath = Path.Combine(Locations.InstallBase,
#if NETFRAMEWORK
            name + ".exe"
#else
            name + ".dll"
#endif
        );
        if (!File.Exists(executablePath)) throw new FileNotFoundException(string.Format(Resources.UnableToLocateAssembly, name), executablePath);

#if NETFRAMEWORK
        if (!WindowsUtils.IsWindows)
        {
            arguments = arguments.Prepend(executablePath);
            executablePath = Process.GetCurrentProcess().MainModule?.FileName ?? "";
            if (!executablePath.EndsWith("mono")) executablePath = "mono";
        }
#else
        arguments = arguments.Prepend(executablePath);
        executablePath = Process.GetCurrentProcess().MainModule?.FileName ?? "";
        if (!executablePath.EndsWith(WindowsUtils.IsWindows ? "\\dotnet.exe" : "/dotnet")) executablePath = "dotnet";
#endif

        return new ProcessStartInfo(executablePath, arguments.JoinEscapeArguments()) {UseShellExecute = false};
    }

    /// <summary>
    /// Modifies a <see cref="ProcessStartInfo"/> to request elevation to Administrator on Windows using UAC.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">The current operating system does not support UAC or it is disabled..</exception>
    public static ProcessStartInfo AsAdmin(this ProcessStartInfo startInfo)
    {
        #region Sanity checks
        if (startInfo == null) throw new ArgumentNullException(nameof(startInfo));
        #endregion

        if (!WindowsUtils.HasUac) throw new PlatformNotSupportedException("The current operating system does not support UAC or it is disabled.");

        startInfo.Verb = "runas";
        startInfo.UseShellExecute = true;
        return startInfo;
    }

    /// <summary>
    /// Workaround for environment variable problems, such variable names that differ only in case when running on Windows.
    /// </summary>
    /// <remarks>Call this before any access to <see cref="ProcessStartInfo.EnvironmentVariables"/> to avoid <see cref="ArgumentException"/>s.</remarks>
    public static void SanitizeEnvironmentVariables()
    {
        if (!WindowsUtils.IsWindows) return;

        var dict = new StringDictionary();
        foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            try
            {
                dict.Add((string)entry.Key, (string?)entry.Value);
            }
            catch (ArgumentException ex)
            {
                Log.Warn("Ignoring environment variable '" + entry.Key + "' in this process due to problem: " + ex.Message);
                Environment.SetEnvironmentVariable((string)entry.Key, null);
            }
        }
    }

    /// <summary>
    /// Combines multiple strings into one for use as a command-line argument using <see cref="EscapeArgument"/>.
    /// </summary>
    /// <param name="parts">The strings to be combined.</param>
    [Pure]
    public static string JoinEscapeArguments(this IEnumerable<string> parts)
    {
        #region Sanity checks
        if (parts == null) throw new ArgumentNullException(nameof(parts));
        #endregion

        var output = new StringBuilder();
        bool first = true;
        foreach (string part in parts)
        {
            // No separator before first or after last part
            if (first) first = false;
            else output.Append(' ');

            output.Append(EscapeArgument(part));
        }

        return output.ToString();
    }

    /// <summary>
    /// Escapes a string for use as a command-line argument, making sure it is encapsulated within <c>"</c> if it contains whitespace characters.
    /// </summary>
    [Pure]
    public static string EscapeArgument(this string value)
    {
        #region Sanity checks
        if (value == null) throw new ArgumentNullException(nameof(value));
        #endregion

        if (value.Length == 0) return "\"\"";

        // Add leading quotation mark if there are whitespaces
        bool containsWhitespace = value.ContainsWhitespace();
        var result = containsWhitespace ? new StringBuilder("\"", value.Length + 2) : new StringBuilder(value.Length);

        // Split by quotation marks
        string[] parts = value.Split('"');
        for (int i = 0; i < parts.Length; i++)
        {
            // Count slashes preceding the quotation mark
            int slashesCount = parts[i].Length - parts[i].TrimEnd('\\').Length;

            result.Append(parts[i]);
            if (i < parts.Length - 1)
            { // Not last part
                for (int j = 0; j < slashesCount; j++) result.Append('\\'); // Double number of slashes
                result.Append("\\\""); // Escaped quotation mark
            }
            else if (containsWhitespace)
            { // Last part if there are whitespaces
                for (int j = 0; j < slashesCount; j++) result.Append('\\'); // Double number of slashes
                result.Append('"'); // Non-escaped quotation mark
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Converts a start into a command-line with proper escaping.
    /// </summary>
    [Pure]
    public static string ToCommandLine(this ProcessStartInfo startInfo)
    {
        #region Sanity checks
        if (startInfo == null) throw new ArgumentNullException(nameof(startInfo));
        #endregion

        return startInfo.FileName.EscapeArgument() + " " + startInfo.Arguments;
    }

    /// <summary>
    /// Converts a command-line into a start info.
    /// </summary>
    [Pure]
    public static ProcessStartInfo FromCommandLine(string commandLine)
    {
        #region Sanity checks
        if (commandLine == null) throw new ArgumentNullException(nameof(commandLine));
        #endregion

        string fileName;
        string arguments;
        if (WindowsUtils.IsWindows)
        {
            string[] parts = WindowsUtils.SplitArgs(commandLine);
            fileName = parts[0];
            arguments = parts.Skip(1).JoinEscapeArguments();
        }
        else
        {
            if (commandLine.StartsWith("\""))
            {
                string trimmedCommandLine = commandLine[1..];
                fileName = trimmedCommandLine.GetLeftPartAtFirstOccurrence("\" ");
                arguments = trimmedCommandLine.GetRightPartAtFirstOccurrence("\" ");
            }
            else
            {
                string[] parts = commandLine.Split(new[] {' '}, count: 2);
                fileName = parts[0];
                arguments = parts.Length == 2 ? parts[1] : "";
            }
        }
        return new(fileName, arguments) {UseShellExecute = false};
    }

#if !NET20 && !NET40
    /// <summary>
    /// Deconstructs a <see cref="ProcessStartInfo"/> like a tuple.
    /// </summary>
    [Pure]
    public static void Deconstruct(this ProcessStartInfo startInfo, out string fileName, out string arguments)
    {
        fileName = startInfo.FileName;
        arguments = startInfo.Arguments;
    }
#endif
}
