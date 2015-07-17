using Codartis.SoftVis.Rendering.Common.UIEvents;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Gestures
{
    /// <summary>
    /// Calculates viewport changes when zooming with UI zoom control.
    /// </summary>
    internal class UIControlZoomViewportGesture : ViewportGestureBase
    {
        public UIControlZoomViewportGesture(IViewportHost viewportHost)
            : base(viewportHost)
        {
            var panAndZoomEventSource = viewportHost as IPanAndZoomEventSource;
            if (panAndZoomEventSource != null)
            {
                panAndZoomEventSource.Zoom += OnZoom;
            }
        }

        private void OnZoom(object sender, ZoomEventArgs e)
        {
            ZoomViewportTo(e.NewZoomValue);
        }
    }
}
