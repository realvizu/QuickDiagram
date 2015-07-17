using System.Windows;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf
{
    public partial class DiagramCanvas
    {
        public static readonly DependencyProperty DiagramProperty =
            DependencyProperty.Register("Diagram", typeof(Diagram), typeof(DiagramCanvas),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, Diagram_PropertyChanged));

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(DiagramCanvas));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(DiagramCanvas));
    }
}
