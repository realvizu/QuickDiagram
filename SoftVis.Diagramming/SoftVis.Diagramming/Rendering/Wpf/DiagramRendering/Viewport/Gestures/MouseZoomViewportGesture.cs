using System;
using System.Windows;
using System.Windows.Input;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Gestures
{
    /// <summary>
    /// Calculates viewport changes when zooming with mouse.
    /// </summary>
    internal class MouseZoomViewportGesture : InputEventViewportGestureBase
    {
        private const double WheelClicksPerZoomRange = 10;
        private readonly double _zoomPerWheelClick;

        public MouseZoomViewportGesture(IDiagramViewport diagramViewport, IInputElement inputElement)
            : base(diagramViewport, inputElement)
        {
            _zoomPerWheelClick = (DiagramViewport.MaxZoom - DiagramViewport.MinZoom) / WheelClicksPerZoomRange;
            InputElement.MouseWheel += OnMouseWheel;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var zoomDirection = e.Delta < 0 ? ZoomDirection.Out : ZoomDirection.In;
            if (!IsZoomLimitReached(zoomDirection))
            {
                var zoomAmount = Math.Abs(e.Delta / Mouse.MouseWheelDeltaForOneLine) * _zoomPerWheelClick;
                ZoomViewportWithCenterInScreenSpaceBy(zoomDirection, zoomAmount, e.GetPosition(InputElement));
            }
        }
    }
}
