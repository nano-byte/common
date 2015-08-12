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
using System.Drawing;
using System.Windows.Forms;
using NanoByte.Common.Values;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// Allows the input of angles between 0 and 360 degrees by clicking in a circle.
    /// </summary>
    public class AngleControl : UserControl
    {
        #region Properties
        /// <summary>
        /// The angle between 0 and 360 degrees.
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// An optional limit to the valid degree values. (Limits beyond 0° and 360° are ignored.)
        /// </summary>
        public FloatRangeAttribute Range { get; set; }
        #endregion

        #region Constructor
        public AngleControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
        #endregion

        //--------------------//

        #region Event hooks
        protected override void OnPaint(PaintEventArgs e)
        {
            #region Sanity checks
            if (e == null) throw new ArgumentNullException(nameof(e));
            #endregion

            // Draw background
            e.Graphics.FillRectangle(new SolidBrush(Color.DarkBlue), e.ClipRectangle);

            // Draw circle
            e.Graphics.FillEllipse(new SolidBrush(Color.White), 1, 1, Width - 3, Height - 3);

            // Draw angle markers
            var markerFont = new Font("Arial", 8);
            var markerBrush = new SolidBrush(Color.DarkGray);
            e.Graphics.DrawString("0", markerFont, markerBrush, (Width / 2) - 10, 10);
            e.Graphics.DrawString("90", markerFont, markerBrush, Width - 18, (Height / 2) - 6);
            e.Graphics.DrawString("180", markerFont, markerBrush, (Width / 2) - 6, Height - 18);
            e.Graphics.DrawString("270", markerFont, markerBrush, 10, (Height / 2) - 6);

            // Draw angle line
            var center = new Point(Width / 2, Height / 2);
            float angle = Angle.DegreeToRadian();
            var endPoint = new Point(
                center.X + (int)(center.X * Math.Sin(angle)),
                center.Y + (int)(center.Y * -Math.Cos(angle)));
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Red), 1), center, endPoint);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            #region Sanity checks
            if (e == null) throw new ArgumentNullException(nameof(e));
            #endregion

            if (e.Button == MouseButtons.Left) UpdateAngle(e.Location);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            #region Sanity checks
            if (e == null) throw new ArgumentNullException(nameof(e));
            #endregion

            if (e.Button == MouseButtons.Left) UpdateAngle(e.Location);
        }
        #endregion

        #region Helpers
        private void UpdateAngle(Point location)
        {
            // Correct ellipsoid distortion
            float widthToHeightRatio = Width / (float)Height;
            int distortedY;
            if (location.Y == 0) distortedY = location.Y;
            else if (location.Y < Height / 2) distortedY = (Height / 2) - (int)(((Height / 2) - location.Y) * widthToHeightRatio);
            else distortedY = (Height / 2) + (int)((location.Y - (Height / 2)) * widthToHeightRatio);

            var center = new Point(Width / 2, Height / 2);
            Angle = GetAngle(center, new Point(location.X, distortedY));
            if (Range != null) Angle = Angle.Clamp(Range.Minimum, Range.Maximum);

            Refresh();
        }

        /// <summary>
        /// Calculates the angle of a vector pointing from <paramref name="p1"/> to <paramref name="p2"/>.
        /// </summary>
        private static float GetAngle(Point p1, Point p2)
        {
            if (p2.X - p1.X == 0) return (p2.Y > p1.Y ? 180 : 0);
            else
            {
                float angle = ((float)Math.Atan((p2.Y - p1.Y) / (float)(p2.X - p1.X))).RadianToDegree() + 90;
                if ((p2.X - p1.X) < 0 || (p2.Y - p1.Y) < 0) angle += 180;
                if ((p2.X - p1.X) > 0 && (p2.Y - p1.Y) < 0) angle -= 180;

                if (angle < 0) angle += 360;
                return angle % 360;
            }
        }
        #endregion
    }
}
