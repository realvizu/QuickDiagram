using Codartis.SoftVis.UI.Common.UIEvents;
using Codartis.SoftVis.UI.Wpf.Common.UIEvents;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Viewport.Viewing.Gestures
{
    /// <summary>
    /// Calculates viewport changes when zooming with UI zoom control.
    /// </summary>
    internal class ZoomWidgetViewportGesture : ViewportGestureBase
    {
        internal ZoomWidgetViewportGesture(IDiagramViewport diagramViewport, IUIEventSource uiEventSource)
            : base(diagramViewport, uiEventSource)
        {
            UIEventSource.ZoomWidget += OnZoom;
        }

        private void OnZoom(object sender, ZoomEventArgs e)
        {
            ZoomViewportTo(e.NewZoomValue);
        }
    }
}
