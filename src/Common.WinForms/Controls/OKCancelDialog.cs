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

using System.Windows.Forms;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// A base-class for creating fixed-size dialog boxes with an OK and a Cancel button.
    /// </summary>
    public partial class OKCancelDialog : Form
    {
        public OKCancelDialog()
        {
            InitializeComponent();
            buttonOK.Text = Resources.OK;
            buttonCancel.Text = Resources.Cancel;
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            OnOKClicked();
        }

        private void buttonCancel_Click(object sender, System.EventArgs e)
        {
            OnCancelClicked();
        }

        /// <summary>
        /// This hook is called when the user clicks the OK button.
        /// </summary>
        protected virtual void OnOKClicked()
        {}

        /// <summary>
        /// This hook is called when the user clicks the Cancel button.
        /// </summary>
        protected virtual void OnCancelClicked()
        {}
    }
}
