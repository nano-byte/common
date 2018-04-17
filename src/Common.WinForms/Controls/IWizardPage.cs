// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// An interface that <see cref="Wizard"/> pages can optionally implement to receive additional event notifications.
    /// </summary>
    public interface IWizardPage
    {
        /// <summary>
        /// Called when the wizard page is shown.
        /// </summary>
        void OnPageShow();
    }
}
