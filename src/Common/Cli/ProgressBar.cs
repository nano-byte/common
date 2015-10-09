using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NanoByte.Common.Native;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Cli
{
    /// <summary>
    /// A progress bar rendered on the <see cref="Console"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "IDisposable is used as a convenience wrapper around Done()")]
    public class ProgressBar : MarshalByRefObject, IDisposable
    {
        private int _maximum = 20;

        /// <summary>
        /// The maximum valid value for <see cref="Value"/>; must be greater than 0. Determines the length of the progress bar in console characters.
        /// </summary>
        [DefaultValue(20), Description("The maximum valid value for Value; must be greater than 0. Determines the length of the progress bar in console characters.")]
        public int Maximum
        {
            get { return _maximum; }
            set
            {
                #region Sanity checks
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", Resources.ArgMustBeGreaterThanZero);
                #endregion

                try
                {
                    value.To(ref _maximum, Draw);
                }
                catch (IOException)
                {}

                if (Value > Maximum) Value = Maximum;
            }
        }

        private int _value;

        /// <summary>
        /// The progress of the task as a value between 0 and <see cref="Maximum"/>; -1 when unknown.
        /// </summary>
        [DefaultValue(0), Description("The progress of the task as a value between 0 and Maximum; -1 when unknown.")]
        public int Value
        {
            get { return _value; }
            set
            {
                #region Sanity checks
                if (value < -1 || value > Maximum)
                    throw new ArgumentOutOfRangeException("value");
                #endregion

                try
                {
                    value.To(ref _value, Draw);
                }
                catch (IOException)
                {}
            }
        }


        private static readonly bool _drawPretty = WindowsUtils.IsWindows && !CliUtils.StandardErrorRedirected;

        /// <summary>
        /// Draws the progress-bar to <see cref="Console.Error"/>.
        /// </summary>
        /// <remarks>The current line is overwritten.</remarks>
        /// <exception cref="IOException">The progress bar could not be drawn to the <see cref="Console"/> (e.g. if it isn't a TTY).</exception>
        public void Draw()
        {
            if (_drawPretty) DrawPretty();
            else DrawSimple();
        }

        protected virtual void DrawPretty()
        {
            // Draw start of progress bar
            Console.CursorLeft = 0;
            Console.Error.Write('[');

            // Draw filled part
            Console.ForegroundColor = ConsoleColor.DarkGray;
            for (int i = 0; i < Value; i++)
                Console.Error.Write('=');
            Console.ResetColor();

            // Draw end of progress bar
            Console.CursorLeft = Maximum + 1;
            Console.Error.Write(']');
        }

        private int _lastValue;

        private void DrawSimple()
        {
            for (int i = _lastValue; i < Value; i++)
                Console.Error.Write('*');

            _lastValue = Value;
        }

        /// <summary>
        /// Stops the progress bar by writing a line break to the <see cref="Console"/>.
        /// </summary>
        public virtual void Done()
        {
            Console.Error.WriteLine();
        }

        /// <summary>
        /// Stops the progress bar by writing a line break to the <see cref="Console"/>.
        /// </summary>
        void IDisposable.Dispose()
        {
            Done();
        }
    }
}