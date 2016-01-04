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
using System.Drawing.Design;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace NanoByte.Common.Values.Design
{
    /// <summary>
    /// An editor that can be associated with <c>string</c> properties. Uses <see cref="TextEditorControl"/>.
    /// </summary>
    /// <seealso cref="FileTypeAttribute"/>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class CodeEditor : UITypeEditor
    {
        /// <inheritdoc/>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <inheritdoc/>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            #region Sanity checks
            if (context == null) throw new ArgumentNullException("context");
            if (provider == null) throw new ArgumentNullException("provider");
            #endregion

            var editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (editorService == null) return value;

            var editorControl = new TextEditorControl {Text = value as string, Dock = DockStyle.Fill};
            var form = new Form
            {
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                ShowInTaskbar = false,
                Controls = {editorControl}
            };

            var fileType = context.PropertyDescriptor.Attributes.OfType<FileTypeAttribute>().FirstOrDefault();
            if (fileType != null)
            {
                editorControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(fileType.FileType);
                form.Text = fileType.FileType + " Editor";
            }
            else form.Text = "Editor";

            editorService.ShowDialog(form);
            return editorControl.Text;
        }
    }
}
