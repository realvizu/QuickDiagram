using System.Windows;
using System.Windows.Input;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures
{
    /// <summary>
    /// Calculates viewport changes when panning with mouse.
    /// </summary>
    internal class MousePanViewportGesture : InputEventViewportGestureBase
    {
        private Point _lastMousePosition;
        private bool _isPanning;
        private Cursor _cursorBeforePanning;

        public MousePanViewportGesture(IViewport viewport, IInputElement inputElement)
            : base(viewport, inputElement)
        {
            InputElement.MouseLeftButtonDown += OnMouseLeftButtonDown;
            InputElement.MouseLeftButtonUp += OnMouseLeftButtonUp;
            InputElement.MouseMove += OnMouseMove;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isPanning)
            {
                _isPanning = true;
                _cursorBeforePanning = Viewport.Cursor;
                Viewport.Cursor = Cursors.Hand;
                Mouse.Capture(InputElement);
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isPanning)
            {
                _isPanning = false;
                Viewport.Cursor = _cursorBeforePanning;
                Mouse.Capture(null);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(InputElement);
            if (_isPanning)
            {
                MoveViewportCenterInScreenSpaceBy(_lastMousePosition - position);
            }
            _lastMousePosition = position;
        }
    }
}
