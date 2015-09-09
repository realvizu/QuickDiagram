using System.Windows;
using System.Windows.Input;
using Codartis.SoftVis.Rendering.Wpf.Common.UIEvents;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Gestures
{
    /// <summary>
    /// Calculates viewport changes when panning with mouse.
    /// </summary>
    internal class MousePanViewportGesture : ViewportGestureBase
    {
        private Point _lastMousePosition;
        private bool _isPanning;
        private Cursor _cursorBeforePanning;

        internal MousePanViewportGesture(IDiagramViewport diagramViewport, IUIEventSource uiEventSource)
            : base(diagramViewport, uiEventSource)
        {
            UIEventSource.MouseLeftButtonDown += OnMouseLeftButtonDown;
            UIEventSource.MouseLeftButtonUp += OnMouseLeftButtonUp;
            UIEventSource.MouseMove += OnMouseMove;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isPanning)
            {
                _isPanning = true;
                _cursorBeforePanning = DiagramViewport.Cursor;
                DiagramViewport.Cursor = Cursors.Hand;
                Mouse.Capture(UIEventSource);
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
            var position = e.GetPosition(UIEventSource);
            if (_isPanning)
            {
                MoveViewportCenterInScreenSpaceBy(_lastMousePosition - position);
            }
            _lastMousePosition = position;
        }
    }
}
