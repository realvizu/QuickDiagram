using System.Windows;
using System.Windows.Input;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Rendering.Extensibility;
using Codartis.SoftVis.Rendering.Wpf.Common.HitTesting;

namespace Codartis.SoftVis.Rendering.Wpf
{
    public partial class DiagramViewerControl
    {
        public static readonly DependencyProperty DiagramProperty =
            DependencyProperty.Register("Diagram", typeof(Diagram), typeof(DiagramViewerControl));

        public static readonly DependencyProperty DiagramBehaviourProviderProperty =
            DependencyProperty.Register("DiagramBehaviourProvider", typeof(IDiagramBehaviourProvider), typeof(DiagramViewerControl));

        public static readonly DependencyProperty DiagramHitTesterProperty =
            DependencyProperty.Register("DiagramHitTester", typeof(IHitTester), typeof(DiagramViewerControl));

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(DiagramViewerControl));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(DiagramViewerControl));

        public static readonly DependencyProperty ShowRelatedEntitySelectorCommandProperty =
            DependencyProperty.Register("ShowRelatedEntitySelectorCommand", typeof(ICommand), typeof(DiagramViewerControl));

        public static readonly DependencyProperty HideRelatedEntitySelectorCommandProperty =
            DependencyProperty.Register("HideRelatedEntitySelectorCommand", typeof(ICommand), typeof(DiagramViewerControl));
    }
}
