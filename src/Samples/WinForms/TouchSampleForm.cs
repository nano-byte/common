// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Drawing.Drawing2D;
using NanoByte.Common.Controls.Touch;

namespace NanoByte.Common.Samples.WinForms;

/// <summary>
/// Demonstrates <see cref="TouchForm"/> by letting the user drag, pinch and twist a shape with multi-touch gestures.
/// </summary>
public sealed class TouchSampleForm : TouchForm
{
    private float _offsetX, _offsetY;
    private float _scale = 1;
    private float _angle;

    public TouchSampleForm()
    {
        Text = "Touch gestures";
        ClientSize = new(400, 400);
        BackColor = Color.White;
        DoubleBuffered = true;
        DoubleTapEnabled = true;

        Tapped += (_, e) =>
        {
            // Double tap resets the transform
            if (e.TapCount >= 2)
            {
                _offsetX = _offsetY = 0;
                _scale = 1;
                _angle = 0;
                Invalidate();
            }

            Text = $"Tapped at {e.Position.X:F0}, {e.Position.Y:F0}";
        };

        ManipulationUpdated += (_, e) =>
        {
            _offsetX += e.Delta.TranslationX;
            _offsetY += e.Delta.TranslationY;
            _scale *= e.Delta.Scale;
            _angle += e.Delta.Rotation;
            Invalidate();

            Text = e.IsInertial ? "Gliding…" : "Manipulating";
        };

        ManipulationCompleted += (_, _) => Text = "Touch gestures";
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        g.DrawString("Drag, pinch or twist with touch. Double-tap to reset.", Font, Brushes.Gray, 10, 10);

        using var transform = new Matrix();
        transform.Translate(ClientSize.Width / 2f + _offsetX, ClientSize.Height / 2f + _offsetY);
        transform.Rotate(_angle * 180f / (float)Math.PI);
        transform.Scale(_scale, _scale);
        g.Transform = transform;

        g.FillRectangle(Brushes.SteelBlue, -80, -50, 160, 100);
        using var format = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
        g.DrawString("Touch me", Font, Brushes.White, new RectangleF(-80, -50, 160, 100), format);
    }
}
