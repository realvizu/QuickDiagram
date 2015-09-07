using System;
using System.Windows;
using Codartis.SoftVis.Rendering.Common.UIEvents;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Gestures
{
    /// <summary>
    /// Calculates viewport changes when panning with UI pan control.
    /// </summary>
    internal class PanWidgetEventViewportGesture : WidgetEventViewportGestureBase
    {
        private const double _panAmount = 50;

        public PanWidgetEventViewportGesture(IDiagramViewport diagramViewport, IWidgetEventSource widgetEventSource)
            : base(diagramViewport, widgetEventSource)
        {
            if (WidgetEventSource != null)
                WidgetEventSource.Pan += OnPan;
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

            MoveViewportCenterInDiagramSpaceBy(vector);
        }
    }
}
