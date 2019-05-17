// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using JetBrains.Annotations;
using NanoByte.Common.Properties;
using TaskDialog;

namespace NanoByte.Common
{
    /// <summary>
    /// Provides easier access to typical <see cref="MessageBox"/> configurations and automatically upgrades to TaskDialogs when available.
    /// </summary>
    public static class Msg
    {
        #region Inform
        /// <summary>
        /// Displays a message to the user using a <see cref="MessageBox"/> or <see cref="TaskDialog"/>.
        /// </summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        public static void Inform([CanBeNull] IWin32Window owner, [NotNull, Localizable(true)] string text, MsgSeverity severity)
        {
            if (TaskDialog.TaskDialog.IsAvailable)
            {
                try
                {
                    ShowTaskDialog(GetTaskDialog(text, severity), owner);
                }
                catch (BadImageFormatException)
                {
                    ShowMesageBox(owner, text, severity, MessageBoxButtons.OK);
                }
                catch (EntryPointNotFoundException)
                {
                    ShowMesageBox(owner, text, severity, MessageBoxButtons.OK);
                }
            }
            else ShowMesageBox(owner, text, severity, MessageBoxButtons.OK);
        }
        #endregion

        #region OK/Cancel
        /// <summary>
        /// Asks the user a OK/Cancel-question using a <see cref="MessageBox"/> or <see cref="TaskDialog"/>.
        /// </summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        /// <param name="okCaption">The title and a short description (separated by a linebreak) of the <see cref="DialogResult.OK"/> option.</param>
        /// <param name="cancelCaption">The title and a short description (separated by a linebreak) of the <see cref="DialogResult.Cancel"/> option; can be <c>null</c>.</param>
        /// <returns><c>true</c> if <paramref name="okCaption"/> was selected, <c>false</c> if <paramref name="cancelCaption"/> was selected.</returns>
        /// <remarks>If a <see cref="MessageBox"/> is used, <paramref name="okCaption"/> and <paramref name="cancelCaption"/> are not display to the user, so don't rely on them!</remarks>
        public static bool OkCancel([CanBeNull] IWin32Window owner, [NotNull, Localizable(true)] string text, MsgSeverity severity, [NotNull, Localizable(true)] string okCaption, [CanBeNull, Localizable(true)] string cancelCaption = null)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrEmpty(okCaption)) throw new ArgumentNullException(nameof(okCaption));
            #endregion

