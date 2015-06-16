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
        public MouseZoomGesture(IGestureTarget gestureTarget)
            : base(gestureTarget)
        {
            Target.MouseWheel += OnMouseWheel;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollAmount = Math.Abs(e.Delta / 3d);
            var zoomDirection = e.Delta < 0 ? ZoomDirection.Out : ZoomDirection.In;
            ZoomBy(e.GetPosition(Target), zoomDirection, scrollAmount);
        }
    }
}
