using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Wpf.Common;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramFixtures
{
    public class DiagramConnectorControl : DiagramShapeControlBase
    {
        static DiagramConnectorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramConnectorControl), new FrameworkPropertyMetadata(typeof(DiagramConnectorControl)));
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(DiagramNodeControl), typeof(DiagramConnectorControl));

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(DiagramNodeControl), typeof(DiagramConnectorControl));

        public static readonly DependencyProperty RoutePointsProperty =
            DependencyProperty.Register("RoutePoints", typeof(Point[]), typeof(DiagramConnectorControl));

        public static readonly DependencyProperty DiagramConnectorProperty =
            DependencyProperty.Register("DiagramConnector", typeof(DiagramConnector), typeof(DiagramConnectorControl));

        public DiagramNodeControl Source
        {
            get { return (DiagramNodeControl)GetValue(SourceProperty); }
            internal set { SetValue(SourceProperty, value); }
        }

        public DiagramNodeControl Target
        {
            get { return (DiagramNodeControl)GetValue(TargetProperty); }
            internal set { SetValue(TargetProperty, value); }
        }

        public Point[] RoutePoints
        {
            get { return (Point[])GetValue(RoutePointsProperty); }
            set { SetValue(RoutePointsProperty, value); }
        }

        public DiagramConnector DiagramConnector
        {
            get { return (DiagramConnector)GetValue(DiagramConnectorProperty); }
            set { SetValue(DiagramConnectorProperty, value); }
        }

        public override void Update()
        {
            Rect = DiagramConnector.Rect.ToWpf();
        }
    }
}
