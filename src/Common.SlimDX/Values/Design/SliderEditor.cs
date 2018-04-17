// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace NanoByte.Common.Values.Design
{
    /// <summary>
    /// An editor that can be associated with <c>float</c> properties with values between 0 and 3 to provide a <see cref="TrackBar"/> interface.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class SliderEditor : FloatEditor
    {
        /// <inheritdoc/>
        protected override float EditValue(float value, IWindowsFormsEditorService editorService)
            => EditValue(value, new FloatRangeAttribute(0, 10), editorService);

        /// <inheritdoc/>
        protected override float EditValue(float value, FloatRangeAttribute range, IWindowsFormsEditorService editorService)
        {
            #region Sanity checks
            if (editorService == null) throw new ArgumentNullException(nameof(editorService));
            if (range == null) throw new ArgumentNullException(nameof(range));
            #endregion

            // Scale up by factor 40 and clamp within [minimum,maximum]
            var trackBar = new TrackBar
            {
                TickFrequency = 40,
                Minimum = (int)(range.Minimum * 40),
                Maximum = (int)(range.Maximum * 40),
            };
            trackBar.Value = ((int)(value * 40)).Clamp(trackBar.Minimum, trackBar.Maximum);

            editorService.DropDownControl(trackBar);
            return trackBar.Value / 40f;
        }
    }
}
