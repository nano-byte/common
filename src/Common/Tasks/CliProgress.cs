// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using NanoByte.Common.Native;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Tasks
{
    /// <summary>
    /// Reports task progress updates using console output.
    /// </summary>
    /// <seealso cref="CliTaskHandler.RunTask"/>
    public class CliProgress : MarshalByRefObject, IProgress<TaskSnapshot>
    {
        private const int Width = 20;

        private TaskState _currentState;
        private int _currentValue;

        /// <inheritdoc/>
        public void Report(TaskSnapshot value)
        {
            bool needsDraw = false;

            value.State.To(ref _currentState, () => needsDraw = true);

            int currentValue = value.State == TaskState.Complete
                ? Width
                : Clamp(value.Value * Width, 0, Width);
            currentValue.To(ref _currentValue, () => needsDraw = true);

            if (needsDraw) Draw(value);
        }

        private static int Clamp(double value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return (int)value;
        }

        private void Draw(TaskSnapshot value)
        {
            try
            {
                if (WindowsUtils.IsWindows) DrawPretty();
                else DrawSimple();
            }
            catch (IOException)
            {}

            if (value.State >= TaskState.Complete)
                Console.Error.WriteLine();
        }

        private void DrawPretty()
        {
            // Draw start of progress bar
            Console.CursorLeft = 0;
            Console.Error.Write('[');

            // Draw filled part
            Console.ForegroundColor = ConsoleColor.DarkGray;
            for (int i = 0; i < _currentValue; i++)
                Console.Error.Write('=');
            Console.ResetColor();

            // Draw end of progress bar
            Console.CursorLeft = Width + 1;
            Console.Error.Write("] ");

            // Write status
            switch (_currentState)
            {
                case TaskState.Header:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Error.Write(Resources.StateHeader);
                    break;

                case TaskState.Data:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Error.Write(Resources.StateData);
                    break;

                case TaskState.Complete:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Error.Write(Resources.StateComplete);
                    break;

                case TaskState.WebError:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.Write(Resources.StateWebError);
                    break;

                case TaskState.IOError:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.Write(Resources.StateIOError);
                    break;
            }
            Console.ResetColor();

            // Blanks at the end to overwrite any excess
            Console.Error.Write(@"          ");
            Console.CursorLeft -= 10;
        }

        private int _previousValue;

        private void DrawSimple()
        {
            for (int i = _previousValue; i < _currentValue; i++)
                Console.Error.Write('*');

            _previousValue = _currentValue;
        }
    }
}
