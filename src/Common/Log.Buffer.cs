// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common;

partial class Log
{
    private static readonly string[] _buffer = new string[64];
    private static int _bufferIndex;
    private static bool _bufferRollover;

    /// <summary>
    /// Returns the log lines collected in the in-memory buffer.
    /// </summary>
    public static string GetBuffer()
    {
        static string Join(int startIndex, int count)
            => string.Join(Environment.NewLine, _buffer, startIndex, count);

        lock (_lock)
        {
            return _bufferRollover
                ? Join(_bufferIndex, _buffer.Length - _bufferIndex) + Join(0, _bufferIndex)
                : Join(0, _bufferIndex);
        }
    }

    /// <summary>
    /// Adds a line to the in-memory log buffer.
    /// Should only be called when <see cref="_lock"/> is held.
    /// </summary>
    private static void AddToBuffer(string logLine)
    {
        _buffer[_bufferIndex] = logLine;
        if (++_bufferIndex >= _buffer.Length)
        {
            _bufferRollover = true;
            _bufferIndex = 0;
        }
    }

    /// <summary>
    /// Resets the in-memory log buffer.
    /// </summary>
    private static void ResetBuffer()
    {
        lock (_buffer)
        {
            _bufferIndex = 0;
            _bufferRollover = false;
        }
    }
}
