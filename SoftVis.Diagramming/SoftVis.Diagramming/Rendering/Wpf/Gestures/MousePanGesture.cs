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
    /// Calculates translate changes when panning with mouse.
    /// </summary>
    internal class MousePanGesture : PanAndZoomGestureBase
    {
        private Point _lastMousePosition;
        private bool _isPanning;
        private Cursor _cursorBeforePanning;

        public MousePanGesture(IGestureTarget gestureTarget)
            : base(gestureTarget)
        {
            Target.MouseLeftButtonDown += OnMouseLeftButtonDown;
            Target.MouseLeftButtonUp += OnMouseLeftButtonUp;
            Target.MouseMove += OnMouseMove;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isPanning)
            {
                _cursorBeforePanning = Target.Cursor;
                _isPanning = true;
                Target.Cursor = Cursors.Hand;
                Mouse.Capture(Target);
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isPanning)
            {
                _isPanning = false;
                Target.Cursor = _cursorBeforePanning;
                Mouse.Capture(null);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(Target);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Translate((Vector)position - (Vector)_lastMousePosition);
            }
            _lastMousePosition = position;
        }
   }
}
