using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport
{
    internal partial class DiagramViewportPanel
    {
        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(DiagramViewportPanel));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(DiagramViewportPanel));
    }
}
