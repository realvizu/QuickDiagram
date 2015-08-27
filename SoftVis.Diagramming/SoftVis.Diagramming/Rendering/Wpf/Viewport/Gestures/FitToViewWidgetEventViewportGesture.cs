using Codartis.SoftVis.Rendering.Common.UIEvents;
using Codartis.SoftVis.Rendering.Wpf.Common;
using System;
using System.Linq;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures
{
    /// <summary>
    /// Calculates viewport changes when pushing the fit-to-view button on the UI control.
    /// </summary>
    internal class FitToViewWidgetEventViewportGesture : WidgetEventViewportGestureBase
    {
        public FitToViewWidgetEventViewportGesture(IViewport viewport, IWidgetEventSource widgetEventSource)
            : base(viewport, widgetEventSource)
        {
            if (WidgetEventSource != null)
                WidgetEventSource.FitToView += OnFitToView;
        }

        private void OnFitToView(object sender, EventArgs e)
        {
            var newZoom = CalculateZoom();
            var contentCenter = Viewport.ContentInDiagramSpace.GetCenter();
            ZoomViewportTo(newZoom);
            MoveViewportCenterInDiagramSpaceTo(contentCenter);
        }

        private double CalculateZoom()
        {
            return new[]
            {
                1.0,
                Viewport.ViewportInScreenSpace.Size.Width / Viewport.ContentInDiagramSpace.Width,
                Viewport.ViewportInScreenSpace.Size.Height / Viewport.ContentInDiagramSpace.Height,
            }.Min();
        }
    }
}
