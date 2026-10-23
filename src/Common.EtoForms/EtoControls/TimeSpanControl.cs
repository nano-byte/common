// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Allows the user to input a <see cref="TimeSpan"/> as days, hours, minutes and seconds.
/// </summary>
public sealed class TimeSpanControl : Panel
{
    private readonly NumericStepper _days = new() {MinValue = 0, MaxValue = 9999, DecimalPlaces = 0, Width = 70};
    private readonly NumericStepper _hours = new() {MinValue = 0, MaxValue = 23, DecimalPlaces = 0, Width = 50};
    private readonly NumericStepper _minutes = new() {MinValue = 0, MaxValue = 59, DecimalPlaces = 0, Width = 50};
    private readonly NumericStepper _seconds = new() {MinValue = 0, MaxValue = 59, DecimalPlaces = 0, Width = 50};

    /// <summary>
    /// Occurs when <see cref="Value"/> was changed by the user.
    /// </summary>
    public event EventHandler? ValueChanged;

    /// <summary>Suppresses <see cref="ValueChanged"/> while <see cref="Value"/> is set programmatically.</summary>
    private bool _suppressEvents;

    /// <summary>
    /// Creates a new time span control.
    /// </summary>
    public TimeSpanControl()
    {
        Content = new StackLayout
        {
            Orientation = Orientation.Horizontal,
            VerticalContentAlignment = VerticalAlignment.Center,
            Spacing = 4,
            Items = {_days, new Label {Text = Resources.Days}, _hours, new Label {Text = ":"}, _minutes, new Label {Text = ":"}, _seconds}
        };

        foreach (var stepper in new[] {_days, _hours, _minutes, _seconds})
        {
            stepper.ValueChanged += delegate
            {
                if (!_suppressEvents) ValueChanged?.Invoke(this, EventArgs.Empty);
            };
        }
    }

    /// <summary>
    /// The time span currently entered.
    /// </summary>
    /// <remarks>Fractions of seconds are discarded.</remarks>
    public TimeSpan Value
    {
        get => new((int)_days.Value, (int)_hours.Value, (int)_minutes.Value, (int)_seconds.Value);
        set
        {
            _suppressEvents = true;
            try
            {
                _days.Value = value.Days;
                _hours.Value = value.Hours;
                _minutes.Value = value.Minutes;
                _seconds.Value = value.Seconds;
            }
            finally
            {
                _suppressEvents = false;
            }
        }
    }
}
