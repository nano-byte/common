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
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace NanoByte.Common.Values.Design
{
    /// <summary>
    /// An editor that can be associated with <see langword="float"/> properties with values between 0 and 3 to provide a <see cref="TrackBar"/> interface.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class SliderEditor : FloatEditor
    {
        /// <inheritdoc/>
        protected override float EditValue(float value, IWindowsFormsEditorService editorService)
        {
            return EditValue(value, new FloatRangeAttribute(0, 10), editorService);
        }

        /// <inheritdoc/>
        protected override float EditValue(float value, FloatRangeAttribute range, IWindowsFormsEditorService editorService)
        {
            #region Sanity checks
            if (editorService == null) throw new ArgumentNullException("editorService");
            if (range == null) throw new ArgumentNullException("range");
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
