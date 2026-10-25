// Copyright Bastian Eicher
// Licensed under the MIT License

using Windows.Win32;
using Windows.Win32.Foundation;

namespace NanoByte.Common.Native;

/// <summary>
/// Provides helper methods and API calls specific to the <see cref="System.Windows.Forms"/> UI toolkit.
/// </summary>
public static class WinFormsUtils
{
    /// <summary>
    /// Centers a window on its parent/owner. Call this from the <see cref="Form.Load"/> event handler.
    /// </summary>
    /// <remarks>This method is an alternative to <see cref="FormStartPosition.CenterParent"/> which only works with <see cref="Form.ShowDialog(IWin32Window)"/> and not <see cref="Form.Show(IWin32Window)"/>.</remarks>
    public static void CenterOnParent(this Form form)
    {
        #region Sanity checks
        if (form == null) throw new ArgumentNullException(nameof(form));
        #endregion

        if (form.Owner == null) return;
        form.Location = new Point(
            x: form.Owner.Location.X + form.Owner.Width / 2 - form.Width / 2,
            y: form.Owner.Location.Y + form.Owner.Height / 2 - form.Height / 2);
    }

    /// <summary>
    /// Forces a window to the foreground or flashes the taskbar if another process has the focus.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This method operates only on windows and not on individual controls.")]
    public static void SetForegroundWindow(this Form form)
    {
        #region Sanity checks
        if (form == null) throw new ArgumentNullException(nameof(form));
        #endregion

        if (!WindowsUtils.IsWindows) return;
        PInvoke.SetForegroundWindow((HWND)form.Handle);
    }

    /// <summary>
    /// Configures a control to move the entire window when clicked and dragged.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This method operates only on windows and not on individual controls.")]
    public static void EnableWindowDrag(this Control control)
    {
        #region Sanity checks
        if (control == null) throw new ArgumentNullException(nameof(control));
        #endregion

        if (!WindowsUtils.IsWindows) return;
        control.MouseDown += (_, args) =>
        {
            if (args.Button == MouseButtons.Left)
            {
                PInvoke.ReleaseCapture();

                var target = control;
                while (target.Parent != null)
                    target = target.Parent;

                PInvoke.SendMessage((HWND)target.Handle, PInvoke.WM_NCLBUTTONDOWN, PInvoke.HTCAPTION, default);
            }
        };
    }

    /// <summary>
    /// Adds a UAC shield icon to a button. Does nothing if not running Windows Vista or newer.
    /// </summary>
    /// <remarks>This is purely cosmetic. UAC elevation is a separate concern.</remarks>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Native API only applies to buttons.")]
    public static void AddShieldIcon(this Button button)
    {
        #region Sanity checks
        if (button == null) throw new ArgumentNullException(nameof(button));
        #endregion

        if (!WindowsUtils.IsWindowsVista) return;
        button.FlatStyle = FlatStyle.System;
        PInvoke.SendMessage((HWND)button.Handle, PInvoke.BCM_SETSHIELD, default, 1);
    }

    /// <summary>
    /// Determines whether <paramref name="key"/> is pressed right now.
    /// </summary>
    /// <remarks>Will always return <c>false</c> on non-Windows OSes.</remarks>
    public static bool IsKeyDown(Keys key)
    {
        if (!WindowsUtils.IsWindows) return false;
        return (PInvoke.GetAsyncKeyState((int)key) & 0x8000) != 0;
    }

    /// <summary>
    /// Determines whether this application is currently idle.
    /// </summary>
    /// <returns><c>true</c> if idle, <c>false</c> if handling window events.</returns>
    /// <remarks>Will always return <c>true</c> on non-Windows OSes.</remarks>
    public static bool AppIdle
    {
        get
        {
            if (!WindowsUtils.IsWindows) return true;
            return !PInvoke.PeekMessage(out _, default, 0, 0, 0);
        }
    }

    /// <summary>
    /// Text-box caret blink time in seconds.
    /// </summary>
    public static float CaretBlinkTime => WindowsUtils.IsWindows
        ? PInvoke.GetCaretBlinkTime() / 1000f
        : 0.5f;

    /// <summary>
    /// Prevents the mouse cursor from leaving a specific window.
    /// </summary>
    /// <param name="handle">The handle to the window to lock the mouse cursor into.</param>
    /// <returns>A handle to the window that had previously captured the mouse.</returns>
    /// <remarks>Will do nothing on non-Windows OSes.</remarks>
    public static IntPtr SetCapture(IntPtr handle)
    {
        if (!WindowsUtils.IsWindows) return IntPtr.Zero;
        return PInvoke.SetCapture((HWND)handle);
    }

    /// <summary>
    /// Releases the mouse cursor after it was locked by <see cref="SetCapture"/>.
    /// </summary>
    /// <returns><c>true</c> if successful; <c>false</c> otherwise.</returns>
    /// <remarks>Will always return <c>false</c> on non-Windows OSes.</remarks>
    public static bool ReleaseCapture()
    {
        if (!WindowsUtils.IsWindows) return false;
        return PInvoke.ReleaseCapture();
    }
}
