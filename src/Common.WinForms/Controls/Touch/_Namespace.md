---
uid: NanoByte.Common.Controls.Touch
summary: Provides support for multi-touch input in WinForms.
---
Use <xref:NanoByte.Common.Controls.Touch.TouchInputProvider> to process multi-touch input in an existing WinForms control. You will need to override your control's `WndProc` method and forward messages to <xref:NanoByte.Common.Controls.Touch.TouchInputProvider.ProcessMessage>. 

Alternatively, you can use <xref:NanoByte.Common.Controls.Touch.TouchForm> or <xref:NanoByte.Common.Controls.Touch.TouchPanel>, which handle the message routing for you.
