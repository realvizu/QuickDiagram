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

        private const double _minScale = .3;
        private const double _maxScale = 3;

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

        protected void ZoomTo(Point zoomCenter, double zoomPercent)
        {
            var newScale = zoomPercent * _maxScale + _minScale;
            var newTranslate = (Target.Translate + (Vector)zoomCenter) * (newScale / Target.Scale) - (Vector)zoomCenter;
            OnZoomChanged(newScale, newTranslate);
        }

        protected bool ZoomBy(Point zoomCenter, ZoomDirection zoomDirection, double zoomAmount)
        {
            var zoomLimitReached = false;

            var zoomSign = zoomDirection == ZoomDirection.In ? 1 : -1;
            var scaleFactor = Math.Pow(2, zoomSign * zoomAmount / 50);
            var newScale = Target.Scale * scaleFactor;

            if (newScale <_minScale)
            {
                newScale = _minScale;
                zoomLimitReached = true;
            }
            else if (newScale > _maxScale)
            {
                newScale = _maxScale;
                zoomLimitReached = true;
            }

            var newTranslate = (Target.Translate + (Vector)zoomCenter) * (newScale/Target.Scale) - (Vector)zoomCenter;

            OnZoomChanged(newScale, newTranslate);

            return zoomLimitReached;
        }

        private void OnTranslateChanged(Vector translate)
        {
            if (TranslateChanged != null)
                TranslateChanged(this, new TranslateChangedEventArgs(translate));
        }

        private void OnZoomChanged(double scale, Vector translate)
        {
            if (ScaleChanged != null)
                ScaleChanged(this, new ScaleChangedEventArgs(scale));

            if (TranslateChanged != null)
                TranslateChanged(this, new TranslateChangedEventArgs(translate));
        }

        protected enum ZoomDirection
        {
            In,
            Out
        }
    }
}
