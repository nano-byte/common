// Copyright Bastian Eicher
// Licensed under the MIT License

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace NanoByte.Common.Values.Design;

/// <summary>
/// Abstract base class for drop-down <see cref="PropertyGrid"/> editors that can be associated with <c>float</c> or <c>double</c> properties.
/// </summary>
public abstract class NumericEditor : UITypeEditor
{
    /// <inheritdoc/>
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;

    /// <inheritdoc/>
    public override object EditValue(ITypeDescriptorContext? context, IServiceProvider? provider, object? value)
    {
        #region Sanity checks
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (provider == null) throw new ArgumentNullException(nameof(provider));
        if (value == null) throw new ArgumentNullException(nameof(value));
        #endregion

        if (!TryGetDouble(value, out double doubleValue) || provider.GetService(typeof(IWindowsFormsEditorService)) is not IWindowsFormsEditorService editorService) return value;

        var range = context.PropertyDescriptor?.Attributes.OfType<RangeAttribute>().FirstOrDefault();
        double newValue = (range == null)
            ? EditValue(doubleValue, editorService)
            : EditValue(doubleValue, range, editorService);
        if (value is float)
            return (float)newValue;
        else
            return newValue;
    }

    /// <summary>
    /// Displays the UI to edit the <c>float</c> value.
    /// </summary>
    /// <param name="value">The current value.</param>
    /// <param name="editorService">The editor service used to display the dropdown control.</param>
    /// <returns>The value set by the user.</returns>
    protected abstract double EditValue(double value, IWindowsFormsEditorService editorService);

    /// <summary>
    /// Displays the UI to edit the <c>float</c> value.
    /// </summary>
    /// <param name="value">The current value.</param>
    /// <param name="range">The range of valid values the user can select.</param>
    /// <param name="editorService">The editor service used to display the dropdown control.</param>
    /// <returns>The value set by the user.</returns>
    protected abstract double EditValue(double value, RangeAttribute range, IWindowsFormsEditorService editorService);

    /// <summary>
    /// Interprets <paramref name="value"/> as a <c>double</c> if it is a <c>float</c> or <c>double</c>.
    /// </summary>
    protected bool TryGetDouble(object? value, out double doubleValue)
    {
        switch (value)
        {
            case double d:
                doubleValue = d;
                return true;
            case float f:
                doubleValue = f;
                return true;
            default:
                doubleValue = 0;
                return false;
        }
    }
}
