using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Wpf.Animations;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Shapes
{
    /// <summary>
    /// This control draws a diagram connector on its parent canvas/panel.
    /// The visual appearance and the data bindings to its ViewModel are defined in XAML.
    /// The PathGeometry of the connector (arrow) is created by PointsToPathConverter.
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
                new FrameworkPropertyMetadata(null, 
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public Point[] RoutePoints
        {
            get { return (Point[])GetValue(RoutePointsProperty); }
            set { SetValue(RoutePointsProperty, value); }
        }

        public DiagramConnectorControl(DiagramConnector diagramConnector)
        {
            _diagramConnector = diagramConnector;
            DataContext = diagramConnector;
        }

        protected override DiagramShape DiagramShape => _diagramConnector;

        public override void RefreshBinding()
        {
            var newRoutePoints = _diagramConnector.RoutePoints.Select(j => j.ToWpf()).ToArray();
            var rect = CalculateRect(_diagramConnector.Source.Rect.ToWpf(), _diagramConnector.Target.Rect.ToWpf(), newRoutePoints);

            Appear();
            MoveTo(rect.Location);
            SizeTo(rect.Size);
            RerouteTo(newRoutePoints);
        }

        private static Rect CalculateRect(Rect sourceRect, Rect targetRect, Point[] routePoints)
        {
            var rectUnion = Rect.Union(sourceRect, targetRect);
            foreach (var routePoint in routePoints)
                rectUnion.Union(routePoint);
            return rectUnion;
        }

        private void RerouteTo(Point[] newRoute)
        {
            if (RoutePoints == null)
                RoutePoints = newRoute;
            else
                AnimateRoute(newRoute);
        }

        private void AnimateRoute(Point[] newRoute)
        {
            var animation = new PointArrayAnimation(RoutePoints, newRoute, ShapeMoveAnimationDuration);
            BeginAnimation(RoutePointsProperty, animation);
        }
    }
}
