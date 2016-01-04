using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Codartis.SoftVis.UI.Wpf.Common.UIEvents;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Viewport.Viewing.Gestures
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
            diagramViewport.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            diagramViewport.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            diagramViewport.PreviewMouseMove += OnMouseMove;
            diagramViewport.LostMouseCapture += OnLostMouseCapture;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("OnMouseLeftButtonDown");
            if (!_isPanning)
            {
                _isPanning = true;
                _cursorBeforePanning = DiagramViewport.Cursor;
                DiagramViewport.Cursor = Cursors.Hand;
                Mouse.Capture(DiagramViewport);
                Debug.WriteLine("Start panning, mouse captured");
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("OnMouseLeftButtonUp");
            if (_isPanning)
            {
                _isPanning = false;
                DiagramViewport.Cursor = _cursorBeforePanning;
                Mouse.Capture(null);
                Debug.WriteLine("Finished panning, mouse released");
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(DiagramViewport);
            if (_isPanning && _lastMousePosition != position)
            {
                MoveViewportCenterInScreenSpaceBy(_lastMousePosition - position);
            }
            _lastMousePosition = position;
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            _isPanning = false;
            DiagramViewport.Cursor = _cursorBeforePanning;
        }
    }
}
