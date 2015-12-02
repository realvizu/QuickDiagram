using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Wpf.Common;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls
{
    /// <summary>
    /// This control draws a diagram connector on its parent canvas/panel.
    /// The visual appearance and the data bindings to its ViewModel are defined in XAML.
    /// The PathGeometry of the connector (arrow) is created by PointsToPathRelativeToRectConverter.
    /// </summary>
    public sealed class DiagramConnectorControl : DiagramShapeControlBase
    {
        private readonly DiagramConnector _diagramConnector;

        static DiagramConnectorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramConnectorControl),
                new FrameworkPropertyMetadata(typeof(DiagramConnectorControl)));
        }

        public static readonly DependencyProperty RoutePointsProperty =
            DependencyProperty.Register("RoutePoints", typeof(Point[]), typeof(DiagramConnectorControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Point[] RoutePoints
        {
            get { return (Point[])GetValue(RoutePointsProperty); }
            set { SetValue(RoutePointsProperty, value); }
        }

        public DiagramConnectorControl(DiagramConnector diagramConnector)
        {
            _diagramConnector = diagramConnector;
            DataContext = diagramConnector;
            AnimateEnter();
            RefreshBinding();
        }

        protected override DiagramShape DiagramShape => _diagramConnector;

        public override void RefreshBinding()
        {
            var rect = CalculateRect(_diagramConnector);
            Size = rect.Size;
            Position = rect.Location;
        }

        private static Rect CalculateRect(DiagramConnector diagramConnector)
        {
            var rectUnion = new[]
            {
                diagramConnector.Source.Rect.ToWpf(),
                diagramConnector.Target.Rect.ToWpf()
            }.Union();

            var routePoints = diagramConnector.RoutePoints.Select(j => j.ToWpf());
            foreach (var routePoint in routePoints)
                rectUnion.Union(routePoint);

            return rectUnion;
        }
    }
}
