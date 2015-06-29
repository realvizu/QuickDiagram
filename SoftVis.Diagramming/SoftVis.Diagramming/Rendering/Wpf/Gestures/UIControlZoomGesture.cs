namespace Codartis.SoftVis.Rendering.Wpf.Gestures
{
    /// <summary>
    /// Calculates scale and translate changes when zooming with UI zoom control.
    /// </summary>
    internal class UIControlZoomGesture : PanAndZoomGestureBase
    {
        public UIControlZoomGesture(IGestureTarget gestureTarget)
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
            ZoomTo(GetTargetCenterPoint(), e.NewZoomValue);
        }
    }
}
