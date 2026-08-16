// Copyright Bastian Eicher
// Licensed under the MIT License

using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace NanoByte.Common.Values.Design;

/// <summary>
/// An editor that can be associated with <c>float</c> or <c>double</c> properties to provide a <see cref="TrackBar"/> interface.
/// </summary>
public class SliderEditor : NumericEditor
{
    /// <inheritdoc/>
    protected override double EditValue(double value, IWindowsFormsEditorService editorService)
        => EditValue(value, new(0, 10), editorService);

    /// <inheritdoc/>
    protected override double EditValue(double value, RangeAttribute range, IWindowsFormsEditorService editorService)
    {
        #region Sanity checks
        if (editorService == null) throw new ArgumentNullException(nameof(editorService));
        if (range == null) throw new ArgumentNullException(nameof(range));
        #endregion

        // Scale up by factor 40 and clamp within [minimum,maximum]
        var trackBar = new TrackBar
        {
            TickFrequency = 40,
            Minimum = (int)(Convert.ToDouble(range.Minimum) * 40),
            Maximum = (int)(Convert.ToDouble(range.Maximum) * 40),
        };
        trackBar.Value = ((int)(value * 40)).Clamp(trackBar.Minimum, trackBar.Maximum);

        editorService.DropDownControl(trackBar);
        return trackBar.Value / 40f;
    }
}
