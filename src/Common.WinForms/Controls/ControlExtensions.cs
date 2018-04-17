// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Windows.Forms;

namespace NanoByte.Common.Controls
{
    /// <summary>
    /// Provides extension methods for <see cref="Control"/>s.
    /// </summary>
    public static class ControlExtensions
    {
        public static void Invoke(this Control control, Action action)
        {
            #region Sanity checks
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (action == null) throw new ArgumentNullException(nameof(action));
            #endregion

            control.Invoke(action);
        }

        public static T Invoke<T>(this Control control, Func<T> action)
        {
            #region Sanity checks
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (action == null) throw new ArgumentNullException(nameof(action));
            #endregion

            return (T)control.Invoke(action);
        }
    }
}
