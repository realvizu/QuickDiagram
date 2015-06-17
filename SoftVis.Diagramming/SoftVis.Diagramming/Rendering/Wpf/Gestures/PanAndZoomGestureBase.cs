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
    /// Calculates scale and translate changes when panning and zooming.
    /// Raises events for the new scale and translate values.
    /// </summary>
    internal abstract class PanAndZoomGestureBase : IGesture
    {
        public event ScaleChangedEventHandler ScaleChanged;
        public event TranslateChangedEventHandler TranslateChanged;

        public IGestureTarget Target { get; private set; }

        protected PanAndZoomGestureBase(IGestureTarget gestureTarget)
        {
            Target = gestureTarget;
        }

        protected Point GetTargetCenterPoint()
        {
            return new Point(Target.ActualWidth / 2, Target.ActualHeight / 2);
        }

        protected void Translate(Vector translate)
        {
            var newTranslate = Target.Translate - translate;
            OnTranslateChanged(newTranslate);
        }

        protected bool IsZoomLimitReached(ZoomDirection zoomDirection)
        {
            return (zoomDirection == ZoomDirection.In && Target.LinearScale >= Target.MaxScale) ||
                  (zoomDirection == ZoomDirection.Out && Target.LinearScale <= Target.MinScale);
        }

        protected void ZoomTo(Point zoomCenter, double newScale)
        {
            OnZoomChanged(newScale, zoomCenter);
        }

        protected void ZoomBy(Point zoomCenter, ZoomDirection zoomDirection, double zoomAmount)
        {
            var zoomSign = zoomDirection == ZoomDirection.In ? 1 : -1;
            var newScale = Target.LinearScale + (zoomAmount * zoomSign);

            if (newScale < Target.MinScale)
            {
                newScale = Target.MinScale;
            }
            else if (newScale > Target.MaxScale)
            {
                newScale = Target.MaxScale;
            }

            OnZoomChanged(newScale, zoomCenter);
        }

        private void OnTranslateChanged(Vector translate)
        {
            if (TranslateChanged != null)
                TranslateChanged(this, new TranslateChangedEventArgs(translate));
        }

        private void OnZoomChanged(double newScale, Point center)
        {
            if (ScaleChanged != null)
                ScaleChanged(this, new ScaleChangedEventArgs(newScale, center));
        }

        protected enum ZoomDirection
        {
            In,
            Out
        }
    }
}
