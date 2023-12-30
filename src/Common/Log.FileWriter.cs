// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Diagnostics;
using System.Globalization;
using System.Text;
using NanoByte.Common.Info;
using NanoByte.Common.Storage;

namespace NanoByte.Common;

partial class Log
{
    private static StreamWriter? _fileWriter;
    private static readonly int _processId;

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "The static constructor is used to add an identification header to the log file")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any kind of problems writing the log file should be ignored")]
    static Log()
    {
        try
        {
            _processId = Process.GetCurrentProcess().Id;

            const int maxSize = 1024 * 1024; // 1MiB
            var file = GetLogFile(AppInfo.Current.Name ?? Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            var encoding = new UTF8Encoding(
                encoderShouldEmitUTF8Identifier: !file.Exists || file.Length > maxSize);
            _fileWriter = new(
                file.Open(
                    file.Exists && file.Length > maxSize
                        ? FileMode.Truncate
                        : FileMode.Append,
                    FileAccess.Write,
                    FileShare.ReadWrite), // Allow concurrent writes to same file by other processes
                encoding);
        }
        #region Error handling
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error writing to log file:");
            Console.Error.WriteLine(ex);
            return;
        }
        #endregion

        AppDomain.CurrentDomain.ProcessExit += delegate { CloseFile(); };

        WriteToFile(string.Join(Environment.NewLine, [
            "",
            $"/// {AppInfo.Current.NameVersion}",
            $"/// Install base: {Locations.InstallBase}",
            $"/// Command-line args: {Environment.GetCommandLineArgs().JoinEscapeArguments()}",
            $"/// Process {_processId} started at: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}",
            ""
        ]));
    }

    private static FileInfo GetLogFile(string appName)
        => new(Path.Combine(Path.GetTempPath(), $"{appName} {Environment.UserName} Log.txt"));

    /// <summary>
    /// Appends a line to the log file.
    /// </summary>
    private static void WriteToFile(string logLine)
    {
        if (_fileWriter is not {} writer) return;

        try
        {
            // Catch up in case other processes have been writing to the same file
            writer.BaseStream.Seek(0, SeekOrigin.End);

            writer.WriteLine(logLine);
            writer.Flush();
        }
        #region Error handling
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error writing to log file:");
            Console.Error.WriteLine(ex);
            CloseFile();
        }
        #endregion
    }

    private static void CloseFile()
    {
        try
        {
            _fileWriter?.Dispose();
        }
        #region Error handling
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error closing log file:");
            Console.Error.WriteLine(ex);
        }
        #endregion

        _fileWriter = null;
    }
}