            if (TaskDialog.TaskDialog.IsAvailable)
            {
                var taskDialog = GetTaskDialog(text, severity);

                // Display default names with custom explanations
                taskDialog.UseCommandLinks = true;

                if (string.IsNullOrEmpty(cancelCaption))
                { // Default cancel button
                    taskDialog.Buttons = new[]
                    {
                        new TaskDialogButton((int)DialogResult.OK, okCaption.Replace("\r\n", "\n"))
                    };
                    taskDialog.CommonButtons = TaskDialogCommonButtons.Cancel;
                }
                else
                { // Custom cancel button
                    taskDialog.Buttons = new[]
                    {
                        new TaskDialogButton((int)DialogResult.OK, okCaption.Replace("\r\n", "\n")),
                        new TaskDialogButton((int)DialogResult.Cancel, cancelCaption.Replace("\r\n", "\n"))
                    };
                }

                // Only Infos should default to OK
                if (severity >= MsgSeverity.Warn) taskDialog.DefaultButton = (int)DialogResult.Cancel;

                try
                {
                    return ShowTaskDialog(taskDialog, owner) == DialogResult.OK;
                }
                #region Error handling
                catch (BadImageFormatException)
                {
                    return ShowMesageBox(owner, text, severity, MessageBoxButtons.OKCancel) == DialogResult.OK;
                }
                catch (EntryPointNotFoundException)
                {
                    return ShowMesageBox(owner, text, severity, MessageBoxButtons.OKCancel) == DialogResult.OK;
                }
                #endregion
            }
            else return ShowMesageBox(owner, text, severity, MessageBoxButtons.OKCancel) == DialogResult.OK;
        }

        /// <summary>
        /// Asks the user a OK/Cancel-question using a <see cref="MessageBox"/> or <see cref="TaskDialog"/>.
        /// </summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        /// <returns><c>true</c> if OK was selected, <c>false</c> if Cancel was selected.</returns>
        /// <remarks>If a <see cref="MessageBox"/> is used, OK and Cancel are not display to the user, so don't rely on them!</remarks>
        public static bool OkCancel([CanBeNull] IWin32Window owner, [NotNull, Localizable(true)] string text, MsgSeverity severity)
            => OkCancel(owner, text, severity, "OK", Resources.Cancel);
        #endregion

        #region Yes/No
        /// <summary>
        /// Asks the user to choose between two options (yes/no) using a <see cref="MessageBox"/> or <see cref="TaskDialog"/>.
        /// </summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        /// <param name="yesCaption">The title and a short description (separated by a linebreak) of the <see cref="DialogResult.Yes"/> option.</param>
        /// <param name="noCaption">The title and a short description (separated by a linebreak) of the <see cref="DialogResult.No"/> option.</param>
        /// <returns><c>true</c> if <paramref name="yesCaption"/> was chosen, <c>false</c> if <paramref name="noCaption"/> was chosen.</returns>
        /// <remarks>If a <see cref="MessageBox"/> is used, <paramref name="yesCaption"/> and <paramref name="noCaption"/> are not display to the user, so don't rely on them!</remarks>
        public static bool YesNo([CanBeNull] IWin32Window owner, [NotNull, Localizable(true)] string text, MsgSeverity severity, [NotNull, Localizable(true)] string yesCaption, [NotNull, Localizable(true)] string noCaption)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrEmpty(yesCaption)) throw new ArgumentNullException(nameof(yesCaption));
            if (string.IsNullOrEmpty(noCaption)) throw new ArgumentNullException(nameof(noCaption));
            #endregion

            if (TaskDialog.TaskDialog.IsAvailable)
            {
                var taskDialog = GetTaskDialog(text, severity);

                // Display fully customized text
                taskDialog.UseCommandLinks = true;
                taskDialog.Buttons = new[]
                {
                    new TaskDialogButton((int)DialogResult.Yes, yesCaption.Replace("\r\n", "\n")),
                    new TaskDialogButton((int)DialogResult.No, noCaption.Replace("\r\n", "\n"))
                };

                try
                {
                    return (ShowTaskDialog(taskDialog, owner) == DialogResult.Yes);
                }
                #region Error handling
                catch (BadImageFormatException)
                {
                    return (ShowMesageBox(owner, text, severity, MessageBoxButtons.YesNo) == DialogResult.Yes);
                }
                catch (EntryPointNotFoundException)
                {
                    return (ShowMesageBox(owner, text, severity, MessageBoxButtons.YesNo) == DialogResult.Yes);
                }
                #endregion
            }
            else return (ShowMesageBox(owner, text, severity, MessageBoxButtons.YesNo) == DialogResult.Yes);
        }

        /// <summary>
        /// Asks the user to choose between two options (yes/no) using a <see cref="MessageBox"/> or <see cref="TaskDialog"/>.
        /// </summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        public static bool YesNo([CanBeNull] IWin32Window owner, [NotNull, Localizable(true)] string text, MsgSeverity severity)
            => YesNo(owner, text, severity, Resources.Yes, Resources.No);
        #endregion

        #region Yes/No/Cancel
        /// <summary>
        /// Asks the user to choose between three options (yes/no/cancel) using a <see cref="MessageBox"/> or <see cref="TaskDialog"/>.
        /// </summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        /// <param name="yesCaption">The title and a short description (separated by a linebreak) of the <see cref="DialogResult.Yes"/> option.</param>
        /// <param name="noCaption">The title and a short description (separated by a linebreak) of the <see cref="DialogResult.No"/> option.</param>
        /// <returns><see cref="DialogResult.Yes"/> if <paramref name="yesCaption"/> was chosen,
        /// <see cref="DialogResult.No"/> if <paramref name="noCaption"/> was chosen,
        /// <see cref="DialogResult.Cancel"/> otherwise.</returns>
        /// <remarks>If a <see cref="MessageBox"/> is used, <paramref name="yesCaption"/> and <paramref name="noCaption"/> are not display to the user, so don't rely on them!</remarks>
        public static DialogResult YesNoCancel([CanBeNull] IWin32Window owner, [NotNull, Localizable(true)] string text, MsgSeverity severity, [NotNull, Localizable(true)] string yesCaption, [NotNull, Localizable(true)] string noCaption)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrEmpty(yesCaption)) throw new ArgumentNullException(nameof(yesCaption));
            if (string.IsNullOrEmpty(noCaption)) throw new ArgumentNullException(nameof(noCaption));
            #endregion

            if (TaskDialog.TaskDialog.IsAvailable)
            {
                var taskDialog = GetTaskDialog(text, severity);
                taskDialog.AllowDialogCancellation = true;
                taskDialog.CommonButtons = TaskDialogCommonButtons.Cancel;

                // Display fully customized text
                taskDialog.UseCommandLinks = true;
                taskDialog.Buttons = new[]
                {
                    new TaskDialogButton((int)DialogResult.Yes, yesCaption.Replace("\r\n", "\n")),
                    new TaskDialogButton((int)DialogResult.No, noCaption.Replace("\r\n", "\n"))
                };

                // Infos and Warnings (like Save) should default to yes
                if (severity >= MsgSeverity.Error) taskDialog.DefaultButton = (int)DialogResult.Cancel;

                try
                {
                    return ShowTaskDialog(taskDialog, owner);
                }
                #region Error handling
                catch (BadImageFormatException)
                {
                    return ShowMesageBox(owner, text, severity, MessageBoxButtons.YesNoCancel);
                }
                catch (EntryPointNotFoundException)
                {
                    return ShowMesageBox(owner, text, severity, MessageBoxButtons.YesNoCancel);
                }
                #endregion
            }
            else return ShowMesageBox(owner, text, severity, MessageBoxButtons.YesNoCancel);
        }

        /// <summary>
        /// Asks the user to choose between three options (yes/no/cancel) using a <see cref="MessageBox"/> or <see cref="TaskDialog"/>.
        /// </summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        /// <returns><see cref="DialogResult.Yes"/> if Yes was chosen,
        /// <see cref="DialogResult.No"/> if No was chosen,
        /// <see cref="DialogResult.Cancel"/> otherwise.</returns>
        public static DialogResult YesNoCancel([CanBeNull] IWin32Window owner, [NotNull, Localizable(true)] string text, MsgSeverity severity)
            => YesNoCancel(owner, text, severity, Resources.Yes, Resources.No);
        #endregion

        //--------------------//

        #region MessageBox
        /// <summary>Displays a message using a <see cref="MessageBox"/>.</summary>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        /// <param name="buttons">The buttons the user can click.</param>
        private static DialogResult ShowMesageBox([CanBeNull] IWin32Window owner, [NotNull, Localizable(true)] string text, MsgSeverity severity, MessageBoxButtons buttons)
        {
            // Handle RTL systems
            MessageBoxOptions localizedOptions;
            if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft) localizedOptions = MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign;
            else localizedOptions = 0;

            // Select icon based on message severity
            MessageBoxIcon icon;
            switch (severity)
            {
                case MsgSeverity.Warn:
                    icon = MessageBoxIcon.Warning;
                    break;
                case MsgSeverity.Error:
                    icon = MessageBoxIcon.Error;
                    break;
                default:
                case MsgSeverity.Info:
                    icon = MessageBoxIcon.Information;
                    break;
            }

            // Display MessageDialog
            return MessageBox.Show(owner, text, Application.ProductName, buttons, icon, MessageBoxDefaultButton.Button1, localizedOptions);
        }
        #endregion

        #region TaskDialog
        /// <summary>
        /// Displays a message using a <see cref="TaskDialog"/>.
        /// </summary>
        /// <param name="text">The message to be displayed.</param>
        /// <param name="severity">How severe/important the message is.</param>
        private static TaskDialog.TaskDialog GetTaskDialog([NotNull, Localizable(true)] string text, MsgSeverity severity)
        {
            // Split everything from the second line onwards off from the main text
            string[] split = text.Replace("\r\n", "\n").Split(new[] {'\n'}, 2);
            var taskDialog = new TaskDialog.TaskDialog
            {
                PositionRelativeToWindow = true,
                WindowTitle = Application.ProductName,
                MainInstruction = split[0]
            };
            if (split.Length == 2) taskDialog.Content = split[1];

            // Handle RTL systems
            taskDialog.RightToLeftLayout = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

            // Select icon based on message severity
            switch (severity)
            {
                case MsgSeverity.Warn:
                    taskDialog.MainIcon = TaskDialogIcon.Warning;
                    taskDialog.AllowDialogCancellation = true;
                    break;

                case MsgSeverity.Error:
                    taskDialog.MainIcon = TaskDialogIcon.Error;
                    taskDialog.AllowDialogCancellation = false; // Real errors shouldn't be easily ESCed away
                    break;

                default:
                case MsgSeverity.Info:
                    taskDialog.MainIcon = TaskDialogIcon.Information;
                    taskDialog.AllowDialogCancellation = true;
                    break;
            }

            return taskDialog;
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/>.
        /// </summary>
        /// <param name="taskDialog">The <see cref="TaskDialog"/> to display.</param>
        /// <param name="owner">The parent window the displayed window is modal to; can be <c>null</c>.</param>
        /// <returns>Indicates the button the user pressed.</returns>
        /// <exception cref="BadImageFormatException">The task-dialog DLL could not be loaded.</exception>
        /// <exception cref="EntryPointNotFoundException">The task-dialog DLL routine could not be called.</exception>
        private static DialogResult ShowTaskDialog([NotNull] TaskDialog.TaskDialog taskDialog, [CanBeNull] IWin32Window owner)
        {
            // Note: If you get an EntryPointNotFoundException here, add this to your application manifest and test outside the IDE:
            // <dependency>
            //   <dependentAssembly>
            //     <assemblyIdentity type="win32" name="Microsoft.Windows.Common-Controls" version="6.0.0.0" processorArchitecture="*" publicKeyToken="6595b64144ccf1df" language="*" />
            //   </dependentAssembly>
            // </dependency>

            int result = (owner == null) ? taskDialog.Show() : taskDialog.Show(owner);
            return (DialogResult)result;
        }
        #endregion
    }
}
