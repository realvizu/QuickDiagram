using System.Windows;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf
{
    public partial class DiagramCanvasControl
    {
        public static readonly DependencyProperty DiagramProperty =
            DependencyProperty.Register("Diagram", typeof(Diagram), typeof(DiagramCanvasControl),
                new FrameworkPropertyMetadata(null, Diagram_PropertyChanged));

        public static readonly DependencyProperty DiagramRectProperty =
            DependencyProperty.Register("DiagramRect", typeof(Rect), typeof(DiagramCanvasControl));

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(DiagramCanvasControl));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(DiagramCanvasControl));
    }
}
