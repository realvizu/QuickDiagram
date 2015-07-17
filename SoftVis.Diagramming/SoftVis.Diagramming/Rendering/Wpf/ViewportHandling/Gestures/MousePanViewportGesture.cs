using System.Windows;
using System.Windows.Input;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Gestures
{
    /// <summary>
    /// Calculates viewport changes when panning with mouse.
    /// </summary>
    internal class MousePanViewportGesture : ViewportGestureBase
    {
        private Point _lastMousePosition;
        private bool _isPanning;
        private Cursor _cursorBeforePanning;

        public MousePanViewportGesture(IViewportHost viewportHost)
            : base(viewportHost)
        {
            ViewportHost.MouseLeftButtonDown += OnMouseLeftButtonDown;
            ViewportHost.MouseLeftButtonUp += OnMouseLeftButtonUp;
            ViewportHost.MouseMove += OnMouseMove;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isPanning)
            {
                _isPanning = true;
                _cursorBeforePanning = ViewportHost.Cursor;
                ViewportHost.Cursor = Cursors.Hand;
                Mouse.Capture(ViewportHost);
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isPanning)
            {
                _isPanning = false;
                ViewportHost.Cursor = _cursorBeforePanning;
                Mouse.Capture(null);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(ViewportHost);
            if (_isPanning)
            {
                MoveViewportCenterInScreenSpaceBy(_lastMousePosition - position);
            }
            _lastMousePosition = position;
        }
    }
}
