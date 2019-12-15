// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Windows.Forms;
using NanoByte.Common.Native;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// A simple dialog displaying selectable multi-line text.
    /// </summary>
    public sealed partial class OutputBox : Form
    {
        private OutputBox()
        {
            InitializeComponent();
            HandleCreated += delegate { WindowsTaskbar.PreventPinning(Handle); };
        }

        /// <summary>
        /// Displays an output box with some text.
        /// </summary>
        /// <param name="owner">The parent window for the dialogs; can be <c>null</c>.</param>
        /// <param name="title">The text to display above the <paramref name="message"/>.</param>
        /// <param name="message">The selectable multi-line text to display to the user.</param>
        public static void Show(IWin32Window? owner, [Localizable(true)] string title, [Localizable(true)] string message)
        {
            #region Sanity checks
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (message == null) throw new ArgumentNullException(nameof(message));
            #endregion

            using var outputBox = new OutputBox
            {
                Text = Application.ProductName,
                labelTitle = {Text = title},
                textMessage = {Text = message.Replace("\n", Environment.NewLine)}
            };
            outputBox.toolTip.SetToolTip(outputBox.labelTitle, outputBox.labelTitle.Text);
            // ReSharper disable once AccessToDisposedClosure
            outputBox.Shown += delegate { outputBox.SetForegroundWindow(); };
            outputBox.ShowDialog(owner);
        }
    }
}
