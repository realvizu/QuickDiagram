using System;
using System.Windows;
using Codartis.SoftVis.Rendering.Common.UIEvents;

namespace Codartis.SoftVis.Rendering.Wpf.Common.UIEvents
{
    /// <summary>
    /// Events and properties for UI input processing (mouse, keyboard, widget and window events).
    /// </summary>
    internal interface IUIEventSource : IInputElement
    {
        event PanEventHandler PanWidget;
        event ZoomEventHandler ZoomWidget;
        event EventHandler FitToViewWidget;
        event ResizeEventHandler WindowResized;
    }
}
