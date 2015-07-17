using System;
using System.Linq;
using Codartis.SoftVis.Rendering.Wpf.Common;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Gestures
{
    /// <summary>
    /// Calculates viewport changes when pushing the fit-to-view button on the UI control.
    /// </summary>
    internal class FitToViewUIEventViewportGesture : UIEventViewportGestureBase
    {
        public FitToViewUIEventViewportGesture(IViewportHost viewportHost)
            : base(viewportHost)
        {
            if (UIEventSource != null)
                UIEventSource.FitToView += OnFitToView;
        }

        private void OnFitToView(object sender, EventArgs e)
        {
            var newZoom = CalculateZoom();
            var contentCenter = ViewportHost.ContentInDiagramSpace.GetCenter();
            ZoomViewportTo(newZoom);
            MoveViewportCenterInDiagramSpaceTo(contentCenter);
        }

        private double CalculateZoom()
        {
            return new[]
            {
                1.0,
                ViewportHost.ViewportInScreenSpace.Size.Width / ViewportHost.ContentInDiagramSpace.Width,
                ViewportHost.ViewportInScreenSpace.Size.Height / ViewportHost.ContentInDiagramSpace.Height,
            }.Min();
        }
    }
}
