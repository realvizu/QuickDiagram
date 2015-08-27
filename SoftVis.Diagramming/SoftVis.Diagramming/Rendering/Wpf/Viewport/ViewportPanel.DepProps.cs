using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport
{
    internal partial class ViewportPanel
    {
        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(ViewportPanel));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(ViewportPanel));

        public static readonly DependencyProperty ContentInDiagramSpaceProperty =
            DependencyProperty.Register("ContentInDiagramSpace", typeof(Rect), typeof(ViewportPanel));
    }
}
