using System;

namespace Codartis.SoftVis.Rendering.Common.UIEvents
{
    internal interface IWidgetEventSource
    {
        event PanEventHandler Pan;
        event ZoomEventHandler Zoom;
        event EventHandler FitToView;
        event ResizeEventHandler Resize;
    }
}
