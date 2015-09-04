using Codartis.SoftVis.Diagramming;
using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering
{
    public partial class DiagramPanelBase
    {
        public static readonly DependencyProperty DiagramProperty =
            DependencyProperty.Register("Diagram", typeof(Diagram), typeof(DiagramPanelBase),
                new FrameworkPropertyMetadata(null, Diagram_PropertyChanged));

        public static readonly DependencyProperty DiagramRectProperty =
            DependencyProperty.Register("DiagramRect", typeof(Rect), typeof(DiagramPanelBase));
    }
}
