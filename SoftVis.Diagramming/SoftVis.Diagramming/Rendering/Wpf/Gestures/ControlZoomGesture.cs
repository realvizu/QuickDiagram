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
    /// Calculates scale and translate changes when zooming with zoom control.
    /// </summary>
    internal class ControlZoomGesture : PanAndZoomGestureBase
    {
        public ControlZoomGesture(IGestureTarget gestureTarget)
            : base(gestureTarget)
        {
            var panAndZoomEventSource = gestureTarget as IPanAndZoomEventSource;
            if (panAndZoomEventSource != null)
            {
                panAndZoomEventSource.Zoom += OnZoom;
            }
        }

        private void OnZoom(object sender, ZoomEventArgs e)
        {
            ZoomTo(GetTargetCenterPoint(), e.NewZoomPercent);
        }
    }
}
