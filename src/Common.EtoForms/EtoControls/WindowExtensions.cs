// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Native;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Provides extension methods for <see cref="Window"/>s and other <see cref="Control"/>s.
/// </summary>
public static class WindowExtensions
{
    /// <summary>
    /// Moves a window to the center of the screen it is on.
    /// </summary>
    /// <remarks>Can be called before the window is shown, e.g., from the <see cref="Control.Load"/> event handler.</remarks>
    public static void CenterOnScreen(this Window window)
    {
        #region Sanity checks
        if (window == null) throw new ArgumentNullException(nameof(window));
        #endregion

        // Actual size may not be known yet before the window is shown
        var size = window.Size;
        if (size.Width <= 0 || size.Height <= 0)
        {
            var preferredSize = Size.Round(window.GetPreferredSize());
            size = new(
                size.Width > 0 ? size.Width : preferredSize.Width,
                size.Height > 0 ? size.Height : preferredSize.Height);
        }

        var area = (window.Screen ?? Screen.PrimaryScreen).WorkingArea;
        window.Location = new(
            (int)(area.X + (area.Width - size.Width) / 2),
            (int)(area.Y + (area.Height - size.Height) / 2));
    }

    /// <summary>
    /// Centers a window on the screen when it is loaded, unless it has an <see cref="Window.Owner"/>.
    /// </summary>
    /// <remarks>Windows without an owner are otherwise placed at the top-left corner of the screen on some platforms.</remarks>
    public static void CenterOnScreenIfOwnerless(this Window window)
    {
        #region Sanity checks
        if (window == null) throw new ArgumentNullException(nameof(window));
        #endregion

        window.Load += delegate
        {
            if (window.Owner == null) window.CenterOnScreen();
        };
    }

    /// <summary>
    /// Prevents a window from being pinned to the Windows taskbar.
    /// </summary>
    /// <remarks>Does nothing on platforms other than Windows 7 or newer.</remarks>
    public static void PreventPinning(this Window window)
    {
        #region Sanity checks
        if (window == null) throw new ArgumentNullException(nameof(window));
        #endregion

        window.WhenLoaded(() =>
        {
            if (WindowsUtils.IsWindows7 && window.NativeHandle != IntPtr.Zero)
                WindowsTaskbar.PreventPinning(window.NativeHandle);
        });
    }

    /// <summary>
    /// Sets the Application User Model ID for a window, controlling how it is grouped in the Windows taskbar.
    /// </summary>
    /// <param name="window">The window to set the ID for.</param>
    /// <param name="appID">The application ID to set.</param>
    /// <param name="relaunchCommand">The command to use for relaunching the application; can be <c>null</c>.</param>
    /// <param name="relaunchIcon">The icon to use for relaunching the application; can be <c>null</c>.</param>
    /// <param name="relaunchName">The name to display for relaunching the application; can be <c>null</c>.</param>
    /// <remarks>Does nothing on platforms other than Windows 7 or newer.</remarks>
    public static void SetWindowAppID(this Window window, [Localizable(false)] string appID, [Localizable(false)] string? relaunchCommand = null, [Localizable(false)] string? relaunchIcon = null, [Localizable(true)] string? relaunchName = null)
    {
        #region Sanity checks
        if (window == null) throw new ArgumentNullException(nameof(window));
        if (string.IsNullOrEmpty(appID)) throw new ArgumentNullException(nameof(appID));
        #endregion

        window.WhenLoaded(() =>
        {
            if (WindowsUtils.IsWindows7 && window.NativeHandle != IntPtr.Zero)
                WindowsTaskbar.SetWindowAppID(window.NativeHandle, appID, relaunchCommand, relaunchIcon, relaunchName);
        });
    }

    /// <summary>
    /// Adds a UAC shield icon to a button.
    /// </summary>
    /// <remarks>
    /// This is purely cosmetic. UAC elevation is a separate concern.
    /// Does nothing on platforms other than Windows Vista or newer or with Eto.Forms backends that do not use native Win32 buttons.
    /// </remarks>
    public static void AddShieldIcon(this Button button)
    {
        #region Sanity checks
        if (button == null) throw new ArgumentNullException(nameof(button));
        #endregion

        button.WhenLoaded(() =>
        {
            if (!WindowsUtils.IsWindowsVista) return;

            // The shield is only drawn by system-styled Win32 buttons (e.g., FlatStyle.System for the Windows Forms backend)
            if (button.ControlObject?.GetType().GetProperty("FlatStyle") is {PropertyType.IsEnum: true, CanWrite: true} flatStyle)
                flatStyle.SetValue(button.ControlObject, Enum.Parse(flatStyle.PropertyType, "System"));

            if (button.NativeHandle != IntPtr.Zero)
                PInvoke.SendMessage((HWND)button.NativeHandle, PInvoke.BCM_SETSHIELD, default, 1);
        });
    }

    /// <summary>
    /// Executes an action immediately if the control is already loaded, otherwise once it is loaded.
    /// </summary>
    private static void WhenLoaded(this Control control, Action action)
    {
        if (control.Loaded) action();
        else control.Load += delegate { action(); };
    }
}
