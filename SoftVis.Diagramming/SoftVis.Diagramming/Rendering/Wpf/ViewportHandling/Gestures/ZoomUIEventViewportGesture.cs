using Codartis.SoftVis.Rendering.Common.UIEvents;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Gestures
{
    /// <summary>
    /// Calculates viewport changes when zooming with UI zoom control.
    /// </summary>
    internal class ZoomUIEventViewportGesture : UIEventViewportGestureBase
    {
        public ZoomUIEventViewportGesture(IViewportHost viewportHost)
            : base(viewportHost)
        {
            if (UIEventSource != null)
                UIEventSource.Zoom += OnZoom;
        }

        private void OnZoom(object sender, ZoomEventArgs e)
        {
            ZoomViewportTo(e.NewZoomValue);
        }
    }
}
