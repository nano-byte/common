// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NanoByte.Common.Properties;
using NanoByte.Common.Tasks;

namespace NanoByte.Common.Cli
{
    /// <summary>
    /// A progress bar rendered on the <see cref="Console"/> that takes <see cref="TaskSnapshot"/> inputs.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "IDisposable is only implemented here to support using() blocks.")]
    internal class TaskProgressBar : ProgressBar, IProgress<TaskSnapshot>
    {
        private TaskState _state;

        /// <summary>
        /// The current State of the task.
        /// </summary>
        [Description("The current State of the task.")]
        public TaskState State
        {
            get { return _state; }
            set
            {
                #region Sanity checks
                if (!Enum.IsDefined(typeof(TaskState), value))
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TaskState));
                #endregion

                try
                {
                    value.To(ref _state, Draw);
                }
                catch (IOException)
                {}
            }
        }

        /// <inheritdoc/>
        public void Report(TaskSnapshot value)
        {
            State = value.State;
            Value = State == TaskState.Complete
                ? Maximum
                : Clamp(value.Value * Maximum, 0, Maximum);
        }

        private static int Clamp(double value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return (int)value;
        }

        protected override void DrawPretty()
        {
            base.DrawPretty();

            Console.Error.Write(' ');
            PrintStatus();

            // Blanks at the end to overwrite any excess
            Console.Error.Write(@"          ");
            Console.CursorLeft -= 10;
        }

        private void PrintStatus()
        {
            switch (State)
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
        }
    }
}
