using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Codartis.SoftVis.Rendering.Wpf.Gestures
{
    /// <summary>
    /// Calculates scale and translate changes when zooming with mouse.
    /// </summary>
    internal class MouseZoomGesture : PanAndZoomGestureBase
    {
        private const double WheelClicksPerZoomRange = 5;
        private readonly double ZoomPerWheelClick;

        public MouseZoomGesture(IGestureTarget gestureTarget)
            : base(gestureTarget)
        {
            ZoomPerWheelClick = (Target.MaxScale - Target.MinScale) / WheelClicksPerZoomRange;
            Target.MouseWheel += OnMouseWheel;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var zoomDirection = e.Delta < 0 ? ZoomDirection.Out : ZoomDirection.In;
            if (!IsZoomLimitReached(zoomDirection))
            {
                var zoomAmount = Math.Abs(e.Delta / Mouse.MouseWheelDeltaForOneLine) * ZoomPerWheelClick;
                ZoomBy(e.GetPosition(Target), zoomDirection, zoomAmount);
            }
        }
    }
}
