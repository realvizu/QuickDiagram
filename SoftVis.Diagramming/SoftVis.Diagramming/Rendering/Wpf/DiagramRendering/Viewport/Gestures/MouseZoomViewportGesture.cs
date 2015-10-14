using System;
using System.Windows.Input;
using Codartis.SoftVis.Rendering.Wpf.Common.UIEvents;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Gestures
{
    /// <summary>
    /// Calculates viewport changes when zooming with mouse.
    /// </summary>
    internal class MouseZoomViewportGesture : ViewportGestureBase
    {
        private const double WheelClicksPerZoomRange = 12;
        private readonly double _zoomPerWheelClick;

        internal MouseZoomViewportGesture(IDiagramViewport diagramViewport, IUIEventSource uiEventSource)
            : base(diagramViewport, uiEventSource)
        {
            _zoomPerWheelClick = (DiagramViewport.MaxZoom - DiagramViewport.MinZoom) / WheelClicksPerZoomRange;

            UIEventSource.MouseWheel += OnMouseWheel;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var zoomDirection = e.Delta < 0 ? ZoomDirection.Out : ZoomDirection.In;
            if (!IsZoomLimitReached(zoomDirection))
            {
                var zoomAmount = Math.Abs(e.Delta / Mouse.MouseWheelDeltaForOneLine) * _zoomPerWheelClick;
                ZoomViewportWithCenterInScreenSpaceBy(zoomDirection, zoomAmount, e.GetPosition(UIEventSource));
            }
        }
    }
}
