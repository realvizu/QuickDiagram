using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport.Gestures.Animated
{
    public class ZoomWithCenterSpecification
    {
        public double Zoom { get; private set; }
        public Point CenterInScreenSpace { get; private set; }

        public ZoomWithCenterSpecification(double zoom, Point centerInScreenSpace)
        {
            Zoom = zoom;
            CenterInScreenSpace = centerInScreenSpace;
        }
    }
}
