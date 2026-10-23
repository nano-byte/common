// Copyright Bastian Eicher
// Licensed under the MIT License

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NanoByte.Common.Net;

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Displays the public properties of an object and optionally allows editing them. A simplified alternative to the Windows Forms PropertyGrid.
/// </summary>
/// <remarks>
/// Respects <see cref="BrowsableAttribute"/>, <see cref="CategoryAttribute"/>, <see cref="DisplayNameAttribute"/>, <see cref="DescriptionAttribute"/>, <see cref="DefaultValueAttribute"/>, <see cref="PasswordPropertyTextAttribute"/> and <see cref="RangeAttribute"/>.
/// Uses check boxes for <see cref="bool"/>s, drop-downs for enums, numeric steppers for <see cref="int"/>s and text boxes with <see cref="TypeConverter"/>s for everything else.
/// </remarks>
public sealed class PropertyEditor : Scrollable
{
    /// <summary>
    /// Only display the property values, do not allow editing them.
    /// </summary>
    [DefaultValue(false), Description("Only display the property values, do not allow editing them.")]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Occurs when the user changed a property value.
    /// </summary>
    public event EventHandler? PropertyValueChanged;

    /// <summary>
    /// Creates a new property editor.
    /// </summary>
    public PropertyEditor()
    {
        Border = BorderType.None;
        ExpandContentWidth = true;
        ExpandContentHeight = false;

        // Small preferred size, otherwise expanding the content width may grow the containing window
        Size = new(100, 100);
    }

    private object? _selectedObject;

    /// <summary>
    /// The object whose properties are displayed; can be <c>null</c>.
    /// </summary>
    public object? SelectedObject
    {
        get => _selectedObject;
        set
        {
            _selectedObject = value;
            Build();
        }
    }

    /// <summary>Callbacks that transfer the current property values to the editor controls.</summary>
    private readonly List<Action> _refreshActions = [];

    /// <summary>Callbacks that transfer pending text input to the properties.</summary>
    private readonly List<Action> _commitActions = [];

    /// <summary>Suppresses change events while editor controls are updated programmatically.</summary>
    private bool _refreshing;

    private void Build()
    {
        _refreshActions.Clear();
        _commitActions.Clear();

        if (_selectedObject == null)
        {
            Content = null;
            return;
        }

        var table = new TableLayout {Spacing = new(8, 4), Padding = 4};
        string? currentCategory = null;
        foreach (var property in TypeDescriptor.GetProperties(_selectedObject).Cast<PropertyDescriptor>()
                                               .Where(x => x.IsBrowsable)
                                               .OrderBy(x => x.Category)
                                               .ThenBy(x => x.DisplayName))
        {
            if (property.Category != currentCategory && property.Category != CategoryAttribute.Default.Category)
                table.Rows.Add(new TableRow(new Label {Text = property.Category, Font = SystemFonts.Bold()}));
            currentCategory = property.Category;

            var label = new Label {Text = property.DisplayName, VerticalAlignment = VerticalAlignment.Center, ToolTip = property.Description};
            if (!ReadOnly && !property.IsReadOnly && property.Attributes[typeof(DefaultValueAttribute)] != null)
                label.ContextMenu = BuildResetMenu(property);

            var editor = BuildEditor(property);
            editor.ToolTip = property.Description;

            table.Rows.Add(new TableRow(label, new TableCell(editor, scaleWidth: true)));
        }
        table.Rows.Add(new TableRow {ScaleHeight = true});

        Content = table;
        RefreshValues();
    }

    private ContextMenu BuildResetMenu(PropertyDescriptor property)
    {
        var resetItem = new ButtonMenuItem {Text = Resources.ResetValue};
        resetItem.Click += delegate
        {
            property.ResetValue(_selectedObject!);
            RefreshValues();
            PropertyValueChanged?.Invoke(this, EventArgs.Empty);
        };
        return new ContextMenu(resetItem);
    }

