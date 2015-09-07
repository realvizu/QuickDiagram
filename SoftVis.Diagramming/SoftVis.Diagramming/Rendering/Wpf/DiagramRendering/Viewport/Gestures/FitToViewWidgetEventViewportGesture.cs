using System;
using System.Linq;
using Codartis.SoftVis.Rendering.Common.UIEvents;
using Codartis.SoftVis.Rendering.Wpf.Common;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Gestures
{
    /// <summary>
    /// Calculates viewport changes when pushing the fit-to-view button on the UI control.
    /// </summary>
    internal class FitToViewWidgetEventViewportGesture : WidgetEventViewportGestureBase
    {
        public FitToViewWidgetEventViewportGesture(IDiagramViewport diagramViewport, IWidgetEventSource widgetEventSource)
            : base(diagramViewport, widgetEventSource)
        {
            if (WidgetEventSource != null)
                WidgetEventSource.FitToView += OnFitToView;
        }

        private void OnFitToView(object sender, EventArgs e)
        {
            var newZoom = CalculateZoom();
            var contentCenter = DiagramViewport.ContentInDiagramSpace.GetCenter();
            ZoomViewportTo(newZoom);
            MoveViewportCenterInDiagramSpaceTo(contentCenter);
        }

        private double CalculateZoom()
        {
            return new[]
            {
                1.0,
                DiagramViewport.ViewportInScreenSpace.Size.Width / DiagramViewport.ContentInDiagramSpace.Width,
                DiagramViewport.ViewportInScreenSpace.Size.Height / DiagramViewport.ContentInDiagramSpace.Height,
            }.Min();
        }
    }
}
