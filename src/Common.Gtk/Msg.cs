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

using System.ComponentModel;
using Gtk;
using JetBrains.Annotations;
using NanoByte.Common.Info;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides easier access to typical <see cref="MessageDialog"/> configurations.
    /// </summary>
    public static class Msg
    {
        #region Inform
        /// <summary>
        /// Displays a message to the user using a <see cref="MessageDialog"/>.
        /// </summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        public static void Inform([CanBeNull] Window owner, [NotNull, Localizable(true)] string text, MsgSeverity severity)
        {
            ShowMessageDialog(owner, text, severity, ButtonsType.Ok);
        }
        #endregion

        #region OK/Cancel
        /// <summary>
        /// Asks the user a OK/Cancel-question using a <see cref="MessageDialog"/>.
        /// </summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        /// <returns><c>true</c> if OK was selected, <c>false</c> if Cancel was selected.</returns>
        public static bool OKCancel([CanBeNull] Window owner, [NotNull, Localizable(true)] string text, MsgSeverity severity)
        {
            return ShowMessageDialog(owner, text, severity, ButtonsType.OkCancel) == ResponseType.Ok;
        }
        #endregion

        #region Yes/No
        /// <summary>
        /// Asks the user to choose between two options (yes/no) using a <see cref="MessageDialog"/>.
        /// </summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        /// <returns><c>true</c> if Yes was chosen, <c>false</c> if No was chosen.</returns>
        public static bool YesNo([CanBeNull] Window owner, [NotNull, Localizable(true)] string text, MsgSeverity severity)
        {
            return ShowMessageDialog(owner, text, severity, ButtonsType.YesNo) == ResponseType.Yes;
        }
        #endregion

        #region Yes/No/Cancel
        /// <summary>
        /// Asks the user to choose between three options (yes/no/cancel) using a <see cref="MessageDialog"/>.
        /// </summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        /// <returns><see cref="ResponseType.Yes"/> if Yes was chosen,
        /// <see cref="ResponseType.No"/> if No was chosen,
        /// <see cref="ResponseType.Cancel"/> otherwise.</returns>
        public static ResponseType YesNoCancel([CanBeNull] Window owner, [NotNull, Localizable(true)] string text, MsgSeverity severity)
        {
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            return ShowMessageDialog(owner, text, severity, ButtonsType.YesNo, addCancel: true);
        }
        #endregion

        //--------------------//

        #region MessageDialog
        /// <summary>Displays a message using a <see cref="MessageDialog"/>.</summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        /// <param name="buttons">The buttons the user can click.</param>
        /// <param name="addCancel">Add an additional "Cancel" button.</param>
        private static ResponseType ShowMessageDialog([CanBeNull] Window owner, [NotNull, Localizable(true)] string text, MsgSeverity severity, ButtonsType buttons, bool addCancel = false)
        {
            // Select icon based on message severity
            MessageType type;
            switch (severity)
            {
                case MsgSeverity.Warn:
                    type = MessageType.Warning;
                    break;
                case MsgSeverity.Error:
                    type = MessageType.Error;
                    break;
                default:
                case MsgSeverity.Info:
                    type = buttons.HasFlag(ButtonsType.YesNo) ? MessageType.Question : MessageType.Info;
                    break;
            }

            // Display MessageDialog
            using (var dialog = new MessageDialog(owner, DialogFlags.Modal, type, buttons, text))
            {
                if (addCancel) dialog.AddButton(Stock.Cancel, 2);
                dialog.Title = AppInfo.Current.ProductName;
                var response = (ResponseType)dialog.Run();
                dialog.Destroy();
                return response;
            }
        }
        #endregion
    }
}
