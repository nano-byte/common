// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Windows.Forms;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// Contains a single property grid for manipulating the properties of an object.
    /// </summary>
    public sealed class PropertyGridForm : Form
    {
        private PropertyGridForm(object value)
        {
            SuspendLayout();
            Text = value.ToString();
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(284, 262);
            Controls.Add(new ResettablePropertyGrid
            {
                Dock = DockStyle.Fill,
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(284, 262),
                SelectedObject = value
            });
            ShowIcon = false;
            ResumeLayout(false);
        }

        /// <summary>
        /// Displays a form with a property grid for manipulating the properties of an object.
        /// </summary>
        /// <param name="value">The object to be inspected.</param>
        public static void Show(object value)
            => new PropertyGridForm(value ?? throw new ArgumentNullException(nameof(value))).Show();
    }
}
