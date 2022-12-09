﻿/*
 * Copyright (c) 2010, wyDay
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * * Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * Original code from: http://wyday.com/splitbutton/
 * Modified by: Bastian Eicher, 2011-2016
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ContentAlignment = System.Drawing.ContentAlignment;

#if NETFRAMEWORK
using System.Security.Permissions;
#endif

namespace NanoByte.Common.Controls;

/// <summary>
/// A <see cref="Button"/> with an additional drop-down menu.
/// </summary>
public class DropDownButton : Button
{
    #region Variables
    private const int SplitSectionWidth = 18;

    private static readonly int _borderSize = SystemInformation.Border3DSize.Width * 2;
    private Rectangle _dropDownRectangle;

    private bool _isDropDownMenuVisible;

    private bool _showSplit;
    private bool _skipNextOpen;
    private ContextMenuStrip? _dropDownMenuStrip;
    private PushButtonState _state;

    private TextFormatFlags _textFormatFlags = TextFormatFlags.Default;
    #endregion

    #region Constructor
    public DropDownButton() => AutoSize = true;
    #endregion

    #region Properties
    [Browsable(false)]
    public override ContextMenuStrip? ContextMenuStrip { get => DropDownMenuStrip; set => DropDownMenuStrip = value; }

    [DefaultValue(null)]
    public ContextMenuStrip? DropDownMenuStrip
    {
        get => _dropDownMenuStrip;
        set
        {
            //remove the event handlers for the old DropDownMenuStrip
            if (_dropDownMenuStrip != null)
            {
                _dropDownMenuStrip.Closing -= DropDownMenuStrip_Closing;
                _dropDownMenuStrip.Opening -= DropDownMenuStrip_Opening;
            }

            //add the event handlers for the new DropDownMenuStrip
            if (value != null)
            {
                value.Closing += DropDownMenuStrip_Closing;
                value.Opening += DropDownMenuStrip_Opening;
            }

            _dropDownMenuStrip = value;
        }
    }

    /// <summary>
    /// Splits the button into a conventional "button" part and a "drop down" part rather than treating the entire button as a "drop down" button.
    /// </summary>
    [DefaultValue(false)]
    [Description("Splits the button into a conventional \"button\" part and a \"drop down\" part rather than treating the entire button as a \"drop down\" button.")]
    public bool ShowSplit
    {
        get => _showSplit;
        set
        {
            if (value != _showSplit)
            {
                _showSplit = value;
                Invalidate();
            }
        }
    }

    private bool IsSplitLineVisible => _showSplit && (State == PushButtonState.Hot || State == PushButtonState.Pressed || !Application.RenderWithVisualStyles);

    private Rectangle EffectiveDropDownRectangle => _showSplit ? _dropDownRectangle : ClientRectangle;

    private PushButtonState State
    {
        get => _state;
        set
        {
            if (!_state.Equals(value))
            {
                _state = value;
                Invalidate();
            }
        }
    }
    #endregion Properties

    #region Event handlers
    protected override bool IsInputKey(Keys keyData)
    {
        if (keyData.Equals(Keys.Down))
            return true;

        return base.IsInputKey(keyData);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        #region Sanity checks
        if (e == null) throw new ArgumentNullException(nameof(e));
        #endregion

        if (!State.Equals(PushButtonState.Pressed) && !State.Equals(PushButtonState.Disabled))
            State = PushButtonState.Default;
    }

    protected override void OnKeyDown(KeyEventArgs kevent)
    {
        #region Sanity checks
        if (kevent == null) throw new ArgumentNullException(nameof(kevent));
        #endregion

        if (kevent.KeyCode.Equals(Keys.Down) && !_isDropDownMenuVisible)
            ShowDropDownMenu();

        else if (kevent.KeyCode.Equals(Keys.Space) && kevent.Modifiers == Keys.None)
            State = PushButtonState.Pressed;

        base.OnKeyDown(kevent);
    }

    protected override void OnKeyUp(KeyEventArgs kevent)
    {
        #region Sanity checks
        if (kevent == null) throw new ArgumentNullException(nameof(kevent));
        #endregion

        if (kevent.KeyCode.Equals(Keys.Space))
        {
            if (MouseButtons == MouseButtons.None)
                State = PushButtonState.Normal;
        }
        else if (kevent.KeyCode.Equals(Keys.Apps))
        {
            if (MouseButtons == MouseButtons.None && !_isDropDownMenuVisible)
                ShowDropDownMenu();
        }

        base.OnKeyUp(kevent);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        State = Enabled ? PushButtonState.Normal : PushButtonState.Disabled;

        base.OnEnabledChanged(e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        if (!State.Equals(PushButtonState.Pressed) && !State.Equals(PushButtonState.Disabled))
            State = PushButtonState.Normal;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        if (!State.Equals(PushButtonState.Pressed) && !State.Equals(PushButtonState.Disabled))
            State = PushButtonState.Hot;
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        if (!State.Equals(PushButtonState.Pressed) && !State.Equals(PushButtonState.Disabled))
            State = Focused ? PushButtonState.Default : PushButtonState.Normal;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        #region Sanity checks
        if (e == null) throw new ArgumentNullException(nameof(e));
        #endregion

        if (EffectiveDropDownRectangle.Contains(e.Location) && !_isDropDownMenuVisible && e.Button == MouseButtons.Left)
            ShowDropDownMenu();
        else
            State = PushButtonState.Pressed;
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        #region Sanity checks
        if (mevent == null) throw new ArgumentNullException(nameof(mevent));
        #endregion

        // if the right button was released inside the button
        if (mevent.Button == MouseButtons.Right && ClientRectangle.Contains(mevent.Location) && !_isDropDownMenuVisible)
            ShowDropDownMenu();
        else if (_dropDownMenuStrip == null || !_isDropDownMenuVisible)
        {
            SetButtonDrawState();

            if (_showSplit && ClientRectangle.Contains(mevent.Location) && !_dropDownRectangle.Contains(mevent.Location))
                OnClick(new EventArgs());
        }
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        #region Sanity checks
        if (pevent == null) throw new ArgumentNullException(nameof(pevent));
        #endregion

        base.OnPaint(pevent);

        Graphics g = pevent.Graphics;
        Rectangle bounds = ClientRectangle;

        // draw the button background as according to the current state.
        if (State != PushButtonState.Pressed && IsDefault && !Application.RenderWithVisualStyles)
        {
            Rectangle backgroundBounds = bounds;
            backgroundBounds.Inflate(-1, -1);
            ButtonRenderer.DrawButton(g, backgroundBounds, State);

            // button renderer doesnt draw the black frame when themes are off
            g.DrawRectangle(SystemPens.WindowFrame, 0, 0, bounds.Width - 1, bounds.Height - 1);
        }
        else
            ButtonRenderer.DrawButton(g, bounds, State);

        // calculate the current dropdown rectangle.
        _dropDownRectangle = new Rectangle(bounds.Right - SplitSectionWidth, 0, SplitSectionWidth, bounds.Height);

        int internalBorder = _borderSize;
        var focusRect =
            new Rectangle(internalBorder - 1,
                internalBorder - 1,
                bounds.Width - (_showSplit ? _dropDownRectangle.Width : 2) - internalBorder,
                bounds.Height - (internalBorder * 2) + 2);

        if (RightToLeft == RightToLeft.Yes && _showSplit)
        {
            _dropDownRectangle.X = bounds.Left + 1;
            focusRect.X = _dropDownRectangle.Right;

            if (IsSplitLineVisible)
            {
                // draw two lines at the edge of the dropdown button
                g.DrawLine(SystemPens.ButtonShadow, bounds.Left + SplitSectionWidth, _borderSize, bounds.Left + SplitSectionWidth, bounds.Bottom - _borderSize);
                g.DrawLine(SystemPens.ButtonFace, bounds.Left + SplitSectionWidth + 1, _borderSize, bounds.Left + SplitSectionWidth + 1, bounds.Bottom - _borderSize);
            }
        }
        else
        {
            if (IsSplitLineVisible)
            {
                // draw two lines at the edge of the dropdown button
                g.DrawLine(SystemPens.ButtonShadow, bounds.Right - SplitSectionWidth, _borderSize, bounds.Right - SplitSectionWidth, bounds.Bottom - _borderSize);
                g.DrawLine(SystemPens.ButtonFace, bounds.Right - SplitSectionWidth - 1, _borderSize, bounds.Right - SplitSectionWidth - 1, bounds.Bottom - _borderSize);
            }
        }

        // Draw an arrow in the correct location
        PaintArrow(g, _dropDownRectangle);

        //paint the image and text in the "button" part of the splitButton
        PaintTextAndImage(g, new Rectangle(0, 0, ClientRectangle.Width - SplitSectionWidth, ClientRectangle.Height));

        // draw the focus rectangle.
        if (State != PushButtonState.Pressed && Focused && ShowFocusCues)
            ControlPaint.DrawFocusRectangle(g, focusRect);
    }

#if NETFRAMEWORK
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
#endif
    protected override void WndProc(ref Message m)
    {
        //0x0212 == WM_EXITMENULOOP
        if (m.Msg == 0x0212)
        {
            //this message is only sent when a ContextMenu is closed (not a ContextMenuStrip)
            _isDropDownMenuVisible = false;
            SetButtonDrawState();
        }

        base.WndProc(ref m);
    }
    #endregion

    #region Paint
    private void PaintTextAndImage(Graphics g, Rectangle bounds)
    {
        // Figure out where our text and image should go
        CalculateButtonTextAndImageLayout(ref bounds, out var textRectangle, out var imageRectangle);

        //draw the image
        if (Image != null)
        {
            if (Enabled)
                g.DrawImage(Image, imageRectangle.X, imageRectangle.Y, Image.Width, Image.Height);
            else
                ControlPaint.DrawImageDisabled(g, Image, imageRectangle.X, imageRectangle.Y, BackColor);
        }

        // If we don't use mnemonic, set formatFlag to NoPrefix as this will show ampersand.
        if (!UseMnemonic)
            _textFormatFlags = _textFormatFlags | TextFormatFlags.NoPrefix;
        else if (!ShowKeyboardCues)
            _textFormatFlags = _textFormatFlags | TextFormatFlags.HidePrefix;

        //draw the text
        if (!string.IsNullOrEmpty(Text))
        {
            if (Enabled)
                TextRenderer.DrawText(g, Text, Font, textRectangle, ForeColor, _textFormatFlags);
            else
                ControlPaint.DrawStringDisabled(g, Text, Font, BackColor, textRectangle, _textFormatFlags);
        }
    }

    private void PaintArrow(Graphics g, Rectangle dropDownRect)
    {
        var middle = new Point(Convert.ToInt32(dropDownRect.Left + dropDownRect.Width / 2), Convert.ToInt32(dropDownRect.Top + dropDownRect.Height / 2));

        //if the width is odd - favor pushing it over one pixel right.
        middle.X += (dropDownRect.Width % 2);

        var arrow = new[] {new Point(middle.X - 2, middle.Y - 1), new Point(middle.X + 3, middle.Y - 1), new Point(middle.X, middle.Y + 2)};

        g.FillPolygon(Enabled ? SystemBrushes.ControlText : SystemBrushes.ButtonShadow, arrow);
    }

    private void SetButtonDrawState()
    {
        if (Parent != null && Bounds.Contains(Parent.PointToClient(Cursor.Position)))
            State = PushButtonState.Hot;
        else if (Focused)
            State = PushButtonState.Default;
        else if (!Enabled)
            State = PushButtonState.Disabled;
        else
            State = PushButtonState.Normal;
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        Size preferredSize = base.GetPreferredSize(proposedSize);

        //autosize correctly for splitbuttons
        if (AutoSize)
            return CalculateButtonAutoSize();

        if (!string.IsNullOrEmpty(Text) && TextRenderer.MeasureText(Text, Font).Width + SplitSectionWidth > preferredSize.Width)
            return preferredSize + new Size(SplitSectionWidth + _borderSize * 2, 0);

        return preferredSize;
    }

    private Size CalculateButtonAutoSize()
    {
        Size retSize = Size.Empty;
        Size textSize = TextRenderer.MeasureText(Text, Font);
        Size imageSize = Image?.Size ?? Size.Empty;

        // Pad the text size
        if (Text.Length != 0)
        {
            textSize.Height += 4;
            textSize.Width += 4;
        }

        switch (TextImageRelation)
        {
            case TextImageRelation.Overlay:
                retSize.Height = Math.Max(Text.Length == 0 ? 0 : textSize.Height, imageSize.Height);
                retSize.Width = Math.Max(textSize.Width, imageSize.Width);
                break;
            case TextImageRelation.ImageAboveText:
            case TextImageRelation.TextAboveImage:
                retSize.Height = textSize.Height + imageSize.Height;
                retSize.Width = Math.Max(textSize.Width, imageSize.Width);
                break;
            case TextImageRelation.ImageBeforeText:
            case TextImageRelation.TextBeforeImage:
                retSize.Height = Math.Max(textSize.Height, imageSize.Height);
                retSize.Width = textSize.Width + imageSize.Width;
                break;
        }

        // Pad the result
        retSize.Height += (Padding.Vertical + 6);
        retSize.Width += (Padding.Horizontal + 6);

        //pad the splitButton arrow region
        retSize.Width += SplitSectionWidth;

        return retSize;
    }
    #endregion

    #region DropDownMenu
    public void ShowDropDownMenu()
    {
        if (_skipNextOpen)
        {
            // we were called because we're closing the context menu strip
            // when clicking the dropdown button.
            _skipNextOpen = false;
            return;
        }

        State = PushButtonState.Pressed;

        _dropDownMenuStrip?.Show(this, new Point(0, Height), ToolStripDropDownDirection.BelowRight);
    }

    private void DropDownMenuStrip_Opening(object? sender, CancelEventArgs e) => _isDropDownMenuVisible = true;

    private void DropDownMenuStrip_Closing(object? sender, ToolStripDropDownClosingEventArgs e)
    {
        _isDropDownMenuVisible = false;

        SetButtonDrawState();

        if (e.CloseReason == ToolStripDropDownCloseReason.AppClicked)
            _skipNextOpen = (EffectiveDropDownRectangle.Contains(PointToClient(Cursor.Position))) && MouseButtons == MouseButtons.Left;
    }
    #endregion

    #region Button Layout Calculations
    //The following layout functions were taken from Mono's Windows.Forms
    //implementation, specifically "ThemeWin32Classic.cs",
    //then modified to fit the context of this splitButton

    private void CalculateButtonTextAndImageLayout(ref Rectangle contentRect, out Rectangle textRectangle, out Rectangle imageRectangle)
    {
        Size textSize = TextRenderer.MeasureText(Text, Font, contentRect.Size, _textFormatFlags);
        Size imageSize = Image?.Size ?? Size.Empty;

        textRectangle = Rectangle.Empty;
        imageRectangle = Rectangle.Empty;

        switch (TextImageRelation)
        {
            case TextImageRelation.Overlay:
                // Overlay is easy, text always goes here
                textRectangle = OverlayObjectRect(ref contentRect, ref textSize, TextAlign); // Rectangle.Inflate(content_rect, -4, -4);

                //Offset on Windows 98 style when button is pressed
                if (_state == PushButtonState.Pressed && !Application.RenderWithVisualStyles)
                    textRectangle.Offset(1, 1);

                // Image is dependent on ImageAlign
                if (Image != null)
                    imageRectangle = OverlayObjectRect(ref contentRect, ref imageSize, ImageAlign);

                break;
            case TextImageRelation.ImageAboveText:
                contentRect.Inflate(-4, -4);
                LayoutTextAboveOrBelowImage(contentRect, false, textSize, imageSize, out textRectangle, out imageRectangle);
                break;
            case TextImageRelation.TextAboveImage:
                contentRect.Inflate(-4, -4);
                LayoutTextAboveOrBelowImage(contentRect, true, textSize, imageSize, out textRectangle, out imageRectangle);
                break;
            case TextImageRelation.ImageBeforeText:
                contentRect.Inflate(-4, -4);
                LayoutTextBeforeOrAfterImage(contentRect, false, textSize, imageSize, out textRectangle, out imageRectangle);
                break;
            case TextImageRelation.TextBeforeImage:
                contentRect.Inflate(-4, -4);
                LayoutTextBeforeOrAfterImage(contentRect, true, textSize, imageSize, out textRectangle, out imageRectangle);
                break;
        }
    }

    private static Rectangle OverlayObjectRect(ref Rectangle container, ref Size sizeOfObject, ContentAlignment alignment)
    {
        int x, y;

        switch (alignment)
        {
            case ContentAlignment.TopLeft:
                x = 4;
                y = 4;
                break;
            case ContentAlignment.TopCenter:
                x = (container.Width - sizeOfObject.Width) / 2;
                y = 4;
                break;
            case ContentAlignment.TopRight:
                x = container.Width - sizeOfObject.Width - 4;
                y = 4;
                break;
            case ContentAlignment.MiddleLeft:
                x = 4;
                y = (container.Height - sizeOfObject.Height) / 2;
                break;
            case ContentAlignment.MiddleCenter:
                x = (container.Width - sizeOfObject.Width) / 2;
                y = (container.Height - sizeOfObject.Height) / 2;
                break;
            case ContentAlignment.MiddleRight:
                x = container.Width - sizeOfObject.Width - 4;
                y = (container.Height - sizeOfObject.Height) / 2;
                break;
            case ContentAlignment.BottomLeft:
                x = 4;
                y = container.Height - sizeOfObject.Height - 4;
                break;
            case ContentAlignment.BottomCenter:
                x = (container.Width - sizeOfObject.Width) / 2;
                y = container.Height - sizeOfObject.Height - 4;
                break;
            case ContentAlignment.BottomRight:
                x = container.Width - sizeOfObject.Width - 4;
                y = container.Height - sizeOfObject.Height - 4;
                break;
            default:
                x = 4;
                y = 4;
                break;
        }

        return new Rectangle(x, y, sizeOfObject.Width, sizeOfObject.Height);
    }

    private void LayoutTextBeforeOrAfterImage(Rectangle totalArea, bool textFirst, Size textSize, Size imageSize, out Rectangle textRect, out Rectangle imageRect)
    {
        int elementSpacing = 0; // Spacing between the Text and the Image
        int totalWidth = textSize.Width + elementSpacing + imageSize.Width;

        if (!textFirst)
            elementSpacing += 2;

        // If the text is too big, chop it down to the size we have available to it
        if (totalWidth > totalArea.Width)
        {
            textSize.Width = totalArea.Width - elementSpacing - imageSize.Width;
            totalWidth = totalArea.Width;
        }

        int excessWidth = totalArea.Width - totalWidth;
        int offset = 0;

        Rectangle finalTextRect;
        Rectangle finalImageRect;

        HorizontalAlignment hText = GetHorizontalAlignment(TextAlign);
        HorizontalAlignment hImage = GetHorizontalAlignment(ImageAlign);

        switch (hImage)
        {
            case HorizontalAlignment.Left:
                offset = 0;
                break;
            case HorizontalAlignment.Right when hText == HorizontalAlignment.Right:
                offset = excessWidth;
                break;
            case HorizontalAlignment.Center when (hText == HorizontalAlignment.Left || hText == HorizontalAlignment.Center):
                offset += excessWidth / 3;
                break;
            default:
                offset += 2 * (excessWidth / 3);
                break;
        }

        if (textFirst)
        {
            finalTextRect = new Rectangle(totalArea.Left + offset, AlignInRectangle(totalArea, textSize, TextAlign).Top, textSize.Width, textSize.Height);
            finalImageRect = new Rectangle(finalTextRect.Right + elementSpacing, AlignInRectangle(totalArea, imageSize, ImageAlign).Top, imageSize.Width, imageSize.Height);
        }
        else
        {
            finalImageRect = new Rectangle(totalArea.Left + offset, AlignInRectangle(totalArea, imageSize, ImageAlign).Top, imageSize.Width, imageSize.Height);
            finalTextRect = new Rectangle(finalImageRect.Right + elementSpacing, AlignInRectangle(totalArea, textSize, TextAlign).Top, textSize.Width, textSize.Height);
        }

        textRect = finalTextRect;
        imageRect = finalImageRect;
    }

    private void LayoutTextAboveOrBelowImage(Rectangle totalArea, bool textFirst, Size textSize, Size imageSize, out Rectangle textRect, out Rectangle imageRect)
    {
        int elementSpacing = 0; // Spacing between the Text and the Image
        int totalHeight = textSize.Height + elementSpacing + imageSize.Height;

        if (textFirst)
            elementSpacing += 2;

        if (textSize.Width > totalArea.Width)
            textSize.Width = totalArea.Width;

        // If the there isn't enough room and we're text first, cut out the image
        if (totalHeight > totalArea.Height && textFirst)
        {
            imageSize = Size.Empty;
            totalHeight = totalArea.Height;
        }

        int excessHeight = totalArea.Height - totalHeight;
        int offset = 0;

        Rectangle finalTextRect;
        Rectangle finalImageRect;

        VerticalAlignment vText = GetVerticalAlignment(TextAlign);
        VerticalAlignment vImage = GetVerticalAlignment(ImageAlign);

        switch (vImage)
        {
            case VerticalAlignment.Top:
                offset = 0;
                break;
            case VerticalAlignment.Bottom when vText == VerticalAlignment.Bottom:
                offset = excessHeight;
                break;
            case VerticalAlignment.Center when (vText == VerticalAlignment.Top || vText == VerticalAlignment.Center):
                offset += excessHeight / 3;
                break;
            default:
                offset += 2 * (excessHeight / 3);
                break;
        }

        if (textFirst)
        {
            finalTextRect = new Rectangle(AlignInRectangle(totalArea, textSize, TextAlign).Left, totalArea.Top + offset, textSize.Width, textSize.Height);
            finalImageRect = new Rectangle(AlignInRectangle(totalArea, imageSize, ImageAlign).Left, finalTextRect.Bottom + elementSpacing, imageSize.Width, imageSize.Height);
        }
        else
        {
            finalImageRect = new Rectangle(AlignInRectangle(totalArea, imageSize, ImageAlign).Left, totalArea.Top + offset, imageSize.Width, imageSize.Height);
            finalTextRect = new Rectangle(AlignInRectangle(totalArea, textSize, TextAlign).Left, finalImageRect.Bottom + elementSpacing, textSize.Width, textSize.Height);

            if (finalTextRect.Bottom > totalArea.Bottom)
                finalTextRect.Y = totalArea.Top;
        }

        textRect = finalTextRect;
        imageRect = finalImageRect;
    }

    private static HorizontalAlignment GetHorizontalAlignment(ContentAlignment align)
    {
        switch (align)
        {
            case ContentAlignment.BottomLeft:
            case ContentAlignment.MiddleLeft:
            case ContentAlignment.TopLeft:
                return HorizontalAlignment.Left;
            case ContentAlignment.BottomCenter:
            case ContentAlignment.MiddleCenter:
            case ContentAlignment.TopCenter:
                return HorizontalAlignment.Center;
            case ContentAlignment.BottomRight:
            case ContentAlignment.MiddleRight:
            case ContentAlignment.TopRight:
                return HorizontalAlignment.Right;
        }

        return HorizontalAlignment.Left;
    }

    private static VerticalAlignment GetVerticalAlignment(ContentAlignment align)
    {
        switch (align)
        {
            case ContentAlignment.TopLeft:
            case ContentAlignment.TopCenter:
            case ContentAlignment.TopRight:
                return VerticalAlignment.Top;
            case ContentAlignment.MiddleLeft:
            case ContentAlignment.MiddleCenter:
            case ContentAlignment.MiddleRight:
                return VerticalAlignment.Center;
            case ContentAlignment.BottomLeft:
            case ContentAlignment.BottomCenter:
            case ContentAlignment.BottomRight:
                return VerticalAlignment.Bottom;
        }

        return VerticalAlignment.Top;
    }

    private static Rectangle AlignInRectangle(Rectangle outer, Size inner, ContentAlignment align)
    {
        int x = 0;
        int y = 0;

        switch (align)
        {
            case ContentAlignment.TopLeft:
            case ContentAlignment.MiddleLeft:
            case ContentAlignment.BottomLeft:
                x = outer.X;
                break;
            case ContentAlignment.TopCenter:
            case ContentAlignment.MiddleCenter:
            case ContentAlignment.BottomCenter:
                x = Math.Max(outer.X + ((outer.Width - inner.Width) / 2), outer.Left);
                break;
            case ContentAlignment.TopRight:
            case ContentAlignment.MiddleRight:
            case ContentAlignment.BottomRight:
                x = outer.Right - inner.Width;
                break;
        }
        switch (align)
        {
            case ContentAlignment.TopRight:
            case ContentAlignment.TopLeft:
            case ContentAlignment.TopCenter:
                y = outer.Y;
                break;
            case ContentAlignment.MiddleRight:
            case ContentAlignment.MiddleLeft:
            case ContentAlignment.MiddleCenter:
                y = outer.Y + (outer.Height - inner.Height) / 2;
                break;
            case ContentAlignment.BottomLeft:
            case ContentAlignment.BottomRight:
            case ContentAlignment.BottomCenter:
                y = outer.Bottom - inner.Height;
                break;
        }

        return new Rectangle(x, y, Math.Min(inner.Width, outer.Width), Math.Min(inner.Height, outer.Height));
    }
    #endregion Button Layout Calculations
}
