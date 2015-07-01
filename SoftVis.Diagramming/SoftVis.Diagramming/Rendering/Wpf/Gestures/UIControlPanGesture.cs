using System;
using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Gestures
{
    /// <summary>
    /// Calculates translate changes when panning with UI pan control.
    /// </summary>
    internal class UIControlPanGesture : PanAndZoomGestureBase
    {
        private const double _panAmount = 50;

        public UIControlPanGesture(IGestureTarget gestureTarget)
            : base(gestureTarget)
        {
            var panAndZoomEventSource = gestureTarget as IPanAndZoomEventSource;
            if (panAndZoomEventSource != null)
            {
                panAndZoomEventSource.Pan += OnPan;
            }
        }

        private void OnPan(object sender, PanEventArgs e)
        {
            Vector vector;
            switch (e.Direction)
            {
                case (PanDirection.Up):
                    vector = new Vector(0, -_panAmount);
                    break;
                case (PanDirection.Down):
                    vector = new Vector(0, _panAmount);
                    break;
                case (PanDirection.Left):
                    vector = new Vector(-_panAmount, 0);
                    break;
                case (PanDirection.Right):
                    vector = new Vector(_panAmount, 0);
                    break;
                default:
                    throw new Exception(string.Format("Unexpected PanDirection: {0}", e.Direction));
            }

            Translate(vector * Target.LinearScale);
        }
    }
}
