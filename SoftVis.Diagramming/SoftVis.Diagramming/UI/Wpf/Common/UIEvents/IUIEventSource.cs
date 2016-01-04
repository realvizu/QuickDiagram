using System;
using System.Windows;
using Codartis.SoftVis.UI.Common.UIEvents;

namespace Codartis.SoftVis.UI.Wpf.Common.UIEvents
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
