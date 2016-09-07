using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Defines the visible properties of diagram connectors.
    /// </summary>
    /// <remarks>
    /// Calculates the enclosing rectangle of the route points 
    /// and translate the route points so they are relative to the bounding rectangle.
    /// </remarks>
    public sealed class DiagramConnectorViewModel : DiagramShapeViewModelBase
    {
        private static readonly DoubleCollection DashPattern = new DoubleCollection(new[] { 5d, 5d });

        private readonly IDiagramConnector _diagramConnector;
        private readonly ConnectorType _connectorType;
        private Point[] _routePoints;

        public DiagramConnectorViewModel(IDiagram diagram, IDiagramConnector diagramConnector)
            : base(diagram)
        {
            _diagramConnector = diagramConnector;
            _connectorType = Diagram.GetConnectorType(diagramConnector.Type);
            UpdatePropertiesFromDiagramShape();
        }

        public override IDiagramShape DiagramShape => _diagramConnector;

        public override void UpdatePropertiesFromDiagramShape()
        {
            var rect = _diagramConnector.Rect.ToWpf();
            Position = rect.Location;
            Size = rect.Size;
            var rectRelativeTranslate = -(Vector)rect.TopLeft;
            RoutePoints = _diagramConnector.RoutePoints?.Select(i => i.ToWpf() + rectRelativeTranslate).ToArray();
        }

        public ArrowHeadType ArrowHeadType => _connectorType.ArrowHeadType;
        private bool IsDashed => _connectorType.ShaftLineType == LineType.Dashed;
        public DoubleCollection StrokeDashArray => IsDashed ? DashPattern : null;

        public Point[] RoutePoints
        {
            get { return _routePoints; }
            set
            {
                if (_routePoints != value)
                {
                    _routePoints = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
