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
using System.Drawing;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms.Design;
using NanoByte.Common.Controls;

namespace NanoByte.Common.Values.Design
{
    /// <summary>
    /// An editor that can be associated with <c>float</c> properties representing angles between 0 and 360 degrees. Uses <see cref="AngleControl"/>.
    /// </summary>
    /// <seealso cref="FloatRangeAttribute"/>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class AngleEditor : FloatEditor
    {
        /// <inheritdoc/>
        protected override float EditValue(float value, IWindowsFormsEditorService editorService)
        {
            #region Sanity checks
            if (editorService == null) throw new ArgumentNullException("editorService");
            #endregion

            var angleControl = new AngleControl {Angle = value};
            editorService.DropDownControl(angleControl);
            return angleControl.Angle;
        }

        /// <inheritdoc/>
        protected override float EditValue(float value, FloatRangeAttribute range, IWindowsFormsEditorService editorService)
        {
            #region Sanity checks
            if (editorService == null) throw new ArgumentNullException("editorService");
            if (range == null) throw new ArgumentNullException("range");
            #endregion

            var angleControl = new AngleControl {Angle = value, Range = range};
            editorService.DropDownControl(angleControl);
            return angleControl.Angle;
        }

        /// <inheritdoc/>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <inheritdoc/>
        public override void PaintValue(PaintValueEventArgs e)
        {
            #region Sanity checks
            if (e == null) throw new ArgumentNullException("e");
            #endregion

            // Draw background
            e.Graphics.FillRectangle(new SolidBrush(Color.DarkBlue), e.Bounds);

            // Draw circle
            e.Graphics.FillEllipse(new SolidBrush(Color.White), e.Bounds.X + 3, e.Bounds.Y + 1, e.Bounds.Width - 6, e.Bounds.Height - 2);

            if (!(e.Value is float)) return;

            // Draw angle line
            var center = new Point(e.Bounds.Width / 2 + e.Bounds.X, e.Bounds.Height / 2 + e.Bounds.Y);
            float angle = ((float)e.Value).DegreeToRadian();
            var endPoint = new Point(
                center.X + (int)(center.X * Math.Sin(angle)),
                center.Y + (int)(center.Y * -Math.Cos(angle)));
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Red), 1), center, endPoint);
        }
    }
}
