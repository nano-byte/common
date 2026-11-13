// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections;
using System.Diagnostics;
using System.Text;
using Windows.Win32.Foundation;
using NanoByte.Common.Native;
using NanoByte.Common.Storage;

namespace NanoByte.Common;

/// <summary>
/// Provides methods for launching child processes.
/// </summary>
public static class ProcessUtils
{
    extension(ProcessStartInfo startInfo)
    {
        /// <summary>
        /// Starts a new <see cref="Process"/> and runs it in parallel with this one. Handles and wraps <see cref="Win32Exception"/>s.
        /// </summary>
        /// <returns>The newly launched process.</returns>
        /// <exception cref="IOException">There was a problem launching the executable.</exception>
        /// <exception cref="FileNotFoundException">The executable file could not be found.</exception>
        /// <exception cref="NotAdminException">The target process requires elevation but the UAC prompt could not be displayed because <see cref="ProcessStartInfo.UseShellExecute"/> is <c>false</c>.</exception>
        /// <exception cref="OperationCanceledException">The user was asked for intervention by the OS (e.g. a UAC prompt) and the user cancelled.</exception>
        public Process Start()
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
                    (int)WIN32_ERROR.ERROR_CANCELLED => new OperationCanceledException(),
                    (int)WIN32_ERROR.ERROR_ELEVATION_REQUIRED => new NotAdminException(string.Format(Resources.LaunchNeedsAdmin, startInfo.FileName)),
                    _ => new IOException(ex.Message, ex)
                };
            }
        }

        /// <summary>
        /// Starts a new <see cref="Process"/> and waits for it to complete. Handles and wraps <see cref="Win32Exception"/>s.
        /// </summary>
        /// <returns>The exit code of the process.</returns>
        /// <exception cref="IOException">There was a problem launching the executable.</exception>
        /// <exception cref="FileNotFoundException">The executable file could not be found.</exception>
        /// <exception cref="NotAdminException">The target process requires elevation but the UAC prompt could not be displayed because <see cref="ProcessStartInfo.UseShellExecute"/> is <c>false</c>.</exception>
        /// <exception cref="OperationCanceledException">The user was asked for intervention by the OS (e.g. a UAC prompt) and the user cancelled.</exception>
        public int Run()
            => Start(startInfo ?? throw new ArgumentNullException(nameof(startInfo))).WaitForExitCode();

        /// <summary>
        /// Modifies a <see cref="ProcessStartInfo"/> to request elevation to Administrator on Windows using UAC.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException">The current operating system does not support UAC or it is disabled.</exception>
        public ProcessStartInfo AsAdmin()
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
        /// Converts a start info into a command-line with proper escaping for the current platform, which <see cref="FromCommandLine"/> can convert back.
        /// </summary>
        [Pure]
        public string ToCommandLine()
        {
            #region Sanity checks
            if (startInfo == null) throw new ArgumentNullException(nameof(startInfo));
            #endregion

            return WindowsUtils.IsWindows
                ? $"{startInfo.FileName.EscapeArgument()} {startInfo.Arguments}"
                : ToCommandLinePosix(startInfo);
        }

        /// <summary>
        /// Deconstructs a <see cref="ProcessStartInfo"/> like a tuple.
        /// </summary>
        [Pure]
        public void Deconstruct(out string fileName, out string arguments)
        {
            fileName = startInfo.FileName;
            arguments = startInfo.Arguments;
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
    public static Process Start(string fileName, IEnumerable<string> arguments)
        => new ProcessStartInfo(fileName, arguments.JoinEscapeArguments()).Start();

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
        => Start(fileName, arguments.AsEnumerable());

    extension(Process process)
    {
        /// <summary>
        /// Waits for a running <see cref="Process"/> to complete.
        /// </summary>
        /// <returns>The exit code of the process.</returns>
        public int WaitForExitCode()
        {
            #region Sanity checks
            if (process == null) throw new ArgumentNullException(nameof(process));
            #endregion

            process.WaitForExit();
            Log.Debug($"Process {process.Id} finished with exit code {process.ExitCode}: {process.StartInfo.ToCommandLine()}");
            return process.ExitCode;
        }

        /// <summary>
        /// Waits for a running <see cref="Process"/> to complete with an exit code of zero.
        /// </summary>
        /// <exception cref="ExitCodeException">The process exited with a non-zero <see cref="Process.ExitCode"/>.</exception>
        public void WaitForSuccess()
        {
            if (process.WaitForExitCode() != 0)
                throw new ExitCodeException(process);
        }
    }

    /// <summary>
    /// Creates a <see cref="ProcessStartInfo"/> for launching an assembly located in <see cref="Locations.InstallBase"/>.
    /// </summary>
    /// <param name="name">The name of the assembly to launch (without the file extension).</param>
    /// <param name="arguments">The command-line arguments to pass to the assembly.</param>
    /// <exception cref="FileNotFoundException">The specified assembly or the <c>dotnet</c> host needed to launch it could not be found.</exception>
    [Pure]
    public static ProcessStartInfo Assembly(string name, IEnumerable<string> arguments)
    {
        #region Sanity checks
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (arguments == null) throw new ArgumentNullException(nameof(arguments));
        #endregion

#if NET
        string basePath = Paths.Combine(Locations.InstallBase, name);
        string dllPath = $"{basePath}.dll";
        string nativePath = WindowsUtils.IsWindows ? $"{basePath}.exe" : basePath;

        bool nativeExists = File.Exists(nativePath);
        bool nativeRunnable = WindowsUtils.IsWindows ? nativeExists : FileUtils.IsExecutable(nativePath);

        string? processPath = Environment.ProcessPath;
        string? hostPath = string.Equals(Path.GetFileName(processPath), DotNetHostFileName, WindowsUtils.IsWindows ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)
            ? processPath
            : null;

        string executablePath;
        if (File.Exists(dllPath) && (hostPath != null || !nativeRunnable))
        {
            executablePath = hostPath ?? FindDotNetHost();
            arguments = [dllPath, ..arguments];
        }
        else if (nativeExists) executablePath = nativePath;
        else throw new FileNotFoundException(string.Format(Resources.UnableToLocateAssembly, name), basePath);
#else
        string executablePath = Paths.Combine(Locations.InstallBase, $"{name}.exe");
        if (!File.Exists(executablePath)) throw new FileNotFoundException(string.Format(Resources.UnableToLocateAssembly, name), executablePath);

        if (!WindowsUtils.IsWindows)
        {
            arguments = [executablePath, ..arguments];
            executablePath = Process.GetCurrentProcess().MainModule?.FileName ?? "";
            if (!executablePath.EndsWith("mono")) executablePath = "mono";
        }
#endif

        return new ProcessStartInfo(executablePath, arguments.JoinEscapeArguments()) {UseShellExecute = false};
    }

#if NET

    private static string DotNetHostFileName => WindowsUtils.IsWindows ? "dotnet.exe" : "dotnet";

    private static string FindDotNetHost()
    {
        if (Environment.GetEnvironmentVariable("DOTNET_ROOT") is {Length: > 0} root)
        {
            string path = Paths.Combine(root, DotNetHostFileName);
            if (File.Exists(path)) return path;
        }

        return Locations.GetInstalledFilePath(DotNetHostFileName);
    }
#endif


    /// <summary>
    /// Creates a <see cref="ProcessStartInfo"/> for launching an assembly located in <see cref="Locations.InstallBase"/>.
    /// </summary>
    /// <param name="name">The name of the assembly to launch (without the file extension).</param>
    /// <param name="arguments">The command-line arguments to pass to the assembly.</param>
    /// <exception cref="FileNotFoundException">The specified assembly or the <c>dotnet</c> host needed to launch it could not be found.</exception>
    [Pure]
    public static ProcessStartInfo Assembly(string name, params string[] arguments)
        => Assembly(name, arguments.AsEnumerable());

    /// <summary>
    /// Workaround for environment variable problems, like variable names that differ only in casing when running on Windows.
    /// </summary>
    /// <remarks>Call this before any access to <see cref="ProcessStartInfo.EnvironmentVariables"/> to avoid <see cref="ArgumentException"/>s.</remarks>
    public static void SanitizeEnvironmentVariables()
    {
        if (!WindowsUtils.IsWindows) return;

        var groups = new Dictionary<string, List<DictionaryEntry>>(StringComparer.OrdinalIgnoreCase);
        foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            string key = (string)entry.Key;
            if (groups.TryGetValue(key, out var list))
                list.Add(entry);
            else
                groups[key] = [entry];
        }

        foreach (var (_, entries) in groups)
        {
            if (entries.Count <= 1)
                continue;

            var canonical = entries.OrderBy(x => (string)x.Key, StringComparer.Ordinal).First();

            foreach (var entry in entries)
                Environment.SetEnvironmentVariable((string)entry.Key, null);
            Environment.SetEnvironmentVariable((string)canonical.Key, canonical.Value as string);
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

        return StringUtils.Join(" ", parts.Select(EscapeArgument));
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
        StringBuilder result = containsWhitespace ? new("\"", value.Length + 2) : new(value.Length);

        // Split by quotation marks
        string[] parts = value.Split('"');
        for (int i = 0; i < parts.Length; i++)
        {
            // Count backslashes preceding the quotation mark
            int backslashCount = parts[i].Length - parts[i].TrimEnd('\\').Length;

            result.Append(parts[i]);
            if (i < parts.Length - 1)
            { // Not last part
                for (int j = 0; j < backslashCount; j++) result.Append('\\'); // Double number of backslashes
                result.Append("\\\""); // Escaped quotation mark
            }
            else if (containsWhitespace)
            { // Last part if there are whitespaces
                for (int j = 0; j < backslashCount; j++) result.Append('\\'); // Double number of backslashes
                result.Append('"'); // Non-escaped quotation mark
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Splits a string of arguments, without a leading program name, into individual arguments using the rules of the current platform.
    /// </summary>
    /// <param name="arguments">The arguments to be split.</param>
    /// <returns>An array of individual arguments.</returns>
    /// <remarks>
    /// On Windows this follows the rules of <c>CommandLineToArgvW</c>.
    /// On other platforms this follows POSIX shell quoting rules, without performing any expansions, so <c>\</c> and <c>'</c> have a special meaning there.
    /// </remarks>
    [Pure]
    public static string[] SplitArguments(string? arguments)
        => WindowsUtils.IsWindows
            ? WindowsUtils.SplitArgs($"_ {arguments}").Skip(1).ToArray() // Placeholder, because the first token follows different rules as a program name
            : SplitArgumentsPosix(arguments);

    /// <summary>
    /// Splits a command-line into individual arguments using POSIX shell quoting rules. Does not perform any expansions. An unclosed quotation mark ends with the string, like on Windows.
    /// </summary>
    [Pure]
    internal static string[] SplitArgumentsPosix(string? commandLine)
    {
        if (string.IsNullOrEmpty(commandLine)) return [];

        var args = new List<string>();
        var current = new StringBuilder();
        bool inArg = false;
        char? quote = null;

        for (int i = 0; i < commandLine.Length; i++)
        {
            char c = commandLine[i];
            if (quote == '\'')
            {
                if (c == '\'') quote = null;
                else current.Append(c);
            }
            else if (c == '\\' && i + 1 < commandLine.Length)
            {
                char next = commandLine[++i];
                if (next == '\n') continue; // Line continuation
                if (quote == '"' && next is not ('"' or '\\' or '$' or '`')) current.Append(c);
                current.Append(next);
                inArg = true;
            }
            else if (c == '"' && quote == '"') quote = null;
            else if (c is '"' or '\'' && quote == null)
            {
                quote = c;
                inArg = true;
            }
            else if (c is ' ' or '\t' or '\n' && quote == null)
            {
                if (inArg) args.Add(current.ToString());
                current.Clear();
                inArg = false;
            }
            else
            {
                current.Append(c);
                inArg = true;
            }
        }

        if (inArg) args.Add(current.ToString());
        return args.ToArray();
    }

    /// <summary>
    /// Splits <see cref="ProcessStartInfo.Arguments"/> into individual arguments the same way .NET does when starting a process on Unix-like systems.
    /// </summary>
    [Pure]
    internal static string[] SplitStartInfoArguments(string arguments)
    {
        var args = new List<string>();
        var current = new StringBuilder();
        bool inArg = false, inQuotes = false;

        for (int i = 0; i < arguments.Length; i++)
        {
            char c = arguments[i];
            if (c == '\\')
            {
                int backslashCount = 1;
                while (i + 1 < arguments.Length && arguments[i + 1] == '\\')
                {
                    backslashCount++;
                    i++;
                }

                bool beforeQuote = i + 1 < arguments.Length && arguments[i + 1] == '"';
                current.Append('\\', beforeQuote ? backslashCount / 2 : backslashCount);
                if (beforeQuote && backslashCount % 2 != 0)
                {
                    current.Append('"');
                    i++;
                }
                inArg = true;
            }
            else if (c == '"')
            {
                if (inQuotes && i + 1 < arguments.Length && arguments[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else inQuotes = !inQuotes;
                inArg = true;
            }
            else if (c is ' ' or '\t' && !inQuotes)
            {
                if (inArg) args.Add(current.ToString());
                current.Clear();
                inArg = false;
            }
            else
            {
                current.Append(c);
                inArg = true;
            }
        }

        if (inArg) args.Add(current.ToString());
        return args.ToArray();
    }

    /// <summary>
    /// Combines multiple strings into one for use as a command-line in a POSIX shell using <see cref="EscapeArgumentPosix"/>.
    /// </summary>
    /// <param name="parts">The strings to be combined. The first one is the command to run.</param>
    [Pure]
    public static string JoinEscapeArgumentsPosix(this IEnumerable<string> parts)
        => StringUtils.Join(" ", parts.Select((part, index) => index == 0 && part.Contains('=') ? QuotePosix(part) : part.EscapeArgumentPosix()));

    /// <summary>
    /// Escapes a string for use as an argument on a POSIX shell command-line, making sure it is encapsulated within <c>"</c> unless it consists solely of characters without a special meaning.
    /// </summary>
    /// <remarks>The result is a single shell word, which can be concatenated with other words without whitespace in between. Not suitable for the first word of a command-line, see <see cref="JoinEscapeArgumentsPosix"/>.</remarks>
    [Pure]
    public static string EscapeArgumentPosix(this string value)
        => value.Length != 0 && value.All(IsShellSafe)
            ? value
            : QuotePosix(value);

    /// <summary>
    /// Encapsulates a string within <c>"</c> for use on a POSIX shell command-line.
    /// </summary>
    [Pure]
    private static string QuotePosix(string value)
    {
        var result = new StringBuilder("\"", value.Length + 2);
        foreach (char c in value)
        {
            if (c is '"' or '\\' or '$' or '`') result.Append('\\');
            result.Append(c);
        }
        return result.Append('"').ToString();
    }

    /// <summary>
    /// Determines whether a character has no special meaning for a POSIX shell, regardless of its position within a word.
    /// </summary>
    [Pure]
    private static bool IsShellSafe(char c)
        => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_' or '@' or '%' or '+' or '=' or ':' or ',' or '.' or '/' or '-'
        || (c > '\x7F' && !char.IsWhiteSpace(c));

    /// <summary>
    /// Converts a start info into a command-line using POSIX shell quoting rules.
    /// </summary>
    [Pure]
    internal static string ToCommandLinePosix(ProcessStartInfo startInfo)
        => SplitStartInfoArguments(startInfo.Arguments).Prepend(startInfo.FileName).JoinEscapeArgumentsPosix();

    /// <summary>
    /// Converts a command-line into a start info using the rules of the current platform.
    /// </summary>
    [Pure]
    public static ProcessStartInfo FromCommandLine(string commandLine)
    {
        #region Sanity checks
        if (commandLine == null) throw new ArgumentNullException(nameof(commandLine));
        #endregion

        return FromCommandLineParts(WindowsUtils.IsWindows
            ? WindowsUtils.SplitArgs(commandLine)
            : SplitArgumentsPosix(commandLine));
    }

    /// <summary>
    /// Converts a command-line that has been split into individual parts into a start info.
    /// </summary>
    [Pure]
    internal static ProcessStartInfo FromCommandLineParts(string[] parts)
        => new(parts.FirstOrDefault() ?? "", parts.Skip(1).JoinEscapeArguments()) {UseShellExecute = false};
}
