using System.Windows;
using System.Windows.Input;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Gestures
{
    /// <summary>
    /// Calculates viewport changes when panning with mouse.
    /// </summary>
    internal class MousePanViewportGesture : InputEventViewportGestureBase
    {
        private Point _lastMousePosition;
        private bool _isPanning;
        private Cursor _cursorBeforePanning;

        public MousePanViewportGesture(IDiagramViewport diagramViewport, IInputElement inputElement)
            : base(diagramViewport, inputElement)
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
                _cursorBeforePanning = DiagramViewport.Cursor;
                DiagramViewport.Cursor = Cursors.Hand;
                Mouse.Capture(InputElement);
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isPanning)
            {
                _isPanning = false;
                DiagramViewport.Cursor = _cursorBeforePanning;
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
