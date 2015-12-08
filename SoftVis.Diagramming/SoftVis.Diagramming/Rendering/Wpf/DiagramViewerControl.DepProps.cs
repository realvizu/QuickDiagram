using System.Windows;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Rendering.Extensibility;

namespace Codartis.SoftVis.Rendering.Wpf
{
    public partial class DiagramViewerControl
    {
        public static readonly DependencyProperty DiagramProperty =
            DependencyProperty.Register("Diagram", typeof(Diagram), typeof(DiagramViewerControl));

        public static readonly DependencyProperty DiagramBehaviourProviderProperty =
            DependencyProperty.Register("DiagramBehaviourProvider", typeof(IDiagramBehaviourProvider), typeof(DiagramViewerControl));

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(DiagramViewerControl));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(DiagramViewerControl));
    }
}
