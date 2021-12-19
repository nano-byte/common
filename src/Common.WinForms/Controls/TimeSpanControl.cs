// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// Allows the input of a <see cref="TimeSpan"/> using <see cref="NumericUpDown"/> boxes.
    /// </summary>
    public partial class TimeSpanControl : UserControl
    {
        /// <summary>
        /// The time span currently represented by the control.
        /// </summary>
        public TimeSpan Value
        {
            get => new((int)upDownDays.Value, (int)upDownHours.Value, (int)upDownMinutes.Value, (int)upDownSeconds.Value);
            set
            {
                upDownDays.Value = value.Days;
                upDownHours.Value = value.Hours;
                upDownMinutes.Value = value.Minutes;
                upDownSeconds.Value = value.Seconds;
            }
        }

        public TimeSpanControl()
        {
            InitializeComponent();
        }
    }
}
