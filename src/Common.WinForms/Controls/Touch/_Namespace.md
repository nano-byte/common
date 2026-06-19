---
uid: NanoByte.Common.Controls.Touch
summary: Provides support for multi-touch input in WinForms.
---
To use <xref:NanoByte.Common.Controls.Touch.TouchEventSource>:

- Override your control's `WndProc` method and forward messages to <xref:NanoByte.Common.Controls.Touch.TouchEventSource.ProcessMessage>.
- Register event handlers for the <xref:NanoByte.Common.Controls.Touch.TouchEventSource.ManipulationStarted>, <xref:NanoByte.Common.Controls.Touch.TouchEventSource.ManipulationUpdated> and <xref:NanoByte.Common.Controls.Touch.TouchEventSource.ManipulationCompleted> events. Taps are reported as regular mouse clicks.

Alternatively, you can use <xref:NanoByte.Common.Controls.Touch.TouchForm> or <xref:NanoByte.Common.Controls.Touch.TouchPanel>, which handle the message routing for you and expose the same set of events.
