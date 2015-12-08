using System;
using System.Linq;
using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.Common.UIEvents;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Viewing.Gestures
{
    /// <summary>
    /// Calculates viewport changes when pushing the fit-to-view button on the UI control.
    /// </summary>
    internal class FitToViewViewportGesture : ViewportGestureBase
    {
        internal FitToViewViewportGesture(IDiagramViewport diagramViewport, IUIEventSource uiEventSource)
            : base(diagramViewport, uiEventSource)
        {
            UIEventSource.FitToViewWidget += OnFitToView;
        }

        private void OnFitToView(object sender, EventArgs e)
        {
            var newZoom = CalculateZoom();
            var contentCenter = DiagramViewport.ContentInDiagramSpace.GetCenter();
            if (!double.IsNaN(newZoom) && !contentCenter.IsExtreme())
            {
                ZoomViewportTo(newZoom);
                MoveViewportCenterInDiagramSpaceTo(contentCenter);
            }
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
