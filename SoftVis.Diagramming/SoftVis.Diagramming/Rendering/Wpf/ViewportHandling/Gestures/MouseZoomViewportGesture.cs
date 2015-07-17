using System;
using System.Windows.Input;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Gestures
{
    /// <summary>
    /// Calculates viewport changes when zooming with mouse.
    /// </summary>
    internal class MouseZoomViewportGesture : ViewportGestureBase
    {
        private const double WheelClicksPerZoomRange = 5;
        private readonly double ZoomPerWheelClick;

        public MouseZoomViewportGesture(IViewportHost viewportHost)
            : base(viewportHost)
        {
            ZoomPerWheelClick = (ViewportHost.MaxZoom - ViewportHost.MinZoom) / WheelClicksPerZoomRange;
            ViewportHost.MouseWheel += OnMouseWheel;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var zoomDirection = e.Delta < 0 ? ZoomDirection.Out : ZoomDirection.In;
            if (!IsZoomLimitReached(zoomDirection))
            {
                var zoomAmount = Math.Abs(e.Delta / Mouse.MouseWheelDeltaForOneLine) * ZoomPerWheelClick;
                ZoomViewportWithCenterInScreenSpaceBy(zoomDirection, zoomAmount, e.GetPosition(ViewportHost));
            }
        }
    }
}