    private Control BuildEditor(PropertyDescriptor property)
    {
        var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        if (ReadOnly || property.IsReadOnly)
        {
            var textBox = new TextBox {ReadOnly = true};
            _refreshActions.Add(() => textBox.Text = FormatValue(property));
            return textBox;
        }

        if (type == typeof(bool))
        {
            var checkBox = new CheckBox();
            _refreshActions.Add(() => checkBox.Checked = property.GetValue(_selectedObject!) is true);
            checkBox.CheckedChanged += delegate { SetValue(property, checkBox.Checked == true); };
            return checkBox;
        }

        if (type.IsEnum)
        {
            var dropDown = new DropDown {DataStore = Enum.GetValues(type).Cast<object>().ToList()};
            _refreshActions.Add(() => dropDown.SelectedValue = property.GetValue(_selectedObject!));
            dropDown.SelectedValueChanged += delegate
            {
                if (dropDown.SelectedValue != null) SetValue(property, dropDown.SelectedValue);
            };
            return dropDown;
        }

        if (type == typeof(int))
        {
            var range = property.Attributes[typeof(RangeAttribute)] as RangeAttribute;
            var stepper = new NumericStepper
            {
                DecimalPlaces = 0,
                MinValue = range?.Minimum is int min ? min : int.MinValue,
                MaxValue = range?.Maximum is int max ? max : int.MaxValue
            };
            _refreshActions.Add(() => stepper.Value = property.GetValue(_selectedObject!) as int? ?? 0);
            stepper.ValueChanged += delegate { SetValue(property, (int)stepper.Value); };
            return stepper;
        }

        if (property.Attributes[typeof(PasswordPropertyTextAttribute)] is PasswordPropertyTextAttribute {Password: true})
        {
            var passwordBox = new PasswordBox();
            _refreshActions.Add(() => passwordBox.Text = FormatValue(property));
            HookTextCommit(passwordBox, property, () => passwordBox.Text);
            return passwordBox;
        }
        else
        {
            var textBox = new TextBox();
            _refreshActions.Add(() => textBox.Text = FormatValue(property));
            HookTextCommit(textBox, property, () => textBox.Text);
            return textBox;
        }
    }

    /// <summary>
    /// Applies text input when the control loses focus or the user presses Enter.
    /// </summary>
    private void HookTextCommit(Control control, PropertyDescriptor property, Func<string> getText)
    {
        void Commit()
        {
            string text = getText();
            if (text == FormatValue(property)) return;

            if (TryParse(property, text, out object? value)) SetValue(property, value);
            else RefreshValues();
        }

        _commitActions.Add(Commit);
        control.LostFocus += delegate { Commit(); };
        control.KeyDown += (_, e) =>
        {
            if (e.Key == Keys.Enter) Commit();
        };
    }

    private string FormatValue(PropertyDescriptor property)
        => property.GetValue(_selectedObject!) switch
        {
            null => "",
            Uri uri => uri.ToStringRfc(),
            var value => property.Converter.ConvertToInvariantString(value) ?? ""
        };

    private bool TryParse(PropertyDescriptor property, string text, out object? value)
    {
        var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        try
        {
            if (string.IsNullOrEmpty(text) && (!type.IsValueType || type != property.PropertyType)) value = null;
            else if (type != typeof(string) && type.GetConstructor([typeof(string)]) != null) value = Activator.CreateInstance(type, text);
            else value = property.Converter.ConvertFromInvariantString(text);
            return true;
        }
        #region Error handling
        catch (Exception ex) when (ex is FormatException or ArgumentException or NotSupportedException or TargetInvocationException)
        {
            Msg.Inform(this, (ex.InnerException ?? ex).Message, MsgSeverity.Warn);
            value = null;
            return false;
        }
        #endregion
    }

    private void SetValue(PropertyDescriptor property, object? value)
    {
        if (_refreshing) return;

        try
        {
            property.SetValue(_selectedObject!, value);
        }
        #region Error handling
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or UnauthorizedAccessException or TargetInvocationException)
        {
            Msg.Inform(this, (ex.InnerException ?? ex).Message, MsgSeverity.Warn);
            RefreshValues();
            return;
        }
        #endregion

        PropertyValueChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Updates the displayed values after the <see cref="SelectedObject"/> was modified externally.
    /// </summary>
    public void RefreshValues()
    {
        _refreshing = true;
        try
        {
            foreach (var action in _refreshActions) action();
        }
        finally
        {
            _refreshing = false;
        }
    }

    /// <summary>
    /// Applies any text input that has not been committed yet (e.g., because the control still has focus).
    /// </summary>
    /// <remarks>Call this before reading the <see cref="SelectedObject"/>, e.g., when the user clicks an OK button.</remarks>
    public void CommitPendingChanges()
    {
        foreach (var action in _commitActions) action();
    }
}
