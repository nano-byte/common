// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.Controls;

/// <summary>
/// Provides extension methods for <see cref="Control"/>s.
/// </summary>
public static class ControlExtensions
{
#if !NET6_0_OR_GREATER
    /// <summary>
    /// Executes the given <paramref name="action"/> on the thread that owns this control and returns immediately.
    /// </summary>
    public static void BeginInvoke(this Control control, Action action)
    {
        #region Sanity checks
        if (control == null) throw new ArgumentNullException(nameof(control));
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        control.BeginInvoke(action);
    }

    /// <summary>
    /// Executes the given <paramref name="action"/> on the thread that owns this control and blocks until it is complete.
    /// </summary>
    public static void Invoke(this Control control, Action action)
    {
        #region Sanity checks
        if (control == null) throw new ArgumentNullException(nameof(control));
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        control.Invoke(action);
    }

    /// <summary>
    /// Executes the given <paramref name="action"/> on the thread that owns this control and blocks until it is complete.
    /// </summary>
    /// <returns>The return value of the <paramref name="action"/>.</returns>
    public static T Invoke<T>(this Control control, Func<T> action)
    {
        #region Sanity checks
        if (control == null) throw new ArgumentNullException(nameof(control));
        if (action == null) throw new ArgumentNullException(nameof(action));
        #endregion

        return (T)control.Invoke(action);
    }
#endif

    /// <summary>
    /// Returns the current DPI scaling factor relative to the default value of 96 DPI.
    /// </summary>
    public static float GetDpiScale(this Control control)
    {
        #region Sanity checks
        if (control == null) throw new ArgumentNullException(nameof(control));
        #endregion

        try
        {
            using var graphics = control.CreateGraphics();
            return graphics.DpiX / 96.0f;
        }
        catch
        {
            return 1;
        }
    }
}
