using System;
using System.Windows;
using Codartis.SoftVis.Rendering.Common.UIEvents;
using Codartis.SoftVis.Rendering.Wpf.Common.UIEvents;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Viewing.Gestures
{
    /// <summary>
    /// Calculates viewport changes when panning with UI pan control.
    /// </summary>
    internal class PanWidgetViewportGesture : ViewportGestureBase
    {
        private const double PanAmount = 50;

        internal PanWidgetViewportGesture(IDiagramViewport diagramViewport, IUIEventSource uiEventSource)
            : base(diagramViewport, uiEventSource)
        {
            UIEventSource.PanWidget += OnPan;
        }

        private void OnPan(object sender, PanEventArgs e)
        {
            var vector = CalculatePanVector(e.Direction);
            MoveViewportCenterInDiagramSpaceBy(vector);
        }

        private static Vector CalculatePanVector(PanDirection panDirection)
        {
            Vector vector;
            switch (panDirection)
            {
                case (PanDirection.Up):
                    vector = new Vector(0, -PanAmount);
                    break;
                case (PanDirection.Down):
                    vector = new Vector(0, PanAmount);
                    break;
                case (PanDirection.Left):
                    vector = new Vector(-PanAmount, 0);
                    break;
                case (PanDirection.Right):
                    vector = new Vector(PanAmount, 0);
                    break;
                default:
                    throw new Exception($"Unexpected PanDirection: {panDirection}");
            }
            return vector;
        }
    }
}
