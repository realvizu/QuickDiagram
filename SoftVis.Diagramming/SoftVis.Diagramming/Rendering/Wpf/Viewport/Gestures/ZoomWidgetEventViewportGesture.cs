using Codartis.SoftVis.Rendering.Common.UIEvents;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures
{
    /// <summary>
    /// Calculates viewport changes when zooming with UI zoom control.
    /// </summary>
    internal class ZoomWidgetEventViewportGesture : WidgetEventViewportGestureBase
    {
        public ZoomWidgetEventViewportGesture(IDiagramViewport diagramViewport, IWidgetEventSource widgetEventSource)
            : base(diagramViewport, widgetEventSource)
        {
            if (WidgetEventSource != null)
                WidgetEventSource.Zoom += OnZoom;
        }

        private void OnZoom(object sender, ZoomEventArgs e)
        {
            ZoomViewportTo(e.NewZoomValue);
        }
    }
}
