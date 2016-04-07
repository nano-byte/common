/*
 * Copyright 2006-2015 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

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
    public class TaskProgressBar : ProgressBar, Tasks.IProgress<TaskSnapshot>
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
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(TaskState));
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
            Value = (State == TaskState.Complete)
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
