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
using System.Text;
using System.Windows.Forms;
using JetBrains.Annotations;
using NanoByte.Common.Native;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// A dialog displaying an error message and details.
    /// </summary>
    public sealed partial class ErrorBox : Form
    {
        private ErrorBox()
        {
            InitializeComponent();
            HandleCreated += delegate { WindowsTaskbar.PreventPinning(Handle); };
        }

        /// <summary>
        /// Displays an error box with a message and details.
        /// </summary>
        /// <param name="owner">The parent window for the dialog; can be <c>null</c>.</param>
        /// <param name="exception">The error message to display.</param>
        /// <param name="logRtf">The details formatted as RTF.</param>
        /// <returns>The text the user entered if he pressed OK; otherwise <c>null</c>.</returns>
        public static void Show([CanBeNull] IWin32Window owner, [NotNull] Exception exception, [NotNull] RtfBuilder logRtf)
        {
            #region Sanity checks
            if (exception == null) throw new ArgumentNullException("exception");
            if (logRtf == null) throw new ArgumentNullException("logRtf");
            #endregion

            Log.Error(exception);

            using (var errorBox = new ErrorBox
            {
                Text = Application.ProductName,
                labelMessage = {Text = exception.Message},
                labelInnerMessage = {Text = GetInnerMessage(exception)},
                textLog = {Rtf = logRtf.ToString()}
            })
            {
                // ReSharper disable once AccessToDisposedClosure
                errorBox.Shown += delegate { errorBox.SetForegroundWindow(); };
                errorBox.ShowDialog(owner);
            }
        }

        private static string GetInnerMessage(Exception exception)
        {
            var builder = new StringBuilder();
            while (exception.InnerException != null && exception.InnerException.Message != exception.Message)
            {
                exception = exception.InnerException;
                builder.AppendLine(exception.Message);
            }
            return builder.ToString();
        }

        private void label_Click(object sender, EventArgs e)
        {
            var control = (Control)sender;
            new ContextMenu(new[]
            {
                new MenuItem(Resources.CopyToClipboard, delegate { Clipboard.SetText(control.Text); })
            }).Show(control, new Point());
        }
    }
}
