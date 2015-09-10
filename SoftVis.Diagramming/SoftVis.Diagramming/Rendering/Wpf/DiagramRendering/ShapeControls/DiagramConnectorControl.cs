using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls
{
    /// <summary>
    /// This control draws a diagram connector on its parent canvas/panel.
    /// The visual appearance and the data bindings to its ViewModel are defined in XAML.
    /// The PathGeometry of the connector (arrow) is created by PointsToPathRelativeToRectConverter.
    /// </summary>
    public class DiagramConnectorControl : DiagramShapeControlBase
    {
        static DiagramConnectorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramConnectorControl),
                new FrameworkPropertyMetadata(typeof(DiagramConnectorControl)));
        }

        public static readonly DependencyProperty RoutePointsProperty =
            DependencyProperty.Register("RoutePoints", typeof(Point[]), typeof(DiagramConnectorControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public Point[] RoutePoints
        {
            get { return (Point[])GetValue(RoutePointsProperty); }
            set { SetValue(RoutePointsProperty, value); }
        }
    }
}
