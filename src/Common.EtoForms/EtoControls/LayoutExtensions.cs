// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common.EtoControls;

/// <summary>
/// Provides extension methods for laying out wrapped text.
/// </summary>
public static class LayoutExtensions
{
    extension(Window window)
    {
        /// <summary>
        /// Gives a window a fixed client width and sizes its height to fit its content at that width.
        /// </summary>
        /// <param name="clientWidth">The width of the window's client area.</param>
        /// <remarks>
        /// Use this instead of setting only <see cref="Control.Width"/> or <c>ClientSize</c> for windows with wrapped <see cref="Label"/>s.
        /// </remarks>
        public void FitContentHeight(int clientWidth)
        {
            #region Sanity checks
            if (window == null) throw new ArgumentNullException(nameof(window));
            #endregion

            // Wait until automatic sizing is done
            window.LoadComplete += delegate
            {
                var oldSize = window.Size;

                // Lay out content at the final width first, so wrapped labels measure their height for that width
                window.ClientSize = new(clientWidth, window.ClientSize.Height);
                if (window.Content is {} content)
                {
                    var preferredSize = content.GetPreferredSize(new SizeF(content.Width, float.PositiveInfinity));
                    window.ClientSize = new(clientWidth, window.ClientSize.Height + (int)Math.Ceiling(preferredSize.Height) - content.Height);
                }

                // Keep the window centered on the position it had before
                var newSize = window.Size;
                window.Location = new(
                    window.Location.X + (oldSize.Width - newSize.Width) / 2,
                    window.Location.Y + (oldSize.Height - newSize.Height) / 2);
            };
        }

        /// <summary>
        /// Gives a window a specific initial client size, once automatic sizing is done.
        /// </summary>
        /// <param name="clientSize">The size of the window's client area.</param>
        /// <remarks>
        /// Use this instead of setting <c>ClientSize</c> directly for resizable windows.
        /// </remarks>
        public void SetInitialClientSize(Size clientSize)
        {
            #region Sanity checks
            if (window == null) throw new ArgumentNullException(nameof(window));
            #endregion

            window.LoadComplete += delegate
            {
                var oldSize = window.Size;
                window.ClientSize = clientSize;

                // Keep the window centered on the position it had before
                var newSize = window.Size;
                window.Location = new(
                    window.Location.X + (oldSize.Width - newSize.Width) / 2,
                    window.Location.Y + (oldSize.Height - newSize.Height) / 2);
            };
        }
    }

    extension(Label label)
    {
        /// <summary>
        /// Makes the layout containing a wrapped label recalculate the label's height whenever its width changes, e.g., when the user resizes the window.
        /// </summary>
        /// <returns>The same <paramref name="label"/>, for use in object initializers.</returns>
        public Label ReflowOnResize()
        {
            #region Sanity checks
            if (label == null) throw new ArgumentNullException(nameof(label));
            #endregion

            // Defer, since the size changes while the layout is still being performed
            label.SizeChanged += delegate { Application.Instance.AsyncInvoke(() => (label.VisualParent as Layout)?.Update()); };
            return label;
        }
    }
}
