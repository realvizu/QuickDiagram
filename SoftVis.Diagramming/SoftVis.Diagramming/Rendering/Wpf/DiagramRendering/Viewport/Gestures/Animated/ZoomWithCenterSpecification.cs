using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Gestures.Animated
{
    internal class ZoomWithCenterSpecification
    {
        public double Zoom { get; private set; }
        public Point CenterInScreenSpace { get; private set; }

        internal ZoomWithCenterSpecification(double zoom, Point centerInScreenSpace)
        {
            Zoom = zoom;
            CenterInScreenSpace = centerInScreenSpace;
        }
    }
}
