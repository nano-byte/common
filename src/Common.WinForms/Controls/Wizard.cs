// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;
using NanoByte.Common.Properties;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// A base class for creating wizard interfaces. Manages pages as a stack of <see cref="UserControl"/>s.
    /// </summary>
    /// <seealso cref="IWizardPage"/>
    public class Wizard : Form
    {
        #region WinForms
        private readonly Panel _panelPage;
        private readonly Button _buttonBack;

        /// <summary>
        /// Creates a new wizard.
        /// </summary>
        public Wizard()
        {
            SuspendLayout();

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = MinimizeBox = ShowIcon = ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(470, 348);

            Controls.Add(_panelPage = new Panel
            {
                BackColor = SystemColors.ControlLightLight,
                Dock = DockStyle.Top,
                Size = new Size(470, 300),
                TabIndex = 0
            });

            var buttonCancel = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Size = new Size(75, 23),
                TabIndex = 2,
                Text = Resources.Cancel,
                UseVisualStyleBackColor = true,
                DialogResult = DialogResult.Cancel
            };
            buttonCancel.Location = new Point(
                ClientSize.Width - buttonCancel.Width - 12,
                ClientSize.Height - buttonCancel.Height - 12);
            buttonCancel.Click += delegate
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
            Controls.Add(buttonCancel);
            CancelButton = buttonCancel;

            Controls.Add(_buttonBack = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Size = new Size(75, 23),
                TabIndex = 1,
                Text = @"< " + Resources.Back,
                UseVisualStyleBackColor = true,
                Visible = false
            });
            _buttonBack.Location = new Point(
                buttonCancel.Left - _buttonBack.Width - 8,
                buttonCancel.Top);
            _buttonBack.Click += delegate { PopPage(); };

            ResumeLayout(false);
        }
        #endregion

        #region Page stack
        /// <summary>
        /// The wizard page history with the currently visible page on top.
        /// </summary>
        protected readonly Stack<UserControl> PageStack = new Stack<UserControl>();

        /// <summary>
        /// Displays a new wizard page and adds it to the <see cref="PageStack"/>.
        /// </summary>
        /// <param name="page">The page to display and add.</param>
        /// <seealso cref="IWizardPage"/>
        protected void PushPage([NotNull] UserControl page)
        {
            #region Sanity checks
            if (page == null) throw new ArgumentNullException(nameof(page));
            #endregion

            if (PageStack.Count != 0) _panelPage.Controls.Remove(PageStack.Peek());
            _panelPage.Controls.Add(page);
            PageStack.Push(page);

            _buttonBack.Visible = PageStack.Count > 1;
            ShowPage(page);
        }

        /// <summary>
        /// Removes the current wizard page form the <see cref="PageStack"/> and displays the previous one.
        /// </summary>
        protected void PopPage()
        {
            _panelPage.Controls.Remove(PageStack.Pop());
            _panelPage.Controls.Add(PageStack.Peek());

            _buttonBack.Visible = PageStack.Count > 1;
            ShowPage(PageStack.Peek());
        }

        private static void ShowPage(Control page)
        {
            page.Focus();

            // ReSharper disable once SuspiciousTypeConversion.Global
            (page as IWizardPage)?.OnPageShow();
        }
        #endregion
    }
}
